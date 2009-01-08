using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;
using ProductsMvcSample.Controllers;
using System.Web.Routing;
using Moq;
using System.Web;

namespace ProductsMvcSample.Tests.Routes
{
	[TestFixture]
	public class ProductsRoutesFixture
	{
		[Test]
		public void ShouldAccept_Products_Category_CategoryId()
		{
			// Arrange
			var routes = new RouteCollection();
			Global.RegisterRoutes(routes);
			var context = new Mock<HttpContextBase> { DefaultValue = DefaultValue.Mock };
			context
				.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath)
				.Returns("~/Products/Category/2");

			// Act
			var routeData = routes.GetRouteData(context.Object);

			// Assert
			Assert.AreEqual("Products", routeData.Values["controller"], "Default controller is HomeController");
			Assert.AreEqual("Category", routeData.Values["action"], "Default action is Index");
			Assert.AreEqual("2", routeData.Values["id"], "Default Id is empty string");

			routeData.VerifyCallsTo<ProductsController>(c => c.Category(2));
		}
	}
}
