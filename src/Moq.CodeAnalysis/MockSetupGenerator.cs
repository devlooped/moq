using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Moq.CodeAnalysis
{
    /// <summary>
    /// Generates C# 14 extension members on <c>IMock&lt;T&gt;</c> for every overridable 
    /// instance member of each distinct mocked <c>T</c>, which set up the member and 
    /// return a typed <c>ISetup</c>. Mocked types are discovered from generic invocations 
    /// of <c>[MockGenerator]</c> methods, object creations of <c>[MockGenerator]</c> types 
    /// and constructors, <c>Get</c> invocations returning mocks (i.e. <c>Mock.Get(instance)</c>) 
    /// and any reference to a closed <c>IMock&lt;T&gt;</c> or <c>Mock&lt;T&gt;</c>.
    /// </summary>
    [Generator(LanguageNames.CSharp)]
    public class MockSetupGenerator : IIncrementalGenerator
    {
        /// <summary>
        /// The minimum C# language version (as its numeric <see cref="LanguageVersion"/> value) 
        /// that supports extension members.
        /// </summary>
        internal const int ExtensionMembersVersion = 1400;

        /// <summary>
        /// Reported when the project's C# version does not support extension members.
        /// </summary>
        public static DiagnosticDescriptor LanguageVersionNotSupported { get; } = new(
            "MOQ010",
            "Typed mock setups require C# 14 or later",
            "Typed setups for mocks are not generated since the project uses C# {0}. Set <LangVersion>latest</LangVersion> in the project, or use Setup(() => ...) instead.",
            "Moq",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <summary>
        /// Names of the tracked incremental steps.
        /// </summary>
        public static class TrackingNames
        {
            /// <summary>Mocked type identifiers found at each usage.</summary>
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
                    static (node, _) => IsCandidate(node),
                    static (context, cancellation) => GetMockedTypes(context, cancellation))
                .SelectMany(static (ids, _) => ids)
                .WithTrackingName(TrackingNames.MockedTypes)
                .Collect()
                .Select(static (ids, _) => ids.Distinct().OrderBy(id => id.Id, StringComparer.Ordinal).ThenBy(id => id.Assembly, StringComparer.Ordinal).ToImmutableArray());

            var language = context.ParseOptionsProvider
                .Select(static (options, _) => options is CSharpParseOptions csharp ? (int)csharp.LanguageVersion : 0);

            var supported = types
                .Combine(language)
                .SelectMany(static (pair, _) => pair.Right >= ExtensionMembersVersion ? pair.Left : ImmutableArray<MockedType>.Empty)
                .WithTrackingName(TrackingNames.DistinctTypes);

            var setups = supported
                .Combine(context.CompilationProvider.Combine(language))
                .Select(static (pair, cancellation) => MockSetupRenderer.Render(pair.Right.Left, pair.Left, pair.Right.Right, cancellation))
                .Where(static setup => setup is not null)
                .Select(static (setup, _) => setup!)
                .WithTrackingName(TrackingNames.Setups);

            context.RegisterSourceOutput(setups, static (context, setup) => context.AddSource(setup.HintName, setup.Source));

            context.RegisterSourceOutput(
                types.Combine(language).Select(static (pair, _) => pair.Left.Length > 0 && pair.Right < ExtensionMembersVersion ? pair.Right : 0),
                static (context, version) =>
                {
                    if (version > 0)
                        context.ReportDiagnostic(Diagnostic.Create(LanguageVersionNotSupported, null, ((LanguageVersion)version).ToDisplayString()));
                });
        }

        static bool IsCandidate(SyntaxNode node) => node switch
        {
            InvocationExpressionSyntax { Expression: GenericNameSyntax or MemberAccessExpressionSyntax { Name: GenericNameSyntax } } => true,
            // Accessors like Mock.Get(instance) return mocks for inferred types.
            InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.ValueText: "Get" } or MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Get" } } => true,
            BaseObjectCreationExpressionSyntax => true,
            GenericNameSyntax { Identifier.ValueText: "IMock" or "Mock" } => true,
            _ => false,
        };

        static ImmutableArray<MockedType> GetMockedTypes(GeneratorSyntaxContext context, CancellationToken cancellation)
        {
            var symbol = context.SemanticModel.GetSymbolInfo(context.Node, cancellation).Symbol;
            var arguments = context.Node switch
            {
                InvocationExpressionSyntax when symbol is IMethodSymbol { TypeArguments.Length: > 0 } method &&
                    method.OriginalDefinition.GetAttributes().Any(IsMockGenerator) => method.TypeArguments,
                InvocationExpressionSyntax when symbol is IMethodSymbol { ReturnType: INamedTypeSymbol returned } && IsMockType(returned) => returned.TypeArguments,
                BaseObjectCreationExpressionSyntax when symbol is IMethodSymbol { MethodKind: MethodKind.Constructor, ContainingType.TypeArguments.Length: > 0 } ctor &&
                    (ctor.OriginalDefinition.GetAttributes().Any(IsMockGenerator) || HasMockGenerator(ctor.ContainingType)) => ctor.ContainingType.TypeArguments,
                GenericNameSyntax when symbol is INamedTypeSymbol type && IsMockType(type) => type.TypeArguments,
                _ => ImmutableArray<ITypeSymbol>.Empty,
            };

            if (arguments.IsEmpty)
                return ImmutableArray<MockedType>.Empty;

            return arguments
                .OfType<INamedTypeSymbol>()
                .Where(CanMock)
                .Select(type => new MockedType(DocumentationCommentId.CreateReferenceId(type), type.ContainingAssembly.Identity.GetDisplayName()))
                .ToImmutableArray();
        }

        static bool HasMockGenerator(INamedTypeSymbol? type)
        {
            for (; type != null; type = type.BaseType)
            {
                if (type.OriginalDefinition.GetAttributes().Any(IsMockGenerator))
                    return true;
            }

            return false;
        }

        static bool IsMockType(INamedTypeSymbol type) => type is
        {
            Name: "IMock" or "Mock",
            ContainingNamespace: { Name: "Moq", ContainingNamespace.IsGlobalNamespace: true },
            IsUnboundGenericType: false,
            TypeArguments.Length: > 0
        };

        static bool IsMockGenerator(AttributeData attribute) => attribute.AttributeClass is
        {
            Name: "MockGeneratorAttribute",
            ContainingNamespace: { Name: "Moq", ContainingNamespace.IsGlobalNamespace: true }
        };

        static bool CanMock(INamedTypeSymbol type)
            => (type.TypeKind is TypeKind.Interface or TypeKind.Delegate || (type.TypeKind == TypeKind.Class && !type.IsSealed && !type.IsStatic)) &&
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
