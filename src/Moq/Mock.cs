// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using Moq.Async;
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
            typeof(Mock).GetMethod(nameof(Get), BindingFlags.Public | BindingFlags.Static)!;

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
                    new[] { mockedType }
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

        internal abstract object?[] ConstructorArguments { get; }

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
                this.DefaultValueProvider = value switch
                {
                    DefaultValue.Empty => DefaultValueProvider.Empty,
                    DefaultValue.Mock => DefaultValueProvider.Mock,
                    _ => throw new ArgumentOutOfRangeException(nameof(value)),
                };
            }
        }

        internal abstract EventHandlerCollection EventHandlers { get; }

        /// <summary>
        ///   Gets the mocked object instance.
        /// </summary>
        public object Object => this.OnGetObject();

        /// <summary>
        ///   Gets the interfaces directly inherited from the mocked type (<see cref="MockedType"/>).
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
        protected abstract object OnGetObject();

        /// <summary>
        /// Retrieves the type of the mocked object, its generic type argument.
        /// This is used in the auto-mocking of hierarchy access.
        /// </summary>
        internal abstract Type MockedType { get; }

        /// <summary>
        /// Gets or sets the <see cref="Moq.DefaultValueProvider"/> instance that will be used
        /// e. g. to produce default return values for unexpected invocations.
        /// </summary>
        public abstract DefaultValueProvider DefaultValueProvider { get; set; }

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
            this.Verify(setup => setup.IsVerifiable, verifiedMocks: new HashSet<Mock>());
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
            this.Verify(setup => true, verifiedMocks: new HashSet<Mock>());
        }

        internal void Verify(Func<ISetup, bool> predicate, HashSet<Mock> verifiedMocks)
        {
            if (verifiedMocks.Add(this) == false)
            {
                // This mock has already been verified; don't verify it again.
                // (We can end up here e.g. when there are loops in the inner mock object graph.)
                return;
            }

            foreach (Invocation invocation in this.MutableInvocations)
            {
                invocation.MarkAsVerifiedIfMatchedBy(predicate);
            }

            var errors = new List<MockException>();

            foreach (var setup in this.MutableSetups.FindAll(setup => !setup.IsConditional && predicate(setup)))
            {
                try
                {
                    setup.Verify(recursive: true, predicate, verifiedMocks);
                }
                catch (MockException error) when (error.IsVerificationError)
                {
                    errors.Add(error);
                }
            }

            if (errors.Count > 0)
            {
                throw MockException.Combined(
                    errors,
                    preamble: string.Format(CultureInfo.CurrentCulture, Resources.VerificationErrorsOfMock, this));
            }
        }

        internal static void Verify(Mock mock, LambdaExpression expression, Times times, string? failMessage)
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

        internal static void VerifyGet(Mock mock, LambdaExpression expression, Times times, string? failMessage)
        {
            Guard.NotNull(expression, nameof(expression));

            if (!expression.IsPropertyIndexer())  // guard because `.ToPropertyInfo()` doesn't (yet) work for indexers
            {
                var property = expression.ToPropertyInfo();
                Guard.CanRead(property);
            }

            Mock.Verify(mock, expression, times, failMessage);
        }

        internal static void VerifySet(Mock mock, LambdaExpression expression, Times times, string? failMessage)
        {
            Guard.NotNull(expression, nameof(expression));
            Guard.IsAssignmentToPropertyOrIndexer(expression, nameof(expression));

            Mock.Verify(mock, expression, times, failMessage);
        }

        internal static void VerifyAdd(Mock mock, LambdaExpression expression, Times times, string? failMessage)
        {
            Guard.NotNull(expression, nameof(expression));
            Guard.IsEventAdd(expression, nameof(expression));

            Mock.Verify(mock, expression, times, failMessage);
        }

        internal static void VerifyRemove(Mock mock, LambdaExpression expression, Times times, string? failMessage)
        {
            Guard.NotNull(expression, nameof(expression));
            Guard.IsEventRemove(expression, nameof(expression));

            Mock.Verify(mock, expression, times, failMessage);
        }

        internal static void VerifyNoOtherCalls(Mock mock)
        {
            Mock.VerifyNoOtherCalls(mock, verifiedMocks: new HashSet<Mock>());
        }

        static void VerifyNoOtherCalls(Mock mock, HashSet<Mock> verifiedMocks)
        {
            if (!verifiedMocks.Add(mock)) return;

            Invocation?[] unverifiedInvocations = mock.MutableInvocations.ToArray(invocation => !invocation.IsVerified);

            var innerMocks = mock.MutableSetups.FindAllInnerMocks();

            if (unverifiedInvocations.Any())
            {
                // There are some invocations that shouldn't require explicit verification by the user.
                // The intent behind a `Verify` call for a call expression like `m.A.B.C.X` is probably
                // to verify `X`. If that succeeds, it's reasonable to expect that `m.A`, `m.A.B`, and
                // `m.A.B.C` have implicitly been verified as well. Below, invocations such as those to
                // the left of `X` are referred to as "transitive" (for lack of a better word).
                if (innerMocks.Any())
                {
                    for (int i = 0, n = unverifiedInvocations.Length; i < n; ++i)
                    {
                        // In order for an invocation to be "transitive", its return value has to be a
                        // sub-object (inner mock); and that sub-object has to have received at least
                        // one call:
                        var wasTransitiveInvocation = mock.MutableSetups.FindLastInnerMock(setup => setup.Matches(unverifiedInvocations[i]!)) is Mock innerMock
                                                      && innerMock.MutableInvocations.Any();
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
                    throw MockException.UnverifiedInvocations(mock, remainingUnverifiedInvocations!);
                }
            }

            // Perform verification for all automatically created sub-objects (that is, those
            // created by "transitive" invocations):
            foreach (var innerMock in innerMocks)
            {
                VerifyNoOtherCalls(innerMock, verifiedMocks);
            }
        }

        static int GetMatchingInvocationCount(
            Mock mock,
            LambdaExpression expression,
            out List<Pair<Invocation, MethodExpectation>> invocationsToBeMarkedAsVerified)
        {
            Debug.Assert(mock != null);
            Debug.Assert(expression != null);

            invocationsToBeMarkedAsVerified = new List<Pair<Invocation, MethodExpectation>>();
            return Mock.GetMatchingInvocationCount(
                mock,
                new ImmutablePopOnlyStack<MethodExpectation>(expression.Split()),
                new HashSet<Mock>(),
                invocationsToBeMarkedAsVerified);
        }

        static int GetMatchingInvocationCount(
            Mock mock,
            in ImmutablePopOnlyStack<MethodExpectation> parts,
            HashSet<Mock> visitedInnerMocks,
            List<Pair<Invocation, MethodExpectation>> invocationsToBeMarkedAsVerified)
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
                invocationsToBeMarkedAsVerified.Add(new Pair<Invocation, MethodExpectation>(matchingInvocation, part));

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
                    if (Awaitable.TryGetResultRecursive(matchingInvocation.ReturnValue) is IMocked mocked)
                    {
                        count += Mock.GetMatchingInvocationCount(mocked.Mock, remainingParts, visitedInnerMocks, invocationsToBeMarkedAsVerified);
                    }
                }
            }

            return count;
        }

        #endregion

        #region Setup

        internal static MethodCall Setup(Mock mock, LambdaExpression expression, Condition? condition)
        {
            Guard.NotNull(expression, nameof(expression));

            return Mock.SetupRecursive(mock, expression, setupLast: (targetMock, originalExpression, part) =>
            {
                var setup = new MethodCall(originalExpression, targetMock, condition, expectation: part);
                targetMock.MutableSetups.Add(setup);
                return setup;
            });
        }

        internal static MethodCall SetupGet(Mock mock, LambdaExpression expression, Condition? condition)
        {
            Guard.NotNull(expression, nameof(expression));

            if (!expression.IsPropertyIndexer())  // guard because `.ToPropertyInfo()` doesn't (yet) work for indexers
            {
                var property = expression.ToPropertyInfo();
                Guard.CanRead(property);
            }

            return Mock.Setup(mock, expression, condition);
        }

        internal static MethodCall SetupSet(Mock mock, LambdaExpression expression, Condition? condition)
        {
            Guard.NotNull(expression, nameof(expression));
            Guard.IsAssignmentToPropertyOrIndexer(expression, nameof(expression));

            return Mock.Setup(mock, expression, condition);
        }

        internal static readonly MethodInfo SetupReturnsMethod =
            typeof(Mock).GetMethod(nameof(SetupReturns), BindingFlags.NonPublic | BindingFlags.Static)!;

        // This specialized setup method is used to set up a single `Mock.Of` predicate.
        // Unlike other setup methods, LINQ to Mocks can set non-interceptable properties, which is handy when initializing DTOs.
        internal static bool SetupReturns(Mock mock, LambdaExpression expression, object value)
        {
            Guard.NotNull(expression, nameof(expression));

            Mock.SetupRecursive<MethodCall?>(mock, expression, setupLast: (targetMock, oe, part) =>
            {
                var originalExpression = (LambdaExpression)oe;

                // There are two special cases involving settable properties where we do something other than creating a new setup:

                if (originalExpression.IsProperty())
                {
                    var pi = originalExpression.ToPropertyInfo();
                    if (pi.CanWrite(out var setter))
                    {
                        if (pi.CanRead(out var getter) && getter.CanOverride() && ProxyFactory.Instance.IsMethodVisible(getter, out _))
                        {
                            if (setter.CanOverride() && ProxyFactory.Instance.IsMethodVisible(setter, out _)
                                && targetMock.MutableSetups.FindLast(s => s is StubbedPropertiesSetup) is StubbedPropertiesSetup sps)
                            {
                                // (a) We have a mock where `SetupAllProperties` was called, and the property can be fully stubbed.
                                //     (A property can be "fully stubbed" if both its accessors can be intercepted.)
                                //     In this case, we set the property's internal backing field directly on the setup.
                                sps.SetProperty(pi.Name, value);
                                return null;
                            }
                        }
                        else
                        {
                            // (b) The property is settable, but Moq is unable to intercept the getter,
                            //     so setting up the setter would be pointless and the property also cannot be fully stubbed.
                            //     In this case, the best thing we can do is to simply invoke the setter.
                            pi.SetValue(targetMock.Object, value, null);
                            return null;
                        }
                    }
                }

                // For all other cases, we create a regular setup.

                Guard.IsOverridable(part.Method, part.Expression);
                Guard.IsVisibleToProxyFactory(part.Method);

                var setup = new MethodCall(originalExpression, targetMock, condition: null, expectation: part);
                setup.SetReturnValueBehavior(value);
                targetMock.MutableSetups.Add(setup);
                return null;
            }, allowNonOverridableLastProperty: true);

            return true;
        }

        internal static MethodCall SetupAdd(Mock mock, LambdaExpression expression, Condition? condition)
        {
            Guard.NotNull(expression, nameof(expression));
            Guard.IsEventAdd(expression, nameof(expression));

            return Mock.Setup(mock, expression, condition);
        }

        internal static MethodCall SetupRemove(Mock mock, LambdaExpression expression, Condition? condition)
        {
            Guard.NotNull(expression, nameof(expression));
            Guard.IsEventRemove(expression, nameof(expression));

            return Mock.Setup(mock, expression, condition);
        }

        internal static SequenceSetup SetupSequence(Mock mock, LambdaExpression expression)
        {
            Guard.NotNull(expression, nameof(expression));

            return Mock.SetupRecursive(mock, expression, setupLast: (targetMock, originalExpression, part) =>
            {
                var setup = new SequenceSetup(originalExpression, targetMock, expectation: part);
                targetMock.MutableSetups.Add(setup);
                return setup;
            });
        }

        internal static StubbedPropertySetup SetupProperty(Mock mock, LambdaExpression expression, object? initialValue)
        {
            Guard.NotNull(expression, nameof(expression));

            var pi = expression.ToPropertyInfo();

            if (!pi.CanRead(out var getter))
            {
                Guard.CanRead(pi);
            }

            if (!pi.CanWrite(out var setter))
            {
                Guard.CanWrite(pi);
            }

            return Mock.SetupRecursive(mock, expression, (targetMock, _, _) =>
            {
                var setup = new StubbedPropertySetup(targetMock, expression, getter, setter, initialValue);
                targetMock.MutableSetups.Add(setup);
                return setup;
            });
        }

        static TSetup SetupRecursive<TSetup>(Mock mock, LambdaExpression expression, Func<Mock, Expression, MethodExpectation, TSetup> setupLast, bool allowNonOverridableLastProperty = false)
            where TSetup : ISetup?
        {
            Debug.Assert(mock != null);
            Debug.Assert(expression != null);
            Debug.Assert(setupLast != null);

            var parts = expression.Split(allowNonOverridableLastProperty);
            return Mock.SetupRecursive(mock, originalExpression: expression, parts, setupLast);
        }

        static TSetup SetupRecursive<TSetup>(Mock mock, LambdaExpression originalExpression, Stack<MethodExpectation> parts, Func<Mock, Expression, MethodExpectation, TSetup> setupLast)
            where TSetup : ISetup?
        {
            var part = parts.Pop();
            var (expr, method, arguments) = part;

            if (parts.Count == 0)
            {
                return setupLast(mock, originalExpression, part);
            }
            else
            {
                Mock? innerMock = mock.MutableSetups.FindLastInnerMock(setup => setup.Matches(part));
                if (innerMock == null)
                {
                    var returnValue = mock.GetDefaultValue(method, out innerMock, useAlternateProvider: DefaultValueProvider.Mock);
                    if (innerMock == null)
                    {
                        throw new ArgumentException(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.UnsupportedExpression,
                                expr.ToStringFixed() + " in " + originalExpression.ToStringFixed() + ":\n" + Resources.TypeNotMockable));
                    }
                    var innerMockSetup = new InnerMockSetup(originalExpression, mock, expectation: part, returnValue);
                    mock.MutableSetups.Add(innerMockSetup);
                }
                Debug.Assert(innerMock != null);

                return Mock.SetupRecursive(innerMock, originalExpression, parts, setupLast);
            }
        }

        internal static void SetupAllProperties(Mock mock)
        {
            mock.MutableSetups.Add(new StubbedPropertiesSetup(mock));
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

        internal static Task RaiseEventAsync<T>(Mock mock, Action<T> action, object?[] arguments)
        {
            Guard.NotNull(action, nameof(action));

            var expression = ExpressionReconstructor.Instance.ReconstructExpression(action, mock.ConstructorArguments);
            var parts = expression.Split();
            
            // TODO: Will this code never return null?
            return (Task)Mock.RaiseEvent(mock, expression, parts, arguments);
        }

        internal static object? RaiseEvent(Mock mock, LambdaExpression expression, Stack<MethodExpectation> parts, object?[] arguments)
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
                    @event = implementingMethod.DeclaringType!.GetEvents(bindingFlags)
                                               .SingleOrDefault(e => e.GetAddMethod(true) == implementingMethod)
                             ?? throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                                          Resources.SetupNotEventAdd,
                                                                          part.Expression));

                }
                else if (method.IsEventRemoveAccessor())
                {
                    var implementingMethod = method.GetImplementingMethod(mock.Object.GetType());
                    @event = implementingMethod.DeclaringType!.GetEvents(bindingFlags)
                                               .SingleOrDefault(e => e.GetRemoveMethod(true) == implementingMethod)
                             ?? throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                                          Resources.SetupNotEventRemove,
                                                                          part.Expression));

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
                    var returnType = handlers.GetMethodInfo().ReturnType;
                    if (returnType == typeof(Task) || returnType == typeof(ValueTask))
                    {
                        var invocationList = handlers.GetInvocationList();
                        var tasks = new List<Task>(invocationList.Length);
                        foreach (var handler in invocationList)
                        {
                            var returnValue = handler.InvokePreserveStack(arguments);
                            if (returnValue is Task task)
                            {
                                tasks.Add(task);
                            }
                            else if (returnValue is ValueTask valueTask)
                            {
                                tasks.Add(valueTask.AsTask());
                            }
                        }
                        return Task.WhenAll(tasks);
                    }

                    return handlers.InvokePreserveStack(arguments);
                }
            }
            else
            {
                var innerMock = mock.MutableSetups.FindLastInnerMock(setup => setup.Matches(part));
                if (innerMock != null)
                {
                    return Mock.RaiseEvent(innerMock, expression, parts, arguments);
                }
            }

            return null;
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
        public abstract Mock<TInterface> As<TInterface>() where TInterface : class;

        internal bool ImplementsInterface(Type interfaceType)
        {
            return this.InheritedInterfaces.Contains(interfaceType)
                || this.AdditionalInterfaces.Contains(interfaceType);
        }

        #endregion

        #region Default Values

        internal abstract Dictionary<Type, object?> ConfiguredDefaultValues { get; }

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

        internal object? GetDefaultValue(MethodInfo method, out Mock? candidateInnerMock, DefaultValueProvider? useAlternateProvider = null)
        {
            Debug.Assert(method != null);
            Debug.Assert(method.ReturnType != null);
            Debug.Assert(method.ReturnType != typeof(void));

            if (this.ConfiguredDefaultValues.TryGetValue(method.ReturnType, out object? configuredDefaultValue))
            {
                candidateInnerMock = null;
                return configuredDefaultValue;
            }

            var result = (useAlternateProvider ?? this.DefaultValueProvider).GetDefaultReturnValue(method, this);
            var unwrappedResult = Awaitable.TryGetResultRecursive(result);

            candidateInnerMock = (unwrappedResult as IMocked)?.Mock;
            return result;
        }

        #endregion
    }
}
