using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	/// <summary>
	/// Provides a mock implementation of <typeparamref name="TInterface"/>.
	/// </summary>
	/// <typeparam name="TInterface">Type of the interface to mock.</typeparam>
	/// <remarks>
	/// The following example shows setting expectations with specific values 
	/// for method invocations:
	/// <code>
	/// //setup - data
	/// var order = new Order(TALISKER, 50);
	/// var mock = new Mock&lt;IWarehouse&gt;();
	/// 
	/// //setup - expectations
	/// mock.Expect(x => x.HasInventory(TALISKER, 50)).Returns(true);
	/// 
	/// //exercise
	/// order.Fill(mock.Object);
	/// 
	/// //verify
	/// Assert.IsTrue(order.IsFilled);
	/// </code>
	/// The following example shows how to use the <see cref="It"/> class 
	/// to specify conditions for arguments instead of specific values:
	/// <code>
	/// //setup - data
	/// var order = new Order(TALISKER, 50);
	/// var mock = new Mock&lt;IWarehouse&gt;();
	/// 
	/// //setup - expectations
	/// //shows how to expect a value within a range
	/// mock.Expect(x => x.HasInventory(It.IsAny&lt;string&gt;(), It.IsInRange(0, 100, Range.Inclusive))).Returns(false);
	/// 
	/// //shows how to throw for unexpected calls. contrast with the "verify" approach of other mock libraries.
	/// mock.Expect(x => x.Remove(It.IsAny&lt;string&gt;(), It.IsAny&lt;int&gt;())).Throws(new InvalidOperationException());
	/// 
	/// //exercise
	/// order.Fill(mock.Object);
	/// 
	/// //verify
	/// Assert.IsFalse(order.IsFilled);
	/// </code>
	/// </remarks>
	public class Mock<TInterface> where TInterface : class
	{
		MockProxy<TInterface> proxy = new MockProxy<TInterface>();

		/// <summary>
		/// Exposes the mocked object instance.
		/// </summary>
		public TInterface Object
		{
			get
			{ 
				return (TInterface)proxy.GetTransparentProxy();
			}
		}

		/// <summary>
		/// Sets an expectation on the mocked interface for a call to 
		/// to a void-returning method.
		/// </summary>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		public ICall Expect(Expression<Action<TInterface>> expression)
		{
			Guard.ArgumentNotNull(expression, "expression");

			MethodCallExpression methodCall = expression.Body as MethodCallExpression;

			var call = new MethodCallReturn<object>(
				methodCall.Method, methodCall.Arguments.ToArray());
			proxy.AddCall(call);
			return call;
		}

		/// <summary>
		/// Sets an expectation on the mocked interface for a call to 
		/// to a value returning method.
		/// </summary>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		public ICall<TResult> Expect<TResult>(Expression<Func<TInterface, TResult>> expression)
		{
			Guard.ArgumentNotNull(expression, "expression");

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

		// NOTE: known issue. See https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=318122
		//public static implicit operator TInterface(Mock<TInterface> mock)
		//{
		//    // TODO: doesn't work as expected but ONLY with interfaces :S
		//    return mock.Object;
		//}

		//public static explicit operator TInterface(Mock<TInterface> mock)
		//{
		//    // TODO: doesn't work as expected but ONLY with interfaces :S
		//    throw new NotImplementedException();
		//}
	}
}