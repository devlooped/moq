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

//    * Neither the name of the Moq Team nor the 
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Indy.IL2CPU.IL;
using Moq.Properties;

namespace Moq
{
	/// <summary>
	/// Base class for mocks and static helper class with methods that 
	/// apply to mocked objects, such as <see cref="Get"/> to 
	/// retrieve a <see cref="Mock{T}"/> from an object instance.
	/// </summary>
	public abstract partial class Mock : IHideObjectMembers
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
		/// The following example shows how to add a new setup to an object 
		/// instance which is not the original <see cref="Mock{T}"/> but rather 
		/// the object associated with it:
		/// <code>
		/// // Typed instance, not the mock, is retrieved from some test API.
		/// HttpContextBase context = GetMockContext();
		/// 
		/// // context.Request is the typed object from the "real" API
		/// // so in order to add a setup to it, we need to get 
		/// // the mock that "owns" it
		/// Mock&lt;HttpRequestBase&gt; request = Mock.Get(context.Request);
		/// mock.Setup(req => req.AppRelativeCurrentExecutionFilePath)
		///     .Returns(tempUrl);
		/// </code>
		/// </example>
		public static Mock<T> Get<T>(T mocked)
			where T : class
		{
			var mockedOfT = mocked as IMocked<T>;
			var mockedPlain = mocked as IMocked;
			if (mockedOfT != null)
			{
				// This would be the fastest check.
				return mockedOfT.Mock;
			}
			else if (mockedPlain != null)
			{
				// We may have received a T of an implemented 
				// interface in the mock.
				var mock = mockedPlain.Mock;
#if SILVERLIGHT
				var imockedType = mocked.GetType().GetInterface("IMocked`1", false);
#else
				var imockedType = mocked.GetType().GetInterface("IMocked`1");
#endif
				var mockedType = imockedType.GetGenericArguments()[0];

				if (mock.ImplementedInterfaces.Contains(typeof(T)))
				{
					var asMethod = mock.GetType().GetMethod("As");
					var asInterface = asMethod.MakeGenericMethod(typeof(T));
					var asMock = asInterface.Invoke(mock, null);

					return (Mock<T>)asMock;
				}
				else
				{
					// Alternatively, we may have been asked 
					// for a type that is assignable to the 
					// one for the mock.
					// This is not valid as generic types 
					// do not support covariance on 
					// the generic parameters.
					var types = String.Join(", ",
							new[] { mockedType }
						// Skip first interface which is always our internal IMocked<T>
							.Concat(mock.ImplementedInterfaces.Skip(1))
							.Select(t => t.Name)
							.ToArray());

					throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
						Properties.Resources.InvalidMockGetType,
						typeof(T).Name, types));
				}
			}
			else
			{
				throw new ArgumentException(Properties.Resources.ObjectInstanceNotMock, "mocked");
			}
		}

		bool callBase = false;
		DefaultValue defaultValue = DefaultValue.Empty;
		IDefaultValueProvider defaultValueProvider = new EmptyDefaultValueProvider();
		List<Type> implementedInterfaces = new List<Type>();
		Dictionary<MethodInfo, Mock> innerMocks = new Dictionary<MethodInfo, Mock>();
		Dictionary<EventInfo, List<Delegate>> invocationLists = new Dictionary<EventInfo, List<Delegate>>();

		internal virtual Interceptor Interceptor { get; set; }
		internal virtual Dictionary<MethodInfo, Mock> InnerMocks { get { return innerMocks; } set { innerMocks = value; } }

		/// <summary>
		/// Exposes the list of extra interfaces implemented by the mock.
		/// </summary>
		internal List<Type> ImplementedInterfaces { get { return implementedInterfaces; } }

		/// <summary>
		/// Behavior of the mock, according to the value set in the constructor.
		/// </summary>
		public virtual MockBehavior Behavior { get; internal set; }

		/// <summary>
		/// Whether the base member virtual implementation will be called 
		/// for mocked classes if no setup is matched. Defaults to <see langword="true"/>.
		/// </summary>
		public virtual bool CallBase { get { return callBase; } set { callBase = value; } }

		/// <summary>
		/// Specifies the behavior to use when returning default values for 
		/// unexpected invocations on loose mocks.
		/// </summary>
		public virtual DefaultValue DefaultValue
		{
			get { return defaultValue; }
			set
			{
				defaultValue = value;
				if (defaultValue == DefaultValue.Mock)
					defaultValueProvider = new MockDefaultValueProvider(this);
				else
					defaultValueProvider = new EmptyDefaultValueProvider();
			}
		}

		/// <summary>
		/// Specifies the class that will determine the default 
		/// value to return when invocations are made that 
		/// have no setups and need to return a default 
		/// value (for loose mocks).
		/// </summary>
		internal IDefaultValueProvider DefaultValueProvider { get { return defaultValueProvider; } }

		/// <summary>
		/// Gets the mocked object instance.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Object", Justification = "Exposes the mocked object instance, so it's appropriate.")]
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The public Object property is the only one visible to Moq consumers. The protected member is for internal use only.")]
		public object Object { get { return GetObject(); } }

		/// <summary>
		/// Returns the mocked object value.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is actually the protected virtual implementation of the property Object.")]
		protected abstract object GetObject();

		/// <summary>
		/// Retrieves the type of the mocked object, its generic type argument.
		/// This is used in the auto-mocking of hierarchy access.
		/// </summary>
		internal abstract Type MockedType { get; }

		#region Verify

		/// <summary>
		/// Verifies that all verifiable expectations have been met.
		/// </summary>
		/// <example group="verification">
		/// This example sets up an expectation and marks it as verifiable. After 
		/// the mock is used, a <c>Verify()</c> call is issued on the mock 
		/// to ensure the method in the setup was invoked:
		/// <code>
		/// var mock = new Mock&lt;IWarehouse&gt;();
		/// this.Setup(x =&gt; x.HasInventory(TALISKER, 50)).Verifiable().Returns(true);
		/// ...
		/// // other test code
		/// ...
		/// // Will throw if the test code has didn't call HasInventory.
		/// this.Verify();
		/// </code>
		/// </example>
		/// <exception cref="MockException">Not all verifiable expectations were met.</exception>
		[SuppressMessage("Microsoft.Usage", "CA2200:RethrowToPreserveStackDetails", Justification = "We want to explicitly reset the stack trace here.")]
		public void Verify()
		{
			try
			{
				this.Interceptor.Verify();
				foreach (var inner in this.InnerMocks.Values)
				{
					inner.Verify();
				}
			}
			catch (Exception ex)
			{
				// Rethrow resetting the call-stack so that 
				// callers see the exception as happening at 
				// this call site.
				// TODO: see how to mangle the stacktrace so 
				// that the mock doesn't even show up there.
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
		/// this.Setup(x =&gt; x.HasInventory(TALISKER, 50)).Returns(true);
		/// ...
		/// // other test code
		/// ...
		/// // Will throw if the test code has didn't call HasInventory, even 
		/// // that expectation was not marked as verifiable.
		/// this.VerifyAll();
		/// </code>
		/// </example>
		/// <exception cref="MockException">At least one expectation was not met.</exception>
		[SuppressMessage("Microsoft.Usage", "CA2200:RethrowToPreserveStackDetails", Justification = "We want to explicitly reset the stack trace here.")]
		public void VerifyAll()
		{
			try
			{
				this.Interceptor.VerifyAll();
				foreach (var inner in this.InnerMocks.Values)
				{
					inner.VerifyAll();
				}
			}
			catch (Exception ex)
			{
				// Rethrow resetting the call-stack so that 
				// callers see the exception as happening at 
				// this call site.
				throw ex;
			}
		}

		internal static void Verify<T>(
			Mock mock,
			Expression<Action<T>> expression,
			Times times,
			string failMessage)
		{
			var lambda = expression.ToLambda();
			var methodCall = lambda.ToMethodCall();
			MethodInfo method = methodCall.Method;
			Expression[] args = methodCall.Arguments.ToArray();

			var expected = new MethodCall(mock, expression, method, args);
			VerifyCalls(GetInterceptor(lambda, mock), expected, expression, times, failMessage);
		}

		internal static void Verify<T, TResult>(
			Mock mock,
			Expression<Func<T, TResult>> expression,
			Times times,
			string failMessage)
		{
			var lambda = expression.ToLambda();
			if (lambda.IsProperty())
			{
				VerifyGet<T, TResult>(mock, expression, times, failMessage);
			}
			else
			{
				var methodCall = lambda.ToMethodCall();
				MethodInfo method = methodCall.Method;
				Expression[] args = methodCall.Arguments.ToArray();

				var expected = new MethodCallReturn<T, TResult>(mock, expression, method, args);
				VerifyCalls(GetInterceptor(lambda, mock), expected, expression, times, failMessage);
			}
		}

		internal static void VerifyGet<T, TProperty>(
			Mock mock,
			Expression<Func<T, TProperty>> expression,
			Times times,
			string failMessage)
		{
			var lambda = expression.ToLambda();
			var prop = lambda.ToPropertyInfo();

			var expected = new MethodCallReturn<T, TProperty>(
				mock,
				expression,
				prop.GetGetMethod(),
				new Expression[0]);
			VerifyCalls(GetInterceptor(lambda, mock), expected, expression, times, failMessage);
		}

		internal static void VerifySet<T, TProperty>(
			Mock mock,
			Expression<Func<T, TProperty>> expression,
			Times times,
			string failMessage)
		{
			var lambda = expression.ToLambda();
			var prop = lambda.ToPropertyInfo();

			var expected = new SetterMethodCall<T, TProperty>(mock, expression, prop.GetSetMethod());
			VerifyCalls(GetInterceptor(lambda, mock), expected, expression, times, failMessage);
		}

		internal static void VerifySet<T, TProperty>(
			Mock mock,
			Expression<Func<T, TProperty>> expression,
			TProperty value,
			Times times,
			string failMessage)
		{
			var lambda = expression.ToLambda();
			var prop = lambda.ToPropertyInfo();

			var expected = new SetterMethodCall<T, TProperty>(mock, expression, prop.GetSetMethod(), value);
			VerifyCalls(GetInterceptor(lambda, mock), expected, expression, times, failMessage);
		}

#if !SILVERLIGHT
		internal static void VerifySet<T>(
			Mock<T> mock,
			Action<T> setterExpression,
			Times times,
			string failMessage)
			where T : class
		{
			Interceptor targetInterceptor = null;
			Expression expression = null;
			var expected = CreateSetterCall<T, MethodCall<T>>(mock, setterExpression,
				(m, expr, method, value) =>
				{
					targetInterceptor = m.Interceptor;
					expression = expr;
					return new MethodCall<T>(m, expr, method, value);
				});

			VerifyCalls(targetInterceptor, expected, expression, times, failMessage);
		}
#endif
		private static void VerifyCalls(
			Interceptor targetInterceptor,
			MethodCall expected,
			Expression expression,
			Times times,
			string failMessage)
		{
			int callCount = targetInterceptor.ActualCalls.Where(i => expected.Matches(i)).Count();
			if (!times.Verify(callCount))
			{
				ThrowVerifyException(expression, times, failMessage);
			}
		}

		private static void ThrowVerifyException(Expression expression, Times times, string failMessage)
		{
			throw new MockException(
				MockException.ExceptionReason.VerificationFailed,
				times.GetExceptionMessage(failMessage, expression.ToStringFixed()));
		}

		#endregion

		#region Setup

		internal static MethodCall<T1> Setup<T1>(Mock mock, Expression<Action<T1>> expression)
		{
			return PexProtector.Invoke(() =>
			{
				var methodCall = expression.ToLambda().ToMethodCall();
				MethodInfo method = methodCall.Method;
				Expression[] args = methodCall.Arguments.ToArray();

				ThrowIfCantOverride(expression, method);
				var call = new MethodCall<T1>(mock, expression, method, args);

				var targetInterceptor = GetInterceptor(expression, mock);

				targetInterceptor.AddCall(call, ExpectKind.Other);

				return call;
			});
		}

		internal static MethodCallReturn<T1, TResult> Setup<T1, TResult>(Mock mock, Expression<Func<T1, TResult>> expression)
		{
			return PexProtector.Invoke(() =>
			{
				var lambda = expression.ToLambda();

				if (lambda.IsProperty())
					return SetupGet(mock, expression);

				var methodCall = lambda.ToMethodCall();
				MethodInfo method = methodCall.Method;
				Expression[] args = methodCall.Arguments.ToArray();

				ThrowIfCantOverride(expression, method);
				var call = new MethodCallReturn<T1, TResult>(mock, expression, method, args);

				var targetInterceptor = GetInterceptor(lambda, mock);

				targetInterceptor.AddCall(call, ExpectKind.Other);

				return call;
			});
		}

		internal static MethodCallReturn<T1, TProperty> SetupGet<T1, TProperty>(Mock mock, Expression<Func<T1, TProperty>> expression)
		{
			return PexProtector.Invoke(() =>
			{
				LambdaExpression lambda = expression.ToLambda();

				if (lambda.IsPropertyIndexer())
				{
					// Treat indexers as regular method invocations.
					return Setup<T1, TProperty>(mock, expression);
				}
				else
				{
					var prop = lambda.ToPropertyInfo();
					ThrowIfPropertyNotReadable(prop);

					var propGet = prop.GetGetMethod(true);
					ThrowIfCantOverride(expression, propGet);

					var call = new MethodCallReturn<T1, TProperty>(mock, expression, propGet, new Expression[0]);

					var targetInterceptor = GetInterceptor(expression, mock);

					targetInterceptor.AddCall(call, ExpectKind.Other);

					return call;
				}
			});
		}

#if !SILVERLIGHT
		internal static SetterMethodCall<T1, TProperty> SetupSet<T1, TProperty>(Mock<T1> mock,
			Action<T1> setterExpression)
			where T1 : class
		{
			return PexProtector.Invoke(() =>
			{
				return CreateSetterCall<T1, SetterMethodCall<T1, TProperty>>(mock, setterExpression,
					(m, expr, method, value) =>
					{
						var call = new SetterMethodCall<T1, TProperty>(m, expr, method, value);
						m.Interceptor.AddCall(call, ExpectKind.PropertySet);
						return call;
					});
			});
		}

		internal static MethodCall<T1> SetupSet<T1>(Mock<T1> mock,
			Action<T1> setterExpression)
			where T1 : class
		{
			return PexProtector.Invoke(() =>
			{
				return CreateSetterCall<T1, MethodCall<T1>>(mock, setterExpression,
					(m, expr, method, value) =>
					{
						var call = new MethodCall<T1>(m, expr, method, value);
						m.Interceptor.AddCall(call, ExpectKind.PropertySet);
						return call;
					});
			});
		}

		private static TCall CreateSetterCall<T1, TCall>(Mock<T1> mock,
			Action<T1> setterExpression, Func<Mock, Expression, MethodInfo, Expression, TCall> callFactory)
			where T1 : class
			where TCall : MethodCall
		{
			var reader = new ILReader(setterExpression.Method);
			MethodBase setter = null;
			MethodInfo matcher = null;

			while (reader.Read())
			{
				if (reader.OpCode == OpCodeEnum.Callvirt &&
					reader.OperandValueMethod.Name.StartsWith("set_"))
				{
					setter = reader.OperandValueMethod;
				}
				else if (reader.OpCode == OpCodeEnum.Call || reader.OpCode == OpCodeEnum.Callvirt)
				{
					// See if we have a supported matcher invocation.
					var m = reader.OperandValueMethod;
					if (m.GetCustomAttribute<AdvancedMatcherAttribute>(true) != null ||
						m.GetCustomAttribute<MatcherAttribute>(true) != null)
					{
						if (m.GetParameters().Length != 0)
							throw new NotSupportedException(Properties.Resources.UnsupportedMatcherParamsForSetter);
						if (!m.IsStatic)
							throw new NotSupportedException(Properties.Resources.UnsupportedNonStaticMatcherForSetter);

						// Can always cast as the matcher attribute is only valid on methods.
						matcher = (MethodInfo)m;
					}
				}
			}

			if (setter == null)
				throw new ArgumentException(Properties.Resources.SetupNotSetter);
			ThrowIfCantOverride<T1>(setter);

			using (var context = new FluentMockContext())
			{
				setterExpression(mock.Object);

				var last = context.LastInvocation;
				var x = Expression.Parameter(last.Invocation.Method.DeclaringType, "x");

				Expression value;
				if (matcher == null)
				{
					value = last.Invocation.Arguments[0] != null &&
						last.Invocation.Arguments[0].GetType() == setter.GetParameters()[0].ParameterType ?
						(Expression)Expression.Constant(last.Invocation.Arguments[0]) :
						// Add a cast if values do not match exactly (i.e. for Nullable<T>)
						(Expression)Expression.Convert(
							Expression.Constant(last.Invocation.Arguments[0]),
							setter.GetParameters()[0].ParameterType);
				}
				else
				{
					value = Expression.Call(matcher);
				}

				var lambda = Expression.Lambda(typeof(Action<>).MakeGenericType(x.Type),
					Expression.Call(
						x,
						last.Invocation.Method,
						value),
					x);

				return callFactory(last.Mock, lambda, last.Invocation.Method, value);
			}
		}

		internal static void SetupAllProperties(Mock mock)
		{
			PexProtector.Invoke(() =>
			{
				// Crazy reflection stuff below. Ah... the goodies of generics :)
				var mockType = mock.MockedType;
				var properties = new List<PropertyInfo>();
				properties.AddRange(mockType.GetProperties());
				// Add all implemented properties too.
				properties.AddRange(
						from i in mockType.GetInterfaces()
						from p in i.GetProperties()
						select p);
				// Add all properties from base classes
				properties = properties.Distinct().ToList();

				foreach (var property in properties)
				{
					if (property.CanRead && property.CanOverrideGet())
					{
						var expect = GetPropertyExpression(mockType, property);
						object initialValue = mock.DefaultValueProvider.ProvideDefault(property.GetGetMethod(), new object[0]);
						var mocked = initialValue as IMocked;
						if (mocked != null)
							SetupAllProperties(mocked.Mock);

						var closure = Activator.CreateInstance(
								typeof(ValueClosure<>).MakeGenericType(property.PropertyType), initialValue);

						var resultGet = mock
								.GetType()
								.GetMethod("SetupGet")
								.MakeGenericMethod(property.PropertyType)
								.Invoke(mock, new[] { expect });

						var returnsGet = resultGet.GetType().GetMethod("Returns", new[] { typeof(Func<>).MakeGenericType(property.PropertyType) });

						var getFunc = Activator.CreateInstance(
								typeof(Func<>).MakeGenericType(property.PropertyType),
								closure,
								closure.GetType().GetMethod("GetValue").MethodHandle.GetFunctionPointer());

						returnsGet.Invoke(resultGet, new[] { getFunc });

						if (property.CanWrite && property.CanOverrideSet())
						{
							var resultSet = typeof(MockLegacyExtensions)
									.GetMethods()
								// Couldn't make it work passing the generic types to GetMethod()
									.Where(m => m.Name == "SetupSet" && m.GetParameters().Length == 2)
									.First()
									.MakeGenericMethod(mock.GetType().GetGenericArguments()[0], property.PropertyType)
									.Invoke(mock, new object[] { mock, expect });

							var callbackSet = resultSet.GetType().GetMethod("Callback", new[] { typeof(Action<>).MakeGenericType(property.PropertyType) });

							var setFunc = Activator.CreateInstance(
									typeof(Action<>).MakeGenericType(property.PropertyType),
									closure,
									closure.GetType().GetMethod("SetValue").MethodHandle.GetFunctionPointer());

							callbackSet.Invoke(resultSet, new[] { setFunc });
						}
					}
				}
			});
		}
#endif

		private static Expression GetPropertyExpression(Type mockType, PropertyInfo property)
		{
			var param = Expression.Parameter(mockType, "m");
			return Expression.Lambda(
					Expression.MakeMemberAccess(param, property),
					param);
		}

		#region Legacy

		internal static SetterMethodCall<T1, TProperty> SetupSet<T1, TProperty>(Mock mock,
			Expression<Func<T1, TProperty>> expression)
		{
			var prop = expression.ToLambda().ToPropertyInfo();
			ThrowIfPropertyNotWritable(prop);

			var propSet = prop.GetSetMethod(true);
			ThrowIfCantOverride(expression, propSet);

			var call = new SetterMethodCall<T1, TProperty>(mock, expression, propSet);
			var targetInterceptor = GetInterceptor(expression, mock);

			targetInterceptor.AddCall(call, ExpectKind.PropertySet);

			return call;
		}

		internal static SetterMethodCall<T1, TProperty> SetupSet<T1, TProperty>(Mock mock,
			Expression<Func<T1, TProperty>> expression, TProperty value)
		{
			var lambda = expression.ToLambda();
			var prop = lambda.ToPropertyInfo();
			ThrowIfPropertyNotWritable(prop);

			var setter = prop.GetSetMethod();
			ThrowIfCantOverride(expression, setter);

			var call = new SetterMethodCall<T1, TProperty>(mock, expression, setter, value);
			var targetInterceptor = GetInterceptor(expression, mock);

			targetInterceptor.AddCall(call, ExpectKind.PropertySet);

			return call;
		}

		#endregion

		/// <summary>
		/// Gets the interceptor target for the given expression and root mock, 
		/// building the intermediate hierarchy of mock objects if necessary.
		/// </summary>
		private static Interceptor GetInterceptor(LambdaExpression lambda, Mock mock)
		{
			var visitor = new AutoMockPropertiesVisitor(mock);
			var target = visitor.SetupMocks(lambda.Body);
			return target.Interceptor;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "This is a helper method for the one receiving the expression.")]
		private static void ThrowIfPropertyNotWritable(PropertyInfo prop)
		{
			if (!prop.CanWrite)
			{
				throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
					Properties.Resources.PropertyNotWritable,
					prop.DeclaringType.Name,
					prop.Name), "expression");
			}
		}

		private static void ThrowIfPropertyNotReadable(PropertyInfo prop)
		{
			// If property is not readable, the compiler won't let 
			// the user to specify it in the lambda :)
			// This is just reassuring that in case they build the 
			// expression tree manually?
			if (!prop.CanRead)
			{
				throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
					Properties.Resources.PropertyNotReadable,
					prop.DeclaringType.Name,
					prop.Name));
			}
		}

		private static void ThrowIfCantOverride(Expression setup, MethodInfo methodInfo)
		{
			if (CantOverride(methodInfo))
				throw new ArgumentException(
					String.Format(CultureInfo.CurrentCulture,
					Properties.Resources.SetupOnNonOverridableMember,
					setup.ToString()));
		}

		private static void ThrowIfCantOverride<T1>(MethodBase setter) where T1 : class
		{
			if (CantOverride(setter))
				throw new ArgumentException(
					String.Format(CultureInfo.CurrentCulture,
					Properties.Resources.SetupOnNonOverridableMember,
					typeof(T1).Name + "." + setter.Name.Substring(4)));
		}

		private static bool CantOverride(MethodBase method)
		{
			return !method.IsVirtual || method.IsFinal || method.IsPrivate;
		}

		class AutoMockPropertiesVisitor : ExpressionVisitor
		{
			Mock ownerMock;
			List<PropertyInfo> properties = new List<PropertyInfo>();
			bool first = true;

			public AutoMockPropertiesVisitor(Mock ownerMock)
			{
				this.ownerMock = ownerMock;
			}

			public Mock SetupMocks(Expression expression)
			{
				var withoutLast = Visit(expression);
				var targetMock = ownerMock;
				var props = properties.AsEnumerable();

				foreach (var prop in props.Reverse())
				{
					Mock mock;
					var propGet = prop.GetGetMethod();
					if (!ownerMock.InnerMocks.TryGetValue(propGet, out mock))
					{
						// TODO: this may throw TargetInvocationException, 
						// cleanup stacktrace.
						ValidateTypeToMock(prop, expression);

						var mockType = typeof(Mock<>).MakeGenericType(prop.PropertyType);

						mock = (Mock)Activator.CreateInstance(mockType, ownerMock.Behavior);
						mock.DefaultValue = ownerMock.DefaultValue;
						ownerMock.InnerMocks.Add(propGet, mock);

						var targetType = targetMock.MockedType;

						// TODO: cache method
						var setupGet = typeof(Mock).GetMethod("SetupGet", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
						setupGet = setupGet.MakeGenericMethod(targetType, prop.PropertyType);
						var param = Expression.Parameter(targetType, "mock");
						var expr = Expression.Lambda(Expression.MakeMemberAccess(param, prop), param);
						var result = setupGet.Invoke(targetMock, new object[] { targetMock, expr });
						var returns = result.GetType().GetMethod("Returns", new[] { prop.PropertyType });
						returns.Invoke(result, new[] { mock.Object });
					}

					targetMock = mock;
				}

				return targetMock;
			}

			private static void ValidateTypeToMock(PropertyInfo prop, Expression expr)
			{
				if (prop.PropertyType.IsValueType || prop.PropertyType.IsSealed)
					throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture,
						Properties.Resources.UnsupportedIntermediateType,
						prop.DeclaringType.Name, prop.Name, prop.PropertyType, expr));
			}

			protected override Expression VisitMethodCall(MethodCallExpression m)
			{
				if (first)
				{
					first = false;
					return base.Visit(m.Object);
				}
				else
				{
					throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture,
						Properties.Resources.UnsupportedIntermediateExpression, m));
				}
			}

			protected override Expression VisitMemberAccess(MemberExpression m)
			{
				if (first)
				{
					first = false;
					return base.Visit(m.Expression);
				}

				if (m.Member is FieldInfo)
					throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture,
						Properties.Resources.FieldsNotSupported, m));

				if (m.Expression.NodeType != ExpressionType.MemberAccess &&
					m.Expression.NodeType != ExpressionType.Parameter)
					throw new NotSupportedException(String.Format(CultureInfo.CurrentCulture,
						Properties.Resources.UnsupportedIntermediateExpression, m));

				var prop = (PropertyInfo)m.Member;
				//var targetType = ((MemberExpression)m.Expression).Type;

				properties.Add(prop);

				return base.VisitMemberAccess(m);
			}
		}

		#endregion

		#region Events

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

		internal void RemoveEventHandler(EventInfo ev, Delegate handler)
		{
			List<Delegate> handlers;
			if (invocationLists.TryGetValue(ev, out handlers))
			{
				handlers.Remove(handler);
			}
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
		[EditorBrowsable(EditorBrowsableState.Never)] // TODO: remove on v3.5
		public virtual MockedEvent<TEventArgs> CreateEventHandler<TEventArgs>() where TEventArgs : EventArgs
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
		[EditorBrowsable(EditorBrowsableState.Never)] // TODO: remove on v3.5
		public virtual MockedEvent<EventArgs> CreateEventHandler()
		{
			return new MockedEvent<EventArgs>(this);
		}

		#endregion
	}
}
