using System;
using Xunit;

namespace Moq.Tests
{
	public class ExtensibilityFixture
	{
		[Fact]
		public void ShouldExtendMatching()
		{
			var mock = new Mock<IOrderRepository>();
			mock.Setup(repo => repo.Save(OrderIs.Big()))
				.Throws(new InvalidOperationException());

			try
			{
				mock.Object.Save(new Order { Amount = 1000 });

				Assert.True(false, "Should have failed for big order");
			}
			catch (InvalidOperationException)
			{
			}
		}
	}

	public interface IOrderRepository
	{
		void Save(Order order);
	}

	public static class OrderIs
	{
		[AdvancedMatcher(typeof(BigOrderMatcher))]
		public static Order Big()
		{
			return null;
		}

		class BigOrderMatcher : IMatcher
		{
			public void Initialize(System.Linq.Expressions.Expression matcherExpression)
			{
			}

			public bool Matches(object value)
			{
				if (value is Order &&
					((Order)value).Amount >= 1000)
					return true;

				return false;
			}
		}
	}

	public class Order
	{
		public int Amount { get; set; }
	}
}
