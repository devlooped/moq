using System;
using Moq.Sdk;
using Sample;
using Xunit;
using static Moq.Syntax;

namespace Moq.Tests
{
    /// <summary>
    /// Showcases creating mocks with the source-provided <see cref="Mock{T}"/>.
    /// </summary>
    public class MockTests
    {
        [Fact]
        public void CreatesMockForSetup()
        {
            var calculator = new Mock<ICalculator>();

            calculator.Add(2, 3).Returns(5);

            Assert.Equal(5, calculator.Object.Add(2, 3));
            Assert.Same(calculator.Object, Mock.Get(calculator.Object).Object);
        }

        [Fact]
        public void CreatesStrictMock()
        {
            var calculator = new Mock<ICalculator>(MockBehavior.Strict);

            calculator.Add(2, 3).Returns(5);

            Assert.Equal(MockBehavior.Strict, calculator.Behavior);
            Assert.Equal(5, calculator.Object.Add(2, 3));
            Assert.Throws<StrictMockException>(() => calculator.Object.TurnOn());
        }

        [Fact]
        public void CreatesClassMockThatCallsBase()
        {
            var calculator = new Mock<Calculator> { CallBase = true };

            calculator.Add(1, 1).Returns(42);

            Assert.Equal(42, calculator.Object.Add(1, 1));
            Assert.Equal(5, calculator.Object.Add(2, 3));
        }

        [Fact]
        public void CreatesMockWithAdditionalInterfaces()
        {
            var calculator = new Mock<ICalculator, IDisposable>();
            var disposable = calculator.As<IDisposable>();
            var disposed = false;

            disposable.Dispose().Callback(() => disposed = true);
            ((IDisposable)calculator.Object).Dispose();

            Assert.True(disposed);
            Assert.Same(calculator.Object, disposable.Object);
        }

        [Fact]
        public void AsThrowsIfInterfaceIsNotImplemented()
            => Assert.Throws<InvalidCastException>(() => new Mock<ICalculator>().As<IDisposable>());

        [Fact]
        public void GetsMockForImplementedInterface()
        {
            var calculator = Mock.Of<ICalculator, IDisposable>();

            var disposable = Mock.Get<IDisposable>(calculator);

            Assert.Same(calculator, disposable.Object);
        }

        [Fact]
        public void GetThrowsIfInterfaceIsNotImplemented()
            => Assert.Throws<InvalidCastException>(() => Mock.Get<IDisposable>(Mock.Of<ICalculator>()));

        [Fact]
        public void GetThrowsIfNotMock()
            => Assert.Throws<ArgumentException>(() => Mock.Get(new Calculator()));

        [Fact]
        public void MocksDelegates()
        {
            var parse = new Mock<Func<string, int>>();

            parse.Invoke("42").Returns(42);

            Assert.Equal(42, parse.Object("42"));
            Assert.Same(parse.Object, Mock.Get(parse.Object).Object);
        }

        [Fact]
        public void CustomizesCreatedMocks()
        {
            var created = new Mock<ICustomized>();

            Assert.Equal(nameof(ICustomized), created.Sdk.Name);
            Assert.Null(Mock.Get(Mock.Of<ICustomized>()).Sdk.Name);
        }

        [Fact]
        public void CustomizesCreatedMocksWithAdditionalInterfaces()
            => Assert.Equal(nameof(ICustomized), new Mock<ICustomized, IDisposable>().Sdk.Name);

        [Fact]
        public void CustomConstructorsCreateMocks()
        {
            var calculator = new Mock<ICalculator>("calc");

            Assert.Equal("calc", calculator.Sdk.Name);
        }

        public interface ICustomized
        {
            void Run();
        }
    }
}

namespace Moq
{
    partial class Mock<T>
    {
        [MockGenerator]
        public Mock(string name) : this() => this.Sdk.Name = name;

        partial void OnCreated()
        {
            if (typeof(T) == typeof(Tests.MockTests.ICustomized))
                this.Sdk.Name = typeof(T).Name;
        }
    }
}
