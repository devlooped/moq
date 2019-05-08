// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Properties;

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
			if (mocked is IMocked<T> mockedOfT)
			{
				// This would be the fastest check.
				return mockedOfT.Mock;
			}

			if (mocked is Delegate aDelegate && aDelegate.Target is IMocked<T> mockedDelegateImpl)
			{
				return mockedDelegateImpl.Mock;
			}

			if (mocked is IMocked mockedPlain)
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
				var imockedType = mocked.GetType().GetInterfaces().Single(i => i.Name.Equals("IMocked`1", StringComparison.Ordinal));
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

		internal abstract object[] ConstructorArguments { get; }

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
			var error = this.TryVerify();
			if (error?.IsVerificationError == true)
			{
				throw error;
			}
		}

		/// <include file='Mock.xdoc' path='docs/doc[@for="Mock.VerifyAll"]/*'/>
		public void VerifyAll()
		{
			var error = this.TryVerifyAll();
			if (error?.IsVerificationError == true)
			{
				throw error;
			}
		}

		internal MockException TryVerify()
		{
			foreach (Invocation invocation in this.MutableInvocations)
			{
				invocation.MarkAsVerifiedIfMatchedByVerifiableSetup();
			}

			return this.TryVerifySetups(setup => setup.TryVerify());
		}

		internal MockException TryVerifyAll()
		{
			foreach (Invocation invocation in this.MutableInvocations)
			{
				invocation.MarkAsVerifiedIfMatchedBySetup();
			}

			return this.TryVerifySetups(setup => setup.TryVerifyAll());
		}

		private MockException TryVerifySetups(Func<Setup, MockException> verifySetup)
		{
			var errors = new List<MockException>();

			foreach (var setup in this.Setups.ToArrayLive(_ => true))
			{
				var error = verifySetup(setup);
				if (error?.IsVerificationError == true)
				{
					errors.Add(error);
				}
			}

			if (errors.Count > 0)
			{
				return MockException.Combined(
					errors,
					preamble: string.Format(CultureInfo.CurrentCulture, Resources.VerificationErrorsOfMock, this));
			}
			else
			{
				return null;
			}
		}

		internal static void Verify(Mock mock, LambdaExpression expression, Times times, string failMessage)
		{
			Guard.NotNull(times, nameof(times));

			var invocationCount = Mock.GetMatchingInvocationCount(mock, expression, out var invocationsToBeMarkedAsVerified);

			if (times.Verify(invocationCount))
			{
				foreach (var invocation in invocationsToBeMarkedAsVerified)
				{
					invocation.MarkAsVerified();
				}
			}
			else
			{
				throw MockException.NoMatchingCalls(mock, expression, failMessage, times, invocationCount);
			}
		}

		internal static void VerifyGet(Mock mock, LambdaExpression expression, Times times, string failMessage)
		{
			Guard.NotNull(expression, nameof(expression));

			if (!expression.IsPropertyIndexer())  // guard because `.ToPropertyInfo()` doesn't (yet) work for indexers
			{
				var property = expression.ToPropertyInfo();
				Guard.CanRead(property);
			}

			Mock.Verify(mock, expression, times, failMessage);
		}

		internal static void VerifySet(Mock mock, LambdaExpression expression, Times times, string failMessage)
		{
			Guard.NotNull(expression, nameof(expression));
			Guard.IsAssignmentToPropertyOrIndexer(expression, nameof(expression));

			Mock.Verify(mock, expression, times, failMessage);
		}

		internal static void VerifyNoOtherCalls(Mock mock)
		{
			var unverifiedInvocations = mock.MutableInvocations.ToArray(invocation => !invocation.Verified);

			var innerMockSetups = mock.Setups.GetInnerMockSetups();

			if (unverifiedInvocations.Any())
			{
				// There are some invocations that shouldn't require explicit verification by the user.
				// The intent behind a `Verify` call for a call expression like `m.A.B.C.X` is probably
				// to verify `X`. If that succeeds, it's reasonable to expect that `m.A`, `m.A.B`, and
				// `m.A.B.C` have implicitly been verified as well. Below, invocations such as those to
				// the left of `X` are referred to as "transitive" (for lack of a better word).
				if (innerMockSetups.Any())
				{
					for (int i = 0, n = unverifiedInvocations.Length; i < n; ++i)
					{
						// In order for an invocation to be "transitive", its return value has to be a
						// sub-object (inner mock); and that sub-object has to have received at least
						// one call:
						var wasTransitiveInvocation = innerMockSetups.TryFind(unverifiedInvocations[i], out var inner)
						                              && inner.GetInnerMock().MutableInvocations.Any();
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
			foreach (var inner in innerMockSetups)
			{
				VerifyNoOtherCalls(inner.GetInnerMock());
			}
		}

		private static int GetMatchingInvocationCount(
			Mock mock,
			LambdaExpression expression,
			out List<Invocation> invocationsToBeMarkedAsVerified)
		{
			Debug.Assert(mock != null);
			Debug.Assert(expression != null);

			invocationsToBeMarkedAsVerified = new List<Invocation>();
			return Mock.GetMatchingInvocationCount(
				mock,
				new ImmutablePopOnlyStack<InvocationShape>(expression.Split()),
				new HashSet<Mock>(),
				invocationsToBeMarkedAsVerified);
		}

		private static int GetMatchingInvocationCount(
			Mock mock,
			in ImmutablePopOnlyStack<InvocationShape> parts,
			HashSet<Mock> visitedInnerMocks,
			List<Invocation> invocationsToBeMarkedAsVerified)
		{
			Debug.Assert(mock != null);
			Debug.Assert(!parts.Empty);
			Debug.Assert(visitedInnerMocks != null);

			// Several different invocations might return the same inner `mock`.
			// Keep track of the mocks whose invocations have already been counted:
			if (visitedInnerMocks.Contains(mock))
			{
				return 0;
			}
			visitedInnerMocks.Add(mock);

			var part = parts.Pop(out var remainingParts);

			var count = 0;
			foreach (var matchingInvocation in mock.MutableInvocations.ToArray().Where(part.IsMatch))
			{
				invocationsToBeMarkedAsVerified.Add(matchingInvocation);

				if (remainingParts.Empty)
				{
					// We are not processing an intermediate part of a fluent expression.
					// Therefore, every matching invocation counts as one:
					++count;
				}
				else
				{
					// We are processing an intermediate part of a fluent expression.
					// Therefore, all matching invocations are assumed to have a return value;
					// otherwise, they wouldn't be "chainable":
					Debug.Assert(matchingInvocation.Method.ReturnType != typeof(void));

					// Intermediate parts of a fluent expression do not contribute to the
					// total count themselves. The matching invocation count of the rightmost
					// expression gets "forwarded" towards the left:
					if (Unwrap.ResultIfCompletedTask(matchingInvocation.ReturnValue) is IMocked mocked)
					{
						count += Mock.GetMatchingInvocationCount(mocked.Mock, remainingParts, visitedInnerMocks, invocationsToBeMarkedAsVerified);
					}
				}
			}

			return count;
		}

		#endregion

		#region Setup

		internal static MethodCall Setup(Mock mock, LambdaExpression expression, Condition condition)
		{
			Guard.NotNull(expression, nameof(expression));

			return Mock.SetupRecursive(mock, expression, setupLast: (part, targetMock) =>
			{
				var setup = new MethodCall(targetMock, condition, expectation: part);
				targetMock.Setups.Add(setup);
				return setup;
			});
		}

		internal static MethodCall SetupGet(Mock mock, LambdaExpression expression, Condition condition)
		{
			Guard.NotNull(expression, nameof(expression));

			if (!expression.IsPropertyIndexer())  // guard because `.ToPropertyInfo()` doesn't (yet) work for indexers
			{
				var property = expression.ToPropertyInfo();
				Guard.CanRead(property);
			}

			return Mock.Setup(mock, expression, condition);
		}

		internal static MethodCall SetupSet(Mock mock, LambdaExpression expression, Condition condition)
		{
			Guard.NotNull(expression, nameof(expression));
			Guard.IsAssignmentToPropertyOrIndexer(expression, nameof(expression));

			return Mock.Setup(mock, expression, condition);
		}

		internal static SequenceSetup SetupSequence(Mock mock, LambdaExpression expression)
		{
			Guard.NotNull(expression, nameof(expression));

			return Mock.SetupRecursive(mock, expression, setupLast: (part, targetMock) =>
			{
				var setup = new SequenceSetup(expectation: part);
				targetMock.Setups.Add(setup);
				return setup;
			});
		}

		private static TSetup SetupRecursive<TSetup>(Mock mock, LambdaExpression expression, Func<InvocationShape, Mock, TSetup> setupLast)
		{
			Debug.Assert(mock != null);
			Debug.Assert(expression != null);
			Debug.Assert(setupLast != null);

			var parts = expression.Split();
			return Mock.SetupRecursive(mock, expression, parts, setupLast);
		}

		private static TSetup SetupRecursive<TSetup>(Mock mock, LambdaExpression expression, Stack<InvocationShape> parts, Func<InvocationShape, Mock, TSetup> setupLast)
		{
			var part = parts.Pop();
			var (expr, method, arguments) = part;

			if (parts.Count == 0)
			{
				return setupLast(part, mock);
			}
			else
			{
				Mock innerMock;
				if (!(mock.Setups.GetInnerMockSetups().TryFind(part, out var setup) && setup.ReturnsInnerMock(out innerMock)))
				{
					var returnValue = mock.GetDefaultValue(method, out innerMock, useAlternateProvider: DefaultValueProvider.Mock);
					if (innerMock == null)
					{
						throw new ArgumentException(
							string.Format(
								CultureInfo.CurrentCulture,
								Resources.UnsupportedExpression,
								expr.ToStringFixed() + " in " + expression.ToStringFixed() + ":\n" + Resources.InvalidMockClass));
					}
					setup = new InnerMockSetup(expectation: part, returnValue);
					mock.Setups.Add((Setup)setup);
				}
				Debug.Assert(innerMock != null);

				return Mock.SetupRecursive(innerMock, expression, parts, setupLast);
			}
		}

		internal static void SetupAllProperties(Mock mock)
		{
			mock.Switches |= Switches.AutoSetupProperties;
		}

		#endregion

		#region Raise

		internal static void RaiseEvent<T>(Mock mock, Action<T> action, object[] arguments)
		{
			Guard.NotNull(action, nameof(action));

			var expression = ExpressionReconstructor.Instance.ReconstructExpression(action, mock.ConstructorArguments);
			var parts = expression.Split();
			Mock.RaiseEvent(mock, expression, parts, arguments);
		}

		internal static void RaiseEvent(Mock mock, LambdaExpression expression, Stack<InvocationShape> parts, object[] arguments)
		{
			var part = parts.Pop();
			var method = part.Method;

			if (parts.Count == 0)
			{
				string eventName;
				if (method.Name.StartsWith("add_", StringComparison.Ordinal))
				{
					eventName = method.Name.Substring(4);
				}
				else if (method.Name.StartsWith("remove_", StringComparison.Ordinal))
				{
					eventName = method.Name.Substring(7);
				}
				else
				{
					throw new ArgumentException(
						string.Format(
							CultureInfo.CurrentCulture,
							Resources.UnsupportedExpression,
							expression));
				}

				foreach (var eventHandler in mock.EventHandlers.ToArray(eventName))
				{
					eventHandler.InvokePreserveStack(arguments);
				}

			}
			else if (mock.Setups.GetInnerMockSetups().TryFind(part, out var innerMockSetup) && innerMockSetup.ReturnsInnerMock(out var innerMock))
			{
				Mock.RaiseEvent(innerMock, expression, parts, arguments);
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

			if (!interfaceType.IsInterface)
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

		internal object GetDefaultValue(MethodInfo method, out Mock candidateInnerMock, DefaultValueProvider useAlternateProvider = null)
		{
			Debug.Assert(method != null);
			Debug.Assert(method.ReturnType != null);
			Debug.Assert(method.ReturnType != typeof(void));

			if (this.ConfiguredDefaultValues.TryGetValue(method.ReturnType, out object configuredDefaultValue))
			{
				candidateInnerMock = null;
				return configuredDefaultValue;
			}

			var result = (useAlternateProvider ?? this.DefaultValueProvider).GetDefaultReturnValue(method, this);
			var unwrappedResult = Unwrap.ResultIfCompletedTask(result);

			candidateInnerMock = (unwrappedResult as IMocked)?.Mock;
			return result;
		}

		#endregion

		#region Inner mocks

		internal void AddInnerMockSetup(Invocation invocation, object returnValue)
		{
			var method = invocation.Method;

			Expression[] arguments;
			{
				var parameterTypes = method.GetParameterTypes();
				var n = parameterTypes.Count;
				arguments = new Expression[n];
				for (int i = 0; i < n; ++i)
				{
					arguments[i] = Expression.Constant(invocation.Arguments[i], parameterTypes[i]);
				}
			}

			LambdaExpression expression;
			{
				var mock = Expression.Parameter(method.DeclaringType, "mock");
				expression = Expression.Lambda(Expression.Call(mock, method, arguments), mock);
			}

			this.AddInnerMockSetup(invocation.Method, arguments, expression, returnValue);
		}

		internal void AddInnerMockSetup(MethodInfo method, IReadOnlyList<Expression> arguments, LambdaExpression expression, object returnValue)
		{
			if (expression.IsProperty())
			{
				var property = expression.ToPropertyInfo();
				Guard.CanRead(property);

				Debug.Assert(method == property.GetGetMethod(true));
			}

			this.Setups.Add(new InnerMockSetup(new InvocationShape(expression, method, arguments), returnValue));
		}

		#endregion
	}
}
