using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Moq.Sdk;
using Sample;
using Stunts;
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

        public interface IArguments
        {
            int Count(IArgumentCollection arguments);
            Task<int> CountAsync(IArgumentCollection arguments);
        }

        [Fact]
        public void TypedSetupPassesMemberArgumentCollection()
        {
            var invocation = InvocationWithTwoArguments();
            var query = Mock.Get(Mock.Of<IArguments>());

            query.Count(invocation).Returns(args => args.Count);
            query.CountAsync(invocation).Returns(args => args.Count);

            Assert.Equal(invocation.Count, query.Object.Count(invocation));
            Assert.Equal(invocation.Count, query.Object.CountAsync(invocation).Result);
        }

        [Fact]
        public void SyntaxSetupPassesInvocationArguments()
        {
            var invocation = InvocationWithTwoArguments();
            var query = Mock.Get(Mock.Of<IArguments>());

            Setup(() => query.Object.Count(invocation)).Returns(args => args.Count);

            Assert.Equal(1, query.Object.Count(invocation));
        }

        [Fact]
        public void DefaultArgumentStaysConstantBesideMatcher()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());
            var matches = 0;

            calculator.Store(null!, Any<int>()).Callback((string name, int value) => matches++);

            calculator.Object.Store(null!, 7);
            calculator.Object.Store(null!, 1);
            calculator.Object.Store("a", 7);

            Assert.Equal(2, matches);
        }

        static IArgumentCollection InvocationWithTwoArguments()
        {
            var calculator = Mock.Get(Mock.Of<ICalculator>());
            IArgumentCollection? invocation = null;
            Setup(() => calculator.Object.Add(2, 3)).Callback(args => invocation = args);
            calculator.Object.Add(2, 3);
            return invocation!;
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

        [Fact]
        public void SetupExposesSdkInvocation()
        {
            var calculator = new Mock<ICalculator>();

            var setup = calculator.Add(2, 3).Sdk;

            Assert.Equal(nameof(ICalculator.Add), setup.Invocation.MethodBase.Name);
            Assert.True(setup.AppliesTo(Invocation(calculator, c => c.Add(2, 3))));
            Assert.False(setup.AppliesTo(Invocation(calculator, c => c.Add(3, 2))));
        }

        [Fact]
        public void SetupUsesArgumentMatchers()
        {
            var calculator = new Mock<ICalculator>();

            var setup = calculator.Add(Any<int>(), Any<int>(i => i > 5)).Sdk;

            Assert.True(setup.AppliesTo(Invocation(calculator, c => c.Add(1, 10))));
            Assert.False(setup.AppliesTo(Invocation(calculator, c => c.Add(1, 2))));
        }

        [Fact]
        public void SetupDistinguishesOverloads()
        {
            var calculator = new Mock<ICalculator>();

            var setup = calculator.Add(1, 2, 3).Sdk;

            Assert.Equal(3, setup.Invocation.Arguments.Count);
            Assert.False(setup.AppliesTo(Invocation(calculator, c => c.Add(1, 2))));
        }

        [Fact]
        public void CanCountInvocationsAgainstSetup()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Object.Store("a", 1);
            calculator.Object.Store("b", 2);
            calculator.Object.Store("a", 3);

            Assert.Equal(2, calculator.Sdk.Invocations.Count(calculator.Store("a", Any<int>()).Sdk.AppliesTo));
        }

        static IMethodInvocation Invocation(IMock<ICalculator> mock, Action<ICalculator> action)
        {
            action(mock.Object);
            return mock.Sdk.Invocations.Last();
        }
    }
}
