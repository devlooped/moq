using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Moq.Sdk;
using Sample;
using Stunts;
using Xunit;
using Xunit.Abstractions;
using static Moq.Syntax;

namespace Moq.Tests
{
    using static Moq.Syntax;

    public class MoqTests
    {
        readonly ITestOutputHelper output;

        public MoqTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void SetupDoesNotRecordCalls()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Setup(c => c.TurnOn());

            Assert.Empty(calculator.AsMock().Invocations);
        }

        [Fact]
        public void CanRaiseEvents()
        {
            var calculator = Mock.Of<ICalculator>().Named("calculator");

            var raised = false;

            EventHandler handler = (sender, args) => raised = true;
            calculator.TurnedOn += handler;

            Assert.Equal(1, calculator.AsMock().Invocations.Count);
            calculator.TurnedOn += Raise();
            // Raising events should not increase invocation count.
            Assert.Equal(1, calculator.AsMock().Invocations.Count);

            Assert.True(raised);

            raised = false;
            calculator.TurnedOn -= handler;
            calculator.TurnedOn -= handler;

            calculator.TurnedOn += Raise();

            Assert.False(raised);
        }

        [Fact]
        public void CanRaiseEventsWithArgs()
        {
            var mock = Mock.Of<INotifyPropertyChanged>();
            mock.InsertBehavior(0, new EventBehavior());

            var property = "";
            mock.PropertyChanged += (sender, args) => property = args.PropertyName;

            mock.PropertyChanged += Raise<PropertyChangedEventHandler>(new PropertyChangedEventArgs("Mode"));

            Assert.Equal("Mode", property);
        }

        [Fact]
        public void CanSetupPropertyViaReturns()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Mode.Returns(CalculatorMode.Standard);

            var mode = calculator.Mode;

            Assert.Equal(CalculatorMode.Standard, mode);
        }

        [Fact]
        public void CanSetupPropertyDirectly()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Mode = CalculatorMode.Scientific;

            var mode = calculator.Mode;

            Assert.Equal(CalculatorMode.Scientific, mode);
        }

        [Fact]
        public void CanSetupPropertyTwiceViaReturns()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Mode.Returns(CalculatorMode.Standard);
            calculator.Mode.Returns(CalculatorMode.Scientific);

            var mode = calculator.Mode;

            Assert.Equal(CalculatorMode.Scientific, mode);
        }

        [Fact]
        public void CanSetupMethodWithArgumentsViaReturns()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Add(2, 3).Returns(5);

            var result = calculator.Add(2, 3);

            Assert.Equal(5, result);
        }

        [Fact]
        public void CanSetupMethodWithDifferentArgumentsViaReturns()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Add(2, 2).Returns(4);
            calculator.Add(2, 3).Returns(5);

            calculator.Add(10, Any<int>()).Returns(10);
            calculator.Add(Any<int>(i => i > 20), Any<int>()).Returns(20);

            Assert.Equal(5, calculator.Add(2, 3));
            Assert.Equal(4, calculator.Add(2, 2));
            Assert.Equal(10, calculator.Add(10, 2));
            Assert.Equal(20, calculator.Add(25, 20));
        }

        [Fact]
        public void CanReturnFunction()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Add(2, 2).Returns(() => 4);

            Assert.Equal(4, calculator.Add(2, 2));
        }

        public interface IAsync
        {
            Task<bool> RunAsync(int arg);
            Task RunVoidAsync();
            ValueTask<bool> RunValueAsync(int arg);
            ValueTask RunVoidValueAsync();
        }

        [Fact]
        public async Task CanReturnAsyncFunction()
        {
            var mock = Mock.Of<IAsync>();

            mock.RunAsync(5).ReturnsAsync(true);

            Assert.True(await mock.RunAsync(5));
        }

        [Fact]
        public async Task CanReturnAsyncValueFunction()
        {
            var mock = Mock.Of<IAsync>();

            mock.RunValueAsync(5).ReturnsAsync(true);

            Assert.True(await mock.RunValueAsync(5));
        }

        [Fact]
        public async Task ThrowsAsync()
        {
            var mock = Mock.Of<IAsync>();

            mock.RunAsync(5).Throws<InvalidOperationException>();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.RunAsync(5));
        }

        [Fact]
        public async Task ThrowsAsyncWithException()
        {
            var mock = Mock.Of<IAsync>();

            mock.RunAsync(5).Throws(new InvalidOperationException());

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.RunAsync(5));
        }

        [Fact]
        public async Task ThrowsValueAsync()
        {
            var mock = Mock.Of<IAsync>();

            mock.RunValueAsync(5).Throws<InvalidOperationException>();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.RunValueAsync(5));
        }

        [Fact]
        public async Task ThrowsValueAsyncWithException()
        {
            var mock = Mock.Of<IAsync>();

            mock.RunValueAsync(5).Throws(new InvalidOperationException());

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.RunValueAsync(5));
        }

        [Fact]
        public async Task ThrowsVoidAsync()
        {
            var mock = Mock.Of<IAsync>();

            mock.RunVoidAsync().Throws<InvalidOperationException>();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.RunVoidAsync());
        }

        [Fact]
        public async Task ThrowsVoidAsyncWithException()
        {
            var mock = Mock.Of<IAsync>();

            mock.RunVoidAsync().Throws(new InvalidOperationException());

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.RunVoidAsync());
        }

        [Fact]
        public async Task ThrowsValueVoidAsync()
        {
            var mock = Mock.Of<IAsync>();

            mock.RunVoidValueAsync().Throws<InvalidOperationException>();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.RunVoidValueAsync());
        }

        [Fact]
        public async Task ThrowsValueVoidAsyncWithException()
        {
            var mock = Mock.Of<IAsync>();

            mock.RunVoidValueAsync().Throws(new InvalidOperationException());

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.RunVoidValueAsync());
        }

        [Fact]
        public void CanReturnFunctionWithArgs()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Add(Any<int>(), Any<int>()).Returns((int x, int y) => x + y);

            Assert.Equal(4, calculator.Add(2, 2));
            Assert.Equal(5, calculator.Add(2, 3));
        }

        [Fact]
        public void CanSetupMethodWithDifferentArgumentsViaTypedReturns()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator
                .Add(Any<int>(), Any<int>())
                .Returns<int, int, int>((x, y) => x + y);

            Assert.Equal(5, calculator.Add(2, 3));
            Assert.Equal(4, calculator.Add(2, 2));
            Assert.Equal(12, calculator.Add(10, 2));
        }

        [Fact]
        public void CanInvokeCallback()
        {
            var calculator = Mock.Of<ICalculator>();
            var called = false;

            calculator.Add(Any<int>(), Any<int>())
                .Callback(() => called = true)
                .Returns((int x, int y) => x + y);

            calculator.Add(2, 2);

            Assert.True(called);
        }

        [Fact]
        public void CanInvokeTwoCallbacks()
        {
            var calculator = Mock.Of<ICalculator>();
            var called1 = false;
            var called2 = false;

            calculator.Add(Any<int>(), Any<int>())
                .Callback((int _, int __) => called1 = true)
                .Callback((int _, int __) => called2 = true)
                .Returns((int x, int y) => x + y);

            calculator.Add(2, 2);

            Assert.True(called1);
            Assert.True(called2);
        }

        [Fact]
        public void CanInvokeCallbackAfterReturn()
        {
            var calculator = Mock.Of<ICalculator>();
            var called = false;

            calculator.Add(Any<int>(), Any<int>())
                .Returns((int x, int y) => x + y)
                .Callback(() => called = true);

            calculator.Add(2, 2);

            Assert.True(called);
        }

        [Fact]
        public void ThrowsWithException()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator
                .Add(2, 3)
                .Throws(new ArgumentException());

            Assert.Throws<ArgumentException>(() => calculator.Add(2, 3));
        }

        [Fact]
        public void ThrowsWithExceptionType()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator
                .Add(2, 3)
                .Throws<ArgumentException>();

            Assert.Throws<ArgumentException>(() => calculator.Add(2, 3));
        }

        [Fact]
        public void CanSetupPropertyViaReturnsForStrictMock()
        {
            var calculator = Mock.Of<ICalculator>(MockBehavior.Strict);

            // NOTE: since the mock is strict, we need to tell we're going to set it up
            using (Setup())
            {
                calculator.Mode.Returns(CalculatorMode.Scientific);
            }

            var mode = calculator.Mode;

            Assert.Equal(CalculatorMode.Scientific, mode);
            Assert.Throws<StrictMockException>(() => calculator.Add(2, 4));
        }

        [Fact]
        public void CanSetupPropertyForStrictMock()
        {
            var calculator = Mock.Of<ICalculator>(MockBehavior.Strict);

            calculator.Setup(c => c.Mode).Returns(CalculatorMode.Scientific);

            var mode = calculator.Mode;

            Assert.Equal(CalculatorMode.Scientific, mode);
            Assert.Throws<StrictMockException>(() => calculator.Add(2, 4));
        }

        [Fact]
        public void CanSetupPropertyWithValueForStrictMock()
        {
            var calculator = Mock.Of<ICalculator>(MockBehavior.Strict);

            calculator.Setup(c => c.Mode).Returns(CalculatorMode.Scientific);

            var mode = calculator.Mode;

            Assert.Equal(CalculatorMode.Scientific, mode);
            Assert.Throws<StrictMockException>(() => calculator.IsOn);
        }

        [Fact]
        public void CanSetupVoidMethod()
        {
            var calculator = Mock.Of<ICalculator>(MockBehavior.Strict);

            calculator.Setup(c => c.TurnOn())
                .Throws<InvalidOperationException>();

            Assert.Throws<InvalidOperationException>(() => calculator.TurnOn());
        }

        [Fact]
        public void CanAccessMockInfoFromInstance()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Add(Any<int>(), Any<int>()).Returns((int x, int y) => x + y);

            Assert.Equal(4, calculator.Add(2, 2));

            Assert.Equal(1, calculator.AsMock().Invocations.Count);
        }

        [Fact]
        public void CanAssertInvocations()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.TurnOn();
            Assert.Single(calculator.AsMock().InvocationsFor(c => c.TurnOn()));

            calculator.Add(2, 3);

            calculator.Verify(c => c.TurnOn());
            calculator.Verify(c => c.Add(Any<int>(), Any<int>()));

            var ex = Record.Exception(() => calculator.Verify(c => c.Store(Any<string>(), Any<int>())));

            Assert.IsAssignableFrom<VerifyException>(ex);
            Assert.Single(calculator.AsMock().InvocationsFor(c => c.Add(2, 3)));
            Assert.Empty(calculator.AsMock().InvocationsFor(c => c.Add(Not(2), Not(3))));
        }

        [Fact]
        public void ChangeDefaultValue()
        {
            var calculator = Mock.Of<ICalculator>();
            var moq = Mock.Get(calculator);

            Assert.Equal(MockBehavior.Loose, moq.Behavior);

            var value = calculator.Add(5, 5);

            Assert.Equal(0, value);

            moq.DefaultValue.Register(() => 10);

            value = calculator.Add(5, 5);

            Assert.Equal(10, value);
        }

        [Fact]
        public void ChangeBehavior()
        {
            var calculator = Mock.Of<ICalculator>();
            var moq = Mock.Get(calculator);

            Assert.Equal(MockBehavior.Loose, moq.Behavior);

            // Does not throw
            calculator.Add(5, 5);

            moq.Behavior = MockBehavior.Strict;

            Assert.Throws<StrictMockException>(() => calculator.Add(5, 5));
        }

        [Fact]
        public void ChangingBehaviorPreservesDefaultValue()
        {
            var calculator = Mock.Of<ICalculator>();
            var moq = Mock.Get(calculator);

            Assert.Equal(MockBehavior.Loose, moq.Behavior);

            var value = calculator.Add(5, 5);
            Assert.Equal(0, value);

            moq.DefaultValue.Register(() => 10);
            value = calculator.Add(5, 5);
            Assert.Equal(10, value);

            moq.Behavior = MockBehavior.Strict;
            Assert.Throws<StrictMockException>(() => calculator.Add(5, 5));

            moq.Behavior = MockBehavior.Loose;
            Assert.Equal(10, calculator.Add(5, 5));
        }
    }
}