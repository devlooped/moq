using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;
using Moq;
using ProductsMvcSample.Models;
using ProductsMvcSample.Controllers;
using ProductsMvcSample.Services;

namespace ProductsMvcSample.Tests.Controllers
{
	[TestFixture]
	public class ProductsControllerFixture
	{
		[Test]
		public void CategoryRendersProductsListWithProductsListViewData()
		{
			// Arrange mocks
			var catalogService = new Mock<IProductsCatalogService>();
			
			// Target object
			var controller = new ProductsController(catalogService.Object);

			// Act
			var result = controller.Category(2);

			// Assert
			Assert.IsNotNull(result);
			Assert.IsTrue(result is ViewResult);
			var viewResult = (ViewResult)result;
			Assert.IsNotNull(viewResult.ViewData.Model);
			Assert.That(viewResult.ViewData.Model is ProductsListViewData);
		}

		[Test]
		public void CategoryRendersRendersViewDataWithCategoryInfo()
		{
			// Arrange mocks
			var catalogService = new Mock<IProductsCatalogService>();
			catalogService
				.Setup(c => c.GetCategoryName(2))
				.Returns("FooCategory");

			// Target object
			var controller = new ProductsController(catalogService.Object);

			// Act
			var result = controller.Category(2);

			// Assert
			// We don't repeat the assertions from the previous test here
			var viewData = ((ViewResult)result).ViewData.Model as ProductsListViewData;
			Assert.IsNotNull(viewData);
			Assert.AreEqual(2, viewData.CategoryId);
			Assert.AreEqual("FooCategory", viewData.CategoryName);
		}

		[Test]
		public void CategoryRendersRendersViewDataWithProdcutsListing()
		{
			// Arrange mocks
			var catalogService = new Mock<IProductsCatalogService>();
			catalogService
				.Setup(c => c.GetCategoryName(2))
				.Returns("FooCategory");
			catalogService.Setup(c => c
				.GetProducts(2))
				.Returns(new List<Product>
		        {
		            new Product { Id = 4 , Name = "Foo"},
		            new Product { Id = 7 , Name = "Bar"}
		        });

			// Target object
			var controller = new ProductsController(catalogService.Object);

			// Act
			var result = controller.Category(2);

			// Assert
			var viewData = ((ViewResult)result).ViewData.Model as ProductsListViewData;
			Assert.IsNotNull(viewData);
			Assert.AreEqual(2, viewData.Products.Count);
			Assert.AreEqual(4, viewData.Products[0].Id);
			Assert.AreEqual("Bar", viewData.Products[1].Name);
		}
	}
}
