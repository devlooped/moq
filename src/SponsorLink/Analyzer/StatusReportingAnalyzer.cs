using System.Collections.Immutable;
using Devlooped.Sponsors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Devlooped.Sponsors.SponsorLink;
using static ThisAssembly.Constants;

namespace Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class StatusReportingAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray<DiagnosticDescriptor>.Empty;

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterCodeBlockAction(c =>
        {
            var status = Diagnostics.GetStatus(Funding.Product);
            Tracing.Trace($"Status: {status}");
        });
    }
}