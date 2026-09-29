using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Moq.CodeFixes
{
    /// <summary>
    /// Rewrites setups of mocked type members that are hidden by a member of the mock, or 
    /// that have no generated typed setup, into <c>Setup(() => mock.Object.Member(...))</c>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HiddenMemberCodeFix)), Shared]
    public class HiddenMemberCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            MockDiagnostics.HiddenMember.Id,
            "CS0305", // Using the generic method requires N type arguments
            "CS0411", // The type arguments for method cannot be inferred from the usage
            "CS1061", // 'IMock<T>' does not contain a definition for 'X'
            "CS1501", // No overload for method 'X' takes N arguments
            "CS1503", // Argument N: cannot convert from 'X' to 'Y'
            "CS1929", // 'X' does not contain a definition for 'Returns' and the best extension method overload requires a receiver of type 'ISetup'
            "CS1955", // Non-invocable member 'X' cannot be used like a method
            "CS7036"  // There is no argument given that corresponds to the required parameter
            );

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var semantic = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
            if (root == null || semantic == null)
                return;

            var diagnostic = context.Diagnostics.First();
            var access = root.FindNode(context.Span, getInnermostNodeForTie: true)
                .AncestorsAndSelf()
                .OfType<MemberAccessExpressionSyntax>()
                .FirstOrDefault(x => IsHiddenMember(x, semantic, context.CancellationToken));

            if (access == null)
                return;

            var target = access.Parent is InvocationExpressionSyntax invocation && invocation.Expression == access ?
                (ExpressionSyntax)invocation : access;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: ThisAssembly.Strings.HiddenMemberCodeFix.TitleFormat(access.Name.Identifier.ValueText),
                    createChangedDocument: cancellation =>
                    {
                        var setup = MockSyntax.CreateSetup(target, access.Expression);
                        return Task.FromResult(context.Document.WithSyntaxRoot(
                            MockSyntax.AddSyntaxUsing(root.ReplaceNode(target, setup), semantic, target)));
                    },
                    equivalenceKey: nameof(HiddenMemberCodeFix)),
                diagnostic);
        }

        static bool IsHiddenMember(MemberAccessExpressionSyntax access, SemanticModel semantic, CancellationToken cancellation)
        {
            if (access.Name.Identifier.ValueText == "Object" ||
                access.Parent is AssignmentExpressionSyntax assignment && assignment.Left == access ||
                MockSyntax.GetMockedType(semantic.GetTypeInfo(access.Expression, cancellation).Type) is not { } mocked ||
                MockSyntax.FindMember(mocked, access.Name.Identifier.ValueText) == null)
                return false;

            // Errors in the arguments of a generated setup aren't fixed by setting up the member on the object.
            var symbol = semantic.GetSymbolInfo(access, cancellation);
            return !MockSyntax.IsGeneratedSetup(symbol.Symbol) && !symbol.CandidateSymbols.Any(MockSyntax.IsGeneratedSetup);
        }
    }
}
