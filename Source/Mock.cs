using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;

namespace Moq
{
	/// <summary>
	/// Provides a mock implementation of <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">Type of the interface to mock.</typeparam>
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
	public class Mock<T> where T : class
	{
		static readonly ProxyGenerator generator = new ProxyGenerator();
		Interceptor interceptor;
		T instance;
		RemotingProxy remotingProxy;

		/// <summary>
		/// Initializes an instance of the mock with the <see cref="MockBehavior.Default">default behavior</see>.
		/// </summary>
		public Mock() : this(MockBehavior.Default) {}

		/// <summary>
		/// Initializes an instance of the mock, optionally changing the 
		/// <see cref="MockBehavior.Default">default behavior</see>
		/// </summary>
		public Mock(MockBehavior behavior)
		{
			interceptor = new Interceptor(behavior);

			if (typeof(MarshalByRefObject).IsAssignableFrom(typeof(T)))
			{
				remotingProxy = new RemotingProxy(typeof(T), x => interceptor.Intercept(x));
				instance = (T)remotingProxy.GetTransparentProxy();
			}
			else if (typeof(T).IsInterface)
			{
				instance = generator.CreateInterfaceProxyWithoutTarget<T>(interceptor);
			}
			else
			{
				try
				{
					instance = generator.CreateClassProxy<T>(interceptor);
				}
				catch (TypeLoadException tle)
				{
					throw new ArgumentException(Properties.Resources.InvalidMockClass, tle);
				}
			}
		}

		/// <summary>
		/// Exposes the mocked object instance.
		/// </summary>
		public T Object
		{
			get
			{ 
				return instance;
			}
		}

		/// <summary>
		/// Sets an expectation on the mocked interface for a call to 
		/// to a void-returning method.
		/// </summary>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		/// <example>
		/// <code>
		/// var mock = new Mock&lt;IProcessor&gt;();
		/// mock.Expect(x =&gt; x.Execute("ping"));
		/// </code>
		/// </example>
		public ICall Expect(Expression<Action<T>> expression)
		{
			Guard.ArgumentNotNull(expression, "expression");

			MethodCallExpression methodCall = expression.Body as MethodCallExpression;

			var call = new MethodCallReturn<object>(
				methodCall.Method, methodCall.Arguments.ToArray());
			interceptor.AddCall(call);
			return call;
		}

		/// <summary>
		/// Sets an expectation on the mocked interface for a call to 
		/// to a value returning method.
		/// </summary>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		/// <example>
		/// <code>
		/// mock.Expect(x =&gt; x.HasInventory("Talisker", 50)).Returns(true);
		/// </code>
		/// </example>
		public ICallReturn<TResult> Expect<TResult>(Expression<Func<T, TResult>> expression)
		{
			return (ICallReturn<TResult>)ExpectImpl(
				expression, 
				(method, args) => new MethodCallReturn<TResult>(method, args));
		}

		private IProxyCall ExpectImpl(
			Expression expression, 
			Func<MethodInfo, Expression[], IProxyCall> factory)
		{
			Guard.ArgumentNotNull(expression, "expression");

			LambdaExpression lambda = (LambdaExpression)expression;
			MethodCallExpression methodCall = lambda.Body as MethodCallExpression;
			MemberExpression propField = lambda.Body as MemberExpression;

			IProxyCall result = null;

			if (methodCall != null)
			{
				result = factory(methodCall.Method, methodCall.Arguments.ToArray());
				interceptor.AddCall(result);
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

					result = factory(prop.GetGetMethod(), new Expression[0]);
					interceptor.AddCall(result);
				}
				else if (field != null)
				{
					throw new NotSupportedException(Properties.Resources.FieldsNotSupported);
				}
			}

			if (result == null)
			{
				throw new NotSupportedException(expression.ToString());
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