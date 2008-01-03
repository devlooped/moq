using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Moq.Tests
{
	[TestFixture]
	public class Demo
	{
		private static string TALISKER = "Talisker";

		[Test]
		public void FillingRemovesInventoryIfInStock()
		{
			//setup - data
			var order = new Order(TALISKER, 50);
			var mock = new Mock<IWarehouse>();

			//setup - expectations
			mock.Expect(x => x.HasInventory(TALISKER, 50)).Returns(true);

			//exercise
			order.Fill(mock.Object);

			//verify
			Assert.IsTrue(order.IsFilled);
		}

		public void FillingDoesNotRemoveIfNotEnoughInStock()
		{
			//setup - data
			var order = new Order(TALISKER, 50);
			var mock = new Mock<IWarehouse>();

			//setup - expectations
			mock.Expect(x => x.HasInventory(It.IsAny<string>(), It.IsInRange(0, 100, Range.Inclusive))).Returns(false);
			mock.Expect(x => x.Remove(It.IsAny<string>(), It.IsAny<int>())).Throws(new InvalidOperationException());

			//exercise
			order.Fill(mock.Object);

			//verify
			Assert.IsFalse(order.IsFilled);
		}

		public interface IWarehouse
		{
			bool HasInventory(string productName, int quantity);
			void Remove(string productName, int quantity);
		}

		public class Order
		{
			public string ProductName { get; private set; }
			public int Quantity { get; private set; }
			public bool IsFilled { get; private set; }

			public Order(string productName, int quantity)
			{
				this.ProductName = productName;
				this.Quantity = quantity;
			}

			public void Fill(IWarehouse warehouse)
			{
				if (warehouse.HasInventory(ProductName, Quantity))
				{
					warehouse.Remove(ProductName, Quantity);
					IsFilled = true;
				}
			}

		}
	}
}
