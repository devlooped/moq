using System;
using System.Collections.Immutable;
using System.Linq;
using Devlooped;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Moq;

[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic, LanguageNames.FSharp)]
class SponsorLinker : SponsorLink
{
    static readonly SponsorLinkSettings settings;

    static SponsorLinker()
    {
        settings = SponsorLinkSettings.Create("devlooped", "Moq",
            version: new Version(ThisAssembly.Info.Version).ToString(3),
            diagnosticsIdPrefix: "MOQ"
#if DEBUG
            , quietDays: 0
#endif
            );

        settings.SupportedDiagnostics = settings.SupportedDiagnostics
            .Select(x => x.IsKind(DiagnosticKind.UserNotSponsoring) ?
                x.With(messageFormat: Properties.Resources.UserNotSponsoring_Message) :
                x)
            .Select(x => x.IsKind(DiagnosticKind.Thanks) ?
                x.With(messageFormat: Properties.Resources.Thanks_Message) :
                x)
            .ToImmutableArray();
    }

    public SponsorLinker() : base(settings) { }
}