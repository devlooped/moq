using System;
using System.ComponentModel;
using Moq.Sdk;
using Sample;
using Xunit;
using static Moq.Syntax;

namespace Moq.Tests
{
    /// <summary>
    /// Showcases the generated typed setups on <see cref="IMock{T}"/>.
    /// </summary>
    public class TypedSetupTests
    {
        [Fact]
        public void SetupDoesNotRecordInvocation()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());

            calculator.TurnOn();
            calculator.Add(2, 3);
            _ = calculator.Mode.Sdk;
            _ = calculator.Item("x").Sdk;

            Assert.Empty(calculator.Sdk.Invocations);
        }

        [Fact]
        public void ReturnsValue()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());

            calculator.Add(2, 3).Returns(5);

            Assert.Equal(5, calculator.Object.Add(2, 3));
            Assert.Equal(0, calculator.Object.Add(1, 1));
        }

        [Fact]
        public void ReturnsFromInferredHandler()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());

            calculator.Add(Any<int>(), Any<int>()).Returns((x, y) => x * y);

            Assert.Equal(6, calculator.Object.Add(2, 3));
        }

        [Fact]
        public void ReturnsFromRefOutHandler()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());
            int a = 0, b = 0;

            calculator.TryAdd(ref a, ref b, out _).Returns((ref x, ref y, out z) =>
            {
                z = x + y;
                return true;
            });

            Assert.True(calculator.Object.TryAdd(ref a, ref b, out var result));
            Assert.Equal(0, result);
        }

        [Fact]
        public void ReturnsFromRefOutHandlerWithMatchers()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());
            int a = Any<int>(), b = Any<int>();

            calculator.TryAdd(ref a, ref b, out _).Returns((ref x, ref y, out z) =>
            {
                z = x + y;
                x = 0;
                return true;
            });

            a = 2;
            b = 3;
            Assert.True(calculator.Object.TryAdd(ref a, ref b, out var result));
            Assert.Equal(5, result);
            Assert.Equal(0, a);
        }

        [Fact]
        public void CallbackReceivesArguments()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());
            string? name = null;
            var value = 0;

            calculator.Store(Any<string>(), Any<int>()).Callback((n, v) => (name, value) = (n, v));

            calculator.Object.Store("x", 5);

            Assert.Equal("x", name);
            Assert.Equal(5, value);
        }

        [Fact]
        public void PropertyReturnsValue()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());

            calculator.Mode.Returns(CalculatorMode.Scientific);
            calculator.IsOn.Returns(true);

            Assert.Equal(CalculatorMode.Scientific, calculator.Object.Mode);
            Assert.True(calculator.Object.IsOn);
        }

        [Fact]
        public void PropertySetterCallback()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());
            CalculatorMode? mode = null;

            calculator.Mode.Set(Any<CalculatorMode>()).Callback(value => mode = value);

            calculator.Object.Mode = CalculatorMode.Scientific;

            Assert.Equal(CalculatorMode.Scientific, mode);
        }

        [Fact]
        public void IndexerReturnsValue()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());

            calculator.Item("x").Returns(10);
            calculator.Item(Any<string>(x => x.StartsWith("a"))).Returns(key => key.Length);

            Assert.Equal(10, calculator.Object["x"]);
            Assert.Equal(3, calculator.Object["abc"]);
            Assert.Null(calculator.Object["y"]);
        }

        [Fact]
        public void RecursiveMockSetup()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());
            ICalculatorMemory memory;

            using (Setup())
                memory = calculator.Object.Memory;

            Mock.Get(memory).Recall().Returns(42);

            Assert.Same(memory, calculator.Object.Memory);
            Assert.Equal(42, calculator.Object.Memory.Recall());
        }

        [Fact]
        public void RaisesEvents()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());
            object? sender = null;
            calculator.Object.TurnedOn += (s, e) => sender = s;

            calculator.RaiseTurnedOn();
            Assert.Same(calculator.Object, sender);

            calculator.RaiseTurnedOn(this, EventArgs.Empty);
            Assert.Same(this, sender);
        }

        [Fact]
        public void RaisesCustomEvents()
        {
            var notify = Mock.Get(Mock.Of<INotifyPropertyChanged>());
            string? property = null;
            notify.Object.PropertyChanged += (s, e) => property = e.PropertyName;

            notify.RaisePropertyChanged(new PropertyChangedEventArgs("Mode"));

            Assert.Equal("Mode", property);
        }

        [Fact]
        public void StrictMockCanBeSetUp()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>(MockBehavior.Strict));

            calculator.Add(2, 3).Returns(5);
            calculator.Mode.Returns(CalculatorMode.Standard);

            Assert.Equal(5, calculator.Object.Add(2, 3));
            Assert.Equal(CalculatorMode.Standard, calculator.Object.Mode);
            Assert.Throws<StrictMockException>(() => calculator.Object.Add(1, 1));
        }

        [Fact]
        public void ClassMockCanCallBase()
        {
            var calculator = Mock.Get(Mock.Of<Calculator>());

            calculator.Add(2, 3).CallBase();

            Assert.Equal(5, calculator.Object.Add(2, 3));
            Assert.Equal(0, calculator.Object.Add(1, 1));
        }

        [Fact]
        public void VerifiesWithGeneratedMembers()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());

            calculator.Object.Add(2, 3);
            calculator.Object.Add(2, 3);
            calculator.Object.Mode = CalculatorMode.Scientific;

            Verify.Called(calculator).Add(2, 3).Exactly(2);
            Verify.Called(calculator).Mode.Set(CalculatorMode.Scientific);
            Verify.NotCalled(calculator).TurnOn();
            Verify.NotCalled(calculator).Mode.Get();

            Assert.Throws<VerifyException>(() => Verify.Called(calculator).TurnOn());
            Assert.Throws<VerifyException>(() => Verify.Called(calculator).Add(2, 3).Once());
            Assert.Throws<VerifyException>(() => Verify.NotCalled(calculator).Add(Any<int>(), Any<int>()));
        }

        [Fact]
        public void VerifiesOccurrenceConstraints()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());

            calculator.TurnOn().Once();
            calculator.Mode.Never();

            Assert.Throws<VerifyException>(() => Verify.Called(calculator));

            calculator.Object.TurnOn();
            Verify.Called(calculator);

            _ = calculator.Object.Mode;
            Assert.Throws<VerifyException>(() => Verify.Called(calculator));
        }
    }
}
