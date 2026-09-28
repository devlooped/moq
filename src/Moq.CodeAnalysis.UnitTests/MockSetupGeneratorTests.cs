using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using static Moq.CodeAnalysis.MockSetupGenerator;

namespace Moq.CodeAnalysis.UnitTests
{
    public class MockSetupGeneratorTests
    {
        const string Calculator =
            """
            using System;

            namespace Sample
            {
                public enum CalculatorMode { Standard, Scientific }

                public interface ICalculator
                {
                    event EventHandler TurnedOn;
                    bool IsOn { get; }
                    CalculatorMode Mode { get; set; }
                    int Add(int x, int y);
                    int Add(int x, int y, int z);
                    bool TryAdd(ref int x, ref int y, out int? z);
                    void TurnOn();
                    int? this[string name] { get; set; }
                    void Store(string name, int value);
                    int? Recall(string name);
                    void Clear(string name);
                }
            }
            """;

        [Fact]
        public void GeneratesNothingWithoutMockGeneratorUsages()
        {
            var result = new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(Calculator), out _);

            Assert.Empty(result.GeneratedTrees);
        }

        [Fact]
        public void GeneratesNothingWithoutMoqSdk()
        {
            var compilation = CSharpCompilation.Create("NoMoq",
                [GeneratorTester.ParseText(
                    """
                    namespace Moq
                    {
                        [System.AttributeUsage(System.AttributeTargets.Method)]
                        public class MockGeneratorAttribute : System.Attribute { }

                        public static class Factory
                        {
                            [MockGenerator]
                            public static T Of<T>() where T : class => null!;
                        }
                    }

                    public interface IFoo { void Do(); }

                    public static class Usage
                    {
                        public static IFoo Create() => Moq.Factory.Of<IFoo>();
                    }
                    """, "NoMoq.cs")],
                [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var result = GeneratorTester.CreateDriver(new MockSetupGenerator()).RunGenerators(compilation).GetRunResult();

            Assert.Empty(result.Diagnostics);
            Assert.Empty(result.GeneratedTrees);
        }

        [Fact]
        public void GeneratesOncePerDistinctMockedType()
        {
            var result = new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(Calculator,
                """
                using System;
                using Moq;
                using Sample;

                static class Usage
                {
                    static void Run()
                    {
                        Mock.Of<ICalculator>();
                        Mock.Of<ICalculator, IDisposable>();
                        Mock.Of2<ICalculator>(MockBehavior.Strict);
                        Mock.Of2<IDisposable>();
                        Mock.Of<IServiceProvider>();
                    }
                }
                """), out _);

            Assert.Equal(
                new[]
                {
                    "MockSetupExtensions.Sample.ICalculator.g.cs",
                    "MockSetupExtensions.System.IDisposable.g.cs",
                    "MockSetupExtensions.System.IServiceProvider.g.cs",
                },
                result.GeneratedTrees.Select(tree => System.IO.Path.GetFileName(tree.FilePath)).OrderBy(name => name, StringComparer.Ordinal));
        }

        [Fact]
        public void GeneratesExtensionForEveryInstanceMember()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(Calculator, Usage("Sample.ICalculator")), out var output);

            Assert.Equal(
                new[]
                {
                    "Add(int, int)",
                    "Add(int, int, int)",
                    "Clear(string)",
                    "IsOn()",
                    "Item(string)",
                    "Item(string, int?)",
                    "Mode()",
                    "Mode(Sample.CalculatorMode)",
                    "Recall(string)",
                    "Store(string, int)",
                    "TryAdd(ref int, ref int, out int?)",
                    "TurnOn()",
                },
                Extensions(output, "Sample.ICalculator").OrderBy(signature => signature, StringComparer.Ordinal));
        }

        [Fact]
        public void ExtensionsReturnMockSetup()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(Calculator, Usage("Sample.ICalculator")), out var output);

            var extensions = output.GetTypeByMetadataName("Moq.MockSetupExtensions")!.GetMembers().OfType<IMethodSymbol>().ToArray();

            Assert.NotEmpty(extensions);
            Assert.All(extensions, method =>
            {
                Assert.True(method.IsExtensionMethod);
                Assert.Equal("Moq.Sdk.IMockSetup", method.ReturnType.ToDisplayString());
                Assert.Equal("Moq.IMock<Sample.ICalculator>", method.Parameters[0].Type.ToDisplayString());
            });
        }

        [Fact]
        public void InvokesTargetThenReturnsCurrentSetupWithoutItsInvocation()
        {
            var result = new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(Calculator, Usage("Sample.ICalculator")), out _);
            var source = result.GeneratedTrees.Single().ToString();

            Assert.Contains(
                """
                        public static global::Moq.Sdk.IMockSetup Add(this global::Moq.IMock<global::Sample.ICalculator> mock, int x, int y)
                        {
                            global::Moq.Sdk.MockContext.CurrentSetup = null;
                            mock.Object.Add(x, y);
                            var setup = global::Moq.Sdk.MockContext.CurrentSetup ?? global::Moq.Sdk.CallContext.ThrowUnexpectedNull<global::Moq.Sdk.IMockSetup>();
                            global::Moq.Sdk.MockRuntime.Get(setup.Invocation.Target).Invocations.Remove(setup.Invocation);
                            return setup;
                        }
                """.Replace("\r\n", "\n"),
                source);

            Assert.Contains("_ = mock.Object.Mode;", source);
            Assert.Contains("mock.Object.Mode = value;", source);
            Assert.Contains("_ = mock.Object[name];", source);
            Assert.Contains("mock.Object[name] = value;", source);
            Assert.Contains("mock.Object.TryAdd(ref x, ref y, out z);", source);
        }

        [Fact]
        public void ExtensionsCanBeInvokedOnMock()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(Calculator,
                """
                using Moq;
                using Moq.Sdk;
                using Sample;

                static class Usage
                {
                    static void Run()
                    {
                        var calculator = Mock.Of2<ICalculator>();
                        int x = 1, y = 2;

                        IMockSetup setup = calculator.Add(1, 2);
                        setup = calculator.Add(1, 2, 3);
                        setup = calculator.TryAdd(ref x, ref y, out var z);
                        setup = calculator.Mode();
                        setup = calculator.Mode(CalculatorMode.Scientific);
                        setup = calculator.Item("foo");
                        setup = calculator.Item("foo", 5);
                        setup = calculator.TurnOn();
                        setup = Mock.Get(Mock.Of<ICalculator>()).Recall("foo");
                    }
                }
                """), out var output);

            Assert.Empty(GeneratedDiagnostics(output));
        }

        [Fact]
        public void SupportsGenericMethodsWithConstraints()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                using System;
                using System.Collections.Generic;

                public interface IRepository
                {
                    T Get<T>(int id) where T : class, new();
                    void Save<T>(T item) where T : struct, IComparable<T>;
                    TResult Map<TSource, TResult>(TSource source, Func<TSource, TResult> map) where TSource : notnull where TResult : TSource?;
                    void Copy<T>(T[] items) where T : unmanaged;
                    IEnumerable<T> Query<T>() where T : class?;
                }
                """,
                Usage("IRepository")), out var output);

            Assert.Equal(
                new[] { "Copy<T>(T[])", "Get<T>(int)", "Map<TSource, TResult>(TSource, System.Func<TSource, TResult>)", "Query<T>()", "Save<T>(T)" },
                Extensions(output, "IRepository").OrderBy(signature => signature, StringComparer.Ordinal));
            Assert.Empty(GeneratedDiagnostics(output));
        }

        [Fact]
        public void SupportsDefaultValuesParamsAndKeywordNames()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                using Moq;

                public enum Level { Info = 1, Error = -1 }

                public interface ILogger
                {
                    void Log(string @event, Level level = Level.Error, string? category = null, double weight = 0.5, params object[] args);
                    void Register(object mock, object setup, object value);
                }

                static class Calls
                {
                    static void Run(Moq.IMock<ILogger> logger)
                    {
                        logger.Log("started");
                        logger.Log("done", Level.Info, "app", 1, "a", "b");
                        logger.Register(1, 2, 3);
                    }
                }
                """,
                Usage("ILogger")), out var output);

            Assert.Empty(GeneratedDiagnostics(output));
        }

        [Fact]
        public void DeduplicatesInheritedMembersAndCastsToDeclaringType()
        {
            var result = new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                public interface IA { void Run(); int Value { get; } }
                public interface IB { void Run(); void Stop(); }
                public interface IC : IA, IB { new int Value { get; set; } }
                """,
                Usage("IC")), out var output);

            Assert.Equal(new[] { "Run()", "Stop()", "Value()", "Value(int)" }, Extensions(output, "IC").OrderBy(signature => signature, StringComparer.Ordinal));

            var source = result.GeneratedTrees.Single().ToString();
            Assert.Contains("((global::IA)mock.Object).Run();", source);
            Assert.Contains("((global::IB)mock.Object).Stop();", source);
            Assert.Contains("_ = mock.Object.Value;", source);
        }

        [Fact]
        public void SupportsConstructedGenericTypes()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(Usage("System.Collections.Generic.IList<int>")), out var output);

            var extensions = Extensions(output, "System.Collections.Generic.IList<int>");

            Assert.Contains("Add(int)", extensions);
            Assert.Contains("Item(int)", extensions);
            Assert.Contains("Item(int, int)", extensions);
            Assert.Contains("GetEnumerator()", extensions);
            Assert.Empty(GeneratedDiagnostics(output));
        }

        [Fact]
        public void SkipsMembersConflictingWithMockApi()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                public interface IConflicts
                {
                    object Object { get; }
                    void Behavior();
                    void CallBase(int count);
                    string ToString(int format);
                    void Run();
                }
                """,
                Usage("IConflicts")), out var output);

            Assert.Equal(new[] { "Run()" }, Extensions(output, "IConflicts"));
        }

        [Fact]
        public void IncludesOnlyOverridableAccessibleClassMembers()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                public abstract class Base
                {
                    public abstract int Abstract();
                    public virtual void Virtual() { }
                    public virtual void Sealed() { }
                    public virtual int Value { get; protected set; }
                    protected virtual void Protected() { }
                    public void NonVirtual() { }
                    public static void Static() { }
                }

                public class Derived : Base
                {
                    public override int Abstract() => 0;
                    public sealed override void Sealed() { }
                    public virtual event System.EventHandler? Changed;
                    public override string ToString() => "";
                }
                """,
                Usage("Derived")), out var output);

            Assert.Equal(new[] { "Abstract()", "Value()", "Virtual()" }, Extensions(output, "Derived").OrderBy(signature => signature, StringComparer.Ordinal));
            Assert.Empty(GeneratedDiagnostics(output));
        }

        [Fact]
        public void PropagatesObsoleteAndSkipsObsoleteErrors()
        {
            var result = new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                public interface ILegacy
                {
                    [System.Obsolete("Use New")] void Old();
                    [System.Obsolete("Gone", true)] void Removed();
                    void New();
                }
                """,
                Usage("ILegacy")), out var output);

            Assert.Equal(new[] { "New()", "Old()" }, Extensions(output, "ILegacy").OrderBy(signature => signature, StringComparer.Ordinal));
            Assert.Contains("[global::System.Obsolete(\"Use New\")]", result.GeneratedTrees.Single().ToString());
            Assert.Empty(GeneratedDiagnostics(output));
        }

        [Fact]
        public void SkipsInaccessibleOpenAndUnmockableTypes()
        {
            var result = new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                using Moq;

                public sealed class Sealed { public void Run() { } }

                public class Outer
                {
                    private interface IPrivate { void Run(); }

                    static void Run<T>() where T : class
                    {
                        Mock.Of<IPrivate>();
                        Mock.Of<T>();
                        Mock.Of<Sealed>();
                        Mock.Of<System.Collections.Generic.IList<T>>();
                    }
                }
                """), out _);

            Assert.Empty(result.GeneratedTrees);
        }

        [Fact]
        public void UnchangedCompilationIsCached()
            => new MockSetupGenerator().AssertIncremental(
                GeneratorTester.CreateCompilation(Calculator, Usage("Sample.ICalculator")),
                TrackingNames.MockedTypes, TrackingNames.DistinctTypes, TrackingNames.Setups);

        [Fact]
        public void UnrelatedChangeIsCached()
        {
            var compilation = GeneratorTester.CreateCompilation(Calculator, Usage("Sample.ICalculator"));

            new MockSetupGenerator().AssertIncremental(
                compilation,
                compilation.AddSyntaxTrees(GeneratorTester.ParseText("public class Unrelated { public void Run() { } }", "Unrelated.cs")),
                TrackingNames.DistinctTypes, TrackingNames.Setups);
        }

        [Fact]
        public void AdditionalUsageOfSameTypeIsCached()
        {
            var compilation = GeneratorTester.CreateCompilation(Calculator, Usage("Sample.ICalculator"));

            new MockSetupGenerator().AssertIncremental(
                compilation,
                compilation.AddSyntaxTrees(GeneratorTester.ParseText(Usage("Sample.ICalculator", "More"), "More.cs")),
                TrackingNames.DistinctTypes, TrackingNames.Setups);
        }

        static string Usage(string type, string name = "Usage") =>
            $$"""
            static class {{name}}
            {
                static object Run() => Moq.Mock.Of2<{{type}}>();
            }
            """;

        static string[] Extensions(Compilation output, string mockedType) =>
            output.GetTypeByMetadataName("Moq.MockSetupExtensions")?.GetMembers().OfType<IMethodSymbol>()
                .Where(method => method.Parameters[0].Type is INamedTypeSymbol { TypeArguments.Length: 1 } mock &&
                    mock.TypeArguments[0].ToDisplayString() == mockedType)
                .Select(method => $"{method.Name}{TypeParameters(method)}({string.Join(", ", method.Parameters.Skip(1).Select(Parameter))})")
                .ToArray() ?? [];

        static string TypeParameters(IMethodSymbol method) => method.TypeParameters.Length == 0 ? "" :
            $"<{string.Join(", ", method.TypeParameters.Select(parameter => parameter.Name))}>";

        static string Parameter(IParameterSymbol parameter) => parameter.RefKind switch
        {
            RefKind.Ref => "ref ",
            RefKind.Out => "out ",
            RefKind.In => "in ",
            _ => "",
        } + parameter.Type.ToDisplayString();

        static Diagnostic[] GeneratedDiagnostics(Compilation output) => output.GetDiagnostics()
            .Where(diagnostic => diagnostic.Location.SourceTree?.FilePath.EndsWith(".g.cs") == true)
            .ToArray();
    }
}
