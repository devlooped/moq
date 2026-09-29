using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Moq.Sdk;
using Sample;
using Stunts;
using Xunit;
using static Moq.Syntax;

namespace Moq.Tests
{
    public class MoqTests
    {
        [Fact]
        public void SetupDoesNotRecordCalls()
        {
            var calculator = new Mock<ICalculator>();

            calculator.TurnOn();

            Assert.Empty(calculator.Sdk.Invocations);
        }

        [Fact]
        public void CanRaiseEvents()
        {
            var calculator = new Mock<ICalculator>();
            calculator.Sdk.Name = "calculator";

            var raised = false;

            EventHandler handler = (sender, args) => raised = true;
            calculator.Object.TurnedOn += handler;

            Assert.Single(calculator.Sdk.Invocations);
            calculator.Object.TurnedOn += Raise();
            // Raising events should not increase invocation count.
            Assert.Single(calculator.Sdk.Invocations);

            Assert.True(raised);

            raised = false;
            calculator.Object.TurnedOn -= handler;
            calculator.Object.TurnedOn -= handler;

            calculator.Object.TurnedOn += Raise();

            Assert.False(raised);
        }

        [Fact]
        public void CanRaiseEventsWithArgs()
        {
            var mock = new Mock<INotifyPropertyChanged>();

            var property = "";
            mock.Object.PropertyChanged += (sender, args) => property = args.PropertyName;

            mock.Object.PropertyChanged += Raise<PropertyChangedEventHandler>(new PropertyChangedEventArgs("Mode"));

            Assert.Equal("Mode", property);
        }

        [Fact]
        public void CanSetupPropertyViaReturns()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Mode.Returns(CalculatorMode.Standard);

            Assert.Equal(CalculatorMode.Standard, calculator.Object.Mode);
        }

        [Fact]
        public void CanSetupPropertyDirectly()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Object.Mode = CalculatorMode.Scientific;

            Assert.Equal(CalculatorMode.Scientific, calculator.Object.Mode);
        }

        [Fact]
        public void CanSetupPropertyTwiceViaReturns()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Mode.Returns(CalculatorMode.Standard);
            calculator.Mode.Returns(CalculatorMode.Scientific);

            Assert.Equal(CalculatorMode.Scientific, calculator.Object.Mode);
        }

        [Fact]
        public void CanSetupMethodWithArgumentsViaReturns()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Add(2, 3).Returns(5);

            Assert.Equal(5, calculator.Object.Add(2, 3));
        }

        [Fact]
        public void CanSetupMethodWithDifferentArgumentsViaReturns()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Add(2, 2).Returns(4);
            calculator.Add(2, 3).Returns(5);

            calculator.Add(10, Any<int>()).Returns(10);
            calculator.Add(Any<int>(i => i > 20), Any<int>()).Returns(20);

            Assert.Equal(5, calculator.Object.Add(2, 3));
            Assert.Equal(4, calculator.Object.Add(2, 2));
            Assert.Equal(10, calculator.Object.Add(10, 2));
            Assert.Equal(20, calculator.Object.Add(25, 20));
        }

        [Fact]
        public void CanReturnFunction()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Add(2, 2).Returns(() => 4);

            Assert.Equal(4, calculator.Object.Add(2, 2));
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
            var mock = new Mock<IAsync>();

            mock.RunAsync(5).Returns(true);

            Assert.True(await mock.Object.RunAsync(5));
        }

        [Fact]
        public async Task CanReturnAsyncValueFunction()
        {
            var mock = new Mock<IAsync>();

            mock.RunValueAsync(5).Returns(true);

            Assert.True(await mock.Object.RunValueAsync(5));
        }

        [Fact]
        public async Task ThrowsAsync()
        {
            var mock = new Mock<IAsync>();

            mock.RunAsync(5).Throws<InvalidOperationException>();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.Object.RunAsync(5));
        }

        [Fact]
        public async Task ThrowsAsyncWithException()
        {
            var mock = new Mock<IAsync>();

            mock.RunAsync(5).Throws(new InvalidOperationException());

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.Object.RunAsync(5));
        }

        [Fact]
        public async Task ThrowsValueAsync()
        {
            var mock = new Mock<IAsync>();

            mock.RunValueAsync(5).Throws<InvalidOperationException>();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.Object.RunValueAsync(5));
        }

        [Fact]
        public async Task ThrowsValueAsyncWithException()
        {
            var mock = new Mock<IAsync>();

            mock.RunValueAsync(5).Throws(new InvalidOperationException());

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.Object.RunValueAsync(5));
        }

        [Fact]
        public async Task ThrowsVoidAsync()
        {
            var mock = new Mock<IAsync>();

            mock.RunVoidAsync().Throws<InvalidOperationException>();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.Object.RunVoidAsync());
        }

        [Fact]
        public async Task ThrowsVoidAsyncWithException()
        {
            var mock = new Mock<IAsync>();

            mock.RunVoidAsync().Throws(new InvalidOperationException());

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.Object.RunVoidAsync());
        }

        [Fact]
        public async Task ThrowsValueVoidAsync()
        {
            var mock = new Mock<IAsync>();

            mock.RunVoidValueAsync().Throws<InvalidOperationException>();

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.Object.RunVoidValueAsync());
        }

        [Fact]
        public async Task ThrowsValueVoidAsyncWithException()
        {
            var mock = new Mock<IAsync>();

            mock.RunVoidValueAsync().Throws(new InvalidOperationException());

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.Object.RunVoidValueAsync());
        }

        [Fact]
        public void CanReturnFunctionWithArgs()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Add(Any<int>(), Any<int>()).Returns((x, y) => x + y);

            Assert.Equal(4, calculator.Object.Add(2, 2));
            Assert.Equal(5, calculator.Object.Add(2, 3));
        }

        [Fact]
        public void CanReturnFunctionWithUntypedArgs()
        {
            var calculator = new Mock<ICalculator>();

            Setup(() => calculator.Object.Add(Any<int>(), Any<int>()))
                .Returns(args => args.Get<int>(0) * args.Get<int>(1));

            Assert.Equal(6, calculator.Object.Add(2, 3));
        }

        [Fact]
        public void CanInvokeCallback()
        {
            var calculator = new Mock<ICalculator>();
            var called = false;

            calculator.Add(Any<int>(), Any<int>())
                .Callback(() => called = true)
                .Returns((x, y) => x + y);

            Assert.Equal(4, calculator.Object.Add(2, 2));
            Assert.True(called);
        }

        [Fact]
        public void CanInvokeTwoCallbacks()
        {
            var calculator = new Mock<ICalculator>();
            var called1 = 0;
            var called2 = 0;

            calculator.Add(Any<int>(), Any<int>())
                .Callback((x, y) => called1 = x)
                .Callback((x, y) => called2 = y)
                .Returns((x, y) => x + y);

            calculator.Object.Add(2, 3);

            Assert.Equal(2, called1);
            Assert.Equal(3, called2);
        }

        [Fact]
        public void CanInvokeCallbackAfterReturn()
        {
            var calculator = new Mock<ICalculator>();
            var called = false;

            calculator.Add(Any<int>(), Any<int>())
                .Returns((x, y) => x + y)
                .Callback(() => called = true);

            Assert.Equal(4, calculator.Object.Add(2, 2));
            Assert.True(called);
        }

        [Fact]
        public void ThrowsWithException()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Add(2, 3).Throws(new ArgumentException());

            Assert.Throws<ArgumentException>(() => calculator.Object.Add(2, 3));
        }

        [Fact]
        public void ThrowsWithExceptionType()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Add(2, 3).Throws<ArgumentException>();

            Assert.Throws<ArgumentException>(() => calculator.Object.Add(2, 3));
        }

        [Fact]
        public void CanSetupPropertyViaReturnsForStrictMock()
        {
            var calculator = new Mock<ICalculator>(MockBehavior.Strict);

            calculator.Mode.Returns(CalculatorMode.Scientific);

            Assert.Equal(CalculatorMode.Scientific, calculator.Object.Mode);
            Assert.Throws<StrictMockException>(() => calculator.Object.Add(2, 4));
        }

        [Fact]
        public void CanSetupPropertyForStrictMock()
        {
            var calculator = new Mock<ICalculator>(MockBehavior.Strict);

            Setup(() => calculator.Object.Mode).Returns(CalculatorMode.Scientific);

            Assert.Equal(CalculatorMode.Scientific, calculator.Object.Mode);
            Assert.Throws<StrictMockException>(() => calculator.Object.Add(2, 4));
            Assert.Throws<StrictMockException>(() => calculator.Object.IsOn);
        }

        [Fact]
        public void CanSetupVoidMethod()
        {
            var calculator = new Mock<ICalculator>(MockBehavior.Strict);

            calculator.TurnOn().Throws<InvalidOperationException>();

            Assert.Throws<InvalidOperationException>(() => calculator.Object.TurnOn());
        }

        [Fact]
        public void CanAccessMockInfoFromInstance()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Add(Any<int>(), Any<int>()).Returns((x, y) => x + y);

            Assert.Equal(4, calculator.Object.Add(2, 2));
            Assert.Single(MockRuntime.Get(calculator.Object).Invocations);
            Assert.Same(calculator.Sdk, Mock.Get(calculator.Object).Sdk);
        }

        [Fact]
        public void CanAssertInvocations()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Object.TurnOn();
            Assert.Single(MockRuntime.InvocationsFor(() => calculator.Object.TurnOn()));

            calculator.Object.Add(2, 3);

            Verify.Called(() => calculator.Object.TurnOn());
            Verify.Called(() => calculator.Object.Add(Any<int>(), Any<int>()));

            var ex = Record.Exception(() => Verify.Called(() => calculator.Object.Store(Any<string>(), Any<int>())));

            Assert.IsAssignableFrom<VerifyException>(ex);
            Assert.Single(MockRuntime.InvocationsFor(() => calculator.Object.Add(2, 3)));
            Assert.Empty(MockRuntime.InvocationsFor(() => calculator.Object.Add(Not(2), Not(3))));
        }

        [Fact]
        public void ChangeDefaultValue()
        {
            var calculator = new Mock<ICalculator>();

            Assert.Equal(MockBehavior.Loose, calculator.Behavior);
            Assert.Equal(0, calculator.Object.Add(5, 5));

            calculator.DefaultValue.Register(() => 10);

            Assert.Equal(10, calculator.Object.Add(5, 5));
        }

        [Fact]
        public void ChangeBehavior()
        {
            var calculator = new Mock<ICalculator>();

            Assert.Equal(MockBehavior.Loose, calculator.Behavior);

            // Does not throw
            calculator.Object.Add(5, 5);

            calculator.Behavior = MockBehavior.Strict;

            Assert.Throws<StrictMockException>(() => calculator.Object.Add(5, 5));
        }

        [Fact]
        public void ChangingBehaviorPreservesDefaultValue()
        {
            var calculator = new Mock<ICalculator>();

            Assert.Equal(0, calculator.Object.Add(5, 5));

            calculator.DefaultValue.Register(() => 10);
            Assert.Equal(10, calculator.Object.Add(5, 5));

            calculator.Behavior = MockBehavior.Strict;
            Assert.Throws<StrictMockException>(() => calculator.Object.Add(5, 5));

            calculator.Behavior = MockBehavior.Loose;
            Assert.Equal(10, calculator.Object.Add(5, 5));
        }
    }
}
