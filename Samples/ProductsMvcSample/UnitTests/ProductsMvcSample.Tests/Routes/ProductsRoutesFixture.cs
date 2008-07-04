using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Web.Mvc;
using ProductsMvcSample.Controllers;

namespace ProductsMvcSample.Tests.Routes
{
	[TestFixture]
	public class ProductsRoutesFixture
	{
		[Test]
		public void ShouldAccept_Products_Category_CategoryId()
		{
			var routes = new RouteCollection();
			Global.RegisterRoutes(routes);

			var httpContext = MvcTestHelper.MockHttpContext();
			var request = MvcTestHelper.MockGet(httpContext, "~/Products/Category/2");

			var routeData = routes.GetRouteData(httpContext.Object);

			routeData.VerifyCallsTo<ProductsController>(c => c.Category(2));
		}
	}
}
