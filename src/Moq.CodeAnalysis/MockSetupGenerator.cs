using System;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Moq.CodeAnalysis
{
    /// <summary>
    /// Generates <c>IMock&lt;T&gt;</c> extension methods for every instance member of 
    /// each distinct <c>T</c> passed to a <c>[MockGenerator]</c>-annotated method. Each 
    /// extension invokes the member on the mock and returns the resulting <c>IMockSetup</c>.
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public class MockSetupGenerator : IIncrementalGenerator
    {
        /// <summary>
        /// Names of the tracked incremental steps.
        /// </summary>
        public static class TrackingNames
        {
            /// <summary>Mocked type identifiers found at each <c>[MockGenerator]</c> invocation.</summary>
            public const string MockedTypes = nameof(MockedTypes);
            /// <summary>Distinct mocked type identifiers across the compilation.</summary>
            public const string DistinctTypes = nameof(DistinctTypes);
            /// <summary>Rendered setup extensions for each distinct mocked type.</summary>
            public const string Setups = nameof(Setups);
        }

        /// <inheritdoc />
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var types = context.SyntaxProvider
                .CreateSyntaxProvider(
                    static (node, _) => node is InvocationExpressionSyntax
                    {
                        Expression: GenericNameSyntax or MemberAccessExpressionSyntax { Name: GenericNameSyntax }
                    },
                    static (context, cancellation) => GetMockedType(context, cancellation))
                .Where(static id => id is not null)
                .Select(static (id, _) => id!)
                .WithTrackingName(TrackingNames.MockedTypes)
                .Collect()
                .SelectMany(static (ids, _) => ids.Distinct().OrderBy(id => id.Id, StringComparer.Ordinal).ThenBy(id => id.Assembly, StringComparer.Ordinal))
                .WithTrackingName(TrackingNames.DistinctTypes);

            var setups = types
                .Combine(context.CompilationProvider)
                .Select(static (pair, cancellation) => MockSetupRenderer.Render(pair.Right, pair.Left, cancellation))
                .Where(static setup => setup is not null)
                .Select(static (setup, _) => setup!)
                .WithTrackingName(TrackingNames.Setups);

            context.RegisterSourceOutput(setups, static (context, setup) => context.AddSource(setup.HintName, setup.Source));
        }

        static MockedType? GetMockedType(GeneratorSyntaxContext context, CancellationToken cancellation)
        {
            if (context.SemanticModel.GetSymbolInfo(context.Node, cancellation).Symbol is not IMethodSymbol { TypeArguments.Length: > 0 } method ||
                !method.OriginalDefinition.GetAttributes().Any(IsMockGenerator) ||
                method.TypeArguments[0] is not INamedTypeSymbol type ||
                !CanMock(type))
                return null;

            return new MockedType(DocumentationCommentId.CreateReferenceId(type), type.ContainingAssembly.Identity.GetDisplayName());
        }

        static bool IsMockGenerator(AttributeData attribute) => attribute.AttributeClass is
        {
            Name: "MockGeneratorAttribute",
            ContainingNamespace: { Name: "Moq", ContainingNamespace.IsGlobalNamespace: true }
        };

        static bool CanMock(INamedTypeSymbol type)
            => (type.TypeKind == TypeKind.Interface || (type.TypeKind == TypeKind.Class && !type.IsSealed && !type.IsStatic)) &&
               IsClosed(type);

        static bool IsClosed(ITypeSymbol type) => type switch
        {
            ITypeParameterSymbol or IErrorTypeSymbol => false,
            IArrayTypeSymbol array => IsClosed(array.ElementType),
            INamedTypeSymbol named => !named.IsUnboundGenericType &&
                named.TypeArguments.All(IsClosed) &&
                (named.ContainingType is null || IsClosed(named.ContainingType)),
            _ => true,
        };
    }
}
