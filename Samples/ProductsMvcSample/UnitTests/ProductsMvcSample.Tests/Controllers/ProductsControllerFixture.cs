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
			// View & ViewFactory Mocks
			var view = new Mock<IView>(MockBehavior.Loose);
			var viewFactory = new Mock<IViewFactory>();
			viewFactory.Expect(x => x
				.CreateView(
					It.IsAny<ControllerContext>(),
					"ProductsList",
					It.IsAny<string>(),
					It.Is<object>(vd => vd is ProductsListViewData)))
				.Returns(view.Object);

			// Services Mocks
			var catalogService = new Mock<IProductsCatalogService>(MockBehavior.Loose);

			// Target object
			var controller = new ProductsController(catalogService.Object);
			MvcTestHelper.MakeControllerTestable(controller, viewFactory.Object);

			controller.Category(2);

			viewFactory.VerifyAll();
		}

		[Test]
		public void CategoryRendersRendersViewDataWithCategoryInfo()
		{
			ProductsListViewData renderedViewData = null;

			// View & ViewFactory Mocks
			var view = new Mock<IView>(MockBehavior.Loose);
			var viewFactory = new Mock<IViewFactory>();
			viewFactory.Expect(x => x
				.CreateView(
					It.IsAny<ControllerContext>(),
					"ProductsList",
					It.IsAny<string>(),
					It.IsAny<object>()))
				.Callback((ControllerContext ctx, string vn, string mn, object viewData) => {
					renderedViewData = viewData as ProductsListViewData;
				})
				.Returns(view.Object);

			// Services Mocks
			var catalogService = new Mock<IProductsCatalogService>(MockBehavior.Loose);
			catalogService.Expect(x => x
				.GetCategoryName(2))
				.Returns("FooCategory");

			// Target object
			var controller = new ProductsController(catalogService.Object);
			MvcTestHelper.MakeControllerTestable(controller, viewFactory.Object);

			controller.Category(2);

			viewFactory.VerifyAll();
			catalogService.VerifyAll();

			// Assertions of rendered data are done here to get better TDD experience
			Assert.IsNotNull(renderedViewData);
			Assert.AreEqual(2, renderedViewData.CategoryId);
			Assert.AreEqual("FooCategory", renderedViewData.CategoryName);
		}

		[Test]
		public void CategoryRendersRendersViewDataWithProdcutsListing()
		{
			ProductsListViewData renderedViewData = null;

			// View & ViewFactory Mocks
			var view = new Mock<IView>(MockBehavior.Loose);
			var viewFactory = new Mock<IViewFactory>();
			viewFactory.Expect(x => x
				.CreateView(
					It.IsAny<ControllerContext>(),
					"ProductsList",
					It.IsAny<string>(),
					It.IsAny<object>()))
				.Callback((ControllerContext ctx, string vn, string mn, object viewData) =>
				{
					renderedViewData = viewData as ProductsListViewData;
				})
				.Returns(view.Object);

			// Services Mocks
			var catalogService = new Mock<IProductsCatalogService>(MockBehavior.Loose);
			catalogService.Expect(x => x
				.GetCategoryName(2))
				.Returns("FooCategory");
			catalogService.Expect(x => x
				.GetProducts(2))
				.Returns(new List<Product>
				{
					new Product { Id = 4 , Name = "Bar"},
					new Product { Id = 7 , Name = "FooBar"}
				});

			// Target object
			var controller = new ProductsController(catalogService.Object);
			MvcTestHelper.MakeControllerTestable(controller, viewFactory.Object);

			controller.Category(2);

			viewFactory.VerifyAll();
			catalogService.VerifyAll();

			// Assertions of rendered data are done here to get better TDD experience
			Assert.IsNotNull(renderedViewData);
			Assert.AreEqual(2, renderedViewData.Products.Count);
			Assert.AreEqual(4, renderedViewData.Products[0].Id);
			Assert.AreEqual("FooBar", renderedViewData.Products[1].Name);
		}
	}
}
