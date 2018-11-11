// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Moq.Language.Flow;
using Moq.Properties;
using Moq.Protected;

namespace Moq
{
	/// <include file='Mock.xdoc' path='docs/doc[@for="Mock"]/*'/>
	public abstract partial class Mock : IFluentInterface
	{
		internal static readonly MethodInfo GetMethod =
			typeof(Mock).GetMethod(nameof(Get), BindingFlags.Public | BindingFlags.Static);

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.ctor"]/*'/>
		protected Mock()
		{
		}

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.Get"]/*'/>
		public static Mock<T> Get<T>(T mocked) where T : class
		{
			var mockedOfT = mocked as IMocked<T>;
			if (mockedOfT != null)
			{
				// This would be the fastest check.
				return mockedOfT.Mock;
			}

			var aDelegate = mocked as Delegate;
			if (aDelegate != null)
			{
				var mockedDelegateImpl = aDelegate.Target as IMocked<T>;
				if (mockedDelegateImpl != null)
					return mockedDelegateImpl.Mock;
			}

			var mockedPlain = mocked as IMocked;
			if (mockedPlain != null)
			{
				// We may have received a T of an implemented 
				// interface in the mock.
				var mock = mockedPlain.Mock;
				if (mock.ImplementsInterface(typeof(T)))
				{
					return mock.As<T>();
				}

				// Alternatively, we may have been asked 
				// for a type that is assignable to the 
				// one for the mock.
				// This is not valid as generic types 
				// do not support covariance on 
				// the generic parameters.
				var imockedType = mocked.GetType().GetTypeInfo().ImplementedInterfaces.Single(i => i.Name.Equals("IMocked`1", StringComparison.Ordinal));
				var mockedType = imockedType.GetGenericArguments()[0];
				var types = string.Join(
					", ",
					new[] {mockedType}
						// Ignore internally defined IMocked<T>
						.Concat(mock.InheritedInterfaces)
						.Concat(mock.AdditionalInterfaces)
						.Select(t => t.Name)
						.ToArray());

				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.InvalidMockGetType,
					typeof(T).Name,
					types));
			}

			throw new ArgumentException(Resources.ObjectInstanceNotMock, "mocked");
		}

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.Verify"]/*'/>
		public static void Verify(params Mock[] mocks)
		{
			foreach (var mock in mocks)
			{
				mock.Verify();
			}
		}

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.VerifyAll"]/*'/>
		public static void VerifyAll(params Mock[] mocks)
		{
			foreach (var mock in mocks)
			{
				mock.VerifyAll();
			}
		}

		/// <summary>
		/// Gets the interfaces additionally implemented by the mock object.
		/// </summary>
		/// <remarks>
		/// This list may be modified by calls to <see cref="As{TInterface}"/> up until the first call to <see cref="Object"/>.
		/// </remarks>
		internal abstract List<Type> AdditionalInterfaces { get; }

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.Behavior"]/*'/>
		public abstract MockBehavior Behavior { get; }

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.CallBase"]/*'/>
		public abstract bool CallBase { get; set; }

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.DefaultValue"]/*'/>
		public DefaultValue DefaultValue
		{
			get
			{
				return this.DefaultValueProvider.Kind;
			}
			set
			{
				switch (value)
				{
					case DefaultValue.Empty:
						this.DefaultValueProvider = DefaultValueProvider.Empty;
						return;

					case DefaultValue.Mock:
						this.DefaultValueProvider = DefaultValueProvider.Mock;
						return;

					default:
						throw new ArgumentOutOfRangeException(nameof(value));
				}
			}
		}

		internal abstract EventHandlerCollection EventHandlers { get; }

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.Object"]/*'/>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Object", Justification = "Exposes the mocked object instance, so it's appropriate.")]
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The public Object property is the only one visible to Moq consumers. The protected member is for internal use only.")]
		public object Object => this.OnGetObject();

		/// <summary>
		/// Gets the interfaces directly inherited from the mocked type (<see cref="TargetType"/>).
		/// </summary>
		internal abstract Type[] InheritedInterfaces { get; }

		internal abstract ConcurrentDictionary<MethodInfo, MockWithWrappedMockObject> InnerMocks { get; }

		internal abstract bool IsObjectInitialized { get; }

		/// <summary>
		/// Gets list of invocations which have been performed on this mock.
		/// </summary>
		public IInvocationList Invocations => MutableInvocations;

		internal abstract InvocationCollection MutableInvocations { get; }

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.OnGetObject"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is actually the protected virtual implementation of the property Object.")]
		protected abstract object OnGetObject();

		/// <summary>
		/// Retrieves the type of the mocked object, its generic type argument.
		/// This is used in the auto-mocking of hierarchy access.
		/// </summary>
		internal abstract Type MockedType { get; }

		internal bool IsDelegateMock => this.TargetType.IsDelegate();

		/// <summary>
		/// Gets or sets the <see cref="DefaultValueProvider"/> instance that will be used
		/// e. g. to produce default return values for unexpected invocations.
		/// </summary>
		public abstract DefaultValueProvider DefaultValueProvider { get; set; }

		internal abstract SetupCollection Setups { get; }

		/// <summary>
		/// A set of switches that influence how this mock will operate.
		/// You can opt in or out of certain features via this property.
		/// </summary>
		public abstract Switches Switches { get; set; }

		internal abstract Type TargetType { get; }

		#region Verify

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.Verify"]/*'/>
		public void Verify()
		{
			if (!this.TryVerify(out MockException error))
			{
				throw error;
			}
		}

		private bool TryVerify(out MockException error)
		{
			foreach (Invocation invocation in this.MutableInvocations)
			{
				invocation.MarkAsVerifiedIfMatchedByVerifiableSetup();
			}

			var uninvokedVerifiableSetups = this.Setups.ToArrayLive(setup => !setup.TryVerify());
			if (uninvokedVerifiableSetups.Length > 0)
			{
				error = MockException.UnmatchedSetups(this, uninvokedVerifiableSetups);
				return false;
			}

			foreach (var inner in this.InnerMocks.Values)
			{
				if (!inner.Mock.TryVerify(out error))
				{
					return false;
				}
			}

			error = null;
			return true;
		}

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.VerifyAll"]/*'/>		
		public void VerifyAll()
		{
			if (!this.TryVerifyAll(out MockException error))
			{
				throw error;
			}
		}

		private bool TryVerifyAll(out MockException error)
		{
			foreach (Invocation invocation in this.MutableInvocations)
			{
				invocation.MarkAsVerifiedIfMatchedBySetup();
			}

			var uninvokedSetups = this.Setups.ToArrayLive(setup => !setup.TryVerifyAll());
			if (uninvokedSetups.Length > 0)
			{
				error = MockException.UnmatchedSetups(this, uninvokedSetups);
				return false;
			}

			foreach (var inner in this.InnerMocks.Values)
			{
				if (!inner.Mock.TryVerifyAll(out error))
				{
					return false;
				}
			}

			error = null;
			return true;
		}

		internal static void VerifyVoid(Mock mock, LambdaExpression expression, Times times, string failMessage)
		{
			Guard.NotNull(times, nameof(times));

			var (obj, method, args) = expression.GetCallInfo(mock);
			ThrowIfVerifyExpressionInvolvesUnsupportedMember(expression, method);

			var expectation = new InvocationShape(method, args);
			VerifyCalls(GetTargetMock(obj, mock), expectation, expression, times, failMessage);
		}

		internal static void VerifyNonVoid(Mock mock, LambdaExpression expression, Times times, string failMessage)
		{
			Guard.NotNull(times, nameof(times));

			if (expression.IsProperty())
			{
				VerifyGet(mock, expression, times, failMessage);
			}
			else
			{
				var (obj, method, args) = expression.GetCallInfo(mock);
				ThrowIfVerifyExpressionInvolvesUnsupportedMember(expression, method);

				var expectation = new InvocationShape(method, args);
				VerifyCalls(GetTargetMock(obj, mock), expectation, expression, times, failMessage);
			}
		}

		internal static void VerifyGet(Mock mock, LambdaExpression expression, Times times, string failMessage)
		{
			var method = expression.ToPropertyInfo().GetGetMethod(true);
			ThrowIfVerifyExpressionInvolvesUnsupportedMember(expression, method);

			var expectation = new InvocationShape(method, new IMatcher[0]);
			VerifyCalls(GetTargetMock(((MemberExpression)expression.Body).Expression, mock), expectation, expression, times, failMessage);
		}

		internal static void VerifySet(Mock mock, Delegate setterExpression, Times times, string failMessage)
		{
			var (targetMock, expression, method, value) = SetupSetImpl(mock, setterExpression);
			var expectation = new InvocationShape(method, value);
			VerifyCalls(targetMock, expectation, expression, times, failMessage);
		}

		internal static void VerifyNoOtherCalls(Mock mock)
		{
			var unverifiedInvocations = mock.MutableInvocations.ToArray(invocation => !invocation.Verified);

			if (unverifiedInvocations.Any())
			{
				// There are some invocations that shouldn't require explicit verification by the user.
				// The intent behind a `Verify` call for a call expression like `m.A.B.C.X` is probably
				// to verify `X`. If that succeeds, it's reasonable to expect that `m.A`, `m.A.B`, and
				// `m.A.B.C` have implicitly been verified as well. Below, invocations such as those to
				// the left of `X` are referred to as "transitive" (for lack of a better word).
				if (mock.InnerMocks.Any())
				{
					for (int i = 0, n = unverifiedInvocations.Length; i < n; ++i)
					{
						// In order for an invocation to be "transitive", its return value has to be a
						// sub-object (inner mock); and that sub-object has to have received at least
						// one call:
						var wasTransitiveInvocation = mock.InnerMocks.TryGetValue(unverifiedInvocations[i].Method, out MockWithWrappedMockObject inner)
						                              && inner.Mock.MutableInvocations.Any();
						if (wasTransitiveInvocation)
						{
							unverifiedInvocations[i] = null;
						}
					}
				}

				// "Transitive" invocations have been nulled out. Let's see what's left:
				var remainingUnverifiedInvocations = unverifiedInvocations.Where(i => i != null);
				if (remainingUnverifiedInvocations.Any())
				{
					throw MockException.UnverifiedInvocations(mock, remainingUnverifiedInvocations);
				}
			}

			// Perform verification for all automatically created sub-objects (that is, those
			// created by "transitive" invocations):
			foreach (var inner in mock.InnerMocks.Values)
			{
				VerifyNoOtherCalls(inner.Mock);
			}
		}

		private static void VerifyCalls(
			Mock targetMock,
			InvocationShape expectation,
			LambdaExpression expression,
			Times times,
			string failMessage)
		{
			var allInvocations = targetMock.MutableInvocations.ToArray();
			var matchingInvocations = allInvocations.Where(expectation.IsMatch).ToArray();
			var matchingInvocationCount = matchingInvocations.Length;
			if (!times.Verify(matchingInvocationCount))
			{
				Setup[] setups;
				if (targetMock.IsDelegateMock)
				{
					// For delegate mocks, there's no need to compare methods as for regular mocks (below)
					// since there's only one single method, so include all setups unconditionally.
					setups = targetMock.Setups.ToArrayLive(s => true);
				}
				else
				{
					setups = targetMock.Setups.ToArrayLive(oc => AreSameMethod(oc.Expression, expression));
				}
				throw MockException.NoMatchingCalls(failMessage, setups, allInvocations, expression, times, matchingInvocationCount);
			}
			else
			{
				foreach (var matchingInvocation in matchingInvocations)
				{
					matchingInvocation.MarkAsVerified();
				}
			}

			bool AreSameMethod(LambdaExpression l, LambdaExpression r) =>
				l.Body is MethodCallExpression lc && r.Body is MethodCallExpression rc && lc.Method == rc.Method;
		}

		#endregion

		#region Setup

		[DebuggerStepThrough]
		internal static MethodCall SetupVoid(Mock mock, LambdaExpression expression, Condition condition)
		{
			return PexProtector.Invoke(SetupVoidPexProtected, mock, expression, condition);
		}

		private static MethodCall SetupVoidPexProtected(Mock mock, LambdaExpression expression, Condition condition)
		{
			var (obj, method, args) = expression.GetCallInfo(mock);

			ThrowIfSetupExpressionInvolvesUnsupportedMember(expression, method);
			ThrowIfSetupMethodNotVisibleToProxyFactory(method);
			var setup = new MethodCall(mock, condition, expression, method, args);

			var targetMock = GetTargetMock(obj, mock);
			targetMock.Setups.Add(setup);

			return setup;
		}

		[DebuggerStepThrough]
		internal static MethodCall SetupNonVoid(Mock mock, LambdaExpression expression, Condition condition)
		{
			return PexProtector.Invoke(SetupNonVoidPexProtected, mock, expression, condition);
		}

		private static MethodCall SetupNonVoidPexProtected(Mock mock, LambdaExpression expression, Condition condition)
		{
			if (expression.IsProperty())
			{
				return SetupGet(mock, expression, condition);
			}

			var (obj, method, args) = expression.GetCallInfo(mock);

			ThrowIfSetupExpressionInvolvesUnsupportedMember(expression, method);
			ThrowIfSetupMethodNotVisibleToProxyFactory(method);
			var setup = new MethodCall(mock, condition, expression, method, args);

			var targetMock = GetTargetMock(obj, mock);
			targetMock.Setups.Add(setup);

			return setup;
		}

		[DebuggerStepThrough]
		internal static MethodCall SetupGet(Mock mock, LambdaExpression expression, Condition condition)
		{
			return PexProtector.Invoke(SetupGetPexProtected, mock, expression, condition);
		}

		private static MethodCall SetupGetPexProtected(Mock mock, LambdaExpression expression, Condition condition)
		{
			if (expression.IsPropertyIndexer())
			{
				// Treat indexers as regular method invocations.
				return SetupNonVoid(mock, expression, condition);
			}

			var prop = expression.ToPropertyInfo();
			Guard.CanRead(prop);

			var propGet = prop.GetGetMethod(true);
			ThrowIfSetupExpressionInvolvesUnsupportedMember(expression, propGet);
			ThrowIfSetupMethodNotVisibleToProxyFactory(propGet);

			var setup = new MethodCall(mock, condition, expression, propGet, new Expression[0]);
			// Directly casting to MemberExpression is fine as ToPropertyInfo would throw if it wasn't
			var targetMock = GetTargetMock(((MemberExpression)expression.Body).Expression, mock);
			targetMock.Setups.Add(setup);

			return setup;
		}

		[DebuggerStepThrough]
		internal static MethodCall SetupSet(Mock mock, Delegate setterExpression, Condition condition)
		{
			return PexProtector.Invoke(SetupSetPexProtected, mock, setterExpression, condition);
		}

		private static MethodCall SetupSetPexProtected(Mock mock, Delegate setterExpression, Condition condition)
		{
			var (m, expr, method, value) = SetupSetImpl(mock, setterExpression);
			var setup = new MethodCall(m, condition, expr, method, value);
			m.Setups.Add(setup);
			return setup;
		}

		internal static MethodCall SetupSet(Mock mock, LambdaExpression expression, Expression valueExpression)
		{
			var prop = expression.ToPropertyInfo();
			Guard.CanWrite(prop);

			var propSet = prop.GetSetMethod(true);
			ThrowIfSetupExpressionInvolvesUnsupportedMember(expression, propSet);
			ThrowIfSetupMethodNotVisibleToProxyFactory(propSet);

			var setup = new MethodCall(mock, null, expression, propSet, new[] { valueExpression });
			var targetMock = GetTargetMock(((MemberExpression)expression.Body).Expression, mock);

			targetMock.Setups.Add(setup);

			return setup;
		}

		private static SetupSetImplResult SetupSetImpl(Mock mock, Delegate setterExpression)
		{
			Mock target;
			Invocation invocation;
			AmbientObserver.Matches matches;

			using (var observer = AmbientObserver.Activate())
			{
				setterExpression.DynamicInvoke(mock.Object);

				if (!observer.LastIsInvocation(out target, out invocation, out matches))
				{
					throw new ArgumentException(string.Format(
						CultureInfo.InvariantCulture,
						Resources.SetupOnNonVirtualMember,
						string.Empty));
				}
			}

			var setter = invocation.Method;
			if (!setter.IsPropertySetter())
			{
				throw new ArgumentException(Resources.SetupNotSetter);
			}

			// No need to call ThrowIfCantOverride as non-overridable would have thrown above already.

			// Get the variable name as used in the actual delegate :)
			// because of delegate currying, look at the last parameter for the Action's backing method, not the first
			var setterExpressionParameters = setterExpression.GetMethodInfo().GetParameters();
			var parameterName = setterExpressionParameters[setterExpressionParameters.Length - 1].Name;
			var x = Expression.Parameter(invocation.Method.DeclaringType, parameterName);

			var arguments = invocation.Arguments;
			var parameters = setter.GetParameters();
			var values = new Expression[arguments.Length];

			if (matches.Count == 0)
			{
				// Length == 1 || Length == 2 (Indexer property)
				for (int i = 0; i < arguments.Length; i++)
				{
					values[i] = GetValueExpression(arguments[i], parameters[i].ParameterType);
				}

				var lambda = Expression.Lambda(
					typeof(Action<>).MakeGenericType(x.Type),
					Expression.Call(x, invocation.Method, values),
					x);

				return new SetupSetImplResult(target, lambda, invocation.Method, values);
			}
			else
			{
				// TODO: Use all observed matchers, not just the last one!
				var lastMatch = matches[matches.Count - 1];

				var matchers = new Expression[arguments.Length];
				var valueIndex = arguments.Length - 1;
				var propertyType = setter.GetParameters()[valueIndex].ParameterType;

				// If the value matcher is not equal to the property 
				// type (i.e. prop is int?, but you use It.IsAny<int>())
				// add a cast.
				if (lastMatch.RenderExpression.Type != propertyType)
				{
					values[valueIndex] = Expression.Convert(lastMatch.RenderExpression, propertyType);
				}
				else
				{
					values[valueIndex] = lastMatch.RenderExpression;
				}

				matchers[valueIndex] = new MatchExpression(lastMatch);

				for (int i = 0; i < arguments.Length - 1; i++)
				{
					// Add the index value for the property indexer
					values[i] = GetValueExpression(arguments[i], parameters[i].ParameterType);
					// TODO: No matcher supported now for the index
					matchers[i] = values[i];
				}

				var lambda = Expression.Lambda(
					typeof(Action<>).MakeGenericType(x.Type),
					Expression.Call(x, invocation.Method, values),
					x);

				return new SetupSetImplResult(target, lambda, invocation.Method, matchers);
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

		internal static SequenceSetup SetupSequence(Mock mock, LambdaExpression expression)
		{
			if (expression.IsProperty())
			{
				var prop = expression.ToPropertyInfo();
				Guard.CanRead(prop);

				var propGet = prop.GetGetMethod(true);
				ThrowIfSetupExpressionInvolvesUnsupportedMember(expression, propGet);
				ThrowIfSetupMethodNotVisibleToProxyFactory(propGet);

				var setup = new SequenceSetup(expression, propGet, new Expression[0]);
				var targetMock = GetTargetMock(((MemberExpression)expression.Body).Expression, mock);
				targetMock.Setups.Add(setup);
				return setup;
			}
			else
			{
				var (obj, method, args) = expression.GetCallInfo(mock);
				var setup = new SequenceSetup(expression, method, args);
				var targetMock = GetTargetMock(obj, mock);
				targetMock.Setups.Add(setup);
				return setup;
			}
		}

		[DebuggerStepThrough]
		internal static void SetupAllProperties(Mock mock)
		{
			PexProtector.Invoke(SetupAllPropertiesPexProtected, mock, mock.DefaultValueProvider);
			//                                                        ^^^^^^^^^^^^^^^^^^^^^^^^^
			// `SetupAllProperties` no longer performs eager recursive property setup like in previous versions.
			// If `mock` uses `DefaultValue.Mock`, mocked sub-objects are now only constructed when queried for
			// the first time. In order for `SetupAllProperties`'s new mode of operation to be indistinguishable
			// from how it worked previously, it's important to capture the default value provider at this precise
			// moment, since it might be changed later (before queries to properties of a mockable type).
		}

		private static void SetupAllPropertiesPexProtected(Mock mock, DefaultValueProvider defaultValueProvider)
		{
			var mockType = mock.MockedType;

			var properties =
				mockType
				.GetAllPropertiesInDepthFirstOrder()
				// ^ Depth-first traversal is important because properties in derived interfaces
				//   that shadow properties in base interfaces should be set up last. This
				//   enables the use case where a getter-only property is redeclared in a derived
				//   interface as a getter-and-setter property.
				.Where(p =>
					   p.CanRead && p.CanOverrideGet() &&
					   p.CanWrite == p.CanOverrideSet() &&
					   // ^ This condition will be true for two kinds of properties:
					   //    (a) those that are read-only; and
					   //    (b) those that are writable and whose setter can be overridden.
					   p.GetIndexParameters().Length == 0 &&
					   ProxyFactory.Instance.IsMethodVisible(p.GetGetMethod(), out _))
				.Distinct();

			foreach (var property in properties)
			{
				var expression = GetPropertyExpression(mockType, property);
				var getter = property.GetGetMethod(true);

				object value = null;
				bool valueNotSet = true;

				mock.Setups.Add(new AutoImplementedPropertyGetterSetup(expression, getter, () =>
				{
					if (valueNotSet)
					{
						object initialValue;
						try
						{
							initialValue = mock.GetDefaultValue(getter, useAlternateProvider: defaultValueProvider);
						}
						catch
						{
							// Since this method performs a batch operation, a single failure of the default value
							// provider should not tear down the whole operation. The empty default value provider
							// is a safe fallback because it does not throw.
							initialValue = mock.GetDefaultValue(getter, useAlternateProvider: DefaultValueProvider.Empty);
						}

						if (initialValue is IMocked mocked)
						{
							SetupAllPropertiesPexProtected(mocked.Mock, defaultValueProvider);
						}

						value = initialValue;
						valueNotSet = false;
					}
					return value;
				}));

				if (property.CanWrite)
				{
					mock.Setups.Add(new AutoImplementedPropertySetterSetup(expression, property.GetSetMethod(true), (newValue) =>
					{
						value = newValue;
						valueNotSet = false;
					}));
				}
			}
		}

		private static LambdaExpression GetPropertyExpression(Type mockType, PropertyInfo property)
		{
			var param = Expression.Parameter(mockType, "m");
			return Expression.Lambda(Expression.MakeMemberAccess(param, property), param);
		}

		/// <summary>
		/// Gets the interceptor target for the given expression and root mock, 
		/// building the intermediate hierarchy of mock objects if necessary.
		/// </summary>
		private static Mock GetTargetMock(Expression fluentExpression, Mock mock)
		{
			if (fluentExpression is ParameterExpression)
			{
				// fast path for single-dot setup expressions;
				// no need for expensive lambda compilation.
				return mock;
			}

			var targetExpression = VisitFluent(mock, fluentExpression);
			var targetLambda = Expression.Lambda<Func<Mock>>(Expression.Convert(targetExpression, typeof(Mock)));

			var targetObject = targetLambda.CompileUsingExpressionCompiler()();
			return targetObject;
		}

		private static void ThrowIfSetupMethodNotVisibleToProxyFactory(MethodInfo method)
		{
			if (ProxyFactory.Instance.IsMethodVisible(method, out string messageIfNotVisible) == false)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.MethodNotVisibleToProxyFactory,
					method.DeclaringType.Name,
					method.Name,
					messageIfNotVisible));
			}
		}

		private static void ThrowIfSetupExpressionInvolvesUnsupportedMember(Expression setup, MethodInfo method)
		{
			if (method.IsStatic)
			{
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					method.IsExtensionMethod() ? Resources.SetupOnExtensionMethod : Resources.SetupOnStaticMember,
					setup.ToStringFixed()));
			}
			else if (!method.CanOverride())
			{
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.SetupOnNonVirtualMember,
					setup.ToStringFixed()));
			}
		}

		private static void ThrowIfVerifyExpressionInvolvesUnsupportedMember(Expression verify, MethodInfo method)
		{
			if (method.IsStatic)
			{
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					method.IsExtensionMethod() ? Resources.VerifyOnExtensionMethod : Resources.VerifyOnStaticMember,
					verify.ToStringFixed()));
			}
			else if (!method.CanOverride())
			{
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.VerifyOnNonVirtualMember,
					verify.ToStringFixed()));
			}
		}

		private static Expression VisitFluent(Mock mock, Expression expression)
		{
			return new FluentMockVisitor(resolveRoot: p => Expression.Constant(mock),
			                             setupRightmost: false)
			       .Visit(expression);
		}

		#endregion

		#region Raise

		/// <summary>
		/// Raises the associated event with the given 
		/// event argument data.
		/// </summary>
		internal void DoRaise(EventInfo ev, EventArgs args)
		{
			if (ev == null)
			{
				throw new InvalidOperationException(Resources.RaisedUnassociatedEvent);
			}

			foreach (var del in this.EventHandlers.ToArray(ev.Name))
			{
				del.InvokePreserveStack(this.Object, args);
			}
		}

		/// <summary>
		/// Raises the associated event with the given
		/// event argument data.
		/// </summary>
		internal void DoRaise(EventInfo ev, params object[] args)
		{
			if (ev == null)
			{
				throw new InvalidOperationException(Resources.RaisedUnassociatedEvent);
			}

			foreach (var del in this.EventHandlers.ToArray(ev.Name))
			{
				// Non EventHandler-compatible delegates get the straight
				// arguments, not the typical "sender, args" arguments.
				del.InvokePreserveStack(args);
			}
		}

		#endregion

		#region As<TInterface>

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.As{TInterface}"]/*'/>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "As", Justification = "We want the method called exactly as the keyword because that's what it does, it adds an implemented interface so that you can cast it later.")]
		public virtual Mock<TInterface> As<TInterface>()
			where TInterface : class
		{
			var interfaceType = typeof(TInterface);

			if (!interfaceType.GetTypeInfo().IsInterface)
			{
				throw new ArgumentException(Resources.AsMustBeInterface);
			}

			if (this.IsObjectInitialized && this.ImplementsInterface(interfaceType) == false)
			{
				throw new InvalidOperationException(Resources.AlreadyInitialized);
			}

			if (this.AdditionalInterfaces.Contains(interfaceType) == false)
			{
				// We get here for either of two reasons:
				//
				// 1. We are being asked to implement an interface that the mocked type does *not* itself
				//    inherit or implement. We need to hand this interface type to DynamicProxy's
				//    `CreateClassProxy` method as an additional interface to be implemented.
				//
				// 2. The user is possibly going to create a setup through an interface type that the
				//    mocked type *does* implement. Since the mocked type might implement that interface's
				//    methods non-virtually, we can only intercept those if DynamicProxy reimplements the
				//    interface in the generated proxy type. Therefore we do the same as for (1).
				this.AdditionalInterfaces.Add(interfaceType);
			}

			return new AsInterface<TInterface>(this);
		}

		internal bool ImplementsInterface(Type interfaceType)
		{
			return this.InheritedInterfaces.Contains(interfaceType)
				|| this.AdditionalInterfaces.Contains(interfaceType);
		}

		#endregion

		#region Default Values

		internal abstract Dictionary<Type, object> ConfiguredDefaultValues { get; }

		/// <summary>
		/// Defines the default return value for all mocked methods or properties with return type <typeparamref name= "TReturn" />.
		/// </summary>
		/// <typeparam name="TReturn">The return type for which to define a default value.</typeparam>
		/// <param name="value">The default return value.</param>
		/// <remarks>
		/// Default return value is respected only when there is no matching setup for a method call.
		/// </remarks>
		public void SetReturnsDefault<TReturn>(TReturn value)
		{
			this.ConfiguredDefaultValues[typeof(TReturn)] = value;
		}

		internal object GetDefaultValue(MethodInfo method, DefaultValueProvider useAlternateProvider = null)
		{
			Debug.Assert(method != null);
			Debug.Assert(method.ReturnType != null);
			Debug.Assert(method.ReturnType != typeof(void));

			if (this.ConfiguredDefaultValues.TryGetValue(method.ReturnType, out object configuredDefaultValue))
			{
				return configuredDefaultValue;
			}

			var result = (useAlternateProvider ?? this.DefaultValueProvider).GetDefaultReturnValue(method, this);
			var unwrappedResult = TryUnwrapResultFromCompletedTaskRecursively(result);

			if (unwrappedResult is IMocked unwrappedMockedResult)
			{
				// TODO: Perhaps the following `InnerMocks` update isn't in quite the right place yet.
				// There are two main places in Moq where `InnerMocks` are used: `Mock<T>.FluentMock` and
				// the `HandleMockRecursion` interception strategy. Both places first query `InnerMocks`,
				// and if no value for a given member is present, the default value provider get invoked
				// via the present method. Querying and updating `InnerMocks` is thus spread over two
				// code locations and therefore non-atomic. It would be good if those could be combined
				// (`InnerMocks.GetOrAdd`), but that might not be easily possible since `InnerMocks` is
				// only mocks while default value providers can also return plain, unmocked values.
				this.InnerMocks.TryAdd(method, new MockWithWrappedMockObject(unwrappedMockedResult.Mock, result));
			}

			return result;
		}

		/// <summary>
		/// Recursively unwraps the result from completed <see cref="Task{TResult}"/> or <see cref="ValueTask{TResult}"/> instances.
		/// If the given value is not a task, the value itself is returned.
		/// </summary>
		/// <param name="obj">The value to be unwrapped.</param>
		private static object TryUnwrapResultFromCompletedTaskRecursively(object obj)
		{
			if (obj != null)
			{
				var objType = obj.GetType();
				if (objType.GetTypeInfo().IsGenericType)
				{
					var genericTypeDefinition = objType.GetGenericTypeDefinition();
					if (genericTypeDefinition == typeof(Task<>) || genericTypeDefinition == typeof(ValueTask<>))
					{
						var isCompleted = (bool)objType.GetProperty("IsCompleted").GetValue(obj, null);
						if (isCompleted)
						{
							var innerObj = objType.GetProperty("Result").GetValue(obj, null);
							return TryUnwrapResultFromCompletedTaskRecursively(innerObj);
						}
					}
				}
			}

			return obj;
		}

		#endregion

		private readonly struct SetupSetImplResult
		{
			private readonly Mock mock;
			private readonly LambdaExpression lambda;
			private readonly MethodInfo method;
			private readonly Expression[] arguments;

			public SetupSetImplResult(Mock mock, LambdaExpression lambda, MethodInfo method, Expression[] arguments)
			{
				this.mock = mock;
				this.lambda = lambda;
				this.method = method;
				this.arguments = arguments;
			}

			public void Deconstruct(out Mock mock, out LambdaExpression lambda, out MethodInfo method, out Expression[] arguments)
			{
				mock = this.mock;
				lambda = this.lambda;
				method = this.method;
				arguments = this.arguments;
			}
		}
	}
}
