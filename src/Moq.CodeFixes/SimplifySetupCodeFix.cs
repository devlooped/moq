using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Moq.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SimplifySetupCodeFix)), Shared]
    public class SimplifySetupCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(MockDiagnostics.SimplifySetup.Id);

        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root == null)
                return;

            var diagnostic = context.Diagnostics.First();

            context.RegisterCodeFix(
                    CodeAction.Create(
                        title: ThisAssembly.Strings.SimplifySetup.Title,
                        createChangedDocument: cancellation => SimplifySetupAsync(context.Document, diagnostic, root, cancellation),
                        equivalenceKey: ThisAssembly.Strings.SimplifySetup.Title),
                    diagnostic);
        }

        async Task<Document> SimplifySetupAsync(Document document, Diagnostic diagnostic, SyntaxNode root, CancellationToken cancellation)
        {
            var node = root.FindNode(diagnostic.Location.SourceSpan);
            if (node == null)
                return document;

            // Get the matching symbol for the given generator attribute from the current compilation.
            var compilation = await document.Project.GetCompilationAsync(cancellation);
            if (compilation == null)
                return document;

            if (node.Parent is UsingStatementSyntax block && block.Expression != null)
            {
                var newblock = block.WithExpression(InvocationExpression(IdentifierName("Setup")));
                root = root.ReplaceNode(block, newblock);
                node = root.FindNode(new TextSpan(block.SpanStart, newblock.Span.Length));
            }

            var setupName = ParseName("Moq.Syntax");

            var ns = node.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
            var cu = node.Ancestors().OfType<CompilationUnitSyntax>().FirstOrDefault();

            if (ns.Usings != null && ns.Usings.Count > 0 && !ns.Usings.Any(x => x.Name.IsEquivalentTo(setupName)))
            {
                root = ns.AddUsings(UsingDirective(Token(SyntaxKind.StaticKeyword), null, setupName))
                    .SyntaxTree.GetRoot(cancellation);
            }
            else if (cu.Usings != null && !cu.Usings.Any(x => x.Name.IsEquivalentTo(setupName)))
            {
                root = cu.AddUsings(UsingDirective(Token(SyntaxKind.StaticKeyword), null, setupName))
                    .SyntaxTree.GetRoot(cancellation);
            }

            return document.WithSyntaxRoot(root);
        }
    }
}
