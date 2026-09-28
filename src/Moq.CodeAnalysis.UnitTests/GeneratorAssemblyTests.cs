using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Moq.CodeAnalysis.UnitTests
{
    public class GeneratorAssemblyTests
    {
        static readonly Assembly assembly = typeof(MockSetupGenerator).Assembly;

        [Fact]
        public void AllGeneratorsAreIncremental()
        {
            var generators = assembly.GetTypes()
                .Where(type => type.GetCustomAttributes<GeneratorAttribute>().Any())
                .ToArray();

            Assert.NotEmpty(generators);
            Assert.All(generators, type =>
            {
                Assert.True(typeof(IIncrementalGenerator).IsAssignableFrom(type), $"{type} must implement {nameof(IIncrementalGenerator)}.");
                Assert.False(typeof(ISourceGenerator).IsAssignableFrom(type), $"{type} must not implement {nameof(ISourceGenerator)}.");
                Assert.Equal(LanguageNames.CSharp, Assert.Single(type.GetCustomAttribute<GeneratorAttribute>()!.Languages));
            });
        }

        [Fact]
        public void ReferencesOnlyCompilerProvidedAssemblies()
        {
            var roslyn = new[] { typeof(Compilation).Assembly, typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation).Assembly };
            var allowed = roslyn
                .SelectMany(compiler => compiler.GetReferencedAssemblies().Append(compiler.GetName()))
                .Select(name => name.Name)
                .Append("netstandard")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            Assert.All(assembly.GetReferencedAssemblies(), reference => Assert.Contains(reference.Name, allowed));
        }
    }
}
