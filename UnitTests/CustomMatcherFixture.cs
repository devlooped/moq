using System;
using Xunit;

namespace Moq.Tests
{
	public class CustomMatcherFixture
	{
		[Fact]
		public void UsesCustomMatcher()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Do(Any<string>())).Returns(true);

			Assert.True(mock.Object.Do("foo"));
		}

		[Fact]
		public void UsesCustomMatcherWithArgument()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Do(Between(1, 5, Range.Inclusive))).Returns(true);

			Assert.False(mock.Object.Do(6));
			Assert.True(mock.Object.Do(1));
			Assert.True(mock.Object.Do(5));
		}

		public TValue Any<TValue>()
		{
			return Match.Create<TValue>(v => true);
		}

		public TValue Between<TValue>(TValue from, TValue to, Range rangeKind)
			where TValue : IComparable
		{
			return Match.Create<TValue>(value =>
			{
				if (value == null)
				{
					return false;
				}

				if (rangeKind == Range.Exclusive)
				{
					return value.CompareTo(from) > 0 &&
						value.CompareTo(to) < 0;
				}
				else
				{
					return value.CompareTo(from) >= 0 &&
						value.CompareTo(to) <= 0;
				}
			});
		}

		public interface IFoo
		{
			bool Do(string value);
			bool Do(int value);
		}
	}
}
