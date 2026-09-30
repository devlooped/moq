using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace Moq.CodeFixes.UnitTests
{
    public class UnsafeSignatureTests
    {
        [Fact]
        public async Task ReportsUnsafeMockGeneratorSignature()
        {
            var document = await CodeFixTester.CreateDocumentAsync(
                """
                using System;
                using Moq;

                public class Buffer
                {
                    public virtual int Sum(Span<int> data) => 0;
                }

                public class Tests
                {
                    public void Run() => Mock.Of<Buffer>();
                }
                """);

            var diagnostic = Assert.Single(await document.GetDiagnosticsAsync(new UnsafeSignatureAnalyzer()));

            Assert.Equal(MockDiagnostics.UnsafeSignature.Id, diagnostic.Id);
            Assert.Equal(
                "'Buffer.Sum' uses a ref struct or pointer and requires compile-time stunts and AllowUnsafeBlocks",
                diagnostic.GetMessage());
        }

        [Fact]
        public async Task DoesNotReportWhenBothPropertiesAreEnabled()
        {
            var document = await CodeFixTester.CreateDocumentAsync(
                """
                using System;
                using Moq;

                public class Buffer
                {
                    public virtual int Sum(Span<int> data) => 0;
                }

                public class Tests
                {
                    public void Run() => new Mock<Buffer>();
                }
                """);

            Assert.Empty(await document.GetDiagnosticsAsync(
                new UnsafeSignatureAnalyzer(),
                new Options(
                    ("build_property.EnableCompileTimeStunts", "true"),
                    ("build_property.AllowUnsafeBlocks", "true"))));
        }

        [Fact]
        public async Task DoesNotReportSafeSignatures()
        {
            var document = await CodeFixTester.CreateDocumentAsync(
                """
                using Moq;

                public interface ICounter
                {
                    int Next();
                }

                public class Tests
                {
                    public void Run() => Mock.Of<ICounter>();
                }
                """);

            Assert.Empty(await document.GetDiagnosticsAsync(new UnsafeSignatureAnalyzer()));
        }

        sealed class Options : AnalyzerConfigOptionsProvider
        {
            readonly AnalyzerConfigOptions global;

            public Options(params (string Key, string Value)[] values)
                => global = new Map(values);

            public override AnalyzerConfigOptions GlobalOptions => global;

            public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => Map.Empty;

            public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => Map.Empty;

            sealed class Map : AnalyzerConfigOptions
            {
                public static AnalyzerConfigOptions Empty { get; } = new Map();

                readonly Dictionary<string, string> values;

                public Map(params (string Key, string Value)[] values)
                    => this.values = values.ToDictionary(pair => pair.Key, pair => pair.Value);

                public override bool TryGetValue(string key, out string? value)
                    => values.TryGetValue(key, out value);
            }
        }
    }
}
