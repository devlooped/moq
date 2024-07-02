using Devlooped.Sponsors;
using Microsoft.CodeAnalysis;
using static Devlooped.Sponsors.SponsorLink;

namespace Analyzer;

[Generator]
public class StatusReportingGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(
            context.GetSponsorManifests(),
            (spc, source) =>
            {
                var status = Diagnostics.GetOrSetStatus(source);
                spc.AddSource("StatusReporting.cs", $"// Status: {status}");
            });
    }
}
