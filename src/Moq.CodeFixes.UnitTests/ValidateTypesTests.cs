using System.Threading.Tasks;
using Xunit;
using static Moq.CodeFixes.UnitTests.Samples;

namespace Moq.CodeFixes.UnitTests
{
    public class ValidateTypesTests
    {
        [Theory]
        [InlineData("Mock.Of<ICalculator, Calculator>();", "ST001")]
        [InlineData("new Mock<ICalculator, Calculator>();", "ST001")]
        [InlineData("new Mock<Sealed>();", "ST003")]
        [InlineData("new Custom<Sealed>();", "ST003")]
        public async Task ReportsInvalidMockedTypes(string code, string id)
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test(code) +
                """

                [MockGenerator]
                public class Custom<T> { }
                """);

            var diagnostic = Assert.Single(await document.GetDiagnosticsAsync(new MockValidateTypesAnalyzer()));

            Assert.Equal(id, diagnostic.Id);
        }

        [Theory]
        [InlineData("Mock.Of<Calculator, ICalculator>();")]
        [InlineData("new Mock<Calculator, ICalculator>();")]
        [InlineData("new Custom<Calculator>();")]
        public async Task DoesNotReportValidMockedTypes(string code)
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test(code) +
                """

                [MockGenerator]
                public class Custom<T> { }
                """);

            Assert.Empty(await document.GetDiagnosticsAsync(new MockValidateTypesAnalyzer()));
        }
    }
}
