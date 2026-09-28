using System;
using Sample;
using Xunit;
using Xunit.Abstractions;
using static Moq.Syntax;

namespace Moq.Tests
{
    public class VerificationTests
    {
        readonly ITestOutputHelper output;

        public VerificationTests(ITestOutputHelper output) => this.output = output;

        [Fact]
        public void VerifySyntaxOnceOnSetup()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Add(2, 3).Returns(5).Once();

            Assert.ThrowsAny<VerifyException>(() => Verify(calculator));

            calculator.Add(2, 3);

            // TODO: We should have a this.Verify() extension method for backs compat.
            Verify(calculator);

            calculator.Add(2, 3);

            Assert.ThrowsAny<VerifyException>(() => Verify(calculator));
        }

        [Fact]
        public void VerifySyntaxOnceOnVerify()
        {
            var calculator = Mock.Of<ICalculator>();

            Assert.Throws<VerifyException>(() => Verify(calculator).Add(2, 3).Once());

            calculator.Add(2, 3);

            Verify(calculator).Add(2, 3).Once();

            calculator.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify(calculator).Add(2, 3).Once());
        }

        [Fact]
        public void VerifySyntaxNeverOnSetup()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Add(2, 3).Returns(5).Never();

            Verify(calculator);

            calculator.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify(calculator));
        }

        [Fact]
        public void VerifySyntaxNeverOnVerify()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Add(2, 3).Returns(5);

            Verify(calculator).Add(2, 3).Never();

            calculator.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify(calculator).Add(2, 3).Never());
        }

        [Fact]
        public void VerifySyntaxExactlyOnSetup()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Add(2, 3).Returns(5).Exactly(2);
            calculator.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify(calculator));

            calculator.Add(2, 3);

            Verify(calculator);

            calculator.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify(calculator));
        }

        [Fact]
        public void VerifySyntaxExactlyOnVerify()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Add(2, 3).Returns(5);
            calculator.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify(calculator).Add(2, 3).Exactly(2));

            calculator.Add(2, 3);

            Verify(calculator).Add(2, 3).Exactly(2);

            calculator.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify(calculator).Add(2, 3).Never());
        }

        [Fact]
        public void VerifyPropertySet()
        {
            var calculator = Mock.Of<ICalculator>();

            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Mode = CalculatorMode.Scientific));

            calculator.Mode = CalculatorMode.Scientific;

            Verify.Called(() => calculator.Mode = CalculatorMode.Scientific);
        }

        [Fact]
        public void VerifyVoidMethod()
        {
            var calculator = Mock.Of<ICalculator>();

            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.TurnOn()));

            calculator.TurnOn();

            Verify.Called(() => calculator.TurnOn());

            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.TurnOn(), 2));

            calculator.TurnOn();

            Verify.Called(() => calculator.TurnOn(), 2);
        }

        [Fact]
        public void VerifyNotCalled()
        {
            var calculator = Mock.Of<ICalculator>();

            Verify.NotCalled(() => calculator.TurnOn());
            Verify.NotCalled(() => calculator.Add(2, 3));

            calculator.TurnOn();
            calculator.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify.NotCalled(() => calculator.TurnOn()));
            Assert.Throws<VerifyException>(() => Verify.NotCalled(() => calculator.Add(2, 3)));
        }

        [Fact]
        public void VerifyNotCalledFluent()
        {
            var calculator = Mock.Of<ICalculator>();

            Verify.NotCalled(calculator).TurnOn();
            Verify.NotCalled(calculator).Add(2, 3);

            calculator.TurnOn();
            calculator.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify.NotCalled(calculator).TurnOn());
            Assert.Throws<VerifyException>(() => Verify.NotCalled(calculator).Add(2, 3));
        }

        [Fact]
        public void VerifyCalls()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Setup(c => c.TurnOn()).Once();
            calculator.Add(2, 3).Returns(5).Once();

            Assert.Throws<VerifyException>(() => Verify.Calls(calculator));

            calculator.TurnOn();
            calculator.Add(2, 3);

            Verify.Calls(calculator);
        }

        [Fact]
        public void VerifyCallsCustom()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Setup(c => c.TurnOn()).Once();
            calculator.Add(2, 3).Returns(5).Once();

            Verify.Calls(
                () => calculator.TurnOn(),
                calls => Assert.Empty(calls));

            calculator.TurnOn();

            Verify.Calls(
                () => calculator.TurnOn(),
                calls => Assert.Single(calls));
        }

        [Fact]
        public void VerifyExtensionAction()
        {
            var calculator = Mock.Of<ICalculator>();

            // At least once
            Assert.Throws<VerifyException>(() => calculator.Verify(x => x.TurnOn()));
            // Once
            Assert.Throws<VerifyException>(() => calculator.Verify(x => x.TurnOn(), 1));
            // At least once with message
            Assert.Throws<VerifyException>(() => calculator.Verify(x => x.TurnOn(), "Should have been called!"));
            // Once with message
            Assert.Throws<VerifyException>(() => calculator.Verify(x => x.TurnOn(), 1, "Should have been called!"));

            calculator.TurnOn();

            // At least once
            calculator.Verify(x => x.TurnOn());
            // Once
            calculator.Verify(x => x.TurnOn(), 1);
            // At least once with message
            calculator.Verify(x => x.TurnOn(), "Should have been called!");
            // Once with message
            calculator.Verify(x => x.TurnOn(), 1, "Should have been called!");
        }

        [Fact]
        public void VerifyExtensionFunction()
        {
            var calculator = Mock.Of<ICalculator>();

            // At least once
            Assert.Throws<VerifyException>(() => calculator.Verify(x => x.Add(2, 3)));
            // Once
            Assert.Throws<VerifyException>(() => calculator.Verify(x => x.Add(2, 3), 1));
            // At least once with message
            Assert.Throws<VerifyException>(() => calculator.Verify(x => x.Add(2, 3), "Should have been called!"));
            // Once with message
            Assert.Throws<VerifyException>(() => calculator.Verify(x => x.Add(2, 3), 1, "Should have been called!"));
            // Times.Once with message
            Assert.Throws<VerifyException>(() => calculator.Verify(x => x.Add(2, 3).Once(), "Should have been called!"));

            calculator.Add(2, 3);

            // At least once
            calculator.Verify(x => x.Add(2, 3));
            // Once
            calculator.Verify(x => x.Add(2, 3), 1);
            // At least once with message
            calculator.Verify(x => x.Add(2, 3), "Should have been called!");
            // Once with message
            calculator.Verify(x => x.Add(2, 3), 1, "Should have been called!");
            // Times.Once with message
            calculator.Verify(x => x.Add(2, 3).Once(), "Should have been called!");
        }

        //[Fact]
        internal void CanVerify()
        {
            var calculator = Mock.Of<ICalculator>();

            calculator.Add(2, 3).Returns(5).Once();
            calculator.Mode.Returns(CalculatorMode.Scientific).Exactly(2);

            var mock = Mock.Get(calculator);
            foreach (var invocation in mock.Sdk.Invocations)
            {
                output.WriteLine((string)invocation.Context[nameof(Environment.StackTrace)]);
            }

            // Syntax-based, follows straightforward setup approach, no lambdas.
            Verify(calculator).Mode = CalculatorMode.Scientific;
            Verify(calculator).Add(2, 3);

            // Verify all "verifiable" calls, i.e. those with 
            // a Once/Never/etc. setup.
            Verify(calculator);
            // Long form, with no lambda
            Verify.Called(calculator);

            // equivalent non-syntax version
            //calculator.Verify().Add(2, 3);
            Verify(calculator).Add(1, 1).Never();

            // Explicit Verify, still no lambdas, long form of Syntax
            Verify.Called(calculator).Add(2, 3).Exactly(2);
            // Explicit Verify, still no lambdas, long form of Syntax
            Verify.NotCalled(calculator).Add(2, 3);

            // For the case where you want keep the mock in "running" mode after the verify. 

            Verify.Called(() => calculator.Add(2, 3).Once());
            Verify.NotCalled(() => calculator.TurnOn());

            // More advanced verification, access calls via lambda
            Verify.Calls(
                () => calculator.Add(2, 3),
                calls => Assert.Single(calls));
            // Works for void/action
            Verify.Calls(
                () => calculator.TurnOn(),
                calls => Assert.Single(calls));

            // Verify all "verifiable" calls, i.e. those with 
            // a Once/Never/etc. setup.
            Verify.Calls(calculator);
            // calculator.Verify();
        }
    }
}
