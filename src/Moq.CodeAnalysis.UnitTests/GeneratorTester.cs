using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Moq.CodeAnalysis.UnitTests
{
    static class GeneratorTester
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
        /// Creates a compilation of the given sources that references Moq.
        /// </summary>
        public static CSharpCompilation CreateCompilation(params string[] sources) => CreateCompilation(LanguageVersion.Latest, sources);

        /// <summary>
        /// Creates a compilation of the given sources that references Moq, using the given language version.
        /// </summary>
        public static CSharpCompilation CreateCompilation(LanguageVersion version, params string[] sources) => CSharpCompilation.Create(
            "TestProject",
            sources.Concat(moqSources).Select((source, index) => CSharpSyntaxTree.ParseText(source, parseOptions.WithLanguageVersion(version), path: $"Test{index}.cs")),
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        public static SyntaxTree ParseText(string source, string path) => CSharpSyntaxTree.ParseText(source, parseOptions, path: path);

        public static GeneratorDriver CreateDriver(params IIncrementalGenerator[] generators) => CreateDriver(parseOptions, generators);

        public static GeneratorDriver CreateDriver(CSharpParseOptions options, params IIncrementalGenerator[] generators) => CSharpGeneratorDriver.Create(
            generators.Select(GeneratorExtensions.AsSourceGenerator),
            parseOptions: options,
            driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true));

        /// <summary>
        /// Runs the generator and asserts that neither the generator nor the
        /// resulting compilation report errors.
        /// </summary>
        public static GeneratorDriverRunResult Run(this IIncrementalGenerator generator, Compilation compilation, out Compilation output)
        {
            var driver = CreateDriver(GetParseOptions(compilation), generator).RunGeneratorsAndUpdateCompilation(compilation, out output, out var diagnostics);

            Assert.Empty(diagnostics);
            Assert.Empty(output.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error));

            return driver.GetRunResult();
        }

        /// <summary>
        /// Runs the generator twice over equivalent compilations and asserts that the
        /// second run is served entirely from the incremental cache, and that the
        /// given tracked steps never hold on to compilation-bound objects.
        /// </summary>
        public static void AssertIncremental(this IIncrementalGenerator generator, Compilation compilation, params string[] trackingNames)
            => generator.AssertIncremental(compilation, compilation.Clone(), trackingNames);

        /// <summary>
        /// Runs the generator over <paramref name="compilation"/> and then over <paramref name="next"/>, 
        /// asserting that the second run is served entirely from the incremental cache, and that the
        /// given tracked steps never hold on to compilation-bound objects.
        /// </summary>
        public static void AssertIncremental(this IIncrementalGenerator generator, Compilation compilation, Compilation next, params string[] trackingNames)
        {
            var driver = CreateDriver(GetParseOptions(compilation), generator).RunGenerators(compilation);
            var first = driver.GetRunResult();
            var second = driver.RunGenerators(next).GetRunResult();

            foreach (var result in first.Results)
            {
                Assert.Null(result.Exception);
                foreach (var name in trackingNames)
                {
                    Assert.True(result.TrackedSteps.TryGetValue(name, out var steps), $"Step '{name}' was not tracked.");
                    foreach (var output in steps.SelectMany(step => step.Outputs))
                        AssertCacheable(output.Value, name);
                }
            }

            foreach (var result in second.Results)
            {
                Assert.Null(result.Exception);

                var steps = result.TrackedOutputSteps
                    .Concat(result.TrackedSteps.Where(step => trackingNames.Contains(step.Key)));

                foreach (var step in steps)
                    foreach (var (_, reason) in step.Value.SelectMany(run => run.Outputs))
                        Assert.True(
                            reason is IncrementalStepRunReason.Cached or IncrementalStepRunReason.Unchanged,
                            $"Step '{step.Key}' was {reason} on an equivalent compilation.");
            }
        }

        /// <summary>
        /// Gets the parse options of the given compilation, so generated sources match its language version.
        /// </summary>
        public static CSharpParseOptions GetParseOptions(Compilation compilation)
            => compilation.SyntaxTrees.FirstOrDefault()?.Options as CSharpParseOptions ?? parseOptions;

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

        static void AssertCacheable(object? value, string step)
        {
            var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
            Visit(value);

            void Visit(object? node)
            {
                if (node is null || !visited.Add(node))
                    return;

                Assert.False(
                    node is Compilation or SemanticModel or ISymbol or SyntaxNode or SyntaxTree,
                    $"Step '{step}' produced a {node.GetType().Name}, which roots the compilation and defeats caching.");

                var type = node.GetType();
                if (type.IsPrimitive || type.IsEnum || node is string)
                    return;

                if (node is IEnumerable items)
                {
                    foreach (var item in items)
                        Visit(item);
                    return;
                }

                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    Visit(field.GetValue(node));
            }
        }

        sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public static ReferenceEqualityComparer Instance { get; } = new();

            public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

            public int GetHashCode(object obj) => System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
        }
    }
}
