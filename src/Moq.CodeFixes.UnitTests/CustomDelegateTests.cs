using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Moq.CodeFixes.UnitTests.Samples;

namespace Moq.CodeFixes.UnitTests
{
    /// <summary>
    /// Single method groups with ref/out parameters have a natural delegate type, so the custom 
    /// delegate is only needed when the type argument can't be inferred from overloaded methods.
    /// </summary>
    public class CustomDelegateTests
    {
        [Fact]
        public async Task GeneratesDelegateForSetupRef()
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test("SetupRef(mock.Object.TryParse);"));
            var error = (await document.GetErrorsAsync()).First(x => x.Id == "CS0411");

            var fixedCode = await document.ApplyFixAsync(new CustomDelegateCodeFix(), error);

            Assert.Contains("SetupRef<TryParse>(mock.Object.TryParse)", fixedCode);
            Assert.Contains(".Returns((string input, out int value) => throw null);", fixedCode);
            Assert.Contains("delegate bool TryParse(string input, out int value);", fixedCode);
        }

        [Fact]
        public async Task WrapsRecursiveMockInLambda()
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test("SetupRef(mock.Object.Parser.TryParse).Returns((string input, out int value) => { value = 1; return true; });"));
            var error = (await document.GetErrorsAsync()).First(x => x.Id == "CS0411");

            var fixedCode = await document.ApplyFixAsync(new CustomDelegateCodeFix(), error);

            Assert.Contains("SetupRef<TryParse>(() => mock.Object.Parser.TryParse).Returns(", fixedCode);
        }

        [Fact]
        public async Task ReusesExistingDelegate()
        {
            var document = await CodeFixTester.CreateDocumentAsync(Test("SetupRef(mock.Object.TryParse);")
                .Replace("public class Tests\n{", "public class Tests\n{\n    delegate bool TryParse(string input, out int value);\n"));
            Assert.Contains("delegate bool TryParse(", (await document.GetTextAsync()).ToString());
            var error = (await document.GetErrorsAsync()).First(x => x.Id == "CS0411");

            var fixedCode = await document.ApplyFixAsync(new CustomDelegateCodeFix(), error);

            Assert.Contains("SetupRef<TryParse>(mock.Object.TryParse)", fixedCode);
            Assert.Single(fixedCode.Split('\n'), line => line.Contains("delegate bool TryParse("));
        }
    }
}
