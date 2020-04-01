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
	/// <summary>
	///   Base class for mocks and static helper class with methods that apply to mocked objects,
	///   such as <see cref="Get"/> to retrieve a <see cref="Mock{T}"/> from an object instance.
	/// </summary>
	public abstract partial class Mock : IFluentInterface
	{
		internal static readonly MethodInfo GetMethod =
			typeof(Mock).GetMethod(nameof(Get), BindingFlags.Public | BindingFlags.Static);

		/// <summary>
		///   Initializes a new instance of the <see cref="Mock"/> class.
		/// </summary>
		protected Mock()
		{
		}

		/// <summary>
		///   Retrieves the mock object for the given object instance.
		/// </summary>
		/// <param name="mocked">The instance of the mocked object.</param>
		/// <typeparam name="T">
		///   Type of the mock to retrieve.
		///   Can be omitted as it's inferred from the object instance passed in as the <paramref name="mocked"/> instance.
		/// </typeparam>
		/// <returns>The mock associated with the mocked object.</returns>
		/// <exception cref="ArgumentException">The received <paramref name="mocked"/> instance was not created by Moq.</exception>
		/// <example group="advanced">
		///   The following example shows how to add a new setup to an object instance
		///   which is not the original <see cref="Mock{T}"/> but rather the object associated with it:
		///   <code>
		///     // Typed instance, not the mock, is retrieved from some test API.
		///     HttpContextBase context = GetMockContext();
		///
		///     // context.Request is the typed object from the "real" API
		///     // so in order to add a setup to it, we need to get
		///     // the mock that "owns" it
		///     Mock&lt;HttpRequestBase&gt; request = Mock.Get(context.Request);
		///
		///     request.Setup(req => req.AppRelativeCurrentExecutionFilePath)
		///            .Returns(tempUrl);
		///   </code>
		/// </example>
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

		/// <summary>
		///   Verifies that all verifiable expectations have been met.
		/// </summary>
		/// <exception cref="MockException">Not all verifiable expectations were met.</exception>
		public static void Verify(params Mock[] mocks)
		{
			foreach (var mock in mocks)
			{
				mock.Verify();
			}
		}

		/// <summary>
		///   Verifies all expectations regardless of whether they have been flagged as verifiable.
		/// </summary>
		/// <exception cref="MockException">At least one expectation was not met.</exception>
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

		/// <summary>
		///   Behavior of the mock, according to the value set in the constructor.
		/// </summary>
		public abstract MockBehavior Behavior { get; }

		/// <summary>
		///   Whether the base member virtual implementation will be called for mocked classes if no setup is matched.
		///   Defaults to <see langword="false"/>.
		/// </summary>
		public abstract bool CallBase { get; set; }

		internal abstract object[] ConstructorArguments { get; }

		/// <summary>
		///   Specifies the behavior to use when returning default values for unexpected invocations on loose mocks.
		/// </summary>
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

		/// <summary>
		///   Gets the mocked object instance.
		/// </summary>
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

		/// <summary>
		///   Returns the mocked object value.
		/// </summary>
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

		/// <summary>
		/// The <see cref="Moq.DefaultValueProvider"/> used to initialize automatically stubbed properties.
		/// It is equal to the value of <see cref="Moq.Mock.DefaultValueProvider"/> at the time when
		/// <see cref="SetupAllProperties"/> was last called.
		/// </summary>
		internal abstract DefaultValueProvider AutoSetupPropertiesDefaultValueProvider { get; set; } 

		internal abstract SetupCollection MutableSetups { get; }

		/// <summary>
		///   Gets the setups that have been configured on this mock,
		///   in chronological order (that is, oldest setup first, most recent setup last).
		/// </summary>
		public ISetupList Setups => this.MutableSetups;

		/// <summary>
		/// A set of switches that influence how this mock will operate.
		/// You can opt in or out of certain features via this property.
		/// </summary>
		public abstract Switches Switches { get; set; }

		internal abstract Type TargetType { get; }

		#region Verify

		/// <summary>
		///   Verifies that all verifiable expectations have been met.
		/// </summary>
		/// <exception cref="MockException">Not all verifiable expectations were met.</exception>
		/// <example group="verification">
		///   This example sets up an expectation and marks it as verifiable.
		///   After the mock is used, a <c>Verify()</c> call is issued on the mock
		///   to ensure the method in the setup was invoked:
		///   <code>
		///     var mock = new Mock&lt;IWarehouse&gt;();
		///     this.Setup(x =&gt; x.HasInventory(TALISKER, 50))
		///         .Returns(true)
		///         .Verifiable();
		///
		///     ...
		///
		///     // Will throw if the test code did not call HasInventory.
		///     this.Verify();
		///   </code>
		/// </example>
		public void Verify()
		{
			this.Verify(setup => !setup.IsOverridden && !setup.IsConditional && setup.IsVerifiable);
		}

		/// <summary>
		///   Verifies all expectations regardless of whether they have been flagged as verifiable.
		/// </summary>
		/// <exception cref="MockException">At least one expectation was not met.</exception>
		/// <example>
		///   This example sets up an expectation without marking it as verifiable.
		///   After the mock is used, a <see cref="VerifyAll"/> call is issued on the mock
		///   to ensure that all expectations are met:
		///   <code>
		///     var mock = new Mock&lt;IWarehouse&gt;();
		///     this.Setup(x =&gt; x.HasInventory(TALISKER, 50))
		///         .Returns(true);
		///
		///     ...
		///
		///     // Will throw if the test code did not call HasInventory,
		///     // even though that expectation was not marked as verifiable.
		///     mock.VerifyAll();
		///   </code>
		/// </example>
		public void VerifyAll()
		{
			this.Verify(setup => !setup.IsOverridden && !setup.IsConditional);
		}

		private void Verify(Func<ISetup, bool> predicate)
		{
			if (!this.TryVerify(predicate, out var error) && error.IsVerificationError)
			{
				throw error;
			}
		}

		internal bool TryVerify(Func<ISetup, bool> predicate, out MockException error)
		{
			foreach (Invocation invocation in this.MutableInvocations)
			{
				invocation.MarkAsVerifiedIfMatchedBy(predicate);
			}

			var errors = new List<MockException>();

			foreach (var setup in this.MutableSetups.ToArray(predicate))
			{
				if (predicate(setup) && !setup.TryVerify(recursive: true, predicate, out var e) && e.IsVerificationError)
				{
					errors.Add(e);
				}
			}

			if (errors.Count > 0)
			{
				error = MockException.Combined(
					errors,
					preamble: string.Format(CultureInfo.CurrentCulture, Resources.VerificationErrorsOfMock, this));
				return false;
			}
			else
			{
				error = null;
				return true;
			}
		}

		internal static void Verify(Mock mock, LambdaExpression expression, Times times, string failMessage)
		{
			Guard.NotNull(times, nameof(times));

			var invocationCount = Mock.GetMatchingInvocationCount(mock, expression, out var invocationsToBeMarkedAsVerified);

			if (times.Validate(invocationCount))
			{
				foreach (var (invocation, part) in invocationsToBeMarkedAsVerified)
				{
					part.SetupEvaluatedSuccessfully(invocation);
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

		internal static void VerifyAdd(Mock mock, LambdaExpression expression, Times times, string failMessage)
		{
			Guard.NotNull(expression, nameof(expression));
			Guard.IsEventAdd(expression, nameof(expression));

			Mock.Verify(mock, expression, times, failMessage);
		}

		internal static void VerifyRemove(Mock mock, LambdaExpression expression, Times times, string failMessage)
		{
			Guard.NotNull(expression, nameof(expression));
			Guard.IsEventRemove(expression, nameof(expression));

			Mock.Verify(mock, expression, times, failMessage);
		}

		internal static void VerifyNoOtherCalls(Mock mock)
		{
			Mock.VerifyNoOtherCalls(mock, verifiedMocks: new HashSet<Mock>());
		}

		private static void VerifyNoOtherCalls(Mock mock, HashSet<Mock> verifiedMocks)
		{
			if (!verifiedMocks.Add(mock)) return;

			var unverifiedInvocations = mock.MutableInvocations.ToArray(invocation => !invocation.WasVerified);

			var innerMockSetups = mock.MutableSetups.GetInnerMockSetups();

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
				VerifyNoOtherCalls(inner.GetInnerMock(), verifiedMocks);
			}
		}

		private static int GetMatchingInvocationCount(
			Mock mock,
			LambdaExpression expression,
			out List<Pair<Invocation, InvocationShape>> invocationsToBeMarkedAsVerified)
		{
			Debug.Assert(mock != null);
			Debug.Assert(expression != null);

			invocationsToBeMarkedAsVerified = new List<Pair<Invocation, InvocationShape>>();
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
			List<Pair<Invocation, InvocationShape>> invocationsToBeMarkedAsVerified)
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
				invocationsToBeMarkedAsVerified.Add(new Pair<Invocation, InvocationShape>(matchingInvocation, part));

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
				targetMock.MutableSetups.Add(setup);
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

		internal static MethodCall SetupAdd(Mock mock, LambdaExpression expression, Condition condition)
		{
			Guard.NotNull(expression, nameof(expression));
			Guard.IsEventAdd(expression, nameof(expression));

			return Mock.Setup(mock, expression, condition);
		}

		internal static MethodCall SetupRemove(Mock mock, LambdaExpression expression, Condition condition)
		{
			Guard.NotNull(expression, nameof(expression));
			Guard.IsEventRemove(expression, nameof(expression));

			return Mock.Setup(mock, expression, condition);
		}

		internal static SequenceSetup SetupSequence(Mock mock, LambdaExpression expression)
		{
			Guard.NotNull(expression, nameof(expression));

			return Mock.SetupRecursive(mock, expression, setupLast: (part, targetMock) =>
			{
				var setup = new SequenceSetup(expectation: part);
				targetMock.MutableSetups.Add(setup);
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
				if (!(mock.MutableSetups.GetInnerMockSetups().TryFind(part, out var setup) && setup.ReturnsInnerMock(out innerMock)))
				{
					var returnValue = mock.GetDefaultValue(method, out innerMock, useAlternateProvider: DefaultValueProvider.Mock);
					if (innerMock == null)
					{
						throw new ArgumentException(
							string.Format(
								CultureInfo.CurrentCulture,
								Resources.UnsupportedExpression,
								expr.ToStringFixed() + " in " + expression.ToStringFixed() + ":\n" + Resources.TypeNotMockable));
					}
					setup = new InnerMockSetup(expectation: part, returnValue);
					mock.MutableSetups.Add((Setup)setup);
				}
				Debug.Assert(innerMock != null);

				return Mock.SetupRecursive(innerMock, expression, parts, setupLast);
			}
		}

		internal static void SetupAllProperties(Mock mock)
		{
			SetupAllProperties(mock, mock.DefaultValueProvider);
		}

		internal static void SetupAllProperties(Mock mock, DefaultValueProvider defaultValueProvider)
		{
			mock.MutableSetups.RemoveAllPropertyAccessorSetups();
			// Removing all the previous properties setups to keep the behaviour of overriding
			// existing setups in `SetupAllProperties`.
			
			mock.AutoSetupPropertiesDefaultValueProvider = defaultValueProvider;
			// `SetupAllProperties` no longer performs properties setup like in previous versions.
			// Instead it just enables a switch to setup properties on-demand at the moment of first access.
			// In order for `SetupAllProperties`'s new mode of operation to be indistinguishable
			// from how it worked previously, it's important to capture the default value provider at this precise
			// moment, since it might be changed later (before queries to properties).
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
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

			var part = parts.Pop();
			var method = part.Method;

			if (parts.Count == 0)
			{
				EventInfo @event;
				if (method.IsEventAddAccessor())
				{
					var implementingMethod = method.GetImplementingMethod(mock.Object.GetType());
					@event = implementingMethod.DeclaringType.GetEvents(bindingFlags).SingleOrDefault(e => e.GetAddMethod(true) == implementingMethod);
					if (@event == null)
					{
						throw new ArgumentException(
							string.Format(
								CultureInfo.CurrentCulture,
								Resources.SetupNotEventAdd,
								part.Expression));
					}
				}
				else if (method.IsEventRemoveAccessor())
				{
					var implementingMethod = method.GetImplementingMethod(mock.Object.GetType());
					@event = implementingMethod.DeclaringType.GetEvents(bindingFlags).SingleOrDefault(e => e.GetRemoveMethod(true) == implementingMethod);
					if (@event == null)
					{
						throw new ArgumentException(
							string.Format(
								CultureInfo.CurrentCulture,
								Resources.SetupNotEventRemove,
								part.Expression));
					}
				}
				else
				{
					throw new ArgumentException(
						string.Format(
							CultureInfo.CurrentCulture,
							Resources.UnsupportedExpression,
							expression));
				}

				if (mock.EventHandlers.TryGet(@event, out var handlers))
				{
					handlers.InvokePreserveStack(arguments);
				}
			}
			else if (mock.MutableSetups.GetInnerMockSetups().TryFind(part, out var innerMockSetup) && innerMockSetup.ReturnsInnerMock(out var innerMock))
			{
				Mock.RaiseEvent(innerMock, expression, parts, arguments);
			}
		}

		#endregion

		#region As<TInterface>

		/// <summary>
		///   Adds an interface implementation to the mock, allowing setups to be specified for it.
		/// </summary>
		/// <remarks>
		///   This method can only be called before the first use of the mock <see cref="Object"/> property,
		///   at which point the runtime type has already been generated and no more interfaces can be added to it.
		///   <para>
		///     Also, <typeparamref name="TInterface"/> must be an interface and not a class,
		///     which must be specified when creating the mock instead.
		///   </para>
		/// </remarks>
		/// <typeparam name="TInterface">Type of interface to cast the mock to.</typeparam>
		/// <exception cref="ArgumentException">The <typeparamref name="TInterface"/> specified is not an interface.</exception>
		/// <exception cref="InvalidOperationException">
		///   The mock type has already been generated by accessing the <see cref="Object"/> property.
		/// </exception>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "As", Justification = "We want the method called exactly as the keyword because that's what it does, it adds an implemented interface so that you can cast it later.")]
		public abstract Mock<TInterface> As<TInterface>() where TInterface : class;

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

			if (expression.IsProperty())
			{
				var property = expression.ToPropertyInfo();
				Guard.CanRead(property);

				Debug.Assert(property.CanRead(out var getter) && invocation.Method == getter);
			}

			this.MutableSetups.Add(new InnerMockSetup(new InvocationShape(expression, invocation.Method, arguments, exactGenericTypeArguments: true), returnValue));
		}

		#endregion
	}
}
