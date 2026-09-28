using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;
using Xunit.Sdk;

namespace Moq.CodeAnalysis.UnitTests
{
    public class GeneratorTesterTests
    {
        const string Source = "interface IFoo { }";

        [Fact]
        public void FailsWhenStepRootsSyntax()
            => Assert.ThrowsAny<XunitException>(() => new SyntaxRootingGenerator()
                .AssertIncremental(GeneratorTester.CreateCompilation(Source), SyntaxRootingGenerator.Step));

        [Fact]
        public void FailsWhenStepIsNotCached()
            => Assert.ThrowsAny<XunitException>(() => new UncachedGenerator()
                .AssertIncremental(GeneratorTester.CreateCompilation(Source), UncachedGenerator.Step));

        [Fact]
        public void FailsWhenStepIsNotTracked()
            => Assert.ThrowsAny<XunitException>(() => new MockSetupGenerator()
                .AssertIncremental(GeneratorTester.CreateCompilation(Source), "Missing"));

        class SyntaxRootingGenerator : IIncrementalGenerator
        {
            public const string Step = "Interfaces";

            public void Initialize(IncrementalGeneratorInitializationContext context)
            {
                var interfaces = context.SyntaxProvider
                    .CreateSyntaxProvider((node, _) => node is InterfaceDeclarationSyntax, (ctx, _) => ctx.Node)
                    .WithTrackingName(Step);

                context.RegisterSourceOutput(interfaces, (spc, _) => spc.AddSource("Interfaces.g.cs", "// interfaces"));
            }
        }

        class UncachedGenerator : IIncrementalGenerator
        {
            public const string Step = "Compilation";

            public void Initialize(IncrementalGeneratorInitializationContext context)
            {
                var state = context.CompilationProvider
                    .Select((_, _) => new object())
                    .WithTrackingName(Step);

                context.RegisterSourceOutput(state, (spc, _) => spc.AddSource("State.g.cs", "// state"));
            }
        }
    }
}
