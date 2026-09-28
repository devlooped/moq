using System.IO;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Moq.CodeFixes.UnitTests.MOQ001
{
    public class SimplifySetupTests : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer? GetCSharpDiagnosticAnalyzer() => new SimplifySetupAnalyzer();

        protected override CodeFixProvider? GetCSharpCodeFixProvider() => new SimplifySetupCodeFix();

        [Theory(Skip = "MOQ001 is being rewritten for the typed setups API.")]
        [InlineData(ThisAssembly.Constants.MOQ001.SingleDiagnostic, 11, 20)]
        public void VerifySingleDiagnostic(string path, int line, int column)
        {
            var expected = new DiagnosticResult
            {
                Id = MockDiagnostics.SimplifySetup.Id,
                Severity = MockDiagnostics.SimplifySetup.DefaultSeverity,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(
                new[]
                {
                    File.ReadAllText(path),
                    File.ReadAllText(@"Moq/Mock.cs"),
                    File.ReadAllText(@"Moq/Mock.Overloads.cs"),
                },
                expected);
        }

        [Theory(Skip = "MOQ001 is being rewritten for the typed setups API.")]
        [InlineData(ThisAssembly.Constants.MOQ001.SingleDiagnostic, ThisAssembly.Constants.MOQ001.SingleDiagnosticFixed)]
        public void VerifySingleDiagnosticFix(string source, string output)
        {
            VerifyCSharpFix(
                new[]
                {
                    File.ReadAllText(source),
                    File.ReadAllText(@"Moq/Mock.cs"),
                    File.ReadAllText(@"Moq/Mock.Overloads.cs"),
                },
                File.ReadAllText(output).Replace(".Fixed", ""));
        }

        [Theory(Skip = "MOQ001 is being rewritten for the typed setups API.")]
        [InlineData(ThisAssembly.Constants.MOQ001.MultipleDiagnosticSameMethod, ThisAssembly.Constants.MOQ001.MultipleDiagnosticSameMethodFixed)]
        public void VerifyMultipleDiagnosticSameMethod(string source, string output)
        {
            VerifyCSharpFix(
                new[]
                {
                    File.ReadAllText(source),
                    File.ReadAllText(@"Moq/Mock.cs"),
                    File.ReadAllText(@"Moq/Mock.Overloads.cs"),
                },
                File.ReadAllText(output).Replace(".Fixed", ""));
        }

        [Theory(Skip = "MOQ001 is being rewritten for the typed setups API.")]
        [InlineData(ThisAssembly.Constants.MOQ001.MultipleDiagnosticDifferentMethod, ThisAssembly.Constants.MOQ001.MultipleDiagnosticDifferentMethodFixed)]
        public void VerifyMultipleDiagnosticDifferentMethod(string source, string output)
        {
            VerifyCSharpFix(
                new[]
                {
                    File.ReadAllText(source),
                    File.ReadAllText(@"Moq/Mock.cs"),
                    File.ReadAllText(@"Moq/Mock.Overloads.cs"),
                },
                File.ReadAllText(output).Replace(".Fixed", ""));
        }

        [Theory(Skip = "MOQ001 is being rewritten for the typed setups API.")]
        [InlineData(ThisAssembly.Constants.MOQ001.MultipleDiagnosticExistingUsing, ThisAssembly.Constants.MOQ001.MultipleDiagnosticExistingUsingFixed)]
        public void VerifyMultipleDiagnosticExistingUsing(string source, string output)
        {
            VerifyCSharpFix(
                new[]
                {
                    File.ReadAllText(source),
                    File.ReadAllText(@"Moq/Mock.cs"),
                    File.ReadAllText(@"Moq/Mock.Overloads.cs"),
                },
                File.ReadAllText(output).Replace(".Fixed", ""));
        }
    }
}
