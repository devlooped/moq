#pragma warning disable CS0436
using Xunit;
using static Moq.Syntax;

namespace Moq.Scenarios.RecursiveSetupMethods
{
    public class Test : IRunnable
    {
        public void Run()
        {
            var mock = Mock.Of<IFoo>();

            Setup(() => mock.GetBar().GetBaz().Name).Returns(() => "hi");

            Assert.Equal("hi", mock.GetBar().GetBaz().Name);
        }
    }

    public interface IFoo
    {
        IBar GetBar();
    }

    public interface IBar
    {
        IBaz GetBaz();
    }

    public interface IBaz
    {
        string Name { get; }
    }
}