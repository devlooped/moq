using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Moq.StaticProxy.UnitTests
{
    /// <summary>
    /// Runs all the scenarios in the Scenarios folder using the source 
    /// generator to process them.
    /// </summary>
    public class Scenarios
    {
        static readonly HashSet<string> additionalSources = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "IRunnable.cs",
            "Mock.cs",
            "Mock.Overloads.cs",
            "Mock.StaticFactory.cs"
        };

        // Run particular scenarios using the TD.NET ad-hoc runner.
        // Uncomment attribute to use the VS runner.
        //[Fact]
        public void RunScenario() => new Scenarios().Run(ThisAssembly.Constants.Scenarios.MultipleUses);

        [Theory]
        [MemberData(nameof(GetScenarios))]
        public void Run(string path)
        {
            // Getting the generated output can be flaky sometimes, so retry
            var (diagnostics, compilation) = Retry(() => GetGeneratedOutput(
                Path.IsPathRooted(path) ? path :
                Path.Combine(ThisAssembly.Project.MSBuildProjectDirectory, path)));

            Assert.True(diagnostics.IsEmpty,
                "Generated code produced diagnostics:\r\n" +
                Environment.NewLine +
                string.Join(Environment.NewLine, diagnostics.Where(d => d.Id != "CS0436").Select(d => d.ToString())));

            var assembly = compilation.Emit();
            Assert.Contains(assembly.GetTypes(), t =>
                t.Namespace?.StartsWith(Sdk.MockNaming.DefaultRootNamespace, StringComparison.Ordinal) == true &&
                t.GetInterfaces().Any(i => i.FullName == typeof(Sdk.IMocked).FullName));

            var type = assembly.GetTypes().FirstOrDefault(t => t.GetInterfaces().Any(i => i.Name == nameof(IRunnable)));

            Assert.NotNull(type);

            try
            {
                var runnable = Activator.CreateInstance(type);
                type.InvokeMember("Run", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, runnable, null);
            }
            catch (TargetInvocationException tie)
            {
                ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
            }
        }

        T Retry<T>(Func<T> func, int times = 2)
        {
            for (var i = 1; i <= times; i++)
            {
                try
                {
                    return func();
                }
                catch (Exception) when (i < times)
                {
                    Thread.Sleep(500);
                    GC.Collect();
                }
            }

            return default;
        }

        public static IEnumerable<object[]> GetScenarios()
            => Directory.EnumerateFiles(Path.Combine(ThisAssembly.Project.MSBuildProjectDirectory, "Scenarios"), "*.cs")
                .Select(file => new object[] { Path.Combine("Scenarios", Path.GetFileName(file)) });

        static (ImmutableArray<Diagnostic>, Compilation) GetGeneratedOutput(string path)
        {
            var libs = new HashSet<string>(File.ReadAllLines("lib.txt"), StringComparer.OrdinalIgnoreCase)
                .Distinct(new FileNameComparer())
                .ToDictionary(x => Path.GetFileName(x), StringComparer.OrdinalIgnoreCase);

            var args = CSharpCommandLineParser.Default.Parse(
                File.ReadAllLines("csc.txt"), ThisAssembly.Project.MSBuildProjectDirectory, sdkDirectory: null);

            // net10 csc passes /features:InterceptorsNamespaces. The generator builds its
            // trees with default parse options, and Roslyn refuses to mix those features.
            var parseOptions = args.ParseOptions
                .WithLanguageVersion(LanguageVersion.Latest)
                .WithFeatures(Enumerable.Empty<KeyValuePair<string, string>>());

            var syntaxTree = CSharpSyntaxTree.ParseText(
                File.ReadAllText(path),
                options: parseOptions,
                path: new FileInfo(path).FullName,
                encoding: Encoding.UTF8);

            var sources = new List<SyntaxTree>
            {
                syntaxTree
            };

            foreach (var source in args.SourceFiles.Where(x => additionalSources.Contains(Path.GetFileName(x.Path))))
            {
                var filePath = source.Path;
                var fileName = filePath.StartsWith(ThisAssembly.Project.MSBuildProjectDirectory) ?
                    filePath.Substring(ThisAssembly.Project.MSBuildProjectDirectory.Length).TrimStart(Path.DirectorySeparatorChar) :
                    filePath;

                sources.Add(CSharpSyntaxTree.ParseText(
                    File.ReadAllText(filePath),
                    options: parseOptions,
                    path: filePath,
                    encoding: Encoding.UTF8));
            }

            foreach (var thisAssemblyFile in Directory.EnumerateFiles(
                Path.Combine(
                    ThisAssembly.Project.MSBuildProjectDirectory,
                    ThisAssembly.Project.IntermediateOutputPath,
                    "generated"),
                "ThisAssembly.*.cs",
                SearchOption.AllDirectories))
            {
                sources.Add(CSharpSyntaxTree.ParseText(
                    File.ReadAllText(thisAssemblyFile),
                    options: parseOptions,
                    path: thisAssemblyFile,
                    encoding: Encoding.UTF8));
            }

            Compilation compilation = CSharpCompilation.Create(
                Path.GetFileNameWithoutExtension(path),
                sources,
                args.MetadataReferences
                    .Select(x => libs.TryGetValue(Path.GetFileName(x.Reference), out var lib) ?
                        MetadataReference.CreateFromFile(lib) :
                        MetadataReference.CreateFromFile(x.Reference)),
                args.CompilationOptions.WithCryptoKeyFile(null).WithOutputKind(OutputKind.DynamicallyLinkedLibrary));

            Predicate<Diagnostic> ignored = d =>
                d.Severity == DiagnosticSeverity.Hidden ||
                d.Severity == DiagnosticSeverity.Info;

            var diagnostics = compilation.GetDiagnostics().RemoveAll(ignored);
            if (diagnostics.Any())
                return (diagnostics, compilation);

            var driver = CSharpGeneratorDriver.Create(
                new[] { new MockGenerator() },
                parseOptions: parseOptions,
                optionsProvider: EditorConfigOptionsProvider.Create(Directory.EnumerateFiles(
                    Path.Combine(ThisAssembly.Project.MSBuildProjectDirectory, ThisAssembly.Project.IntermediateOutputPath),
                    "*.editorconfig", SearchOption.TopDirectoryOnly)));

            // Don't timeout if we're debugging.
            var token = Debugger.IsAttached ? default : new CancellationTokenSource(5000).Token;

            driver.RunGeneratorsAndUpdateCompilation(compilation, out var output, out diagnostics, token);
            diagnostics = diagnostics.RemoveAll(ignored);

            return (diagnostics, output);
        }

        class FileNameComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y) => Path.GetFileName(x).Equals(Path.GetFileName(y), StringComparison.Ordinal);

            public int GetHashCode(string obj) => Path.GetFileName(obj).GetHashCode();
        }
    }
}
