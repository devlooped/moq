using System;
using System.Linq.Expressions;
using System.Reflection;

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
