#pragma warning disable CS0436
using System;
using Moq.Sdk;
using Xunit;
using static Moq.Syntax;

namespace Moq.Scenarios.MockGet
{
    public interface IRoot
    {
        IDisposable Child { get; }
    }

    /// <summary>
    /// Mock.Get returns the Mock{T} for a statically generated mock, 
    /// which also generates recursive mocks.
    /// </summary>
    public class Test : IRunnable
    {
        public void Run()
        {
            var mock = Mock.Get(Mock.Of<IRoot>());

            Assert.NotNull(mock);
            Assert.IsAssignableFrom<IRoot>(mock.Object);
            Assert.StartsWith(MockNaming.DefaultRootNamespace, mock.Object.GetType().Namespace, StringComparison.Ordinal);

            using (Setup())
            {
                _ = mock.Object.Child;
            }

            var child = mock.Object.Child;

            Assert.NotNull(child);
            Assert.StartsWith(MockNaming.DefaultRootNamespace, child.GetType().Namespace, StringComparison.Ordinal);
        }
    }
}
