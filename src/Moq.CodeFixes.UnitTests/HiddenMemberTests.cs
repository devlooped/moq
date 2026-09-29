using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Moq.CodeFixes.UnitTests.Samples;

namespace Moq.CodeFixes.UnitTests
{
    public class HiddenMemberTests
    {
        [Fact]
        public async Task ReportsMockMemberHidingMockedMember()
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test("var callBase = mock.CallBase;"));

            var diagnostic = Assert.Single(await document.GetDiagnosticsAsync(new HiddenMemberAnalyzer()));

            Assert.Equal(MockDiagnostics.HiddenMember.Id, diagnostic.Id);
            Assert.Equal("'CallBase' binds to the mock rather than setting up 'ICalculator.CallBase'", diagnostic.GetMessage());
        }

        [Theory]
        [InlineData("mock.CallBase = true;")]
        [InlineData("_ = mock.Object.CallBase;")]
        [InlineData("mock.Add(1, 2).Returns(3);")]
        [InlineData("_ = mock.Object;")]
        public async Task DoesNotReport(string code)
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test(code));
            Assert.Empty(await document.GetErrorsAsync());

            Assert.Empty(await document.GetDiagnosticsAsync(new HiddenMemberAnalyzer()));
        }

        [Fact]
        public async Task FixesHiddenMember()
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test("mock.CallBase.Returns(true);"));
            var diagnostic = Assert.Single(await document.GetDiagnosticsAsync(new HiddenMemberAnalyzer()));

            var fixedCode = await document.ApplyFixAsync(new HiddenMemberCodeFix(), diagnostic);

            Assert.Equal(Test("Setup(() => mock.Object.CallBase).Returns(true);"), fixedCode);
        }

        [Theory]
        // Hidden by Mock<T>.As<TInterface>()
        [InlineData("CS0411", "mock.As();", "Setup(() => mock.Object.As());")]
        // Skipped by the generator since it returns by reference
        [InlineData("CS1061", "mock.GetRef().Returns(1);", "Setup(() => mock.Object.GetRef()).Returns(1);")]
        // Hidden by the IMock<T>.CallBase property
        [InlineData("CS1929", "mock.CallBase.Returns(true);", "Setup(() => mock.Object.CallBase).Returns(true);")]
        public async Task FixesCompilerErrors(string id, string code, string expected)
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test(code));
            var error = (await document.GetErrorsAsync()).First(x => x.Id == id);

            var fixedCode = await document.ApplyFixAsync(new HiddenMemberCodeFix(), error);

            Assert.Equal(Test(expected), fixedCode);
        }

        [Fact]
        public async Task AddsSyntaxUsing()
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test("mock.As();", usings: "using Moq;"));
            var error = (await document.GetErrorsAsync()).First(x => x.Id == "CS0411");

            var fixedCode = await document.ApplyFixAsync(new HiddenMemberCodeFix(), error);

            Assert.Equal(Test("Setup(() => mock.Object.As());", usings: "using Moq;\nusing static Moq.Syntax;"), fixedCode);
        }

        [Fact]
        public async Task DoesNotFixErrorsInTypedSetups()
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test("mock.Add(1, \"2\");"));
            var error = (await document.GetErrorsAsync()).First(x => x.Id == "CS1503");

            await document.AssertNoFixAsync(new HiddenMemberCodeFix(), error);
        }
    }
}
