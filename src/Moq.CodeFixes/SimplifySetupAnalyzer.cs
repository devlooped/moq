using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Moq.CodeFixes
{
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public class SimplifySetupAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(MockDiagnostics.SimplifySetup);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterOperationAction(AnalyzeOperation, OperationKind.Invocation);
        }

        void AnalyzeOperation(OperationAnalysisContext context)
        {
            var invocation = (IInvocationOperation)context.Operation;
            // We're looking for the Setup<T>(T mock) generic extension method call
            if (!invocation.TargetMethod.IsGenericMethod ||
                !invocation.TargetMethod.IsExtensionMethod)
                return;

            // Get the matching symbol for the given generator attribute from the current compilation.
            var setupType = context.Compilation.GetTypeByMetadataName("Moq.SetupExtension");
            if (setupType == null)
                return;

            var setupMethod = setupType.GetMembers().OfType<IMethodSymbol>().First(x => x.Name == "Setup" && x.Parameters.Length == 1);

            if (SymbolEqualityComparer.Default.Equals(invocation.TargetMethod.OriginalDefinition, setupMethod))
                context.ReportDiagnostic(Diagnostic.Create(
                    MockDiagnostics.SimplifySetup,
                    invocation.Syntax.GetLocation()));
        }
    }
}
