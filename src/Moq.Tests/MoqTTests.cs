using System;
using System.Linq;
using Moq.Sdk;
using Sample;
using Stunts;
using Xunit;
using static Moq.Syntax;

namespace Moq.Tests
{
    /// <summary>
    /// Showcases the <see cref="IMock{T}"/>-based setup API, where each member of 
    /// the mocked type is available as a generated extension method on the 
    /// <see cref="IMock{T}"/> returned by <c>Mock.Of2</c>, which returns the 
    /// resulting <see cref="IMockSetup"/>.
    /// </summary>
    public class MoqTTests
    {
        [Fact]
        public void ReturnsMockIntrospection()
        {
            var calculator = Mock.Of2<ICalculator>();

            Assert.IsAssignableFrom<IMock<ICalculator>>(calculator);
            Assert.IsAssignableFrom<ICalculator>(calculator.Object);
            Assert.Same(calculator.Object, calculator.Object.AsMock().Object);
        }

        [Fact]
        public void ImplementsAdditionalInterfaces()
        {
            var calculator = Mock.Of2<ICalculator, IDisposable>();

            Assert.IsAssignableFrom<IDisposable>(calculator.Object);
        }

        [Fact]
        public void SetupDoesNotRecordInvocation()
        {
            var calculator = Mock.Of2<ICalculator>();

            calculator.TurnOn();
            calculator.Add(2, 3);
            calculator.Mode();

            Assert.Empty(calculator.Invocations);
        }

        [Fact]
        public void SetupMatchesInvocationArguments()
        {
            var calculator = Mock.Of2<ICalculator>();

            var setup = calculator.Add(2, 3);

            Assert.Equal(nameof(ICalculator.Add), setup.Invocation.MethodBase.Name);
            Assert.True(setup.AppliesTo(Invocation(calculator, c => c.Add(2, 3))));
            Assert.False(setup.AppliesTo(Invocation(calculator, c => c.Add(3, 2))));
        }

        [Fact]
        public void SetupUsesArgumentMatchers()
        {
            var calculator = Mock.Of2<ICalculator>();

            var setup = calculator.Add(Any<int>(), Any<int>(i => i > 5));

            Assert.True(setup.AppliesTo(Invocation(calculator, c => c.Add(1, 10))));
            Assert.False(setup.AppliesTo(Invocation(calculator, c => c.Add(1, 2))));
        }

        [Fact]
        public void SetupDistinguishesOverloads()
        {
            var calculator = Mock.Of2<ICalculator>();

            var setup = calculator.Add(1, 2, 3);

            Assert.Equal(3, setup.Invocation.Arguments.Count);
            Assert.False(setup.AppliesTo(Invocation(calculator, c => c.Add(1, 2))));
        }

        [Fact]
        public void CanConfigureReturnValueOnSetup()
        {
            var calculator = Mock.Of2<ICalculator>();

            calculator.Returns(calculator.Add(2, 3), 5);

            Assert.Equal(5, calculator.Object.Add(2, 3));
            Assert.Equal(0, calculator.Object.Add(1, 1));
        }

        [Fact]
        public void CanConfigureReturnValueWithMatchers()
        {
            var calculator = Mock.Of2<ICalculator>();

            calculator.Returns(calculator.Add(Any<int>(), Any<int>()), 42);

            Assert.Equal(42, calculator.Object.Add(1, 2));
            Assert.Equal(42, calculator.Object.Add(3, 4));
        }

        [Fact]
        public void CanSetupPropertyGetter()
        {
            var calculator = Mock.Of2<ICalculator>();

            calculator.Returns(calculator.Mode(), CalculatorMode.Scientific);

            Assert.Equal(CalculatorMode.Scientific, calculator.Object.Mode);
        }

        [Fact]
        public void CanSetupPropertySetter()
        {
            var calculator = Mock.Of2<ICalculator>();
            var called = false;

            var setup = calculator.Mode(CalculatorMode.Scientific);
            calculator.GetPipeline(setup).Behaviors.Add(new AnonymousMockBehavior((m, i, next) =>
            {
                called = true;
                return i.CreateValueReturn(null, i.Arguments);
            }, "Callback"));

            calculator.Object.Mode = CalculatorMode.Standard;
            Assert.False(called);

            calculator.Object.Mode = CalculatorMode.Scientific;
            Assert.True(called);
        }

        [Fact]
        public void CanSetupIndexer()
        {
            var calculator = Mock.Of2<ICalculator>();

            calculator.Returns(calculator.Item("x"), 10);

            Assert.Equal(10, calculator.Object["x"]);
            Assert.Null(calculator.Object["y"]);
        }

        [Fact]
        public void CanSetupRefOutMethod()
        {
            var calculator = Mock.Of2<ICalculator>();
            int x = 5, y = 10;

            var setup = calculator.TryAdd(ref x, ref y, out _);
            calculator.GetPipeline(setup).Behaviors.Add(new AnonymousMockBehavior((m, i, next) =>
            {
                i.Arguments.Set(2, (int?)(i.Arguments.Get<int>(0) + i.Arguments.Get<int>(1)));
                return i.CreateValueReturn(true, i.Arguments);
            }, "TryAdd"));

            Assert.True(calculator.Object.TryAdd(ref x, ref y, out var z));
            Assert.Equal(15, z);
        }

        [Fact]
        public void CanVerifyInvocationsAgainstSetup()
        {
            var calculator = Mock.Of2<ICalculator>();

            calculator.Object.Store("a", 1);
            calculator.Object.Store("b", 2);
            calculator.Object.Store("a", 3);

            var setup = calculator.Store("a", Any<int>());

            Assert.Equal(2, calculator.Invocations.Count(setup.AppliesTo));
        }

        [Fact]
        public void CanSetupStrictMockWithinSetupScope()
        {
            var calculator = Mock.Of2<ICalculator>(MockBehavior.Strict);

            using (Setup())
            {
                calculator.Returns(calculator.Add(2, 3), 5);
            }

            Assert.Equal(5, calculator.Object.Add(2, 3));
            Assert.Throws<StrictMockException>(() => calculator.Object.Add(1, 1));
        }

        [Fact]
        public void CanSetupClassMock()
        {
            var calculator = Mock.Of2<Calculator>();

            calculator.Returns(calculator.Add(2, 3), 42);
            Assert.Empty(calculator.Invocations);

            Assert.Equal(42, calculator.Object.Add(2, 3));
            Assert.Single(calculator.Invocations);
        }

        static IMethodInvocation Invocation(IMock<ICalculator> mock, Action<ICalculator> action)
        {
            action(mock.Object);
            return mock.Invocations.Last();
        }
    }

    static class MockSetupReturns
    {
        /// <summary>
        /// Makes the given <paramref name="setup"/> return the given <paramref name="value"/>.
        /// </summary>
        public static void Returns<T>(this IMock<T> mock, IMockSetup setup, object? value) where T : class
            => mock.GetPipeline(setup).Behaviors.Add(new AnonymousMockBehavior(
                (m, i, next) => i.CreateValueReturn(value, i.Arguments), "Returns"));
    }
}
