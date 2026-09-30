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

        public static DiagnosticDescriptor HiddenMember { get; } = new DiagnosticDescriptor(
            "MOQ002",
            ThisAssembly.Strings.HiddenMember.Title,
            Resources.HiddenMember_Message,
            "Usage",
            DiagnosticSeverity.Info,
            true,
            ThisAssembly.Strings.HiddenMember.Description);

        public static DiagnosticDescriptor UnsafeSignature { get; } = new DiagnosticDescriptor(
            "MOQ003",
            "Ref structs and pointers require compile-time stunts",
            "'{0}.{1}' uses a ref struct or pointer and requires compile-time stunts and AllowUnsafeBlocks",
            "Build",
            DiagnosticSeverity.Error,
            true,
            "Proxying ref structs and pointers generates unsafe code. EnableCompileTimeStunts and AllowUnsafeBlocks must both be true, or that code does not compile.");
    }
}
