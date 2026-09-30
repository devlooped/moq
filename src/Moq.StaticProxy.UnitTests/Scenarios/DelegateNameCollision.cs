#pragma warning disable CS0436
using Moq.Sdk;
using Xunit;

namespace Moq.Scenarios.DelegateNameCollision
{
    public delegate int Callback(Callback callback, object pipeline);

    public interface ICallback
    {
        int Invoke(Callback implementation, object pipeline);
    }

    public class Base
    {
        public virtual int Invoke(Callback implementation, object pipeline) => implementation(implementation, pipeline);
    }

    public class Test : IRunnable
    {
        public void Run()
        {
            var interfaceMock = new Mock<ICallback>();
            Assert.Equal(0, interfaceMock.Object.Invoke((_, _) => 0, new object()));

            var classMock = new Mock<Base> { CallBase = true };
            var marker = new object();
            Assert.Equal(42, classMock.Object.Invoke((_, value) => ReferenceEquals(value, marker) ? 42 : 0, marker));
        }
    }
}
