using System;
using System.Reflection;
using Stunts;
using Xunit;

namespace Moq.Sdk.Tests
{
    public class MockExtensionsTests
    {
        [Fact]
        public void MockAsT()
        {
            var target = new FakeMock();
            var mock = target.Mock;
            var generic = target.AsMock();

            Assert.Equal(generic.Behaviors, mock.Behaviors);
            Assert.Same(generic.Object, mock.Object);
            Assert.Same(((IMock)generic).Object, mock.Object);
            Assert.Same(generic.Invocations, mock.Invocations);
            Assert.Equal(generic.Setups, mock.Setups);
            Assert.Same(generic.State, mock.State);
        }

        [Fact]
        public void ThrowsArgumentExceptionForNonMocked()
            => Assert.Throws<ArgumentException>(() => new object().AsMock());

        [Fact]
        public void ThrowsArgumentExceptionForNull()
            => Assert.Throws<ArgumentException>(() => default(object).AsMock());

        [Fact]
        public void CanAssertInvocations()
        {
            var target = new FakeCalls();
            target.AddBehavior(new DefaultValueBehavior());

            target.TurnOn();
            Assert.Single(target.AsMock().InvocationsFor(c => c.TurnOn()));

            Assert.Equal(0, target.Add(2, 3));
            Assert.Single(target.AsMock().InvocationsFor(c => c.Add(2, 3)));
        }

        class FakeCalls : FakeMock
        {
            public void TurnOn() => Pipeline.Execute(MethodInvocation.Create(this, MethodBase.GetCurrentMethod()));

            public int Add(int x, int y) => Pipeline.Execute<int>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), x, y));
        }
    }
}
