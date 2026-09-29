using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Moq.CodeFixes.UnitTests.Samples;

namespace Moq.CodeFixes.UnitTests
{
    public class SimplifySetupTests
    {
        [Theory]
        [InlineData("Setup(() => mock.Object.Add(1, 2)).Returns(3);", "mock.Add(1, 2).Returns(3);")]
        [InlineData("Setup(() => mock.Object.Add(1, 2));", "mock.Add(1, 2);")]
        [InlineData("Setup(() => mock.Object.Add(Any<int>(), 2)).Returns(() => 3).Once();", "mock.Add(Any<int>(), 2).Returns(() => 3).Once();")]
        [InlineData("Setup(() => mock.Object.Mode).Returns(3);", "mock.Mode.Returns(3);")]
        [InlineData("Setup(() => mock.Object[\"a\"]).Returns(3);", "mock.Item(\"a\").Returns(3);")]
        public async Task SimplifiesToTypedSetup(string setup, string simplified)
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test(setup));
            Assert.Empty(await document.GetErrorsAsync());

            var diagnostic = Assert.Single(await document.GetDiagnosticsAsync(new SimplifySetupAnalyzer()));
            Assert.Equal(MockDiagnostics.SimplifySetup.Id, diagnostic.Id);

            var fixedCode = await document.ApplyFixAsync(new SimplifySetupCodeFix(), diagnostic);

            Assert.Equal(Test(simplified), fixedCode);
        }

        [Theory]
        // Recursive mocks
        [InlineData("Setup(() => mock.Object.Parser.TryParse(\"\", out _)).Returns(true);")]
        // Handlers receiving the argument collection
        [InlineData("Setup(() => mock.Object.Add(1, 2)).Returns(args => 3);")]
        // Members hidden by the mock
        [InlineData("Setup(() => mock.Object.CallBase).Returns(true);")]
        [InlineData("Setup(() => mock.Object.As());")]
        // Members without typed setups
        [InlineData("Setup(() => mock.Object.GetRef()).Returns(1);")]
        // Setups whose value is consumed
        [InlineData("var setup = Setup(() => mock.Object.Add(1, 2));")]
        // Property setups need a verb to be a valid statement
        [InlineData("Setup(() => mock.Object.Mode);")]
        public async Task DoesNotReport(string setup)
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test(setup));
            Assert.Empty(await document.GetErrorsAsync());

            Assert.Empty(await document.GetDiagnosticsAsync(new SimplifySetupAnalyzer()));
        }

        [Fact]
        public async Task SimplifiesAllSetups()
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test(
                """
                Setup(() => mock.Object.Add(1, 2)).Returns(3);
                Setup(() => mock.Object.Mode).Returns(4);
                """));

            var diagnostics = await document.GetDiagnosticsAsync(new SimplifySetupAnalyzer());
            Assert.Equal(2, diagnostics.Length);

            var fixedCode = await document.ApplyFixAsync(new SimplifySetupCodeFix(), diagnostics[0]);
            document = document.WithText(Microsoft.CodeAnalysis.Text.SourceText.From(fixedCode));
            fixedCode = await document.ApplyFixAsync(new SimplifySetupCodeFix(), (await document.GetDiagnosticsAsync(new SimplifySetupAnalyzer())).Single());

            Assert.Equal(Test(
                """
                mock.Add(1, 2).Returns(3);
                mock.Mode.Returns(4);
                """), fixedCode);
        }
    }
}
