using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Tags;
using Microsoft.CodeAnalysis.Text;

namespace Moq.CodeFixes
{
    [ExportCompletionProvider(nameof(CustomDelegateCompletion), LanguageNames.CSharp)]
    public class CustomDelegateCompletion : CompletionProvider
    {
        static readonly CompletionItemRules rules = CompletionItemRules.Create(selectionBehavior: CompletionItemSelectionBehavior.SoftSelection);

        public override Task<CompletionDescription> GetDescriptionAsync(Document document, CompletionItem item, CancellationToken cancellationToken)
        {
            if (!item.Tags.Contains(nameof(CustomDelegateCompletion)))
                return base.GetDescriptionAsync(document, item, cancellationToken);

            return Task.FromResult(CompletionDescription.FromText(ThisAssembly.Strings.CustomDelegateCompletion.Description));
        }

        public override Task<CompletionChange> GetChangeAsync(Document document, CompletionItem item, char? commitKey, CancellationToken cancellationToken)
        {
            File.AppendAllText(Path.Combine(Path.GetTempPath(), nameof(CustomDelegateCompletion) + ".txt"), nameof(GetChangeAsync));
            return base.GetChangeAsync(document, item, commitKey, cancellationToken);
        }

        public override async Task ProvideCompletionsAsync(CompletionContext context)
        {
            if (context.Document.SupportsSemanticModel != true)
                return;

            var position = context.Position;
            var document = context.Document;
            var cancellation = context.CancellationToken;

            var root = await document.GetSyntaxRootAsync(cancellation).ConfigureAwait(false);
            if (root == null)
                return;

            var span = context.CompletionListSpan;
            var token = root.FindToken(span.Start);
            if (token.Parent == null)
                return;

            var node = token.Parent.AncestorsAndSelf().FirstOrDefault(a => a.FullSpan.Contains(span));
            if (node == null)
                return;

            if (node.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().FirstOrDefault() is not InvocationExpressionSyntax invocation)
                return;

            var semantic = await document.GetSemanticModelAsync(cancellation).ConfigureAwait(false);
            if (semantic == null)
                return;

            var scope = semantic.Compilation.GetTypeByMetadataName(typeof(SetupScopeAttribute).FullName);
            if (scope == null)
                return;

            bool IsSetupScope(ISymbol? symbol) => symbol is IMethodSymbol &&
                symbol.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, scope));

            if (!IsSetupScope(semantic.GetSymbolInfo(invocation, cancellation).Symbol) &&
                !semantic.GetSymbolInfo(invocation, cancellation).CandidateSymbols.Any(IsSetupScope))
                return;

            if (invocation.Expression is not MemberAccessExpressionSyntax member)
                return;

            var symbol = semantic.GetSymbolInfo(member.Expression, cancellation).Symbol;
            var target = symbol as ITypeSymbol ?? (symbol as ILocalSymbol)?.Type;

            if (symbol == null || target == null)
                return;

            var start = invocation.ArgumentList.Span.Start + 1;
            var length = span.End - start;
            // Wrong length, shouldn't happen, but bail just in case.
            if (length < 0)
                return;

            var existing = (await document.GetTextAsync(cancellation).ConfigureAwait(false))
                .GetSubText(new TextSpan(start, length)).ToString();

            // In this case, completion would already have the right items, no need to annotate.
            if (existing.StartsWith(symbol.Name + "."))
                return;

            // List all the members of the target type that have ref/out parameter
            var members = target.GetMembers().OfType<IMethodSymbol>()
                .Where(m => m.Parameters.Any(p => p.RefKind == RefKind.Ref || p.RefKind == RefKind.Out)).ToArray();

            foreach (var candidate in members)
            {
                context.AddItem(CompletionItem.Create(
                    displayText: symbol.Name + "." + candidate.Name,
                    sortText: symbol.Name + "." + candidate.Name,
                    filterText: symbol.Name + "." + candidate.Name,
                    tags: ImmutableArray.Create(WellKnownTags.Method).Add(nameof(CustomDelegateCompletion)),
                    rules: rules,
                    inlineDescription: "Setup via delegate"));
            }
        }

        public override bool ShouldTriggerCompletion(SourceText text, int caretPosition, CompletionTrigger trigger, OptionSet options)
        {
            if (trigger.Kind == CompletionTriggerKind.Invoke ||
                trigger.Kind == CompletionTriggerKind.InvokeAndCommitIfUnique)
                return true;

            if (trigger.Kind == CompletionTriggerKind.Insertion &&
                (trigger.Character == '.' || trigger.Character == '('))
                return true;

            return base.ShouldTriggerCompletion(text, caretPosition, trigger, options);
        }
    }
}
