//Copyright (c) 2007, Moq Team 
//http://code.google.com/p/moq/
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of the <ORGANIZATION> nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Moq.Language.Flow;

namespace Moq
{
	/// <summary>
	/// Core implementation of the <see cref="IMock{T}"/> interface. 
	/// </summary>
	/// <seealso cref="IMock{T}"/>
	/// <typeparam name="T">Type to mock.</typeparam>
	public class Mock<T> : Mock, IVerifiable, IMock<T>
		where T : class
	{
		static readonly ProxyGenerator generator = new ProxyGenerator();
		Interceptor interceptor;
		T instance;
		MockBehavior behavior;
		object[] constructorArguments;

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
		/// <param name="behavior">Behavior of the mock.</param>
		/// <param name="args">Optional constructor arguments if the mocked type is a class.</param>
		public Mock(MockBehavior behavior, params object[] args)
		{
			if (args == null) args = new object[0];

			this.behavior = behavior;
			interceptor = new Interceptor(behavior, typeof(T), this);
			this.constructorArguments = args;
			this.ImplementedInterfaces.Add(typeof(IMocked<T>));

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
						= (T)generator.CreateInterfaceProxyWithoutTarget(mockType, base.ImplementedInterfaces.ToArray(), interceptor);
				}
				else
				{
					try
					{
						if (constructorArguments.Length > 0)
						{
							var generatedType = generator.ProxyBuilder.CreateClassProxy(mockType, base.ImplementedInterfaces.ToArray(), new ProxyGenerationOptions());
							instance
								= (T)Activator.CreateInstance(generatedType,
									new object[] { new IInterceptor[] { interceptor } }.Concat(constructorArguments).ToArray());
						}
						else
						{
							instance = (T)generator.CreateClassProxy(mockType, base.ImplementedInterfaces.ToArray(), interceptor);
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
		/// <param name="args">Optional constructor arguments if the mocked type is a class.</param>
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
		/// <param name="behavior">Behavior of the mock.</param>
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
		/// Implements <see cref="IMock{T}.Expect(Expression{Action{T}})"/>.
		/// </summary>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
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
		/// Implements <see cref="IMock{T}.Expect{TResult}(Expression{Func{T, TResult}})"/>.
		/// </summary>
		/// <typeparam name="TResult">Type of the return value. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <param name="expression">Lambda expression that specifies the expected method invocation.</param>
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
		/// Implements <see cref="IMock{T}.ExpectGet{TProperty}(Expression{Func{T, TProperty}})"/>.
		/// </summary>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <param name="expression">Lambda expression that specifies the expected property getter.</param>
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
		/// Implements <see cref="IMock{T}.ExpectSet{TProperty}(Expression{Func{T, TProperty}})"/>.
		/// </summary>
		/// <typeparam name="TProperty">Type of the property. Typically omitted as it can be inferred from the expression.</typeparam>
		/// <param name="expression">Lambda expression that specifies the expected property setter.</param>
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
		/// Implements <see cref="IMock{T}.Verify(Expression{Action{T}})"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		public virtual void Verify(Expression<Action<T>> expression)
		{
			Verify(expression, interceptor);
		}

		private static void Verify(Expression<Action<T>> expression, Interceptor interceptor)
		{
			// Made static so that it can be called from the AsInterface private class

			Guard.ArgumentNotNull(expression, "expression");
			Guard.ArgumentNotNull(interceptor, "interceptor");

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
		/// Implements <see cref="IMock{T}.Verify{TResult}(Expression{Func{T, TResult}})"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TResult">Type of return value from the expression.</typeparam>
		public virtual void Verify<TResult>(Expression<Func<T, TResult>> expression)
		{
			Verify(expression, interceptor);
		}

		private static void Verify<TResult>(Expression<Func<T, TResult>> expression, Interceptor interceptor)
		{
			// Made static so that it can be called from the AsInterface private class

			Guard.ArgumentNotNull(expression, "expression");
			Guard.ArgumentNotNull(interceptor, "interceptor");

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
		/// Implements <see cref="IMock{T}.VerifyGet{TProperty}(Expression{Func{T, TProperty}})"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can 
		/// be inferred from the expression's return type.</typeparam>
		public virtual void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			Verify(expression, interceptor);
		}

		private static void VerifyGet<TProperty>(Expression<Func<T, TProperty>> expression, Interceptor interceptor)
		{
			// Made static so that it can be called from the AsInterface private class

			// Just for consistency with the Expect/ExpectGet pair.
			Verify(expression, interceptor);
		}

		/// <summary>
		/// Implements <see cref="IMock{T}.VerifySet{TProperty}(Expression{Func{T, TProperty}})"/>.
		/// </summary>
		/// <param name="expression">Expression to verify.</param>
		/// <typeparam name="TProperty">Type of the property to verify. Typically omitted as it can 
		/// be inferred from the expression's return type.</typeparam>
		public virtual void VerifySet<TProperty>(Expression<Func<T, TProperty>> expression)
		{
			VerifySet(expression, interceptor);
		}

		private static void VerifySet<TProperty>(Expression<Func<T, TProperty>> expression, Interceptor interceptor)
		{
			// Made static so that it can be called from the AsInterface private class

			Guard.ArgumentNotNull(expression, "expression");
			Guard.ArgumentNotNull(interceptor, "interceptor");


			var propSet = GetProperySetter(expression);
			var expected = new MethodCall<TProperty>(expression, propSet, new Expression[0]);

			var actual = interceptor.ActualCalls.FirstOrDefault(i => expected.Matches(i));

			if (actual == null)
				throw new MockException(MockException.ExceptionReason.VerificationFailed,
					Properties.Resources.NoMatchingCall);
		}

		/// <summary>
		/// Implements <see cref="IMock{T}.Verify()"/>.
		/// </summary>
		public virtual void Verify()
		{
			Verify(interceptor);
		}

		private static void Verify(Interceptor interceptor)
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
		/// Implements <see cref="IMock{T}.VerifyAll()"/>.
		/// </summary>
		public virtual void VerifyAll()
		{
			VerifyAll(interceptor);
		}

		private static void VerifyAll(Interceptor interceptor)
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
		/// Implements <see cref="IMock{T}.As{TInterface}"/>.
		/// </summary>
		/// <typeparam name="TInterface">Type of interface to cast the mock to.</typeparam>
		public virtual IMock<TInterface> As<TInterface>()
			where TInterface : class
		{
			if (this.instance != null && !base.ImplementedInterfaces.Contains(typeof(TInterface)))
			{
				throw new InvalidOperationException(Properties.Resources.AlreadyInitialized);
			}
			if (!typeof(TInterface).IsInterface)
			{
				throw new ArgumentException(Properties.Resources.AsMustBeInterface);
			}

			if (!base.ImplementedInterfaces.Contains(typeof(TInterface)))
			{
				base.ImplementedInterfaces.Add(typeof(TInterface));
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

		private class AsInterface<TInterface> : IMock<TInterface>
			where TInterface : class
		{
			Mock<T> owner;

			public AsInterface(Mock<T> owner)
			{
				this.owner = owner;
			}

			public IMock<TNewInterface> As<TNewInterface>() where TNewInterface : class
			{
				return owner.As<TNewInterface>();
			}

			public TInterface Object
			{
				get { return owner.Object as TInterface; }
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

			public void Verify()
			{
				Mock<T>.Verify(owner.interceptor);
			}

			public void VerifyAll()
			{
				Mock<T>.VerifyAll(owner.interceptor);
			}

			public void Verify(Expression<Action<TInterface>> expression)
			{
				Mock<TInterface>.Verify(expression, owner.interceptor);
			}

			public void Verify<TResult>(Expression<Func<TInterface, TResult>> expression)
			{
				Mock<TInterface>.Verify<TResult>(expression, owner.interceptor);
			}

			public void VerifyGet<TProperty>(Expression<Func<TInterface, TProperty>> expression)
			{
				Mock<TInterface>.VerifyGet<TProperty>(expression, owner.interceptor);
			}

			public void VerifySet<TProperty>(Expression<Func<TInterface, TProperty>> expression)
			{
				Mock<TInterface>.VerifySet<TProperty>(expression, owner.interceptor);
			}
		}
	}
}
