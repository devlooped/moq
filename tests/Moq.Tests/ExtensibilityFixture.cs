// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
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

		[Fact]
		public void ShouldExtendWithSimpleMatchers()
		{
			var order = new Order();
			var mock = new Mock<IOrderRepository>();

			Order repo = Match.Create<Order>(r => true);

			mock.Setup(x => x.Save(Orders.Contains(order)))
				 .Throws<ArgumentException>();

			Assert.Throws<ArgumentException>(() => mock.Object.Save(new[] { order }));
		}

		[Fact]
		public void ShouldExtendWithPropertyMatchers()
		{
			var mock = new Mock<IOrderRepository>();
			mock.Setup(repo => repo.Save(Orders.IsSmall))
				.Throws(new InvalidOperationException());

			try
			{
				mock.Object.Save(new Order { Amount = 50 });

				Assert.True(false, "Should have failed for small order");
			}
			catch (InvalidOperationException)
			{
			}
		}

		//[Fact]
		//public void SetterMatcherRendersNicely()
		//{
		//	var mock = new Mock<IOrderRepository>();

		//	try
		//	{
		//		mock.VerifySet(repo => repo.Value = It.IsAny<int>());
		//	}
		//	catch (MockException me)
		//	{
		//		Console.WriteLine(me.Message);
		//	}

		//	mock.Object.Value = 25;

		//	mock.VerifySet(repo => repo.Value = It.IsInRange(10, 25, Range.Inclusive));
		//}
	}

	public static class Orders
	{
		public static IEnumerable<Order> Contains(Order order)
		{
			return Match.Create<IEnumerable<Order>>(orders => orders.Contains(order));
		}

		public static Order IsBig()
		{
			return Match.Create<Order>(o => o.Amount >= 1000);
		}

		public static Order IsSmall
		{
			get
			{
				return Match.Create<Order>(o => o.Amount <= 1000);
			}
		}
	}

	public interface IOrderRepository
	{
		void Save(Order order);
		void Save(IEnumerable<Order> orders);
		int Value { get; set; }
	}

	public static class OrderIs
	{
		public static Order Big()
		{
			return Match.Create<Order>(o => o.Amount >= 1000);
		}
	}

	public class Order
	{
		public int Amount { get; set; }
	}
}
