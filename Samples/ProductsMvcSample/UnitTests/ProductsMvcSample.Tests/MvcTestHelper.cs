using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using Moq;
using System.Linq.Expressions;
using NUnit.Framework;
using System.ComponentModel;

namespace ProductsMvcSample.Tests
{
	public static class MvcTestHelper
	{
		public static void VerifyCallsTo<T>(this RouteData route, Expression<Action<T>> action)
					where T : IController
		{
			Assert.IsNotNull(route);

			Assert.IsTrue(route.Values.ContainsKey("controller"), "Controller's name doesn't exists.");
			Assert.AreEqual(typeof(T).Name, route.Values["controller"].ToString() + "Controller", "Controller's name.");

			var methodCall = action.Body as MethodCallExpression;
			if (methodCall == null)
				throw new NotSupportedException("Action body must be a MethodCallExpression.");

			Assert.IsTrue(route.Values.ContainsKey("action"), "Action's name doesn't exists.");
			Assert.AreEqual(methodCall.Method.Name, route.Values["action"], "Action's name.");

			foreach (var parameter in methodCall.Method.GetParameters())
			{
				Assert.IsTrue(route.Values.ContainsKey(parameter.Name), String.Format("Argument for '{0}' doesn't exists.", parameter.Name));

				object expectedValue;
				var argumentExpr = methodCall.Arguments[parameter.Position] as ConstantExpression;
				if (argumentExpr != null)
					expectedValue = argumentExpr.Value;
				else
					throw new NotSupportedException("Arguments must be ConstantExpression.");
				object actualValue = TypeDescriptor.GetConverter(expectedValue.GetType()).ConvertFromString(route.Values[parameter.Name].ToString());
				Assert.AreEqual(expectedValue, actualValue, String.Format("Argument for '{0}'.", parameter.Name));
			}
		}

		public static Mock<IHttpContext> MockHttpContext()
		{
			var context = new Mock<IHttpContext>();
			return context;
		}

		public static Mock<IHttpRequest> MockGet(Mock<IHttpContext> httpContext, string relativeUrl)
		{
			var request = new Mock<IHttpRequest>();
			httpContext.Expect(x => x.Request).Returns(request.Object);
			#region parse relativeUrl
			var fragments = relativeUrl.Split('?');
			var filePath = fragments[0];
			var queryStringFragment = fragments.Length < 2 ? string.Empty : fragments[1];
			var queryString = new System.Collections.Specialized.NameValueCollection();
			foreach (var token in queryStringFragment.Split('&'))
			{
				if (!string.IsNullOrEmpty(token))
					queryString.Add(token.Split('=')[0], token.Split('=')[1]);
			}
			#endregion
			request.Expect(x => x.AppRelativeCurrentExecutionFilePath).Returns(filePath);
			request.Expect(x => x.QueryString).Returns(queryString);
			request.Expect(x => x.PathInfo).Returns(string.Empty);
			request.Expect(x => x.HttpMethod).Returns("GET");
			return request;
		}

		public static void MakeControllerTestable(Controller controller, IViewFactory viewFactory)
		{
			var httpContext = new Mock<IHttpContext>();
			httpContext.Expect(x => x.Session).Returns((IHttpSessionState)null);
			var controllerContext = new ControllerContext(httpContext.Object, new RouteData(), new Mock<IController>().Object);
			var tempData = new TempDataDictionary(httpContext.Object);
			controller.ControllerContext = controllerContext;
			controller.GetType().GetProperty("TempData").SetValue(controller, tempData, null);

			controller.ViewFactory = viewFactory;
		}
	}
}
