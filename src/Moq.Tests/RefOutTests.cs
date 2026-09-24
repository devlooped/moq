using System;
using Avatars;
using Sample;
using Xunit;
using Xunit.Abstractions;

namespace Moq.Tests.RefOut
{
    public class RefOutTests
    {
        readonly ITestOutputHelper output;

        public RefOutTests(ITestOutputHelper output) => this.output = output;

        [Fact]
        public void CanUseRefOut()
        {
            var mock = Mock.Of<ICalculator>();
            int x = 10;
            int y = 20;
            int? z;

            mock.TryAdd(ref x, ref y, out z)
                .Returns(true);

            Assert.True(mock.TryAdd(ref x, ref y, out z));
        }

        [Fact]
        public void CanSetRefOutReturns()
        {
            var mock = Mock.Of<ICalculator>();
            int x = 10;
            int y = 20;
            int? z;

            mock.TryAdd(ref x, ref y, out z)
                .Returns(c =>
                {
                    c.Set(2, (int?)(c.Get<int>(0) + c.Get<int>(1)));
                    c.Set(0, 15);
                    c.Set(1, 25);
                    return true;
                });

            Assert.True(mock.TryAdd(ref x, ref y, out z));
            Assert.Equal(15, x);
            Assert.Equal(25, y);
            Assert.Equal(30, z);
        }

        [Fact]
        public void CanSetTypedOut()
        {
            var mock = Mock.Of<ICalculator>();

            mock.Setup<TryAdd>(mock.TryAdd)
                .Returns((ref int x, ref int y, out int? z) => (z = x + y) == z);

            var x1 = 10;
            var y1 = 20;
            int? z1;

            Assert.True(mock.TryAdd(ref x1, ref y1, out z1));
            Assert.Equal(30, z1);
        }

        [Fact]
        public void CanSetTypedOutInRecursiveMock()
        {
            var mock = Mock.Of<IRefOutParent>();
            var expected = DateTimeOffset.Now;
            var value = expected.ToString("O");

            mock.Setup<TryParse>(() => mock.RefOut.TryParse)
                .Returns((string input, out DateTimeOffset date) => DateTimeOffset.TryParse(value, out date));

            Assert.True(mock.RefOut.TryParse(value, out var actual));
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
