using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Stunts.CodeAnalysis;

namespace Moq
{
    /// <summary>
    /// Analyzer that validates the used types for a mock generator method or constructor, 
    /// such as <c>Mock.Of&lt;T&gt;()</c> or <c>new Mock&lt;T, T1&gt;()</c>.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public class MockValidateTypesAnalyzer : DiagnosticAnalyzer
    {
        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
            StuntDiagnostics.BaseTypeNotFirst,
            StuntDiagnostics.DuplicateBaseType,
            StuntDiagnostics.SealedBaseType,
            StuntDiagnostics.EnumType);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationStartAction(start =>
            {
                if (start.Compilation.GetTypeByMetadataName(typeof(MockGeneratorAttribute).FullName!) is not { } generator)
                    return;

                start.RegisterOperationAction(context =>
                {
                    var invocation = (IInvocationOperation)context.Operation;
                    if (IsGenerator(invocation.TargetMethod, generator))
                        Validate(context, invocation.TargetMethod.TypeArguments);
                }, OperationKind.Invocation);

                start.RegisterOperationAction(context =>
                {
                    // For mock constructors, the mocked types are the type arguments of the created mock type.
                    var creation = (IObjectCreationOperation)context.Operation;
                    if (creation.Constructor is { } constructor && IsGenerator(constructor, generator))
                        Validate(context, constructor.ContainingType.TypeArguments);
                }, OperationKind.ObjectCreation);
            });
        }

        static bool IsGenerator(IMethodSymbol method, INamedTypeSymbol generator)
            => HasGenerator(method.OriginalDefinition, generator) ||
                (method.MethodKind == MethodKind.Constructor && HasGenerator(method.ContainingType.OriginalDefinition, generator));

        static bool HasGenerator(ISymbol symbol, INamedTypeSymbol generator)
            => symbol.GetAttributes().Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, generator));

        static void Validate(OperationAnalysisContext context, ImmutableArray<ITypeSymbol> types)
        {
            var location = context.Operation.Syntax.GetLocation();

            foreach (var type in types.Where(x => x.TypeKind == TypeKind.Enum))
                context.ReportDiagnostic(Diagnostic.Create(StuntDiagnostics.EnumType, location, type.Name));

            var classes = types.Where(x => x.TypeKind == TypeKind.Class).ToArray();
            if (classes.Length > 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(StuntDiagnostics.DuplicateBaseType, location));
            }
            else if (classes.Length == 1)
            {
                if (classes[0].IsSealed)
                    context.ReportDiagnostic(Diagnostic.Create(StuntDiagnostics.SealedBaseType, location, classes[0].Name));
                else if (types.IndexOf(classes[0]) != 0)
                    context.ReportDiagnostic(Diagnostic.Create(StuntDiagnostics.BaseTypeNotFirst, location, classes[0].Name));
            }
        }
    }
}
