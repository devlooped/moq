using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using System;

namespace Store.Tests
{
	[TestFixture]
	public class ProductsPresenterFixturecs
	{
		[Test]
		public void ShouldSetViewCategories()
		{
			// Arrange
			var catalog = new Mock<ICatalogService>();
			var view = new Mock<IProductsView>();

			// Act
			var presenter = new ProductsPresenter(catalog.Object, view.Object);

			// Assert
			view.Verify(v => v.SetCategories(It.IsAny<IEnumerable<Category>>()));
		}


		[Test]
		public void ShouldCategorySelectionSetProducts()
		{
			// Arrange
			var catalog = new Mock<ICatalogService>();
			var view = new Mock<IProductsView>();
			var presenter = new ProductsPresenter(catalog.Object, view.Object);
			
			// Act
			view.Raise(
				v => v.CategorySelected += null, 
				new CategoryEventArgs(new Category { Id = 1 }));

			// Assert
			view.Verify(v => v.SetProducts(It.IsAny<IEnumerable<Product>>()));
		}

		[Test]
		public void ShouldPlaceOrderIfEnoughInventory()
		{
			// Arrange
			var catalog = new Mock<ICatalogService>();
			var view = new Mock<IProductsView>();
			var presenter = new ProductsPresenter(catalog.Object, view.Object);
			var order = new Order 
			{
				Product = new Product { Id = 1 }, 
				Quantity = 5
			};

			catalog
				.Setup(c => c.HasInventory(1, 5))
				.Returns(true);

			// Act
			presenter.PlaceOrder(order);

			// Assert
			Assert.IsTrue(order.Filled);
			catalog.Verify(c => c.HasInventory(1, 5));
		}

		[Test]
		public void ShouldNotPlaceOrderIfNotEnoughInventory()
		{
			// Arrange
			var catalog = new Mock<ICatalogService>();
			var view = new Mock<IProductsView>();
			var presenter = new ProductsPresenter(catalog.Object, view.Object);
			var order = new Order
			{
				Product = new Product { Id = 1 },
				Quantity = 5
			};

			catalog
				.Setup(c => c.HasInventory(1, 5))
				.Returns(false);

			// Act
			presenter.PlaceOrder(order);

			// Assert
			Assert.IsFalse(order.Filled);
			catalog.Verify(c => c.HasInventory(1, 5));
		}

		[Test]
		public void ShouldNotPlaceOrderIfFailsToRemove()
		{
			// Arrange
			var catalog = new Mock<ICatalogService>();
			var view = new Mock<IProductsView>();
			var presenter = new ProductsPresenter(catalog.Object, view.Object);
			var order = new Order
			{
				Product = new Product { Id = 1 },
				Quantity = 5
			};

			catalog
				.Setup(c => c.HasInventory(1, 5))
				.Returns(true);
			catalog
				.Setup(c => c.Remove(1, 5))
				.Throws<InvalidOperationException>();

			// Act
			presenter.PlaceOrder(order);

			// Assert
			Assert.IsFalse(order.Filled);
			catalog.Verify(c => c.HasInventory(1, 5));
			catalog.Verify(c => c.Remove(1, 5));
		}
	}
}
