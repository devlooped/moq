using System;
using System.Reflection;
using Sample;
using Stunts;
using Xunit;

namespace Moq.Sdk.Tests
{
    public class MockBehaviorTests
    {
        [Fact]
        public void CreatesBehaviorWithNullDisplayName()
            => Assert.Equal("<unnamed>", new AnonymousMockBehavior((m, i, n) => n().Invoke(m, i, n), default(string)).ToString());

        [Fact]
        public void CreatesBehaviorWithDisplayName()
            => Assert.Equal("test", new AnonymousMockBehavior((m, i, n) => n().Invoke(m, i, n), "test").ToString());

        [Fact]
        public void CreatesBehaviorWithLazyDisplayName()
            => Assert.Equal("test", new AnonymousMockBehavior((m, i, n) => n().Invoke(m, i, n), new Lazy<string>(() => "test")).ToString());

        [Fact]
        public void ExecutesAnonymousBehavior()
        {
            var called = false;
            var behavior = new AnonymousMockBehavior((m, i, n) => { called = true; return i.CreateReturn(); }, "test");
            var mock = new FakeMock();

            behavior.Execute(mock.Runtime, new MethodInvocation(mock, typeof(object).GetMethod(nameof(object.ToString))), () => null);

            Assert.True(called);
        }

        [Fact]
        public void RecordsInvocation()
        {
            var behavior = new MockRecordingBehavior();
            var mock = new Mocked();

            behavior.Execute(new MethodInvocation(mock, typeof(object).GetMethod(nameof(object.ToString))),
                (m, n) => m.CreateReturn());

            Assert.Equal(1, mock.Runtime.Invocations.Count);
        }

        [Fact]
        public void ThrowsForNonIMocked()
        {
            var behavior = new MockRecordingBehavior();

            Assert.Throws<ArgumentException>(() => behavior.Execute(new MethodInvocation(
                new object(),
                typeof(Mocked).GetProperty(nameof(IMocked.Runtime)).GetGetMethod()),
                (m, n) => m.CreateReturn()));
        }

        [Fact]
        public void WhenAddingMockBehavior_ThenCanInterceptSelectively()
        {
            var calculator = new SelectiveCalculator();

            // TODO: this is not adding a mock behavior but a regular stunt behavior
            calculator.AddBehavior((m, n) => m.CreateValueReturn(CalculatorMode.Scientific), m => m.MethodBase.Name == "get_Mode");
            calculator.AddBehavior(new DefaultValueBehavior());
            calculator.AddBehavior(new DefaultEqualityBehavior());

            var mode = calculator.Mode;
            var add = calculator.Add(3, 2);

            Assert.Equal(CalculatorMode.Scientific, mode);
            Assert.Equal(0, add);
        }

        class SelectiveCalculator : FakeMock
        {
            public CalculatorMode Mode => Pipeline.Execute<CalculatorMode>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod()));

            public int Add(int x, int y) => Pipeline.Execute<int>(MethodInvocation.Create(this, MethodBase.GetCurrentMethod(), x, y));
        }
    }
}