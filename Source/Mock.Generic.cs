using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Castle.DynamicProxy;
using Moq.Language.Flow;
using System.Linq.Expressions;
using Castle.Core.Interceptor;

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
	public class Mock<T> : Mock, IVerifiable, IHideObjectMembers, ISetExpectations<T>
		where T : class
	{
		static readonly ProxyGenerator generator = new ProxyGenerator();
		Interceptor interceptor;
		T instance;
		MockBehavior behavior;
		object[] constructorArguments;
		List<Type> interfacesTypes;

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
			this.constructorArguments = args;
			this.interfacesTypes = new List<Type> { typeof(IMocked<T>) };

			CheckParameters();
		}

		private void CheckParameters()
		{
			if (typeof(T).IsInterface)
			{
				if (this.constructorArguments.Length > 0)
					throw new ArgumentException(Properties.Resources.ConstructorArgsForInterface);
			}
			else
			{
				if (!(typeof(T).IsAbstract || !typeof(T).IsSealed))
				{
					throw new ArgumentException(Properties.Resources.InvalidMockClass);
				}
			}
		}

		private void InitializeInstance()
		{
			var mockType = typeof(T);

			try
			{
				if (mockType.IsInterface)
				{
					instance
						= (T)generator.CreateInterfaceProxyWithoutTarget(mockType, interfacesTypes.ToArray(), interceptor);
				}
				else
				{
					try
					{
						if (constructorArguments.Length > 0)
						{
							var generatedType = generator.ProxyBuilder.CreateClassProxy(mockType, interfacesTypes.ToArray(), new ProxyGenerationOptions());
							instance
								= (T)Activator.CreateInstance(generatedType,
									new object[] { new IInterceptor[] { interceptor } }.Concat(constructorArguments).ToArray());
						}
						else
						{
							instance = (T)generator.CreateClassProxy(mockType, interfacesTypes.ToArray(), interceptor);
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
				if (this.instance == null)
				{
					InitializeInstance();

				}
				return instance;
			}
		}

		/// <summary>
		/// Returns the mocked object value.
		/// </summary>
		protected override object GetObject()
		{
			return Object;
		}

		/// <devdoc>
		/// Used for testing the mock factory.
		/// </devdoc>
		internal MockBehavior Behavior { get { return behavior; } }

		/// <summary>
		/// See <see cref="ISetExpectations{T}.Expect(Expression{Action{T}})"/>.
		/// </summary>
		public IExpect Expect(Expression<Action<T>> expression)
		{
			return SetUpExpect<T>(expression, this.interceptor);
		}

		private static IExpect SetUpExpect<T1>(Expression<Action<T1>> expression, Interceptor interceptor)
		{
			Guard.ArgumentNotNull(expression, "expression");
			Guard.ArgumentNotNull(interceptor, "interceptor");

			// Made static so that it can be called from the AsInterface private 
			// class when adding interfaces via As<TInterface>

			MethodInfo method;
			Expression[] args;
			GetMethodArguments(expression, out method, out args);

			ThrowIfCantOverride(expression, method);
			var call = new MethodCall(expression, method, args);
			interceptor.AddCall(call, ExpectKind.Other);

			return call;
		}

		/// <summary>
		/// See <see cref="ISetExpectations{T}.Expect{TResult}(Expression{Func{T, TResult}})"/>.
		/// </summary>
		public IExpect<TResult> Expect<TResult>(Expression<Func<T, TResult>> expression)
		{
			return SetUpExpect(expression, this.interceptor);
		}

		private static IExpect<TResult> SetUpExpect<T1, TResult>(Expression<Func<T1, TResult>> expression, Interceptor interceptor)
		{
			Guard.ArgumentNotNull(expression, "expression");
			Guard.ArgumentNotNull(interceptor, "interceptor");

			// Made static so that it can be called from the AsInterface private 
			// class when adding interfaces via As<TInterface>

			MethodInfo method;
			Expression[] args;
			GetMethodOrPropertyArguments<T1, TResult>(expression, out method, out args);

			ThrowIfCantOverride(expression, method);
			var call = new MethodCallReturn<TResult>(expression, method, args);
			interceptor.AddCall(call, ExpectKind.Other);

			return call;
		}

		/// <summary>
		/// See <see cref="ISetExpectations{T}.ExpectGet{TProperty}(Expression{Func{T, TProperty}})"/>.
		/// </summary>
		public IExpectGetter<TProperty> ExpectGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			return SetUpExpectGet(expression, this.interceptor);
		}

		private static IExpectGetter<TProperty> SetUpExpectGet<T1, TProperty>(Expression<Func<T1, TProperty>> expression, Interceptor interceptor)
		{
			// Made static so that it can be called from the AsInterface private 
			// class when adding interfaces via As<TInterface>

			Guard.ArgumentNotNull(expression, "expression");
			Guard.ArgumentNotNull(interceptor, "interceptor");
			LambdaExpression lambda = GetLambda(expression);

			if (IsPropertyIndexer(lambda.Body))
			{
				// Treat indexers as regular method invocations.
				return (IExpectGetter<TProperty>)SetUpExpect<T1, TProperty>(expression, interceptor);
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
		/// See <see cref="ISetExpectations{T}.ExpectSet{TProperty}(Expression{Func{T, TProperty}})"/>.
		/// </summary>
		public IExpectSetter<TProperty> ExpectSet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			return SetUpExpectSet<T, TProperty>(expression, this.interceptor);
		}

		private static IExpectSetter<TProperty> SetUpExpectSet<T1, TProperty>(Expression<Func<T1, TProperty>> expression, Interceptor interceptor)
		{
			Guard.ArgumentNotNull(expression, "expression");
			Guard.ArgumentNotNull(interceptor, "interceptor");

			// Made static so that it can be called from the AsInterface private 
			// class when adding interfaces via As<TInterface>

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
			GetMethodOrPropertyArguments<T, TResult>(expression, out method, out args);

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

		/// <summary>
		/// Adds an interface implementation to the mock, 
		/// allowing expectations to be set for it.
		/// </summary>
		/// <remarks>
		/// This method can only be called before the first use 
		/// of the mock <see cref="Object"/> property, at which 
		/// point the runtime type has already been generated 
		/// and no more interfaces can be added to it.
		/// <para>
		/// Also, <typeparamref name="TInterface"/> must be an 
		/// interface and not a class, which must be specified 
		/// when creating the mock instead.
		/// </para>
		/// </remarks>
		/// <exception cref="InvalidOperationException">The mock type 
		/// has already been generated by accessing the <see cref="Object"/> property.</exception>
		/// <exception cref="ArgumentException">The <typeparamref name="TInterface"/> specified 
		/// is not an interface.</exception>
		/// <example>
		/// The following example creates a mock for the main interface 
		/// and later adds <see cref="IDisposable"/> to it to verify 
		/// it's called by the consumer code:
		/// <code>
		/// var mock = new Mock&lt;IProcessor&gt;();
		/// mock.Expect(x =&gt; x.Execute("ping"));
		/// 
		/// // add IDisposable interface
		/// var disposable = mock.As&lt;IDisposable&gt;();
		/// disposable.Expect(d => d.Dispose()).Verifiable();
		/// </code>
		/// </example>
		public virtual ISetExpectations<TInterface> As<TInterface>()
			where TInterface : class
		{
			if (this.instance != null)
			{
				throw new InvalidOperationException(Properties.Resources.AlreadyInitialized);
			}
			if (!typeof(TInterface).IsInterface)
			{
				throw new ArgumentException(Properties.Resources.AsMustBeInterface);
			}

			if (!this.interfacesTypes.Contains(typeof(TInterface)))
			{
				this.interfacesTypes.Add(typeof(TInterface));
			}

			return new AsInterface<TInterface>(this);
		}

		private static LambdaExpression GetLambda(Expression expression)
		{
			LambdaExpression lambda = (LambdaExpression)expression;
			// Remove convert expressions which are passed-in by the MockProtectedExtensions.
			// They are passed because LambdaExpression constructor checks the type of 
			// the returned values, even if the return type is Object and everything 
			// is able to convert to it. It forces you to be explicit about the conversion.
			var convert = lambda.Body as UnaryExpression;
			if (convert != null && convert.NodeType == ExpressionType.Convert)
				lambda = Expression.Lambda(convert.Operand, lambda.Parameters.ToArray());
			return lambda;
		}

		private static void GetMethodArguments<T1>(Expression<Action<T1>> expression, out MethodInfo method, out Expression[] args)
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

		private static void GetMethodOrPropertyArguments<T1, TResult>(Expression<Func<T1, TResult>> expression, out MethodInfo method, out Expression[] args)
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

		private static MethodInfo GetProperySetter<T1, TProperty>(Expression<Func<T1, TProperty>> expression)
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

		private static bool IsPropertyIndexer(Expression expression)
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

		private static void ThrowIfCantOverride(Expression expectation, MethodInfo methodInfo)
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

		private class AsInterface<TInterface> : ISetExpectations<TInterface>
			where TInterface : class
		{
			Mock<T> owner;

			public AsInterface(Mock<T> owner)
			{
				this.owner = owner;
			}

			public IExpect<TResult> Expect<TResult>(Expression<Func<TInterface, TResult>> expression)
			{
				return Mock<T>.SetUpExpect(expression, this.owner.interceptor);
			}

			public IExpect Expect(Expression<Action<TInterface>> expression)
			{
				return Mock<T>.SetUpExpect(expression, this.owner.interceptor);
			}

			public IExpectGetter<TProperty> ExpectGet<TProperty>(Expression<Func<TInterface, TProperty>> expression)
			{
				return Mock<T>.SetUpExpectGet(expression, this.owner.interceptor);
			}

			public IExpectSetter<TProperty> ExpectSet<TProperty>(Expression<Func<TInterface, TProperty>> expression)
			{
				return Mock<T>.SetUpExpectSet(expression, this.owner.interceptor);
			}
		}
	}
}
