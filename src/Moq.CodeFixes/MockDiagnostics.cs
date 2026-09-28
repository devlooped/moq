using Microsoft.CodeAnalysis;

namespace Moq.CodeFixes
{
    public static class MockDiagnostics
    {
        public static DiagnosticDescriptor SimplifySetup { get; } = new DiagnosticDescriptor(
            "MOQ001",
            ThisAssembly.Strings.SimplifySetup.Title,
            Resources.SimplifySetup_Message,
            "Style",
            DiagnosticSeverity.Info,
            true,
            ThisAssembly.Strings.SimplifySetup.Description);
    }
}
