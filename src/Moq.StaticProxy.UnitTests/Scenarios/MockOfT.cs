#pragma warning disable CS0436
using System;
using Moq.Sdk;
using Xunit;
using static Moq.Syntax;

namespace Moq.Scenarios.MockOfT
{
    public interface IRoot
    {
        IChild Child { get; }
    }

    public interface IChild
    {
        int Value { get; }
    }

    /// <summary>
    /// Creating a Mock{T, T1} generates the mock with the additional 
    /// interfaces, as well as recursive mocks set up through it.
    /// </summary>
    public class Test : IRunnable
    {
        public void Run()
        {
            var mock = new Mock<IRoot, IDisposable>();

            Assert.StartsWith(MockNaming.DefaultRootNamespace, mock.Object.GetType().Namespace, StringComparison.Ordinal);
            Assert.IsAssignableFrom<IDisposable>(mock.Object);
            Assert.Same(mock.Object, mock.As<IDisposable>().Object);

            Setup(() => mock.Object.Child.Value).Returns(42);

            Assert.Equal(42, mock.Object.Child.Value);
            Assert.StartsWith(MockNaming.DefaultRootNamespace, mock.Object.Child.GetType().Namespace, StringComparison.Ordinal);
        }
    }
}
