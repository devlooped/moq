using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Moq.CodeAnalysis;
using Xunit;

namespace Moq.CodeFixes.UnitTests
{
    /// <summary>
    /// Creates C# documents that reference Moq and include the generated typed setups, 
    /// to run analyzers and code fixes over them.
    /// </summary>
    static class CodeFixTester
    {
        static readonly CSharpParseOptions parseOptions = new(LanguageVersion.Latest);

        static readonly MetadataReference[] references = GetFrameworkReferences()
            .Concat(new[] { typeof(Sdk.IMockRuntime).Assembly, typeof(IMock).Assembly, typeof(Stunts.IStunt).Assembly }.Select(x => x.Location))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToArray();

        static readonly string[] moqSources =
        [
            File.ReadAllText(Path.Combine("Moq", "Mock.cs")),
            File.ReadAllText(Path.Combine("Moq", "Mock.Overloads.cs")),
            File.ReadAllText(Path.Combine("Moq", "Mock`1.cs")),
            File.ReadAllText(Path.Combine("Moq", "Mock`1.Overloads.cs")),
        ];

        /// <summary>
        /// Creates the <c>Test0.cs</c> document with the given source, in a project that 
        /// also contains the Moq sources and the typed setups generated for them.
        /// </summary>
        public static async Task<Document> CreateDocumentAsync(string source)
        {
            var workspace = new AdhocWorkspace(WorkspaceServices.HostServices);
            var project = workspace.AddProject(ProjectInfo.Create(
                ProjectId.CreateNewId(), VersionStamp.Create(), "TestProject", "TestProject", LanguageNames.CSharp,
                compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable),
                parseOptions: parseOptions,
                metadataReferences: references));

            var document = project.AddDocument("Test0.cs", source);
            project = document.Project;
            for (var i = 0; i < moqSources.Length; i++)
                project = project.AddDocument($"Moq{i}.cs", moqSources[i]).Project;

            var compilation = await project.GetCompilationAsync() ?? throw new InvalidOperationException();
            CSharpGeneratorDriver
                .Create(new[] { new MockSetupGenerator().AsSourceGenerator() }, parseOptions: parseOptions)
                .RunGeneratorsAndUpdateCompilation(compilation, out var output, out _);

            foreach (var tree in output.SyntaxTrees.Except(compilation.SyntaxTrees))
                project = project.AddDocument(Path.GetFileName(tree.FilePath), await tree.GetTextAsync()).Project;

            return project.GetDocument(document.Id) ?? throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets the diagnostics reported by the analyzer in the given document, sorted by location.
        /// </summary>
        public static async Task<Diagnostic[]> GetDiagnosticsAsync(this Document document, DiagnosticAnalyzer analyzer)
        {
            var compilation = await document.Project.GetCompilationAsync() ?? throw new InvalidOperationException();
            var tree = await document.GetSyntaxTreeAsync();

            return (await compilation.WithAnalyzers(ImmutableArray.Create(analyzer)).GetAnalyzerDiagnosticsAsync())
                .Where(x => x.Location.SourceTree == tree)
                .OrderBy(x => x.Location.SourceSpan.Start)
                .ToArray();
        }

        /// <summary>
        /// Gets the compiler errors in the given document.
        /// </summary>
        public static async Task<Diagnostic[]> GetErrorsAsync(this Document document)
        {
            var compilation = await document.Project.GetCompilationAsync() ?? throw new InvalidOperationException();
            var tree = await document.GetSyntaxTreeAsync();

            return compilation.GetDiagnostics()
                .Where(x => x.Severity == DiagnosticSeverity.Error && x.Location.SourceTree == tree)
                .ToArray();
        }

        /// <summary>
        /// Applies the first code fix registered by the provider for the given diagnostic, 
        /// and asserts that the fixed document compiles.
        /// </summary>
        public static async Task<string> ApplyFixAsync(this Document document, CodeFixProvider provider, Diagnostic diagnostic)
        {
            var actions = new List<CodeAction>();
            await provider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None));
            Assert.NotEmpty(actions);

            var operation = (await actions[0].GetOperationsAsync(CancellationToken.None)).OfType<ApplyChangesOperation>().Single();
            var fixedDocument = operation.ChangedSolution.GetDocument(document.Id) ?? throw new InvalidOperationException();
            var text = (await fixedDocument.GetTextAsync()).ToString();

            Assert.True((await fixedDocument.GetErrorsAsync()).Length == 0,
                "Fixed code has errors:\n" + string.Join("\n", (await fixedDocument.GetErrorsAsync()).Select(x => x.ToString())) + "\n\n" + text);

            return text;
        }

        /// <summary>
        /// Asserts that the provider registers no code fix for the given diagnostic.
        /// </summary>
        public static async Task AssertNoFixAsync(this Document document, CodeFixProvider provider, Diagnostic diagnostic)
        {
            var actions = new List<CodeAction>();
            await provider.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None));
            Assert.Empty(actions);
        }

        static IEnumerable<string> GetFrameworkReferences()
        {
            if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string assemblies)
                return assemblies.Split([Path.PathSeparator], StringSplitOptions.RemoveEmptyEntries);

            var runtime = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            return new[] { "mscorlib.dll", "System.dll", "System.Core.dll", "System.Runtime.dll", "netstandard.dll" }
                .Select(file => Path.Combine(runtime, file))
                .Where(File.Exists)
                .Concat(new[] { typeof(System.Threading.Tasks.ValueTask<>).Assembly.Location, typeof(Span<>).Assembly.Location });
        }
    }
}
