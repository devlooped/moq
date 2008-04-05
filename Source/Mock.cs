using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Moq.Language.Flow;
using System.ComponentModel;

namespace Moq
{
	// TODO: uncomment documentation when C# bug is fixed: 
	///// <typeparam name="T">Type to mock, which can be an interface or a class.</typeparam>
	/// <summary>
	/// Provides a mock implementation of <typeparamref name="T"/>.
	/// </summary>
	/// <remarks>
	/// If the mocked <typeparamref name="T"/> is a <see cref="MarshalByRefObject"/> (such as a 
	/// Windows Forms control or another <see cref="System.ComponentModel.Component"/>-derived class) 
	/// all members will be mockeable, even if they are not virtual or abstract.
	/// <para>
	/// For regular .NET classes ("POCOs" or Plain Old CLR Objects), only abstract and virtual 
	/// members can be mocked. 
	/// </para>
	/// <para>
	/// The behavior of the mock with regards to the expectations and the actual calls is determined 
	/// by the optional <see cref="MockBehavior"/> that can be passed to the <see cref="Mock{T}(MockBehavior)"/> 
	/// constructor.
	/// </para>
	/// </remarks>
	/// <example>
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
	/// </example>
	public class Mock<T> : IVerifiable, IHideObjectMembers
		where T : class
	{
		static readonly ProxyGenerator generator = new ProxyGenerator();
		Interceptor interceptor;
		T instance;
		RemotingProxy remotingProxy;
		MockBehavior behavior;

		/// <summary>
		/// Initializes an instance of the mock with a specific <see cref="MockBehavior">behavior</see> with 
		/// the given constructor arguments for the class.
		/// </summary>
		/// <remarks>
		/// The mock will try to find the best match constructor given the constructor arguments, and invoke that 
		/// to initialize the instance. This applies only to classes, not interfaces.
		/// <para>
		/// <b>Note:</b> For a <see cref="MarshalByRefObject"/> derived class, any calls done in the constructor itself 
		/// (i.e. calls to private members, initialization methods, etc. invoked from the constructor) 
		/// will not go through the proxied mock and will instead be direct invocations in the underlying 
		/// object. This is a known limitation.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code>var mock = new Mock&lt;MyProvider&gt;(someArgument, 25);</code>
		/// </example>
		public Mock(MockBehavior behavior, params object[] args)
		{
			if (args == null) args = new object[0];

			this.behavior = behavior;
			interceptor = new Interceptor(behavior, typeof(T), this);
			var interfacesTypes = new Type[] { typeof(IMocked<T>) };
			var mockType = typeof(T);

			try
			{
				if (typeof(MarshalByRefObject).IsAssignableFrom(mockType))
				{
					var generatedType = generator.ProxyBuilder.CreateClassProxy(mockType, interfacesTypes, new ProxyGenerationOptions());

					remotingProxy = new RemotingProxy(generatedType, x => interceptor.Intercept(x), args);
					instance = (T)remotingProxy.GetTransparentProxy();
				}
				else if (typeof(T).IsInterface)
				{
					if (args.Length > 0)
						throw new ArgumentException(Properties.Resources.ConstructorArgsForInterface);

					instance = (T)generator.CreateInterfaceProxyWithoutTarget(mockType, interfacesTypes, interceptor);
				}
				else
				{
					try
					{
						if (args.Length > 0)
						{
							var generatedType = generator.ProxyBuilder.CreateClassProxy(mockType, interfacesTypes, new ProxyGenerationOptions());
							instance = (T)Activator.CreateInstance(generatedType,
								new object[] { new IInterceptor[] { interceptor } }.Concat(args).ToArray());
						}
						else
						{
							instance = (T)generator.CreateClassProxy(mockType, interfacesTypes, interceptor);
						}
					}
					catch (TypeLoadException tle)
					{
						throw new ArgumentException(Properties.Resources.InvalidMockClass, tle);
					}
				}

			}
			catch (MissingMethodException mme)
			{
				throw new ArgumentException(Properties.Resources.ConstructorNotFound, mme);
			}
		}

		/// <summary>
		/// Initializes an instance of the mock with <see cref="MockBehavior.Default">default behavior</see> and with 
		/// the given constructor arguments for the class. (Only valid when <typeparamref name="T"/> is a class)
		/// </summary>
		/// <remarks>
		/// The mock will try to find the best match constructor given the constructor arguments, and invoke that 
		/// to initialize the instance. This applies only for classes, not interfaces.
		/// <para>
		/// <b>Note:</b> For a <see cref="MarshalByRefObject"/> derived class, any calls done in the constructor itself 
		/// will not go through the proxied mock and will instead be direct invocations in the underlying 
		/// object. This is known limitation.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code>var mock = new Mock&lt;MyProvider&gt;(someArgument, 25);</code>
		/// </example>
		public Mock(params object[] args) : this(MockBehavior.Default, args) { }

		/// <summary>
		/// Initializes an instance of the mock with <see cref="MockBehavior.Default">default behavior</see>.
		/// </summary>
		/// <example>
		/// <code>var mock = new Mock&lt;IFormatProvider&gt;();</code>
		/// </example>
		public Mock() : this(MockBehavior.Default) { }

		/// <summary>
		/// Initializes an instance of the mock with the specified <see cref="MockBehavior">behavior</see>.
		/// </summary>
		/// <example>
		/// <code>var mock = new Mock&lt;IFormatProvider&gt;(MockBehavior.Relaxed);</code>
		/// </example>
		public Mock(MockBehavior behavior) : this(behavior, new object[0]) { }

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

		/// <devdoc>
		/// Used for testing the mock factory.
		/// </devdoc>
		internal MockBehavior Behavior { get { return behavior; } }

		/// <summary>
		/// Sets an expectation on the mocked type for a call to 
		/// to a void method.
		/// </summary>
		/// <remarks>
		/// If more than one expectation is set for the same method or property, 
		/// the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		/// <example>
		/// <code>
		/// var mock = new Mock&lt;IProcessor&gt;();
		/// mock.Expect(x =&gt; x.Execute("ping"));
		/// </code>
		/// </example>
		public IExpect Expect(Expression<Action<T>> expression)
		{
			return (IExpect)ExpectImpl(
				expression,
				ExpectKind.MethodOrPropertyGet,
				(original, method, args) => new MethodCall(original, method, args));
		}

		/// <summary>
		/// Sets an expectation on the mocked type for a call to 
		/// to a value returning method.
		/// </summary>
		/// <typeparam name="TResult">Type of the return value. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <remarks>
		/// If more than one expectation is set for the same method or property, 
		/// the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
		/// <example>
		/// <code>
		/// mock.Expect(x =&gt; x.HasInventory("Talisker", 50)).Returns(true);
		/// </code>
		/// </example>
		public IExpect<TResult> Expect<TResult>(Expression<Func<T, TResult>> expression)
		{
			return (IExpect<TResult>)ExpectImpl(
				expression,
				ExpectKind.MethodOrPropertyGet,
				(original, method, args) => new MethodCallReturn<TResult>(original, method, args));
		}

		/// <summary>
		/// Sets an expectation on the mocked type for a call to 
		/// to a property getter.
		/// </summary>
		/// <remarks>
		/// If more than one expectation is set for the same property getter, 
		/// the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <param name="expression">Lambda expression that specifies the expected property getter.</param>
		/// <example>
		/// <code>
		/// mock.ExpectGet(x =&gt; x.Suspended)
		///     .Returns(true);
		/// </code>
		/// </example>
		public IExpectGetter<TProperty> ExpectGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			return (IExpectGetter<TProperty>)ExpectImpl(
				expression,
				ExpectKind.PropertyGet,
				(original, method, args) => new MethodCallReturn<TProperty>(original, method, args));
		}

		/// <summary>
		/// Sets an expectation on the mocked type for a call to 
		/// to a property setter.
		/// </summary>
		/// <remarks>
		/// If more than one expectation is set for the same property setter, 
		/// the latest one wins and is the one that will be executed.
		/// </remarks>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <param name="expression">Lambda expression that specifies the expected property setter.</param>
		/// <example>
		/// <code>
		/// mock.ExpectSet(x =&gt; x.Suspended);
		/// </code>
		/// </example>
		public IExpectSetter<TProperty> ExpectSet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			return (IExpectSetter<TProperty>)ExpectImpl(
				expression,
				ExpectKind.PropertySet,
				(original, method, args) => new MethodCall<TProperty>(original, method, args));
		}

		/// <summary>
		/// Verifies that all verifiable expectations have been met.
		/// </summary>
		/// <example>
		/// This example sets up an expectation and marks it as verifiable. After 
		/// the mock is used, a <see cref="Verify"/> call is issued on the mock 
		/// to ensure the method in the expectation was invoked:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// mock.Expect(x =&gt; x.HasInventory(TALISKER, 50)).Verifiable().Returns(true);
		/// ...
		/// // other test code
		/// ...
		/// // Will throw if the test code has didn't call HasInventory.
		/// mock.Verify();
		/// </code>
		/// </example>
		/// <exception cref="MockException">Not all verifiable expectations were met.</exception>
		public void Verify()
		{
			try
			{
				interceptor.Verify();
			}
			catch (Exception ex)
			{
				// Rethrow resetting the call-stack so that 
				// callers see the exception as happening at 
				// this call site.
				throw ex;
			}
		}

		/// <summary>
		/// Verifies all expectations regardless of whether they have 
		/// been flagged as verifiable.
		/// </summary>
		/// <example>
		/// This example sets up an expectation without marking it as verifiable. After 
		/// the mock is used, a <see cref="VerifyAll"/> call is issued on the mock 
		/// to ensure that all expectations are met:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// mock.Expect(x =&gt; x.HasInventory(TALISKER, 50)).Returns(true);
		/// ...
		/// // other test code
		/// ...
		/// // Will throw if the test code has didn't call HasInventory, even 
		/// // that expectation was not marked as verifiable.
		/// mock.VerifyAll();
		/// </code>
		/// </example>
		/// <exception cref="MockException">At least one expectation was not met.</exception>
		public void VerifyAll()
		{
			try
			{
				interceptor.VerifyAll();
			}
			catch (Exception ex)
			{
				// Rethrow resetting the call-stack so that 
				// callers see the exception as happening at 
				// this call site.
				throw ex;
			}
		}

		private IProxyCall ExpectImpl(
			Expression expression,
			ExpectKind expectKind,
			Func<Expression, MethodInfo, Expression[], IProxyCall> factory)
		{
			Guard.ArgumentNotNull(expression, "expression");

			LambdaExpression lambda = (LambdaExpression)expression;
			// Remove convert expressions which are passed-in by the MockProtectedExtensions.
			var convert = lambda.Body as System.Linq.Expressions.UnaryExpression;
			if (convert != null && convert.NodeType == ExpressionType.Convert)
				lambda = Expression.Lambda(convert.Operand, lambda.Parameters.ToArray());

			IProxyCall result = null;

			// Verify kind of Expect being used.
			switch (expectKind)
			{
				case ExpectKind.MethodOrPropertyGet:
					{
						var methodCall = lambda.Body as MethodCallExpression;
						var memberExpr = lambda.Body as MemberExpression;

						MethodInfo method;
						Expression[] args = new Expression[0];

						if (methodCall != null)
						{
							method = methodCall.Method;
							args = methodCall.Arguments.ToArray();
						}
						else if (memberExpr != null && memberExpr.Member is PropertyInfo)
						{
							var prop = (PropertyInfo)memberExpr.Member;
							ThrowIfPropertyNotReadable(prop);

							method = prop.GetGetMethod(true);
						}
						else
						{
							throw new MockException(MockException.ExceptionReason.ExpectedMethodOrProperty,
								String.Format(Properties.Resources.ExpressionNotMethodOrProperty, expression.ToStringFixed()));
						}

						ThrowIfCantOverride(expression, method);
						result = factory(expression, method, args);
						interceptor.AddCall(result, expectKind);
						break;
					}
				case ExpectKind.PropertyGet:
					{
						if (IsPropertyIndexer(lambda.Body))
						{
							return ExpectImpl(expression, ExpectKind.MethodOrPropertyGet, factory);
						}
						else
						{
							ThrowIfNotProperty(lambda.Body);
							var prop = (PropertyInfo)((MemberExpression)lambda.Body).Member;

							ThrowIfPropertyNotReadable(prop);

							var propertyMethod = prop.GetGetMethod(true);
							ThrowIfCantOverride(expression, propertyMethod);
							result = factory(expression, propertyMethod, new Expression[0]);
							interceptor.AddCall(result, expectKind);

							break;
						}
					}
				case ExpectKind.PropertySet:
					{
						ThrowIfNotProperty(lambda.Body);
						var prop = (PropertyInfo)((MemberExpression)lambda.Body).Member;

						if (!prop.CanWrite)
						{
							throw new ArgumentException(String.Format(
								Properties.Resources.PropertyNotWritable,
								prop.DeclaringType.Name,
								prop.Name), "expression");
						}

						var propertyMethod = prop.GetSetMethod(true);
						ThrowIfCantOverride(expression, propertyMethod);
						result = factory(expression, propertyMethod, new Expression[0]);
						interceptor.AddCall(result, expectKind);
						break;
					}
				default:
					throw new NotSupportedException(expression.ToStringFixed());
			}


			if (result == null)
			{
				throw new NotSupportedException(expression.ToStringFixed());
			}

			return result;
		}

		private bool IsPropertyIndexer(Expression expression)
		{
			var call = expression as MethodCallExpression;

			return call != null && call.Method.IsSpecialName;
		}

		private static void ThrowIfPropertyNotReadable(PropertyInfo prop)
		{
			// If property is not readable, the compiler won't let 
			// the user to specify it in the lambda :)
			// This is just reassuring that in case they build the 
			// expression tree manually?
			if (!prop.CanRead)
			{
				throw new MockException(MockException.ExceptionReason.ExpectedProperty,
					String.Format(
					Properties.Resources.PropertyNotReadable,
					prop.DeclaringType.Name,
					prop.Name));
			}
		}

		private static void ThrowIfNotProperty(Expression expression)
		{
			var prop = expression as MemberExpression;
			if (prop != null && prop.Member is PropertyInfo)
				return;

			throw new MockException(MockException.ExceptionReason.ExpectedProperty,
				String.Format(Properties.Resources.ExpressionNotProperty, expression.ToStringFixed()));
		}

		private void ThrowIfCantOverride(Expression expectation, MethodInfo methodInfo)
		{
			if (!methodInfo.IsVirtual && !methodInfo.DeclaringType.IsMarshalByRef)
				throw new ArgumentException(
					String.Format(Properties.Resources.ExpectationOnNonOverridableMember,
					expectation.ToString()));
		}

		// NOTE: known issue. See https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=318122
		//public static implicit operator TInterface(Mock<T> mock)
		//{
		//    // TODO: doesn't work as expected but ONLY with interfaces :S
		//    return mock.Object;
		//}

		//public static explicit operator TInterface(Mock<T> mock)
		//{
		//    // TODO: doesn't work as expected but ONLY with interfaces :S
		//    throw new NotImplementedException();
		//}
	}

	/// <summary>
	/// Static methods that apply to mocked objects, such as <see cref="Get"/> to 
	/// retrieve a <see cref="Mock{T}"/> from an object instance.
	/// </summary>
	public static class Mock
	{
		/// <summary>
		/// Retrieves the mock object for the given object instance.
		/// </summary>
		/// <typeparam name="T">Type of the mock to retrieve. Can be omitted as it's inferred 
		/// from the object instance passed in as the <paramref name="mocked"/> instance.</typeparam>
		/// <param name="mocked">The instance of the mocked object.</param>
		/// <returns>The mock associated with the mocked object.</returns>
		/// <exception cref="ArgumentException">The received <paramref name="mocked"/> instance 
		/// was not created by Moq.</exception>
		/// <example>
		/// The following example shows how to add a new expectation to an object 
		/// instance which is not the original <see cref="Mock{T}"/> but rather 
		/// the object associated with it:
		/// <code>
		/// // Typed instance, not the mock, is retrieved from some API.
		/// HttpContextBase context = GetMockContext();
		/// 
		/// // context.Request is the typed object from the "real" API
		/// // so in order to add an expectation to it, we need to get 
		/// // the mock that's managing them
		/// Mock&lt;HttpRequestBase&gt; request = Mock.Get(context.Request);
		/// mock.Expect(req => req.AppRelativeCurrentExecutionFilePath)
		///     .Returns(tempUrl);
		/// </code>
		/// </example>
		public static Mock<T> Get<T>(T mocked)
			where T : class
		{
			if (mocked is IMocked<T>)
			{
				return (mocked as IMocked<T>).Mock;
			}
			else
			{
				throw new ArgumentException(Properties.Resources.ObjectInstanceNotMock, "mocked");
			}
		}
	}
}