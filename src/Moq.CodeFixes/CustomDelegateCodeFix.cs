using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;
using Moq.CodeFixes;
using CS = Microsoft.CodeAnalysis.CSharp.Syntax;
using CSFactory = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using VB = Microsoft.CodeAnalysis.VisualBasic.Syntax;
using VBFactory = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory;

namespace Moq
{
    /// <summary>
    /// Generates code for custom delegates used for ref/out mocking with <c>SetupRef</c>, 
    /// like <c>SetupRef&lt;TryParse&gt;(mock.Object.TryParse)</c>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, LanguageNames.VisualBasic, Name = nameof(CustomDelegateCodeFix))]
    public class CustomDelegateCodeFix : CodeFixProvider
    {
        /// <inheritdoc />
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            "CS0411",  // The type arguments for method 'SetupRef<TDelegate>' cannot be inferred from the usage.
            "BC36645", // Data type(s) of the type parameter(s) in method 'SetupRef' cannot be inferred from these arguments.
            "CS1503",  // cannot convert from 'method group' to 'Action<...>'
            "CS1593",  // Delegate 'Action<...>' does not take 0 arguments
            "BC31143", // Method '..' does not have a signature compatible with delegate '...'.
            "BC30581"  //'AddressOf' expression cannot be converted to 'Object' because 'Object' is not a delegate type.
            );

        /// <inheritdoc />
        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var document = context.Document;
            var span = context.Span;
            var root = await document.GetSyntaxRootAsync(context.CancellationToken);
            if (root == null)
                return;

            var token = root.FindToken(span.Start);
            if (!token.Span.IntersectsWith(span))
                return;

            var diagnostic = context.Diagnostics.FirstOrDefault(d => FixableDiagnosticIds.Contains(d.Id));
            if (diagnostic == null)
                return;

            var semantic = await document.GetSemanticModelAsync(context.CancellationToken);

            var node = root.FindNode(span);
            if (node == null || semantic == null)
                return;

            if (node.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.Argument))
                node = ((CS.ArgumentSyntax)node).Expression;

            // Type inference errors are reported on the setup method name or the whole invocation.
            if (node is CS.InvocationExpressionSyntax { ArgumentList.Arguments.Count: 1 } csInvocation)
                node = csInvocation.ArgumentList.Arguments[0].Expression;
            else if (node.Parent is CS.InvocationExpressionSyntax { ArgumentList.Arguments.Count: 1 } csParent && csParent.Expression == node)
                node = csParent.ArgumentList.Arguments[0].Expression;
            else if (node is VB.InvocationExpressionSyntax { ArgumentList.Arguments.Count: 1 } vbInvocation)
                node = vbInvocation.ArgumentList.Arguments[0].GetExpression();
            else if (node.Parent is VB.InvocationExpressionSyntax { ArgumentList.Arguments.Count: 1 } vbParent && vbParent.Expression == node)
                node = vbParent.ArgumentList.Arguments[0].GetExpression();

            var setup = document.Project.Language == LanguageNames.CSharp ?
                (SyntaxNode)node.Ancestors().OfType<CS.InvocationExpressionSyntax>().FirstOrDefault() :
                node.Ancestors().OfType<VB.InvocationExpressionSyntax>().FirstOrDefault();

            if (setup == null)
                return;

            var setupSymbol = semantic.GetSymbolInfo(setup);
            if (setupSymbol.CandidateSymbols.IsDefaultOrEmpty)
                return;

            var compilation = await document.Project.GetCompilationAsync();
            var scope = compilation?.GetTypeByMetadataName(typeof(SetupScopeAttribute).FullName);
            if (compilation == null || scope == null)
                return;

            // We only generate for [SetupScope] annotated methods. 
            // TODO: should integrate seamlessly with recursive mocks
            if (!setupSymbol.CandidateSymbols.Any(c => c.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, scope))))
                return;

            IMethodSymbol? targetMethod = null;
            SyntaxNode? receiver = null;

            // CS1593 case
            if (node is CS.LambdaExpressionSyntax)
            {
                var memberNode = node.ChildNodes().OfType<CS.MemberAccessExpressionSyntax>()
                    .Cast<SyntaxNode>()
                    .FirstOrDefault();

                if (memberNode == null)
                    return;

                targetMethod = GetMethod(semantic.GetSymbolInfo(memberNode));
            }
            // "BC30581" case
            else if (node is VB.UnaryExpressionSyntax && node.IsKind(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfExpression))
            {
                var memberNode = node.ChildNodes().OfType<VB.MemberAccessExpressionSyntax>()
                    .Cast<SyntaxNode>()
                    .FirstOrDefault();

                if (memberNode == null)
                    return;

                targetMethod = GetMethod(semantic.GetSymbolInfo(memberNode));
                receiver = ((VB.MemberAccessExpressionSyntax)memberNode).Expression;
            }
            else
            {
                // CS0411, CS1503 and BC31143 of direct method group
                targetMethod = GetMethod(semantic.GetSymbolInfo(node));
                receiver = (node as CS.MemberAccessExpressionSyntax)?.Expression;
            }

            // A method group on a recursive mock, like mock.Object.Parser.TryParse, must be 
            // resolved within the setup, so it's wrapped in a lambda.
            var recursive = receiver != null &&
                semantic.GetSymbolInfo(receiver).Symbol is { Kind: SymbolKind.Property or SymbolKind.Method } member &&
                !(member.Name == "Object" && MockSyntax.GetMockedType(member.ContainingType) != null);

            if (targetMethod != null)
                context.RegisterCodeFix(new SetupDelegateCodeAction(document, setup, targetMethod, recursive), context.Diagnostics);
        }

        static IMethodSymbol? GetMethod(SymbolInfo info)
            => info.Symbol as IMethodSymbol ?? info.CandidateSymbols.OfType<IMethodSymbol>().FirstOrDefault();

        /// <inheritdoc />
        public sealed override FixAllProvider? GetFixAllProvider() => null;

        class SetupDelegateCodeAction : CodeAction
        {
            readonly Document document;
            readonly IMethodSymbol symbol;
            readonly SyntaxNode setup;
            readonly bool recursive;

            public SetupDelegateCodeAction(Document document, SyntaxNode setup, IMethodSymbol symbol, bool recursive)
            {
                this.document = document;
                this.setup = setup;
                this.symbol = symbol;
                this.recursive = recursive;
            }

            public override string? EquivalenceKey => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) +
                "(" + string.Join(",", symbol.Parameters.Select(x => x.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))) + ")";

            public override string Title => ThisAssembly.Strings.CustomDelegateCodeFix.TitleFormat(symbol.Name);

            protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
            {
                var generator = SyntaxGenerator.GetGenerator(document);
                var root = await setup.SyntaxTree.GetRootAsync(cancellationToken);
                var node = FindSetup(root);
                var member = setup.Ancestors().First(x => generator.GetDeclarationKind(x) == DeclarationKind.Method);
                var @class = member.Ancestors().First(x => generator.GetDeclarationKind(x) == DeclarationKind.Class);
                var @delegate = generator.GetMembers(@class)
                    // Just check by name for now. Might need to also ensure signature compatibility.
                    .FirstOrDefault(x => generator.GetName(x) == symbol.Name);

                var delegateName = symbol.Name;
                var signature = generator.DelegateDeclaration(
                        delegateName,
                        parameters: symbol.Parameters.Select(prm => generator.ParameterDeclaration(prm)),
                        returnType: symbol.ReturnsVoid ? null : generator.TypeExpression(symbol.ReturnType));

                if (@delegate == null)
                {
                    root = root.InsertNodesAfter(member, new[] { signature });
                    // Find the updated setup
                    node = FindSetup(root);
                }
                else
                {
                    var tempDoc = document.WithSyntaxRoot(generator.ReplaceNode(root, @delegate, signature));
                    tempDoc = await Simplifier.ReduceAsync(tempDoc);
                    var tempRoot = await tempDoc.GetSyntaxRootAsync(cancellationToken);
                    if (tempRoot == null)
                        return document;

                    var className = generator.GetName(@class);
                    var tempClass = tempRoot.DescendantNodes().First(x =>
                        generator.GetDeclarationKind(x) == DeclarationKind.Class &&
                        generator.GetName(x) == className);
                    var tempDelegate = generator.GetMembers(tempClass)
                        .First(x => generator.GetName(x) == delegateName);

                    if (!@delegate.IsEquivalentTo(tempDelegate, true))
                    {
                        // Generate the delegate name using full Type+Member name.
                        delegateName = symbol.ContainingType.Name + symbol.Name;
                        signature = generator.WithName(signature, delegateName);
                        root = root.InsertNodesAfter(member, new[] { signature });
                        // Find the updated setup
                        node = FindSetup(root);
                    }
                }

                root = generator.ReplaceNode(root, node, generator.WithTypeArguments(node, generator.IdentifierName(delegateName)));
                // Find the updated setup
                node = FindSetup(root);
                if (node == null || node.Parent == null)
                    return document;

                // Wrap recursive mock access in a Func<TDelegate>
                if (recursive)
                {
                    var expression = node.Parent.ChildNodes().Last()
                        .DescendantNodes()
                        .Where(x =>
                            x.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.SimpleMemberAccessExpression) ||
                            // For VB, we actually wrap the AddressOf
                            x.IsKind(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfExpression))
                        .First();

                    root = generator.ReplaceNode(root, expression,
                        generator.ValueReturningLambdaExpression(expression));
                    // Find the updated setup
                    node = FindSetup(root);
                    if (node == null || node.Parent == null)
                        return document;
                }

                // If there is no Returns, generate one
                if (node.Parent?.Parent != null &&
                    node.Parent.Parent.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ExpressionStatement))
                {
                    var returns = (CS.InvocationExpressionSyntax)generator.InvocationExpression(
                        generator.MemberAccessExpression(
                            node.Parent.WithTrailingTrivia(
                                node.Parent.Parent.GetLeadingTrivia().Add(CSFactory.Whitespace("\t"))),
                            "Returns"),
                        generator.ValueReturningLambdaExpression(
                            symbol.Parameters.Select(prm => generator.ParameterDeclaration(prm)),
                            generator.ThrowExpression(generator.NullLiteralExpression())));

                    // Replace the parent InvocationExpression with the returning one.
                    root = generator.ReplaceNode(root, node.Parent, returns);

                    // Find the updated setup
                    node = FindSetup(root);

                    var statement = node.Ancestors().OfType<CS.ExpressionStatementSyntax>().FirstOrDefault();
                    if (statement != null &&
                        (statement.GetTrailingTrivia().Count == 0 ||
                        !statement.GetTrailingTrivia().Any(t => t.Token == statement.SemicolonToken)))
                    {
                        root = generator.ReplaceNode(root, statement, statement
                            .WithSemicolonToken(CSFactory.Token(Microsoft.CodeAnalysis.CSharp.SyntaxKind.SemicolonToken)
                            .WithTrailingTrivia(statement.GetTrailingTrivia().Insert(0, CSFactory.ElasticCarriageReturnLineFeed))));
                    }
                }
                else if (node.Parent?.Parent != null &&
                    node.Parent.Parent.IsKind(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExpressionStatement))
                {
                    var lambda = VBFactory.MultiLineFunctionLambdaExpression(
                        VBFactory.FunctionLambdaHeader().WithParameterList(
                            VBFactory.ParameterList(
                                VBFactory.SeparatedList(
                                    symbol.Parameters.Select(prm => (VB.ParameterSyntax)generator.ParameterDeclaration(prm))))),
                        VBFactory.List(new VB.StatementSyntax[]
                        {
                            VBFactory.ThrowStatement(
                                VBFactory.ObjectCreationExpression(
                                    VBFactory.ParseTypeName(nameof(NotImplementedException))))
                        }),
                        VBFactory.EndFunctionStatement());

                    var returns = generator.InvocationExpression(
                        generator.MemberAccessExpression(
                            node.Parent.WithTrailingTrivia(
                                node.Parent.Parent.GetLeadingTrivia()
                                    .Insert(0, VBFactory.Whitespace(" "))
                                    .Insert(1, VBFactory.LineContinuationTrivia(""))
                                    .Add(VBFactory.Whitespace("\t"))),
                            "Returns"),
                        lambda);

                    // Replace the parent InvocationExpression with the returning one.
                    root = generator.ReplaceNode(root, node.Parent, returns);
                }

                return document.WithSyntaxRoot(root);
            }

            SyntaxNode FindSetup(SyntaxNode root)
            {
                var node = root.FindNode(setup.Span, getInnermostNodeForTie: true);

                if (node.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.InvocationExpression))
                    node = ((CS.InvocationExpressionSyntax)node).Expression;
                else if (node.IsKind(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression))
                    node = ((VB.InvocationExpressionSyntax)node).Expression;
                return node;
            }
        }
    }
}
