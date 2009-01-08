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
using System.Web.Routing;

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
	}
}
