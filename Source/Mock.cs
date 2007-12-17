using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace Moq
{
	public class Mock<TInterface> where TInterface : class
	{
		MockProxy<TInterface> proxy = new MockProxy<TInterface>();

		public TInterface Value
		{
			get
			{
				return (TInterface)proxy.GetTransparentProxy();
			}
		}

		public ICall Expect(Expression<Action<TInterface>> expression)
		{
			MethodCallExpression methodCall = expression.Body as MethodCallExpression;

			var call = new MethodCallReturn<object>(
				methodCall.Method, methodCall.Arguments.ToArray());
			proxy.AddCall(call);
			return call;
		}

		public ICall<TResult> Expect<TResult>(Expression<Func<TInterface, TResult>> expression)
		{
			MethodCallExpression methodCall = expression.Body as MethodCallExpression;
			MemberExpression propField = expression.Body as MemberExpression;

			ICall<TResult> result = null;

			if (methodCall != null)
			{
				var call = new MethodCallReturn<TResult>(
					methodCall.Method, methodCall.Arguments.ToArray());
				proxy.AddCall(call);
				result = call;
			}
			else if (propField != null)
			{
				PropertyInfo prop = propField.Member as PropertyInfo;
				FieldInfo field = propField.Member as FieldInfo;
				if (prop != null)
				{
					// If property is not readable, the compiler won't let 
					// the user to specify it in the lambda :)
					// This is just reassuring that in case they build the 
					// expression tree manually?
					if (!prop.CanRead)
					{
						throw new ArgumentException(String.Format(
							Properties.Resources.PropertyNotReadable,
							prop.DeclaringType.Name,
							prop.Name), "expression");
					}

					var call = new MethodCallReturn<TResult>(prop.GetGetMethod());
					proxy.AddCall(call);
					result = call;
				}
				else if (field != null)
				{
					throw new NotSupportedException(Properties.Resources.FieldsNotSupported);
				}
			}

			if (result == null)
			{
				throw new NotSupportedException();
			}

			return result;
		}

		public static implicit operator TInterface(Mock<TInterface> mock)
		{
			// TODO: doesn't work as expected but ONLY with interfaces :S
			return mock.Value;
		}

		//public static explicit operator TInterface(Mock<TInterface> mock)
		//{
		//    // TODO: doesn't work as expected but ONLY with interfaces :S
		//    throw new NotImplementedException();
		//}
	}
}