using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Moq.CodeFixes
{
    /// <summary>
    /// Reports setups like <c>Setup(() => mock.Object.Add(1, 2))</c> that can use 
    /// the generated typed setup instead, like <c>mock.Add(1, 2)</c>.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SimplifySetupAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(MockDiagnostics.SimplifySetup);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        static void Analyze(SyntaxNodeAnalysisContext context)
        {
            var setup = (InvocationExpressionSyntax)context.Node;
            if (TryGetReplacement(setup, context.SemanticModel, context.CancellationToken) is { } replacement)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    MockDiagnostics.SimplifySetup,
                    setup.GetLocation(),
                    replacement.ToString()));
            }
        }

        /// <summary>
        /// Gets the typed setup that replaces the given static <c>Setup(() => ...)</c> invocation, 
        /// if the member is invoked directly on the mocked object and has a generated typed setup.
        /// </summary>
        internal static ExpressionSyntax? TryGetReplacement(InvocationExpressionSyntax setup, SemanticModel semantic, CancellationToken cancellation)
        {
            if (setup.Expression is not IdentifierNameSyntax { Identifier.ValueText: "Setup" } ||
                setup.ArgumentList.Arguments.Count != 1 ||
                setup.ArgumentList.Arguments[0].Expression is not ParenthesizedLambdaExpressionSyntax { ParameterList.Parameters.Count: 0, ExpressionBody: { } body } ||
                semantic.GetSymbolInfo(setup, cancellation).Symbol is not IMethodSymbol { ContainingType: { Name: "Syntax", ContainingNamespace.Name: "Moq" } })
                return null;

            // Changing the setup type is only safe when its value isn't consumed other than by fluent verbs.
            var chain = MockSyntax.GetFluentChain(setup);
            if (chain.Parent is not ExpressionStatementSyntax)
                return null;

            // Handlers for typed setups receive the member arguments rather than the argument collection.
            if (chain.DescendantNodes().OfType<LambdaExpressionSyntax>().Any(lambda => !setup.Span.Contains(lambda.Span) && lambda switch
            {
                SimpleLambdaExpressionSyntax => true,
                ParenthesizedLambdaExpressionSyntax parenthesized => parenthesized.ParameterList.Parameters.Count > 0,
                _ => false,
            }))
                return null;

            foreach (var candidate in GetCandidates(body, semantic, cancellation))
            {
                // Property and indexer handles on their own aren't valid statements.
                if (chain == setup && candidate is not InvocationExpressionSyntax)
                    continue;

                if (!MockSyntax.IsGeneratedSetup(semantic.GetSpeculativeSymbolInfo(setup.SpanStart, candidate, SpeculativeBindingOption.BindAsExpression).Symbol))
                    continue;

                if (chain != setup &&
                    semantic.GetSpeculativeSymbolInfo(chain.SpanStart, chain.ReplaceNode(setup, candidate), SpeculativeBindingOption.BindAsExpression).Symbol == null)
                    continue;

                return candidate.WithTriviaFrom(setup);
            }

            return null;
        }

        static IEnumerable<ExpressionSyntax> GetCandidates(ExpressionSyntax body, SemanticModel semantic, CancellationToken cancellation)
        {
            switch (body)
            {
                case InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax access } invocation
                    when MockSyntax.IsMockObject(access.Expression, semantic, cancellation, out var mock):
                    yield return invocation.WithExpression(access.WithExpression(mock));
                    break;
                case MemberAccessExpressionSyntax access
                    when MockSyntax.IsMockObject(access.Expression, semantic, cancellation, out var mock):
                    yield return access.WithExpression(mock);
                    break;
                case ElementAccessExpressionSyntax element
                    when MockSyntax.IsMockObject(element.Expression, semantic, cancellation, out var mock):
                    // Indexers are generated as extension indexers or Item methods, depending on the language version.
                    yield return element.WithExpression(mock);
                    yield return InvocationExpression(
                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, mock, IdentifierName("Item")),
                        ArgumentList(element.ArgumentList.Arguments));
                    break;
            }
        }
    }
}
