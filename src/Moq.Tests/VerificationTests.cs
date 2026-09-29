using Sample;
using Xunit;

namespace Moq.Tests
{
    public class VerificationTests
    {
        [Fact]
        public void VerifySyntaxOnceOnSetup()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Add(2, 3).Returns(5).Once();

            Assert.ThrowsAny<VerifyException>(() => Syntax.Verify(calculator));

            calculator.Object.Add(2, 3);

            Syntax.Verify(calculator);

            calculator.Object.Add(2, 3);

            Assert.ThrowsAny<VerifyException>(() => Syntax.Verify(calculator));
        }

        [Fact]
        public void VerifySyntaxOnceOnVerify()
        {
            var calculator = new Mock<ICalculator>();

            Assert.Throws<VerifyException>(() => Syntax.Verify(calculator).Add(2, 3).Once());

            calculator.Object.Add(2, 3);

            Syntax.Verify(calculator).Add(2, 3).Once();

            calculator.Object.Add(2, 3);

            Assert.Throws<VerifyException>(() => Syntax.Verify(calculator).Add(2, 3).Once());
        }

        [Fact]
        public void VerifySyntaxNeverOnSetup()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Add(2, 3).Returns(5).Never();

            Syntax.Verify(calculator);

            calculator.Object.Add(2, 3);

            Assert.Throws<VerifyException>(() => Syntax.Verify(calculator));
        }

        [Fact]
        public void VerifySyntaxNeverOnVerify()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Add(2, 3).Returns(5);

            Verify.NotCalled(calculator).Add(2, 3);

            calculator.Object.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify.NotCalled(calculator).Add(2, 3));
        }

        [Fact]
        public void VerifySyntaxExactlyOnSetup()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Add(2, 3).Returns(5).Exactly(2);
            calculator.Object.Add(2, 3);

            Assert.Throws<VerifyException>(() => Syntax.Verify(calculator));

            calculator.Object.Add(2, 3);

            Syntax.Verify(calculator);

            calculator.Object.Add(2, 3);

            Assert.Throws<VerifyException>(() => Syntax.Verify(calculator));
        }

        [Fact]
        public void VerifySyntaxExactlyOnVerify()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Add(2, 3).Returns(5);
            calculator.Object.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify.Called(calculator).Add(2, 3).Exactly(2));

            calculator.Object.Add(2, 3);

            Verify.Called(calculator).Add(2, 3).Exactly(2);

            calculator.Object.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify.Called(calculator).Add(2, 3).Exactly(2));
        }

        [Fact]
        public void VerifyPropertySet()
        {
            var calculator = new Mock<ICalculator>();

            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Object.Mode = CalculatorMode.Scientific));
            Assert.Throws<VerifyException>(() => Verify.Called(calculator).Mode.Set(CalculatorMode.Scientific));

            calculator.Object.Mode = CalculatorMode.Scientific;

            Verify.Called(() => calculator.Object.Mode = CalculatorMode.Scientific);
            Verify.Called(calculator).Mode.Set(CalculatorMode.Scientific);
        }

        [Fact]
        public void VerifyVoidMethod()
        {
            var calculator = new Mock<ICalculator>();

            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Object.TurnOn()));

            calculator.Object.TurnOn();

            Verify.Called(() => calculator.Object.TurnOn());

            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Object.TurnOn(), 2));

            calculator.Object.TurnOn();

            Verify.Called(() => calculator.Object.TurnOn(), 2);
        }

        [Fact]
        public void VerifyNotCalled()
        {
            var calculator = new Mock<ICalculator>();

            Verify.NotCalled(() => calculator.Object.TurnOn());
            Verify.NotCalled(() => calculator.Object.Add(2, 3));

            calculator.Object.TurnOn();
            calculator.Object.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify.NotCalled(() => calculator.Object.TurnOn()));
            Assert.Throws<VerifyException>(() => Verify.NotCalled(() => calculator.Object.Add(2, 3)));
        }

        [Fact]
        public void VerifyNotCalledFluent()
        {
            var calculator = new Mock<ICalculator>();

            Verify.NotCalled(calculator).TurnOn();
            Verify.NotCalled(calculator).Add(2, 3);

            calculator.Object.TurnOn();
            calculator.Object.Add(2, 3);

            Assert.Throws<VerifyException>(() => Verify.NotCalled(calculator).TurnOn());
            Assert.Throws<VerifyException>(() => Verify.NotCalled(calculator).Add(2, 3));
        }

        [Fact]
        public void VerifyCalls()
        {
            var calculator = new Mock<ICalculator>();

            calculator.TurnOn().Once();
            calculator.Add(2, 3).Returns(5).Once();

            Assert.Throws<VerifyException>(() => Verify.Calls(calculator));

            calculator.Object.TurnOn();
            calculator.Object.Add(2, 3);

            Verify.Calls(calculator);
        }

        [Fact]
        public void VerifyCallsCustom()
        {
            var calculator = new Mock<ICalculator>();

            calculator.TurnOn().Once();
            calculator.Add(2, 3).Returns(5).Once();

            Verify.Calls(
                () => calculator.Object.TurnOn(),
                calls => Assert.Empty(calls));

            calculator.Object.TurnOn();

            Verify.Calls(
                () => calculator.Object.TurnOn(),
                calls => Assert.Single(calls));
        }

        [Fact]
        public void VerifyActionWithTimesAndMessage()
        {
            var calculator = new Mock<ICalculator>();

            // At least once
            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Object.TurnOn()));
            // Once
            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Object.TurnOn(), 1));
            // At least once with message
            var ex = Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Object.TurnOn(), "Should have been called!"));
            Assert.Contains("Should have been called!", ex.Message);
            // Once with message
            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Object.TurnOn(), 1, "Should have been called!"));

            calculator.Object.TurnOn();

            Verify.Called(() => calculator.Object.TurnOn());
            Verify.Called(() => calculator.Object.TurnOn(), 1);
            Verify.Called(() => calculator.Object.TurnOn(), "Should have been called!");
            Verify.Called(() => calculator.Object.TurnOn(), 1, "Should have been called!");
        }

        [Fact]
        public void VerifyFunctionWithTimesAndMessage()
        {
            var calculator = new Mock<ICalculator>();

            // At least once
            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Object.Add(2, 3)));
            // Once
            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Object.Add(2, 3), 1));
            // At least once with message
            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Object.Add(2, 3), "Should have been called!"));
            // Once with message
            Assert.Throws<VerifyException>(() => Verify.Called(() => calculator.Object.Add(2, 3), 1, "Should have been called!"));

            calculator.Object.Add(2, 3);

            Verify.Called(() => calculator.Object.Add(2, 3));
            Verify.Called(() => calculator.Object.Add(2, 3), 1);
            Verify.Called(() => calculator.Object.Add(2, 3), "Should have been called!");
            Verify.Called(() => calculator.Object.Add(2, 3), 1, "Should have been called!");
        }
    }
}
