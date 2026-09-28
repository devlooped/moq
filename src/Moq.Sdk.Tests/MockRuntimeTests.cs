using System;
using System.Reflection;
using Stunts;
using Xunit;

namespace Moq.Sdk.Tests
{
    public class MockRuntimeTests
    {
        [Fact]
        public void GetReturnsMockedRuntime()
        {
            var target = new FakeMock();

            Assert.Same(target.Runtime, MockRuntime.Get(target));
            Assert.Same(target, MockRuntime.Get(target).Object);
        }

        [Fact]
        public void TryGetReturnsFalseForNonMocked()
            => Assert.False(MockRuntime.TryGet(new object(), out _));

        [Fact]
        public void ThrowsArgumentExceptionForNonMocked()
            => Assert.Throws<ArgumentException>(() => MockRuntime.Get(new object()));

        [Fact]
        public void ThrowsArgumentExceptionForNull()
            => Assert.Throws<ArgumentException>(() => MockRuntime.Get(default!));

        [Fact]
        public void CanAssertInvocations()
        {
            var target = new FakeCalls();
            target.AddBehavior(new DefaultValueBehavior());

            target.TurnOn();
            Assert.Single(MockRuntime.InvocationsFor(() => target.TurnOn()));

            Assert.Equal(0, target.Add(2, 3));
            Assert.Single(MockRuntime.InvocationsFor(() => target.Add(2, 3)));
        }

        class FakeCalls : FakeMock
        {
            public void TurnOn() => Pipeline.Execute(MethodInvocation.Create(this, MethodBase.GetCurrentMethod()));

            public int Add(int x, int y) => Pipeline.Execute<int>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), x, y));
        }
    }
}
