using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace Moq
{
	internal class Reflector<T>
	{
		public static MethodInfo GetMethod<Q>(Expression<Func<T, Q>> expr)
		{
			MethodCallExpression methodCall = expr.Body as MethodCallExpression;
			if (methodCall != null)
				return methodCall.Method;

			throw new InvalidOperationException();
		}
	}
}
