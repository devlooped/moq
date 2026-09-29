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
    /// Replaces <c>Setup(() => mock.Object.Add(1, 2))</c> with the generated typed setup <c>mock.Add(1, 2)</c>.
    /// </summary>
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SimplifySetupCodeFix)), Shared]
    public class SimplifySetupCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(MockDiagnostics.SimplifySetup.Id);

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root?.FindNode(context.Span, getInnermostNodeForTie: true)?.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault() is not { } setup)
                return;

            var diagnostic = context.Diagnostics.First();
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: ThisAssembly.Strings.SimplifySetup.Title,
                    createChangedDocument: cancellation => SimplifySetupAsync(context.Document, root, setup, cancellation),
                    equivalenceKey: nameof(SimplifySetupCodeFix)),
                diagnostic);
        }

        static async Task<Document> SimplifySetupAsync(Document document, SyntaxNode root, InvocationExpressionSyntax setup, CancellationToken cancellation)
        {
            var semantic = await document.GetSemanticModelAsync(cancellation).ConfigureAwait(false);
            if (semantic == null || SimplifySetupAnalyzer.TryGetReplacement(setup, semantic, cancellation) is not { } replacement)
                return document;

            return document.WithSyntaxRoot(root.ReplaceNode(setup, replacement));
        }
    }
}
