using Moq.Sdk;
using Sample;
using Xunit;

namespace Moq.Tests
{
    public class CallBaseTests
    {
        [Fact]
        public void CallBaseNotCalled()
        {
            var mock = new Mock<Calculator>();

            mock.Object.TurnOn();

            Assert.False(mock.Object.TurnOnCalled);
        }

        [Fact]
        public void CallBaseCalledForMockConfig()
        {
            var mock = new Mock<Calculator> { CallBase = true };

            mock.Object.TurnOn();

            Assert.True(mock.Object.TurnOnCalled);
        }

        [Fact]
        public void CallBaseCalledForInvocationConfig()
        {
            var mock = new Mock<Calculator>();

            mock.TurnOn().CallBase();

            mock.Object.TurnOn();

            Assert.True(mock.Object.TurnOnCalled);
        }

        [Fact]
        public void ThrowsForStrictMockAndMissingSetup()
        {
            // Configure CallBase at the Mock level
            var mock = new Mock<Calculator>(MockBehavior.Strict) { CallBase = true };

            Assert.Throws<StrictMockException>(() => mock.Object.TurnOn());
        }

        [Fact]
        public void CallBaseCalledForStrictMockAndMockConfig()
        {
            // Configure CallBase at the Mock level
            var mock = new Mock<Calculator>(MockBehavior.Strict) { CallBase = true };

            mock.TurnOn().CallBase();

            mock.Object.TurnOn();

            Assert.True(mock.Object.TurnOnCalled);

            // And we make sure we throw for other missing setups
            Assert.Throws<StrictMockException>(() => mock.Object.Recall(""));
        }

        [Fact]
        public void CallBaseCalledForStrictMockAndInvocationConfig()
        {
            var mock = new Mock<Calculator>(MockBehavior.Strict);

            // Configure CallBase at the invocation level
            mock.TurnOn().CallBase();

            mock.Object.TurnOn();

            Assert.True(mock.Object.TurnOnCalled);

            // And we make sure we throw for other missing setups
            Assert.Throws<StrictMockException>(() => mock.Object.Recall(""));
        }
    }
}
