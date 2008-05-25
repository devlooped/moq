using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Moq.Language.Flow;

namespace Moq
{
	// TODO: uncomment documentation when C# bug is fixed: 
	///// <typeparam name="T">Type to mock, which can be an interface or a class.</typeparam>
	/// <summary>
	/// Provides a mock implementation of <typeparamref name="T"/>.
	/// </summary>
	/// <remarks>
	/// Only abstract and virtual members of classes can be mocked.
	/// <para>
	/// The behavior of the mock with regards to the expectations and the actual calls is determined 
	/// by the optional <see cref="MockBehavior"/> that can be passed to the <see cref="Mock{T}(MockBehavior)"/> 
	/// constructor.
	/// </para>
	/// </remarks>
	/// <example group="overview" order="0">
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
	/// Assert.True(order.IsFilled);
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
	/// mock.Expect(x => x.HasInventory(
	///			It.IsAny&lt;string&gt;(), 
	///			It.IsInRange(0, 100, Range.Inclusive)))
	///     .Returns(false);
	/// 
	/// //shows how to throw for unexpected calls. contrast with the "verify" approach of other mock libraries.
	/// mock.Expect(x => x.Remove(
	///			It.IsAny&lt;string&gt;(), 
	///			It.IsAny&lt;int&gt;()))
	///     .Throws(new InvalidOperationException());
	/// 
	/// //exercise
	/// order.Fill(mock.Object);
	/// 
	/// //verify
	/// Assert.False(order.IsFilled);
	/// </code>
	/// </example>
	public class Mock<T> : Mock, IVerifiable, IHideObjectMembers
		where T : class
	{
		static readonly ProxyGenerator generator = new ProxyGenerator();
		Interceptor interceptor;
		T instance;
		MockBehavior behavior;

		/// <summary>
		/// Initializes an instance of the mock with a specific <see cref="MockBehavior">behavior</see> with 
		/// the given constructor arguments for the class.
		/// </summary>
		/// <remarks>
		/// The mock will try to find the best match constructor given the constructor arguments, and invoke that 
		/// to initialize the instance. This applies only to classes, not interfaces.
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
				if (typeof(T).IsInterface)
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
		public new T Object
		{
			get
			{
				return instance;
			}
		}

		/// <summary>
		/// Returns the mocked object value.
		/// </summary>
		protected override object GetObject()
		{
			return instance;
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
		/// <example group="expectations">
		/// <code>
		/// var mock = new Mock&lt;IProcessor&gt;();
		/// mock.Expect(x =&gt; x.Execute("ping"));
		/// </code>
		/// </example>
		public IExpect Expect(Expression<Action<T>> expression)
		{
			MethodInfo method;
			Expression[] args;
			GetMethodArguments(expression, out method, out args);

			ThrowIfCantOverride(expression, method);
			var call = new MethodCall(expression, method, args);
			interceptor.AddCall(call, ExpectKind.Other);

			return call;
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
		/// <example group="expectations">
		/// <code>
		/// mock.Expect(x =&gt; x.HasInventory("Talisker", 50)).Returns(true);
		/// </code>
		/// </example>
		public IExpect<TResult> Expect<TResult>(Expression<Func<T, TResult>> expression)
		{
			MethodInfo method;
			Expression[] args;
			GetMethodOrPropertyArguments<TResult>(expression, out method, out args);

			ThrowIfCantOverride(expression, method);
			var call = new MethodCallReturn<TResult>(expression, method, args);
			interceptor.AddCall(call, ExpectKind.Other);

			return call;
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
		/// <example group="expectations">
		/// <code>
		/// mock.ExpectGet(x =&gt; x.Suspended)
		///     .Returns(true);
		/// </code>
		/// </example>
		public IExpectGetter<TProperty> ExpectGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			Guard.ArgumentNotNull(expression, "expression");
			LambdaExpression lambda = GetLambda(expression);

			if (IsPropertyIndexer(lambda.Body))
			{
				// Treat indexers as regular method invocations.
				return (IExpectGetter<TProperty>)Expect(expression);
			}
			else
			{
				ThrowIfNotProperty(lambda.Body);
				var prop = (PropertyInfo)((MemberExpression)lambda.Body).Member;

				ThrowIfPropertyNotReadable(prop);

				var propGet = prop.GetGetMethod(true);
				ThrowIfCantOverride(expression, propGet);

				var call = new MethodCallReturn<TProperty>(expression, propGet, new Expression[0]);
				interceptor.AddCall(call, ExpectKind.Other);

				return call;
			}
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
		/// <example group="expectations">
		/// <code>
		/// mock.ExpectSet(x =&gt; x.Suspended);
		/// </code>
		/// </example>
		public IExpectSetter<TProperty> ExpectSet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			var propSet = GetProperySetter(expression);
			ThrowIfCantOverride(expression, propSet);
			var call = new MethodCall<TProperty>(expression, propSet, new Expression[0]);
			interceptor.AddCall(call, ExpectKind.PropertySet);

			return call;
		}

		/// <summary>
		/// Verifies that a specific invocation matching the given 
		/// expression was performed on the mock. Use in conjuntion 
		/// with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, 
		/// and later we want to verify that a given invocation 
		/// with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IProcessor&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't call Execute with a "ping" string argument.
		/// mock.Verify(proc =&gt; proc.Execute("ping"));
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		public virtual void Verify(Expression<Action<T>> expression)
		{
			MethodInfo method;
			Expression[] args;
			GetMethodArguments(expression, out method, out args);

			var expected = new MethodCall(expression, method, args);
			var actual = interceptor.ActualCalls.FirstOrDefault(i => expected.Matches(i));

			if (actual == null)
				throw new MockException(MockException.ExceptionReason.VerificationFailed,
					Properties.Resources.NoMatchingCall);
		}

		/// <summary>
		/// Verifies that a specific invocation matching the given 
		/// expression was performed on the mock. Use in conjuntion 
		/// with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, 
		/// and later we want to verify that a given invocation 
		/// with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't call HasInventory.
		/// mock.Verify(warehouse =&gt; warehouse.HasInventory(TALISKER, 50));
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		public virtual void Verify<TResult>(Expression<Func<T, TResult>> expression)
		{
			MethodInfo method;
			Expression[] args;
			GetMethodOrPropertyArguments<TResult>(expression, out method, out args);

			var expected = new MethodCallReturn<TResult>(expression, method, args);
			var actual = interceptor.ActualCalls.FirstOrDefault(i => expected.Matches(i));

			if (actual == null)
				throw new MockException(MockException.ExceptionReason.VerificationFailed,
					Properties.Resources.NoMatchingCall);
		}

		/// <summary>
		/// Verifies that a property was read on the mock. 
		/// Use in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, 
		/// and later we want to verify that a given property 
		/// was retrieved from it:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't retrieve the IsClosed property.
		/// mock.VerifyGet(warehouse =&gt; warehouse.IsClosed);
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		public virtual void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			// Just for consistency with the Expect/ExpectGet pair.
			Verify(expression);
		}

		/// <summary>
		/// Verifies that a property has been set on the mock. 
		/// Use in conjuntion with the default <see cref="MockBehavior.Loose"/>.
		/// </summary>
		/// <example group="verification">
		/// This example assumes that the mock has been used, 
		/// and later we want to verify that a given invocation 
		/// with specific parameters was performed:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// // exercise mock
		/// //...
		/// // Will throw if the test code didn't set the IsClosed property.
		/// mock.VerifySet(warehouse =&gt; warehouse.IsClosed);
		/// </code>
		/// </example>
		/// <exception cref="MockException">The invocation was not performed on the mock.</exception>
		public virtual void VerifySet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			var propSet = GetProperySetter(expression);
			var expected = new MethodCall<TProperty>(expression, propSet, new Expression[0]);

			var actual = interceptor.ActualCalls.FirstOrDefault(i => expected.Matches(i));

			if (actual == null)
				throw new MockException(MockException.ExceptionReason.VerificationFailed,
					Properties.Resources.NoMatchingCall);
		}


		/// <summary>
		/// Verifies that all verifiable expectations have been met.
		/// </summary>
		/// <example group="verification">
		/// This example sets up an expectation and marks it as verifiable. After 
		/// the mock is used, a <see cref="Verify()"/> call is issued on the mock 
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
		public virtual void Verify()
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
		/// <example group="verification">
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
		public virtual void VerifyAll()
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

		private static LambdaExpression GetLambda(Expression expression)
		{
			LambdaExpression lambda = (LambdaExpression)expression;
			// Remove convert expressions which are passed-in by the MockProtectedExtensions.
			// They are passed because LambdaExpression constructor checks the type of 
			// the returned values, even if the return type is Object and everything 
			// is able to convert to it. It forces you to be explicit about the conversion.
			var convert = lambda.Body as System.Linq.Expressions.UnaryExpression;
			if (convert != null && convert.NodeType == ExpressionType.Convert)
				lambda = Expression.Lambda(convert.Operand, lambda.Parameters.ToArray());
			return lambda;
		}

		private static void GetMethodArguments(Expression<Action<T>> expression, out MethodInfo method, out Expression[] args)
		{
			Guard.ArgumentNotNull(expression, "expression");
			LambdaExpression lambda = GetLambda(expression);

			var methodCall = lambda.Body as MethodCallExpression;
			args = new Expression[0];

			if (methodCall != null)
			{
				method = methodCall.Method;
				args = methodCall.Arguments.ToArray();
			}
			else
			{
				throw new MockException(MockException.ExceptionReason.ExpectedMethod,
					String.Format(Properties.Resources.ExpressionNotMethod, expression.ToStringFixed()));
			}
		}

		private static void GetMethodOrPropertyArguments<TResult>(Expression<Func<T, TResult>> expression, out MethodInfo method, out Expression[] args)
		{
			Guard.ArgumentNotNull(expression, "expression");
			LambdaExpression lambda = GetLambda(expression);

			var methodCall = lambda.Body as MethodCallExpression;
			var memberExpr = lambda.Body as MemberExpression;

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
				args = new Expression[0];
			}
			else
			{
				throw new MockException(MockException.ExceptionReason.ExpectedMethodOrProperty,
					String.Format(Properties.Resources.ExpressionNotMethodOrProperty, expression.ToStringFixed()));
			}
		}

		private static MethodInfo GetProperySetter<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			Guard.ArgumentNotNull(expression, "expression");
			LambdaExpression lambda = GetLambda(expression);

			ThrowIfNotProperty(lambda.Body);
			var prop = (PropertyInfo)((MemberExpression)lambda.Body).Member;

			if (!prop.CanWrite)
			{
				throw new ArgumentException(String.Format(
					Properties.Resources.PropertyNotWritable,
					prop.DeclaringType.Name,
					prop.Name), "expression");
			}

			return prop.GetSetMethod(true);
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
			if (!methodInfo.IsVirtual || methodInfo.IsFinal || methodInfo.IsPrivate)
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
	public abstract class Mock : IHideObjectMembers
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
		/// <example group="advanced">
		/// The following example shows how to add a new expectation to an object 
		/// instance which is not the original <see cref="Mock{T}"/> but rather 
		/// the object associated with it:
		/// <code>
		/// // Typed instance, not the mock, is retrieved from some test API.
		/// HttpContextBase context = GetMockContext();
		/// 
		/// // context.Request is the typed object from the "real" API
		/// // so in order to add an expectation to it, we need to get 
		/// // the mock that "owns" it
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

		Dictionary<EventInfo, List<Delegate>> invocationLists = new Dictionary<EventInfo, List<Delegate>>();

		/// <summary>
		/// Exposes the mocked object instance.
		/// </summary>
		public object Object { get { return GetObject(); } }

		/// <summary>
		/// Returns the mocked object value.
		/// </summary>
		protected abstract object GetObject();

		internal void AddEventHandler(EventInfo ev, Delegate handler)
		{
			List<Delegate> handlers;
			if (!invocationLists.TryGetValue(ev, out handlers))
			{
				handlers = new List<Delegate>();
				invocationLists.Add(ev, handlers);
			}

			handlers.Add(handler);
		}

		internal IEnumerable<Delegate> GetInvocationList(EventInfo ev)
		{
			List<Delegate> handlers;
			if (!invocationLists.TryGetValue(ev, out handlers))
				return new Delegate[0];
			else
				return handlers;
		}

		/// <summary>
		/// Creates a handler that can be associated to an event receiving 
		/// the given <typeparamref name="TEventArgs"/> and can be used 
		/// to raise the event.
		/// </summary>
		/// <typeparam name="TEventArgs">Type of <see cref="EventArgs"/> 
		/// data passed in to the event.</typeparam>
		/// <example>
		/// This example shows how to invoke an event with a custom event arguments 
		/// class in a view that will cause its corresponding presenter to 
		/// react by changing its state:
		/// <code>
		/// var mockView = new Mock&lt;IOrdersView&gt;();
		/// var mockedEvent = mockView.CreateEventHandler&lt;OrderEventArgs&gt;();
		/// 
		/// var presenter = new OrdersPresenter(mockView.Object);
		/// 
		/// // Check that the presenter has no selection by default
		/// Assert.Null(presenter.SelectedOrder);
		/// 
		/// // Create a mock event handler of the appropriate type
		/// var handler = mockView.CreateEventHandler&lt;OrderEventArgs&gt;();
		/// // Associate it with the event we want to raise
		/// mockView.Object.Cancel += handler;
		/// // Finally raise the event with a specific arguments data
		/// handler.Raise(new OrderEventArgs { Order = new Order("moq", 500) });
		/// 
		/// // Now the presenter reacted to the event, and we have a selected order
		/// Assert.NotNull(presenter.SelectedOrder);
		/// Assert.Equal("moq", presenter.SelectedOrder.ProductName);
		/// </code>
		/// </example>
		public MockedEvent<TEventArgs> CreateEventHandler<TEventArgs>() where TEventArgs : EventArgs
		{
			return new MockedEvent<TEventArgs>(this);
		}

		/// <summary>
		/// Creates a handler that can be associated to an event receiving 
		/// a generic <see cref="EventArgs"/> and can be used 
		/// to raise the event.
		/// </summary>
		/// <example>
		/// This example shows how to invoke a generic event in a view that will 
		/// cause its corresponding presenter to react by changing its state:
		/// <code>
		/// var mockView = new Mock&lt;IOrdersView&gt;();
		/// var mockedEvent = mockView.CreateEventHandler();
		/// 
		/// var presenter = new OrdersPresenter(mockView.Object);
		/// 
		/// // Check that the presenter is not in the "Canceled" state
		/// Assert.False(presenter.IsCanceled);
		/// 
		/// // Create a mock event handler of the appropriate type
		/// var handler = mockView.CreateEventHandler();
		/// // Associate it with the event we want to raise
		/// mockView.Object.Cancel += handler;
		/// // Finally raise the event
		/// handler.Raise(EventArgs.Empty);
		/// 
		/// // Now the presenter reacted to the event, and changed its state
		/// Assert.True(presenter.IsCanceled);
		/// </code>
		/// </example>
		public MockedEvent<EventArgs> CreateEventHandler()
		{
			return new MockedEvent<EventArgs>(this);
		}

		class NullMockedEvent : MockedEvent
		{
			public NullMockedEvent(Mock mock)
				: base(mock)
			{
			}
		}
	}
}
