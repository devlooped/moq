//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
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

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
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
using System.Runtime.CompilerServices;
using IQToolkit;
using Moq.Properties;

namespace Moq
{
	/// <include file='Mock.xdoc' path='docs/doc[@for="Mock"]/*'/>
	public abstract partial class Mock : IHideObjectMembers
	{
		private bool isInitialized;
		private bool callBase;
		private DefaultValue defaultValue = DefaultValue.Empty;
		private IDefaultValueProvider defaultValueProvider = new EmptyDefaultValueProvider();
		private Dictionary<EventInfo, List<Delegate>> invocationLists = new Dictionary<EventInfo, List<Delegate>>();

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.ctor"]/*'/>
		protected Mock()
		{
			this.ImplementedInterfaces = new List<Type>();
			this.InnerMocks = new Dictionary<MethodInfo, Mock>();
		}

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.Get"]/*'/>
		public static Mock<T> Get<T>(T mocked) where T : class
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
					var types = string.Join(
						", ",
						new[] { mockedType }
						// Skip first interface which is always our internal IMocked<T>
							.Concat(mock.ImplementedInterfaces.Skip(1))
							.Select(t => t.Name)
							.ToArray());

					throw new ArgumentException(string.Format(
						CultureInfo.CurrentCulture,
						Resources.InvalidMockGetType,
						typeof(T).Name,
						types));
				}
			}
			else
			{
				throw new ArgumentException(Properties.Resources.ObjectInstanceNotMock, "mocked");
			}
		}

		internal virtual Interceptor Interceptor { get; set; }

		internal virtual Dictionary<MethodInfo, Mock> InnerMocks { get; private set; }

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.Behavior"]/*'/>
		public virtual MockBehavior Behavior { get; internal set; }

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.CallBase"]/*'/>
		public virtual bool CallBase { get { return callBase; } set { callBase = value; } }

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.DefaultValue"]/*'/>
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

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.Object"]/*'/>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Object", Justification = "Exposes the mocked object instance, so it's appropriate.")]
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The public Object property is the only one visible to Moq consumers. The protected member is for internal use only.")]
		public object Object
		{
			get { return this.GetObject(); }
		}

		private object GetObject()
		{
			var value = OnGetObject();
			isInitialized = true;
			return value;
		}

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.OnGetObject"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is actually the protected virtual implementation of the property Object.")]
		protected abstract object OnGetObject();

		/// <summary>
		/// Retrieves the type of the mocked object, its generic type argument.
		/// This is used in the auto-mocking of hierarchy access.
		/// </summary>
		internal abstract Type MockedType { get; }

		/// <summary>
		/// Specifies the class that will determine the default 
		/// value to return when invocations are made that 
		/// have no setups and need to return a default 
		/// value (for loose mocks).
		/// </summary>
		internal IDefaultValueProvider DefaultValueProvider
		{
			get { return this.defaultValueProvider; }
		}

		/// <summary>
		/// Exposes the list of extra interfaces implemented by the mock.
		/// </summary>
		internal List<Type> ImplementedInterfaces { get; private set; }

		#region Verify

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.Verify"]/*'/>
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

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.VerifyAll"]/*'/>		
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

			var expected = new MethodCall(mock, expression, method, args) { FailMessage = failMessage };
			VerifyCalls(GetInterceptor(methodCall.Object, mock), expected, expression, times);
		}

		internal static void Verify<T, TResult>(
			Mock mock,
			Expression<Func<T, TResult>> expression,
			Times times,
			string failMessage)
			where T : class
		{
			var lambda = expression.ToLambda();
			if (lambda.IsProperty())
			{
				VerifyGet<T, TResult>(mock, expression, times, failMessage);
			}
			else
			{
				var methodCall = lambda.ToMethodCall();
				var method = methodCall.Method;
				var args = methodCall.Arguments.ToArray();

				var expected = new MethodCallReturn<T, TResult>(mock, expression, method, args) { FailMessage = failMessage };
				VerifyCalls(GetInterceptor(methodCall.Object, mock), expected, expression, times);
			}
		}

		internal static void VerifyGet<T, TProperty>(
			Mock mock,
			Expression<Func<T, TProperty>> expression,
			Times times,
			string failMessage)
			where T : class
		{
			var lambda = expression.ToLambda();
			var prop = lambda.ToPropertyInfo();

			var expected = new MethodCallReturn<T, TProperty>(
				mock,
				expression,
				prop.GetGetMethod(),
				new Expression[0]) { FailMessage = failMessage };
			VerifyCalls(GetInterceptor(((MemberExpression)lambda.Body).Expression, mock), expected, expression, times);
		}

		internal static void VerifySet<T, TProperty>(
			Mock mock,
			Expression<Func<T, TProperty>> expression,
			Times times,
			string failMessage)
			where T : class
		{
			var lambda = expression.ToLambda();
			var prop = lambda.ToPropertyInfo();

			var expected = new SetterMethodCall<T, TProperty>(mock, expression, prop.GetSetMethod()) { FailMessage = failMessage };
			VerifyCalls(GetInterceptor(((MemberExpression)lambda.Body).Expression, mock), expected, expression, times);
		}

		internal static void VerifySet<T, TProperty>(
			Mock mock,
			Expression<Func<T, TProperty>> expression,
			TProperty value,
			Times times,
			string failMessage)
			where T : class
		{
			var lambda = expression.ToLambda();
			var prop = lambda.ToPropertyInfo();

			var expected = new SetterMethodCall<T, TProperty>(mock, expression, prop.GetSetMethod(), value) { FailMessage = failMessage };
			VerifyCalls(GetInterceptor(((MemberExpression)lambda.Body).Expression, mock), expected, expression, times);
		}

		internal static void VerifySet<T>(
			Mock<T> mock,
			Action<T> setterExpression,
			Times times,
			string failMessage)
			where T : class
		{
			Interceptor targetInterceptor = null;
			Expression expression = null;
			var expected = SetupSetImpl<T, MethodCall<T>>(mock, setterExpression,
				(m, expr, method, value) =>
				{
					targetInterceptor = m.Interceptor;
					expression = expr;
					return new MethodCall<T>(m, expr, method, value) { FailMessage = failMessage };
				});

			VerifyCalls(targetInterceptor, expected, expression, times);
		}

		private static void VerifyCalls(
			Interceptor targetInterceptor,
			MethodCall expected,
			Expression expression,
			Times times)
		{
			var callCount = targetInterceptor.ActualCalls.Where(ac => expected.Matches(ac)).Count();
			if (!times.Verify(callCount))
			{
				ThrowVerifyException(expected, expression, times, callCount);
			}
		}

		private static void ThrowVerifyException(
			IProxyCall expected,
			Expression expression,
			Times times,
			int callCount)
		{
			throw new MockException(
				MockException.ExceptionReason.VerificationFailed,
				times.GetExceptionMessage(expected.FailMessage, expression.ToStringFixed(), callCount));
		}

		#endregion

		#region Setup

		internal static MethodCall<T1> Setup<T1>(Mock mock, Expression<Action<T1>> expression)
			where T1 : class
		{
			return PexProtector.Invoke(() =>
			{
				var methodCall = expression.ToLambda().ToMethodCall();
				MethodInfo method = methodCall.Method;
				Expression[] args = methodCall.Arguments.ToArray();

				ThrowIfNotMember(expression, method);
				ThrowIfCantOverride(expression, method);
				var call = new MethodCall<T1>(mock, expression, method, args);

				var targetInterceptor = GetInterceptor(methodCall.Object, mock);

				targetInterceptor.AddCall(call, SetupKind.Other);

				return call;
			});
		}

		internal static MethodCallReturn<T1, TResult> Setup<T1, TResult>(Mock mock, Expression<Func<T1, TResult>> expression)
			where T1 : class
		{
			return PexProtector.Invoke(() =>
			{
				var lambda = expression.ToLambda();

				if (lambda.IsProperty())
				{
					return SetupGet(mock, expression);
				}

				var methodCall = lambda.ToMethodCall();
				var method = methodCall.Method;
				var args = methodCall.Arguments.ToArray();

				ThrowIfNotMember(expression, method);
				ThrowIfCantOverride(expression, method);
				var call = new MethodCallReturn<T1, TResult>(mock, expression, method, args);

				var targetInterceptor = GetInterceptor(methodCall.Object, mock);

				targetInterceptor.AddCall(call, SetupKind.Other);

				return call;
			});
		}

		internal static MethodCallReturn<T1, TProperty> SetupGet<T1, TProperty>(Mock mock, Expression<Func<T1, TProperty>> expression)
			where T1 : class
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
					// Directly casting to MemberExpression is fine as ToPropertyInfo would throw if it wasn't
					var targetInterceptor = GetInterceptor(((MemberExpression)expression.Body).Expression, mock);

					targetInterceptor.AddCall(call, SetupKind.Other);

					return call;
				}
			});
		}

		internal static SetterMethodCall<T1, TProperty> SetupSet<T1, TProperty>(Mock<T1> mock,
			Action<T1> setterExpression)
			where T1 : class
		{
			return PexProtector.Invoke(() =>
			{
				return SetupSetImpl<T1, SetterMethodCall<T1, TProperty>>(mock, setterExpression,
					(m, expr, method, value) =>
					{
						var call = new SetterMethodCall<T1, TProperty>(m, expr, method, value[0]);
						m.Interceptor.AddCall(call, SetupKind.PropertySet);
						return call;
					});
			});
		}

		internal static MethodCall<T1> SetupSet<T1>(Mock<T1> mock, Action<T1> setterExpression)
			where T1 : class
		{
			return PexProtector.Invoke(() =>
			{
				return SetupSetImpl<T1, MethodCall<T1>>(
					mock,
					setterExpression,
					(m, expr, method, values) =>
					{
						var call = new MethodCall<T1>(m, expr, method, values);
						m.Interceptor.AddCall(call, SetupKind.PropertySet);
						return call;
					});

			});
		}

		internal static SetterMethodCall<T1, TProperty> SetupSet<T1, TProperty>(Mock mock,
			Expression<Func<T1, TProperty>> expression)
			where T1 : class
		{
			var prop = expression.ToLambda().ToPropertyInfo();
			ThrowIfPropertyNotWritable(prop);

			var propSet = prop.GetSetMethod(true);
			ThrowIfCantOverride(expression, propSet);

			var call = new SetterMethodCall<T1, TProperty>(mock, expression, propSet);
			var targetInterceptor = GetInterceptor(((MemberExpression)expression.Body).Expression, mock);

			targetInterceptor.AddCall(call, SetupKind.PropertySet);

			return call;
		}

		private static TCall SetupSetImpl<T1, TCall>(
			Mock<T1> mock,
			Action<T1> setterExpression,
			Func<Mock, Expression, MethodInfo, Expression[], TCall> callFactory)
			where T1 : class
			where TCall : MethodCall
		{
			using (var context = new FluentMockContext())
			{
				setterExpression(mock.Object);

				var last = context.LastInvocation;

				if (last == null)
					throw new ArgumentException(String.Format(
						CultureInfo.InvariantCulture,
						Properties.Resources.SetupOnNonOverridableMember,
						""));

				var setter = last.Invocation.Method;
				if (!setter.IsSpecialName || !setter.Name.StartsWith("set_", StringComparison.Ordinal))
				{
					throw new ArgumentException(Resources.SetupNotSetter);
				}

				// No need to call ThrowIfCantOverride as non-overridable would have thrown above already.

				var x = Expression.Parameter(last.Invocation.Method.DeclaringType,
					// Get the variable name as used in the actual delegate :)
					setterExpression.Method.GetParameters()[0].Name);

				var arguments = last.Invocation.Arguments;
				var parameters = setter.GetParameters();
				var values = new Expression[arguments.Length];

				if (last.Match == null)
				{
					// Length == 1 || Length == 2 (Indexer property)
					for (int i = 0; i < arguments.Length; i++)
					{
						values[i] = GetValueExpression(arguments[i], parameters[i].ParameterType);
					}

					var lambda = Expression.Lambda(
						typeof(Action<>).MakeGenericType(x.Type),
						Expression.Call(x, last.Invocation.Method, values),
						x);

					return callFactory(last.Mock, lambda, last.Invocation.Method, values);
				}
				else
				{
					var matchers = new Expression[arguments.Length];
					var valueIndex = arguments.Length - 1;
					var propertyType = setter.GetParameters()[valueIndex].ParameterType;

					// If the value matcher is not equal to the property 
					// type (i.e. prop is int?, but you use It.IsAny<int>())
					// add a cast.
					if (last.Match.RenderExpression.Type != propertyType)
						values[valueIndex] = Expression.Convert(last.Match.RenderExpression, propertyType);
					else
						values[valueIndex] = last.Match.RenderExpression;

					matchers[valueIndex] = new MatchExpression(last.Match);

					if (arguments.Length == 2)
					{
						// TODO: what about multi-index setters?
						// Add the index value for the property indexer
						values[0] = GetValueExpression(arguments[0], parameters[0].ParameterType);
						// TODO: No matcher supported now for the index
						matchers[0] = values[0];
					}

					var lambda = Expression.Lambda(
						typeof(Action<>).MakeGenericType(x.Type),
						Expression.Call(x, last.Invocation.Method, values),
						x);

					return callFactory(last.Mock, lambda, last.Invocation.Method, matchers);
				}
			}
		}

		private static Expression GetValueExpression(object value, Type type)
		{
			if (value != null && value.GetType() == type)
			{
				return Expression.Constant(value);
			}

			// Add a cast if values do not match exactly (i.e. for Nullable<T>)
			return Expression.Convert(Expression.Constant(value), type);
		}

#if !SILVERLIGHT
		internal static void SetupAllProperties(Mock mock)
		{
			// Dunno why the conditional is not kicking in. I'm getting 
			// a compilation error on MethodHandle.GetFunctionPointer.
			PexProtector.Invoke(() =>
			{
				// Crazy reflection stuff below. Ah... the goodies of generics :)
				var mockType = mock.MockedType;
				var poperties = mockType.GetProperties()
					.Concat(from i in mockType.GetInterfaces()
							from p in i.GetProperties()
							select p)
					.Distinct();

				foreach (var property in poperties)
				{
					if (property.CanRead && property.CanOverrideGet())
					{
						var expect = GetPropertyExpression(mockType, property);
						object initialValue = mock.DefaultValueProvider.ProvideDefault(property.GetGetMethod());
						var mocked = initialValue as IMocked;
						if (mocked != null)
						{
							SetupAllProperties(mocked.Mock);
						}

						var closure = Activator.CreateInstance(
								typeof(ValueClosure<>).MakeGenericType(property.PropertyType), initialValue);

						var resultGet = mock.GetType().GetMethod("SetupGet")
							.MakeGenericMethod(property.PropertyType)
							.Invoke(mock, new[] { expect });

						var returnsGet = resultGet.GetType()
							.GetMethod("Returns", new[] { typeof(Func<>).MakeGenericType(property.PropertyType) });

						var getFunc = Activator.CreateInstance(
								typeof(Func<>).MakeGenericType(property.PropertyType),
								closure,
								closure.GetType().GetMethod("GetValue").MethodHandle.GetFunctionPointer());

						returnsGet.Invoke(resultGet, new[] { getFunc });

						if (property.CanWrite && property.CanOverrideSet())
						{
							var resultSet = typeof(MockExtensions).GetMethod("SetupSet")
								.MakeGenericMethod(mockType, property.PropertyType)
								.Invoke(mock, new object[] { mock, expect });

							var callbackSet = resultSet.GetType().GetMethod(
								"Callback",
								new[] { typeof(Action<>).MakeGenericType(property.PropertyType) });

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
			return Expression.Lambda(Expression.MakeMemberAccess(param, property), param);
		}

		/// <summary>
		/// Gets the interceptor target for the given expression and root mock, 
		/// building the intermediate hierarchy of mock objects if necessary.
		/// </summary>
		private static Interceptor GetInterceptor(Expression fluentExpression, Mock mock)
		{
			//var visitor = new AutoMockPropertiesVisitor(mock);
			//var target = visitor.SetupMocks(lambda.Body);
			//return target.Interceptor;

			var targetExpression = FluentMockVisitor.Accept(fluentExpression, mock);
			var targetLambda = Expression.Lambda<Func<Mock>>(
				Expression.Convert(
					targetExpression,
					typeof(Mock)));

			var targetObject = targetLambda.Compile()();

			return targetObject.Interceptor;
		}

		[SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Justification = "This is a helper method for the one receiving the expression.")]
		private static void ThrowIfPropertyNotWritable(PropertyInfo prop)
		{
			if (!prop.CanWrite)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.PropertyNotWritable,
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
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.PropertyNotReadable,
					prop.DeclaringType.Name,
					prop.Name));
			}
		}

		private static void ThrowIfCantOverride(Expression setup, MethodInfo methodInfo)
		{
			if (CantOverride(methodInfo))
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.SetupOnNonOverridableMember,
					setup.ToStringFixed()));
			}
		}

		private static void ThrowIfNotMember(Expression setup, MethodInfo method)
		{
			if (method.IsStatic)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.SetupOnNonMemberMethod,
					setup.ToStringFixed()));
			}
		}

		private static void ThrowIfCantOverride<T>(MethodBase setter) where T : class
		{
			if (CantOverride(setter))
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.SetupOnNonOverridableMember,
					typeof(T).Name + "." + setter.Name.Substring(4)));
			}
		}

		private static bool CantOverride(MethodBase method)
		{
			return !method.IsVirtual || method.IsFinal || method.IsPrivate;
		}

		private class FluentMockVisitor : ExpressionVisitor
		{
			static readonly MethodInfo FluentMockGenericMethod = ((Func<Mock<string>, Expression<Func<string, string>>, Mock<string>>)
				QueryableMockExtensions.FluentMock<string, string>).Method.GetGenericMethodDefinition();
			static readonly MethodInfo MockGetGenericMethod = ((Func<string, Mock<string>>)Moq.Mock.Get<string>)
				.Method.GetGenericMethodDefinition();

			Expression expression;
			Mock mock;

			public FluentMockVisitor(Expression expression, Mock mock)
			{
				this.expression = expression;
				this.mock = mock;
			}

			public static Expression Accept(Expression expression, Mock mock)
			{
				return new FluentMockVisitor(expression, mock).Accept();
			}

			public Expression Accept()
			{
				return Visit(expression);
			}

			protected override Expression VisitParameter(ParameterExpression p)
			{
				// the actual first object being used in a fluent expression, 
				// which will be against the actual mock rather than 
				// the parameter.
				return Expression.Constant(mock);
			}

			protected override Expression VisitMethodCall(MethodCallExpression m)
			{
				var lambdaParam = Expression.Parameter(m.Object.Type, "mock");
				Expression lambdaBody = Expression.Call(lambdaParam, m.Method, m.Arguments);
				var targetMethod = GetTargetMethod(m.Object.Type, m.Method.ReturnType);

				return TranslateFluent(m.Object.Type, m.Method.ReturnType, targetMethod, Visit(m.Object), lambdaParam, lambdaBody);
			}

			protected override Expression VisitMemberAccess(MemberExpression m)
			{
				// Translate differently member accesses over transparent
				// compiler-generated types as they are typically the 
				// anonymous types generated to build up the query expressions.
				if (m.Expression.NodeType == ExpressionType.Parameter &&
					m.Expression.Type.GetCustomAttribute<CompilerGeneratedAttribute>(false) != null)
				{
					var memberType = m.Member is FieldInfo ?
						((FieldInfo)m.Member).FieldType :
						((PropertyInfo)m.Member).PropertyType;

					// Generate a Mock.Get over the entire member access rather.
					// <anonymous_type>.foo => Mock.Get(<anonymous_type>.foo)
					return Expression.Call(null,
						MockGetGenericMethod.MakeGenericMethod(memberType), m);
				}

				// If member is not mock-able, actually, including being a sealed class, etc.?
				if (m.Member is FieldInfo)
					throw new NotSupportedException();

				var lambdaParam = Expression.Parameter(m.Expression.Type, "mock");
				Expression lambdaBody = Expression.MakeMemberAccess(lambdaParam, m.Member);
				var targetMethod = GetTargetMethod(m.Expression.Type, ((PropertyInfo)m.Member).PropertyType);

				return TranslateFluent(m.Expression.Type, ((PropertyInfo)m.Member).PropertyType, targetMethod, Visit(m.Expression), lambdaParam, lambdaBody);
			}

			private Expression TranslateFluent(Type objectType, Type returnType, MethodInfo targetMethod, Expression instance, ParameterExpression lambdaParam, Expression lambdaBody)
			{
				var mockType = typeof(Mock<>).MakeGenericType(returnType);
				var funcType = typeof(Func<,>).MakeGenericType(objectType, returnType);

				// This is the fluent extension method one, so pass the instance as one more arg.
				return Expression.Call(
					targetMethod,
					instance,
					Expression.Lambda(
						funcType,
						lambdaBody,
						lambdaParam
					)
				);
			}

			private MethodInfo GetTargetMethod(Type objectType, Type returnType)
			{
				returnType.ThrowIfNotMockeable();
				//.FluentMock(mock => mock.Solution)
				return FluentMockGenericMethod.MakeGenericMethod(objectType, returnType);
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
			{
				return new Delegate[0];
			}

			return handlers;
		}

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.CreateEventHandler{TEventArgs}"]/*'/>
		[EditorBrowsable(EditorBrowsableState.Never)] // TODO: remove on v3.5
		public virtual MockedEvent<TEventArgs> CreateEventHandler<TEventArgs>() where TEventArgs : EventArgs
		{
			return new MockedEvent<TEventArgs>(this);
		}

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.CreateEventHandler"]/*'/>
		[EditorBrowsable(EditorBrowsableState.Never)] // TODO: remove on v3.5
		public virtual MockedEvent<EventArgs> CreateEventHandler()
		{
			return new MockedEvent<EventArgs>(this);
		}

		#endregion

		#region As<TInterface>

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.As{TInterface}"]/*'/>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "As", Justification = "We want the method called exactly as the keyword because that's what it does, it adds an implemented interface so that you can cast it later.")]
		public virtual Mock<TInterface> As<TInterface>()
			where TInterface : class
		{
			if (isInitialized && !ImplementedInterfaces.Contains(typeof(TInterface)))
			{
				throw new InvalidOperationException(Properties.Resources.AlreadyInitialized);
			}
			if (!typeof(TInterface).IsInterface)
			{
				throw new ArgumentException(Properties.Resources.AsMustBeInterface);
			}

			if (!ImplementedInterfaces.Contains(typeof(TInterface)))
			{
				ImplementedInterfaces.Add(typeof(TInterface));
			}

			return new AsInterface<TInterface>(this);
		}

		#endregion
	}
}