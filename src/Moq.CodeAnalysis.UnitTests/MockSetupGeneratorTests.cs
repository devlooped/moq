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
        public void GeneratesNothingWithoutMockUsages()
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
                        Mock.Get(Mock.Of<ICalculator>(MockBehavior.Strict));
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
                HintNames(result));
        }

        [Fact]
        public void DiscoversMockedTypesFromMockReferencesAndCreations()
        {
            var result = new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                using Moq;

                public interface IParameter { void Run(); }
                public interface IField { void Run(); }
                public interface ICreated { void Run(); }
                public interface ITargetTyped { void Run(); }
                public interface IConstructor { void Run(); }
                public interface IDerived { void Run(); }
                public interface IGot { void Run(); }
                public interface IPrimary { void Run(); }
                public interface IAdditional { void Run(); }
                public interface INotMocked { void Run(); }

                [MockGenerator]
                public class Factory<T> where T : class { }

                public class DerivedFactory<T> : Factory<T> where T : class { }

                public class Constructor<T> where T : class
                {
                    [MockGenerator]
                    public Constructor() { }
                }

                public class Plain<T> { }

                class Usage
                {
                    Moq.IMock<IField>? field;

                    void Run(IMock<IParameter> mock, IGot got)
                    {
                        Mock.Get(got);
                        new Factory<ICreated>();
                        Factory<ITargetTyped> targetTyped = new();
                        new Constructor<IConstructor>();
                        new DerivedFactory<IDerived>();
                        new Plain<INotMocked>();
                        IMock<IPrimary> primary = new Mock<IPrimary, IAdditional>(MockBehavior.Strict);
                    }
                }
                """), out _);

            Assert.Equal(
                new[]
                {
                    "MockSetupExtensions.IAdditional.g.cs",
                    "MockSetupExtensions.IConstructor.g.cs",
                    "MockSetupExtensions.ICreated.g.cs",
                    "MockSetupExtensions.IDerived.g.cs",
                    "MockSetupExtensions.IField.g.cs",
                    "MockSetupExtensions.IGot.g.cs",
                    "MockSetupExtensions.IParameter.g.cs",
                    "MockSetupExtensions.IPrimary.g.cs",
                    "MockSetupExtensions.ITargetTyped.g.cs",
                },
                HintNames(result));
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
                    "IsOn",
                    "Item(string)",
                    "Mode",
                    "RaiseTurnedOn()",
                    "RaiseTurnedOn(System.EventArgs)",
                    "RaiseTurnedOn(object?, System.EventArgs)",
                    "Recall(string)",
                    "Store(string, int)",
                    "TryAdd(ref int, ref int, out int?)",
                    "TurnOn()",
                },
                Members(output, "Sample.ICalculator"));
        }

        [Fact]
        public void ExtensionsReturnTypedSetups()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(Calculator, Usage("Sample.ICalculator")), out var output);

            var block = Assert.Single(Blocks(output, "Sample.ICalculator"));
            string TypeOf(string name, int parameters = -1) => block.GetMembers(name)
                .Where(x => parameters == -1 || x is IMethodSymbol method && method.Parameters.Length == parameters)
                .Select(x => x is IMethodSymbol method ? method.ReturnType : ((IPropertySymbol)x).Type)
                .Single().ToDisplayString();

            Assert.Equal("Moq.ISetup<System.Func<int, int, int>, int>", TypeOf("Add", 2));
            Assert.Equal("Moq.ISetup<System.Action>", TypeOf("TurnOn"));
            Assert.Equal("Moq.ISetup<System.Action<string, int>>", TypeOf("Store"));
            Assert.Equal("Moq.ISetup<System.Func<string, int?>, int?>", TypeOf("Recall"));
            Assert.Equal("Moq.ISetup<Moq.MockSetupExtensions.Sample_ICalculator.TryAdd, bool>", TypeOf("TryAdd"));
            Assert.Equal("Moq.IPropertySetup<System.Func<bool>, bool>", TypeOf("IsOn"));
            Assert.Equal("Moq.IPropertySetup<System.Func<Sample.CalculatorMode>, System.Action<Sample.CalculatorMode>, Sample.CalculatorMode>", TypeOf("Mode"));
            Assert.Equal("Moq.IPropertySetup<System.Func<string, int?>, System.Action<string, int?>, int?>", TypeOf("Item"));
        }

        [Fact]
        public void InvokesTargetWithinSetupScope()
        {
            var result = new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(Calculator, Usage("Sample.ICalculator")), out _);
            var source = result.GeneratedTrees.Single().ToString();

            Assert.Contains(
                """
                            public global::Moq.ISetup<global::System.Func<int, int, int>, int> Add(int x, int y)
                                => global::Moq.Sdk.SetupFactory.Capture<global::System.Func<int, int, int>, int>(() => mock.Object.Add(x, y));
                """.Replace("\r\n", "\n"),
                source);

            Assert.Contains("static x => _ = x.Mode, static (x, value) => x.Mode = value);", source);
            Assert.Contains("x => _ = x[name], (x, value) => x[name] = value);", source);
            Assert.Contains("mock.Object.TryAdd(ref moq, ref moq2, out moq3), x, y, z);", source);
            Assert.Contains("public delegate bool TryAdd(ref int x, ref int y, out int? z);", source);
            Assert.Contains("global::Moq.Sdk.SetupFactory.GetEventHandler<global::System.EventHandler>(mock, \"TurnedOn\")?.Invoke(mock.Object, global::System.EventArgs.Empty);", source);
        }

        [Fact]
        public void ExtensionsCanBeUsedWithInferredHandlers()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(Calculator,
                """
                using Moq;
                using Sample;
                using static Moq.Syntax;

                static class Usage
                {
                    static void Run()
                    {
                        var calculator = Mock.Get(Mock.Of<ICalculator>());
                        int a = 1, b = 2;

                        calculator.Add(1, 2).Returns(3);
                        calculator.Add(Any<int>(), Any<int>()).Returns((x, y) => x + y).Once();
                        calculator.Add(1, 2, 3).Callback((x, y, z) => { }).Returns(() => 6);
                        calculator.TryAdd(ref a, ref b, out var c).Returns((ref x, ref y, out z) =>
                        {
                            z = x + y;
                            return true;
                        });
                        calculator.Mode.Returns(CalculatorMode.Scientific);
                        calculator.Mode.Get().Callback(() => { });
                        calculator.Mode.Set(CalculatorMode.Standard).Callback(mode => { });
                        calculator.IsOn.Returns(true);
                        calculator.Item("foo").Returns(5);
                        calculator.Item(Any<string>()).Set(5).Throws<System.InvalidOperationException>();
                        calculator.TurnOn().Callback(() => { }).Throws(new System.InvalidOperationException());
                        calculator.Store("a", 1).Callback((name, value) => { });
                        calculator.Recall("a").Returns((int?)null);
                        calculator.RaiseTurnedOn();
                        calculator.RaiseTurnedOn(System.EventArgs.Empty);
                        calculator.RaiseTurnedOn(null, System.EventArgs.Empty);
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
                    bool TryGet<T>(int id, out T? value) where T : class;
                }
                """,
                Usage("IRepository")), out var output);

            Assert.Equal(
                new[] { "Copy<T>(T[])", "Get<T>(int)", "Map<TSource, TResult>(TSource, System.Func<TSource, TResult>)", "Query<T>()", "Save<T>(T)", "TryGet<T>(int, out T?)" },
                Members(output, "IRepository"));
            Assert.Equal(
                "Moq.ISetup<Moq.MockSetupExtensions.IRepository.TryGet<T>, bool>",
                Assert.Single(Blocks(output, "IRepository")).GetMembers("TryGet").OfType<IMethodSymbol>().Single().ReturnType.ToDisplayString());
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
                    string this[int mock] { get; }
                }

                static class Calls
                {
                    static void Run(Moq.IMock<ILogger> logger)
                    {
                        logger.Log("started");
                        logger.Log("done", Level.Info, "app", 1, "a", "b");
                        logger.Register(1, 2, 3).Callback((mock, setup, value) => { });
                        logger.Item(1).Returns("one");
                    }
                }
                """), out var output);

            Assert.Empty(GeneratedDiagnostics(output));
        }

        [Fact]
        public void UsesCustomDelegatesForSignaturesNotRepresentableByFuncOrAction()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                using Moq;

                public interface IMath
                {
                    int Sum(in int x, in int y);
                    int Sum(ref int x);
                    void Many(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10, int a11, int a12, int a13, int a14, int a15, int a16, int a17);
                }

                static class Calls
                {
                    static void Run(Moq.IMock<IMath> math)
                    {
                        int a = 1;
                        math.Sum(in a, in a).Returns((in x, in y) => x + y);
                        math.Sum(ref a).Returns((ref x) => x);
                    }
                }
                """), out var output);

            var block = Assert.Single(Blocks(output, "IMath"));
            Assert.Equal(
                new[]
                {
                    "Moq.ISetup<Moq.MockSetupExtensions.IMath.Many>",
                    "Moq.ISetup<Moq.MockSetupExtensions.IMath.Sum, int>",
                    "Moq.ISetup<Moq.MockSetupExtensions.IMath.Sum2, int>",
                },
                block.GetMembers().OfType<IMethodSymbol>().Select(x => x.ReturnType.ToDisplayString()).OrderBy(x => x, StringComparer.Ordinal));
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

            Assert.Equal(new[] { "Run()", "Stop()", "Value" }, Members(output, "IC"));

            var source = result.GeneratedTrees.Single().ToString();
            Assert.Contains("((global::IA)mock.Object).Run())", source);
            Assert.Contains("((global::IB)mock.Object).Stop())", source);
            Assert.Contains("static x => _ = x.Value, static (x, value) => x.Value = value);", source);
        }

        [Fact]
        public void SkipsMembersWhoseNameIsTakenByAnotherKindOfMember()
        {
            var result = new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                using System;

                public interface IA { int Value { get; } }
                public interface IB { void Value(); }
                public interface IC : IA, IB
                {
                    event EventHandler Changed;
                    void RaiseChanged(string reason);
                }
                """,
                Usage("IC")), out var output);

            Assert.Equal(new[] { "RaiseChanged(string)", "Value" }, Members(output, "IC"));
        }

        [Fact]
        public void SupportsConstructedGenericTypes()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(Usage("System.Collections.Generic.IList<int>")), out var output);

            var members = Members(output, "System.Collections.Generic.IList<int>");

            Assert.Contains("Add(int)", members);
            Assert.Contains("Item(int)", members);
            Assert.Contains("Count", members);
            Assert.Contains("GetEnumerator()", members);
            Assert.Empty(GeneratedDiagnostics(output));
        }

        [Fact]
        public void SupportsDelegateTypes()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                using System;
                using Moq;

                public delegate bool TryParse(string value, out int result);

                static class Calls
                {
                    static void Run(IMock<Func<int, string>> format, IMock<TryParse> parse, IMock<Action> run)
                    {
                        format.Invoke(1).Returns("one");
                        parse.Invoke("1", out _).Returns((string value, out int result) =>
                        {
                            result = 1;
                            return true;
                        });
                        run.Invoke().Callback(() => { });
                    }
                }
                """), out var output);

            Assert.Equal("Moq.ISetup<System.Func<int, string>, string>", Assert.Single(Blocks(output, "System.Func<int, string>")).GetMembers("Invoke").OfType<IMethodSymbol>().Single().ReturnType.ToDisplayString());
            Assert.Equal("Moq.ISetup<TryParse, bool>", Assert.Single(Blocks(output, "TryParse")).GetMembers("Invoke").OfType<IMethodSymbol>().Single().ReturnType.ToDisplayString());
            Assert.Empty(GeneratedDiagnostics(output));
        }

        [Fact]
        public void RaisesEventsWithSenderOrMirroredParameters()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                using System;
                using System.ComponentModel;
                using Moq;

                public delegate void Progress(int percent, string message);

                public interface IWorker : INotifyPropertyChanged
                {
                    event Progress Progressed;
                    event EventHandler<int> Completed;
                    event Action<string> Logged;
                    event Func<int> Requested;
                }

                static class Calls
                {
                    static void Run(Moq.IMock<IWorker> worker)
                    {
                        worker.RaisePropertyChanged(new PropertyChangedEventArgs("Name"));
                        worker.RaiseProgressed(50, "half");
                        worker.RaiseCompleted(42);
                        worker.RaiseCompleted(worker, 42);
                        worker.RaiseLogged("hi");
                        worker.RaiseRequested();
                    }
                }
                """), out var output);

            Assert.Equal(
                new[]
                {
                    "RaiseCompleted(int)",
                    "RaiseCompleted(object?, int)",
                    "RaiseLogged(string)",
                    "RaiseProgressed(int, string)",
                    "RaisePropertyChanged(System.ComponentModel.PropertyChangedEventArgs)",
                    "RaisePropertyChanged(object?, System.ComponentModel.PropertyChangedEventArgs)",
                    "RaiseRequested()",
                },
                Members(output, "IWorker"));
            Assert.Empty(GeneratedDiagnostics(output));
        }

        [Fact]
        public void SkipsMembersConflictingWithMockApi()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                namespace Moq
                {
                    partial class Mock<T>
                    {
                        public void Reset() { }
                    }
                }

                public interface IConflicts
                {
                    object Object { get; }
                    void Behavior();
                    void CallBase(int count);
                    string ToString(int format);
                    int Sdk { get; }
                    void Reset();
                    void As();
                    void Run();
                }
                """,
                Usage("IConflicts")), out var output);

            Assert.Equal(new[] { "Run()" }, Members(output, "IConflicts"));
        }

        [Fact]
        public void SkipsMembersWithRefLikeTypesOrRefReturns()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(
                """
                using System;

                public interface IBuffer
                {
                    void Write(ReadOnlySpan<byte> data);
                    Span<byte> GetSpan();
                    ref int GetRef();
                    void Flush();
                }
                """,
                Usage("IBuffer")), out var output);

            Assert.Equal(new[] { "Flush()" }, Members(output, "IBuffer"));
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

            Assert.Equal(
                new[] { "Abstract()", "RaiseChanged()", "RaiseChanged(System.EventArgs)", "RaiseChanged(object?, System.EventArgs)", "Value", "Virtual()" },
                Members(output, "Derived"));
            Assert.Equal(
                "Moq.IPropertySetup<System.Func<int>, int>",
                Assert.Single(Blocks(output, "Derived")).GetMembers("Value").OfType<IPropertySymbol>().Single().Type.ToDisplayString());
            Assert.Empty(GeneratedDiagnostics(output));
        }

        [Fact]
        public void PrefersMostDerivedMockedType()
        {
            new MockSetupGenerator().Run(GeneratorTester.CreateCompilation(Calculator,
                """
                using Moq;
                using Sample;

                public class Calculator : ICalculator
                {
                    public virtual event System.EventHandler? TurnedOn;
                    public virtual bool IsOn { get; }
                    public virtual CalculatorMode Mode { get; set; }
                    public virtual int Add(int x, int y) => x + y;
                    public virtual int Add(int x, int y, int z) => x + y + z;
                    public virtual bool TryAdd(ref int x, ref int y, out int? z) { z = x + y; return true; }
                    public virtual void TurnOn() { }
                    public virtual int? this[string name] { get => null; set { } }
                    public virtual void Store(string name, int value) { }
                    public virtual int? Recall(string name) => null;
                    public virtual void Clear(string name) { }
                }

                static class Calls
                {
                    static void Run(IMock<ICalculator> calculator, IMock<Calculator> concrete)
                    {
                        calculator.Add(1, 2).Returns(3);
                        concrete.Add(1, 2).Returns(3);
                        concrete.Mode.Returns(CalculatorMode.Scientific);
                        concrete.Item("a").Returns(1);
                        concrete.RaiseTurnedOn();
                    }
                }
                """), out var output);

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

            Assert.Equal(new[] { "New()", "Old()" }, Members(output, "ILegacy"));
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
        public void RendersIndexersAsExtensionIndexersAfterCSharp14()
        {
            var compilation = GeneratorTester.CreateCompilation(LanguageVersion.Preview, Calculator, Usage("Sample.ICalculator"));

            var result = GeneratorTester.CreateDriver(GeneratorTester.GetParseOptions(compilation), new MockSetupGenerator())
                .RunGenerators(compilation).GetRunResult();

            var source = result.GeneratedTrees.Single().ToString();
            Assert.Contains("global::System.Action<string, int?>, int?> this[string name]", source);
            Assert.DoesNotContain(" Item(string name)", source);
        }

        [Fact]
        public void ReportsUnsupportedLanguageVersion()
        {
            var compilation = GeneratorTester.CreateCompilation(LanguageVersion.CSharp13, Calculator, Usage("Sample.ICalculator"));

            var result = GeneratorTester.CreateDriver(GeneratorTester.GetParseOptions(compilation), new MockSetupGenerator())
                .RunGenerators(compilation).GetRunResult();

            Assert.Empty(result.GeneratedTrees);
            var diagnostic = Assert.Single(result.Diagnostics);
            Assert.Equal(LanguageVersionNotSupported.Id, diagnostic.Id);
            Assert.Contains("13.0", diagnostic.GetMessage());
        }

        [Fact]
        public void DoesNotReportUnsupportedLanguageVersionWithoutMocks()
        {
            var compilation = GeneratorTester.CreateCompilation(LanguageVersion.CSharp13, Calculator);

            var result = GeneratorTester.CreateDriver(GeneratorTester.GetParseOptions(compilation), new MockSetupGenerator())
                .RunGenerators(compilation).GetRunResult();

            Assert.Empty(result.Diagnostics);
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
                static object Run() => Moq.Mock.Of<{{type}}>();
            }
            """;

        static string[] HintNames(GeneratorDriverRunResult result) => result.GeneratedTrees
            .Select(tree => System.IO.Path.GetFileName(tree.FilePath))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        static INamedTypeSymbol[] Blocks(Compilation output, string mockedType) =>
            output.GetTypeByMetadataName("Moq.MockSetupExtensions")?.GetTypeMembers()
                .Where(block => block.IsExtension &&
                    block.ExtensionParameter?.Type is INamedTypeSymbol { TypeArguments.Length: 1 } mock &&
                    mock.TypeArguments[0].ToDisplayString() == mockedType)
                .ToArray() ?? [];

        static string[] Members(Compilation output, string mockedType) => Blocks(output, mockedType)
            .SelectMany(block => block.GetMembers())
            .Select(member => member switch
            {
                IMethodSymbol { MethodKind: MethodKind.Ordinary } method =>
                    $"{method.Name}{TypeParameters(method)}({string.Join(", ", method.Parameters.Select(Parameter))})",
                IPropertySymbol { IsIndexer: true } indexer => $"this[{string.Join(", ", indexer.Parameters.Select(Parameter))}]",
                IPropertySymbol property => property.Name,
                _ => null,
            })
            .OfType<string>()
            .OrderBy(signature => signature, StringComparer.Ordinal)
            .ToArray();

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
