using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;

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

		[Fact]
		public void ShouldExtendWithSimpleMatchers()
		{
			var order = new Order();
			var mock = new Mock<IOrderRepository>();

			Order repo = new Match<Order>(r => true);

			mock.Setup(x => x.Save(Orders.Contains(order)))
				 .Throws<ArgumentException>();

			Assert.Throws<ArgumentException>(() => mock.Object.Save(new [] { order }));
		}
	}

	public static class Orders
	{
		public static IEnumerable<Order> Contains(Order order)
		{
			return new Match<IEnumerable<Order>>(orders => orders.Contains(order)).Convert();
		}

		public static Order IsBig()
		{
			return new Match<Order>(o => o.Amount >= 1000);
		}
	}

	public interface IOrderRepository
	{
		void Save(Order order);
		void Save(IEnumerable<Order> orders);
	}

	public static class OrderIs
	{
		[AdvancedMatcher(typeof(BigOrderMatcher))]
		public static Order Big()
		{
			return null;
		}

		public class BigOrderMatcher : IMatcher
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
