using System;
using Sample;
using Stunts;
using Xunit;
using static Moq.Syntax;

namespace Moq.Tests.RefOut
{
    public class RefOutTests
    {
        [Fact]
        public void CanUseRefOut()
        {
            var mock = new Mock<ICalculator>();
            int x = 10;
            int y = 20;

            mock.TryAdd(ref x, ref y, out _).Returns(true);

            Assert.True(mock.Object.TryAdd(ref x, ref y, out _));
        }

        [Fact]
        public void CanSetRefOutReturns()
        {
            var mock = new Mock<ICalculator>();
            int x = 10;
            int y = 20;

            mock.TryAdd(ref x, ref y, out _)
                .Returns((ref int a, ref int b, out int? c) =>
                {
                    c = a + b;
                    a = 15;
                    b = 25;
                    return true;
                });

            Assert.True(mock.Object.TryAdd(ref x, ref y, out var z));
            Assert.Equal(15, x);
            Assert.Equal(25, y);
            Assert.Equal(30, z);
        }

        [Fact]
        public void CanSetRefOutReturnsFromUntypedArguments()
        {
            var mock = new Mock<ICalculator>();
            int x = 10;
            int y = 20;
            int? z;

            Setup(() => mock.Object.TryAdd(ref x, ref y, out z))
                .Returns(c =>
                {
                    c.Set(2, (int?)(c.Get<int>(0) + c.Get<int>(1)));
                    c.Set(0, 15);
                    c.Set(1, 25);
                    return true;
                });

            Assert.True(mock.Object.TryAdd(ref x, ref y, out z));
            Assert.Equal(15, x);
            Assert.Equal(25, y);
            Assert.Equal(30, z);
        }

        [Fact]
        public void CanSetTypedOut()
        {
            var mock = new Mock<ICalculator>();

            SetupRef<TryAdd>(mock.Object.TryAdd)
                .Returns((ref int x, ref int y, out int? z) => (z = x + y) == z);

            var x1 = 10;
            var y1 = 20;

            Assert.True(mock.Object.TryAdd(ref x1, ref y1, out var z1));
            Assert.Equal(30, z1);
        }

        [Fact]
        public void CanSetTypedOutInRecursiveMock()
        {
            var mock = new Mock<IRefOutParent>();
            var expected = DateTimeOffset.Now;
            var value = expected.ToString("O");

            SetupRef<TryParse>(() => mock.Object.RefOut.TryParse)
                .Returns((string input, out DateTimeOffset date) => DateTimeOffset.TryParse(value, out date));

            Assert.True(mock.Object.RefOut.TryParse(value, out var actual));
            Assert.Equal(expected, actual);
        }

        delegate bool TryParse(string input, out DateTimeOffset date);

        delegate bool TryAdd(ref int x, ref int y, out int? z);
    }

    public interface IRefOutParent
    {
        IRefOut RefOut { get; }
    }

    public interface IRefOut
    {
        bool TryParse(string input, out DateTimeOffset date);
    }
}
