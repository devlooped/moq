using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq.Sdk;
using Stunts;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Moq.Processors
{
    /// <summary>
    /// Generates the C# implementation of the mock interfaces.
    /// </summary>
    class CSharpMocked : ISyntaxProcessor
    {
        public string Language => LanguageNames.CSharp;

        public ProcessorPhase Phase => ProcessorPhase.Rewrite;

        public SyntaxNode Process(SyntaxNode syntax, ProcessorContext context)
            => new CSharpRewriteVisitor().Visit(syntax);

        class CSharpRewriteVisitor : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                node = (ClassDeclarationSyntax)base.VisitClassDeclaration(node)!;

                node = (ClassDeclarationSyntax)new FieldReferenceRewriter().Visit(node)!;

                if (node.BaseList != null && !node.BaseList.Types.Any(x =>
                    x.ToString() == nameof(IMocked) ||
                    x.ToString() == typeof(IMocked).FullName))
                {
                    // Only add the base type if it isn't already there
                    node = node.AddBaseListTypes(SimpleBaseType(IdentifierName(nameof(IMocked))));
                }

                if (!node.Members.OfType<FieldDeclarationSyntax>().Any(field =>
                    field.Declaration.Variables.Any(decl => decl.Identifier.ToString() == "mock")))
                {
                    var field = FieldDeclaration(
                        VariableDeclaration(IdentifierName(Identifier(nameof(IMockRuntime))))
                            .WithVariables(SingletonSeparatedList(VariableDeclarator(Identifier("mock"))))
                        );

                    // Try to insert the mock field following the pipeline field
                    var pipeline = node.Members.OfType<FieldDeclarationSyntax>().FirstOrDefault(field =>
                        field.Declaration.Variables.Any(decl => decl.Identifier.ToString() == "pipeline"));

                    if (pipeline != null)
                    {
                        node = node.InsertNodesAfter(pipeline, new[]
                        {
                            field.WithLeadingTrivia(pipeline.GetLeadingTrivia())
                        });
                    }
                    else
                    {
                        node = node.InsertNodesBefore(node.Members.First(), new[]
                        {
                            field.WithLeadingTrivia(ElasticTab, ElasticTab)
                                 .NormalizeWhitespace()
                                 .WithTrailingTrivia(CarriageReturnLineFeed, CarriageReturnLineFeed)
                        });
                    }
                }

                if (!node.Members.OfType<PropertyDeclarationSyntax>().Any(prop => prop.Identifier.ToString() == nameof(IMocked.Runtime)))
                {
                    var property = PropertyDeclaration(IdentifierName(nameof(IMockRuntime)), nameof(IMocked.Runtime))
                        // Make IMocked properties explicit.
                        .WithExplicitInterfaceSpecifier(
                            ExplicitInterfaceSpecifier(
                                IdentifierName(nameof(IMocked))))
                        .WithModifiers(TokenList())
                        // => LazyInitializer.EnsureInitialized(ref mock, () => new MockInfo(pipeline.Behaviors));
                        .WithExpressionBody(
                            ArrowExpressionClause(
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(nameof(LazyInitializer)),
                                        IdentifierName(nameof(LazyInitializer.EnsureInitialized))),
                                    ArgumentList(SeparatedList(new ArgumentSyntax[]
                                    {
                                        Argument(NameColon("target"), Token(SyntaxKind.RefKeyword), IdentifierName("mock")),
                                        Argument(ParenthesizedLambdaExpression(
                                            ObjectCreationExpression(
                                                IdentifierName(nameof(DefaultMockRuntime)))
                                            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(
                                                ThisExpression()
                                            ))))
                                        ))
                                    }))
                                )
                        ))
                      .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

                    // Try to insert the Runtime property following the Behaviors property
                    var behaviors = node.Members.OfType<PropertyDeclarationSyntax>().FirstOrDefault(prop => prop.Identifier.ToString() == nameof(IStunt.Behaviors));
                    if (behaviors != null)
                        node = node.InsertNodesAfter(behaviors, new[] { property });
                    else
                        node = node.AddMembers(property);
                }

                return node;
            }

            class FieldReferenceRewriter : CSharpSyntaxRewriter
            {
                public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
                {
                    if (node.Expression is IdentifierNameSyntax { Identifier.ValueText: "pipeline" } pipeline)
                    {
                        node = node.WithExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ThisExpression(),
                            pipeline));
                    }

                    return base.VisitMemberAccessExpression(node)!;
                }

                public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
                {
                    if (node.Expression is IdentifierNameSyntax { Identifier.ValueText: "implementation" } implementation)
                    {
                        node = node.WithExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ThisExpression(),
                            implementation));
                    }

                    return base.VisitInvocationExpression(node)!;
                }

                public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
                {
                    if (node.IsKind(SyntaxKind.EqualsExpression) || node.IsKind(SyntaxKind.NotEqualsExpression))
                    {
                        if (node.Left is IdentifierNameSyntax { Identifier.ValueText: "implementation" } left &&
                            node.Right.IsKind(SyntaxKind.NullLiteralExpression))
                            node = node.WithLeft(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), left));
                        else if (node.Right is IdentifierNameSyntax { Identifier.ValueText: "implementation" } right &&
                            node.Left.IsKind(SyntaxKind.NullLiteralExpression))
                            node = node.WithRight(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), right));
                    }

                    return base.VisitBinaryExpression(node)!;
                }

                public override SyntaxNode VisitIsPatternExpression(IsPatternExpressionSyntax node)
                {
                    if (node.Expression is IdentifierNameSyntax { Identifier.ValueText: "implementation" } implementation)
                        node = node.WithExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), implementation));

                    return base.VisitIsPatternExpression(node)!;
                }
            }
        }
    }
}