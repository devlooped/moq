using System;
using System.Collections.Generic;
using System.Linq;
using Avatars;
using Moq.Sdk;

namespace Moq
{
    /// <summary>
    /// Verifies calls to mocks.
    /// </summary>
    public static class Verify
    {
        /// <summary>
        /// Gets whether the given mock is being verified.
        /// </summary>
        internal static bool IsVerifying(IMock mock)
            => mock.State.TryGetValue<bool>(typeof(Verify), out var verifying) && verifying;

        /// <summary>
        /// Verifies all setups that had an occurrence constraint applied, 
        /// and allows specific verifications to be performed on the returned 
        /// object too.
        /// </summary>
        /// <returns>An object that can be used to perform additional call verifications.</returns>
        public static T Called<T>(T target) where T : class => Calls(target);

        /// <summary>
        /// Verifies a method invocation matching the <paramref name="function"/> was executed 
        /// the specified number of <paramref name="times"/>.
        /// </summary>
        /// <param name="function">The method invocation to match against actual calls.</param>
        /// <param name="times">Number of times the method should have been called.</param>
        public static void Called<T>(Func<T> function, int times) => CalledImpl(function, times);

        /// <summary>
        /// Verifies a method invocation matching the <paramref name="function"/> was called at 
        /// least once.
        /// </summary>
        /// <param name="function">The method invocation to match against actual calls.</param>
        /// <param name="message">User message to show.</param>
        public static void Called<T>(Func<T> function, string message) => CalledImpl(function, default, message: message);

        /// <summary>
        /// Verifies a method invocation matching the <paramref name="function"/> was executed at 
        /// least once. If <paramref name="times"/> is provided, the number of calls is verified too.
        /// </summary>
        /// <param name="function">The method invocation to match against actual calls.</param>
        /// <param name="times">Optional number of times the method should have been called. Defaults to <see cref="Times.AtLeastOnce"/>.
        /// An integer value can also be specificed since there is built-in conversion support from integer to <see cref="Times"/>.</param>
        /// <param name="message">Optional user message to show.</param>
        public static void Called<T>(Func<T> function, int times = -1, string? message = null) => CalledImpl(function, times, message);

        /// <summary>
        /// Verifies a method invocation matching the <paramref name="function"/> was executed at 
        /// least once. If <paramref name="times"/> is provided, the number of calls is verified too.
        /// </summary>
        /// <param name="function">The method invocation to match against actual calls.</param>
        /// <param name="times">Optional number of times the method should have been called. Defaults to <see cref="Times.AtLeastOnce"/>.
        /// An integer value can also be specificed since there is built-in conversion support from integer to <see cref="Times"/>.</param>
        /// <param name="message">Optional user message to show.</param>
        internal static void CalledImpl<T>(Func<T> function, Sdk.Times times = default, string? message = null)
        {
            using (new SetupScope())
            {
                function();
                var setup = MockContext.CurrentSetup ?? CallContext.ThrowUnexpectedNull<IMockSetup>();
                var mock = (MockContext.CurrentInvocation ?? CallContext.ThrowUnexpectedNull<IMethodInvocation>()).Target.AsMock();
                var calls = mock.Invocations.Where(x => setup.AppliesTo(x));
                if (!times.Validate(calls.Count()))
                    throw new VerifyException(mock, setup, message);
            }
        }

        /// <summary>
        /// Verifies a method invocation matching the <paramref name="action"/> was executed the 
        /// given number of times.
        /// </summary>
        /// <param name="action">The method invocation to match against actual calls.</param>
        /// <param name="times">Number of times the method should have been called.</param>
        public static void Called(Action action, int times) => CalledImpl(action, times);

        /// <summary>
        /// Verifies a method invocation matching the <paramref name="action"/> was executed at 
        /// least once.
        /// </summary>
        /// <param name="action">The method invocation to match against actual calls.</param>
        /// <param name="message">Optional user message to show.</param>
        public static void Called(Action action, string message) => CalledImpl(action, default, message: message);

        /// <summary>
        /// Verifies a method invocation matching the <paramref name="action"/> was executed at 
        /// least once. If <paramref name="times"/> is provided, the number of calls is verified too.
        /// </summary>
        /// <param name="action">The method invocation to match against actual calls.</param>
        /// <param name="times">Optional number of times the method should have been called. Defaults to <see cref="Times.AtLeastOnce"/>. 
        /// An integer value can also be specificed since there is built-in conversion support from integer to <see cref="Times"/>.</param>
        /// <param name="message">Optional user message to show.</param>
        public static void Called(Action action, int times = -1, string? message = null) => CalledImpl(action, times, message);

        /// <summary>
        /// Verifies a method invocation matching the <paramref name="action"/> was executed at 
        /// least once. If <paramref name="times"/> is provided, the number of calls is verified too.
        /// </summary>
        /// <param name="action">The method invocation to match against actual calls.</param>
        /// <param name="times">Optional number of times the method should have been called. Defaults to <see cref="Times.AtLeastOnce"/>. 
        /// An integer value can also be specificed since there is built-in conversion support from integer to <see cref="Times"/>.</param>
        /// <param name="message">Optional user message to show.</param>
        internal static void CalledImpl(Action action, Sdk.Times times = default, string? message = null)
        {
            using (new SetupScope())
            {
                action();
                var setup = MockContext.CurrentSetup ?? CallContext.ThrowUnexpectedNull<IMockSetup>();
                var mock = (MockContext.CurrentInvocation ?? CallContext.ThrowUnexpectedNull<IMethodInvocation>()).Target.AsMock();
                var calls = mock.Invocations.Where(x => setup.AppliesTo(x));
                if (!times.Validate(calls.Count()))
                    throw new VerifyException(mock, setup, message);
            }
        }

        /// <summary>
        /// Verifies all setups that had an occurrence constraint applied, 
        /// and allows specific verifications to be performed on the returned 
        /// object too.
        /// </summary>
        /// <returns>An object that can be used to perform additional call verifications.</returns>
        public static T NotCalled<T>(T target) where T : class => GetVerifier(GetVerified(target), true);

        /// <summary>
        /// Verifies a method invocation matching the <paramref name="function"/> was never called.
        /// </summary>
        /// <param name="function">The method invocation to match against actual calls.</param>
        /// <param name="message">Optional user message to show.</param>
        public static void NotCalled<T>(Func<T> function, string? message = null) => CalledImpl(function, Sdk.Times.Never, message);

        /// <summary>
        /// Verifies a method invocation matching the <paramref name="action"/> was never called.
        /// </summary>
        /// <param name="action">The method invocation to match against actual calls.</param>
        /// <param name="message">Optional user message to show.</param>
        public static void NotCalled(Action action, string? message = null) => CalledImpl(action, Sdk.Times.Never, message);

        /// <summary>
        /// Verifies all setups that had an occurrence constraint applied, 
        /// and allows specific verifications to be performed on the returned 
        /// object too.
        /// </summary>
        /// <returns>An object that can be used to perform additional call verifications.</returns>
        public static T Calls<T>(T target) where T : class => GetVerifier(GetVerified(target));

        /// <summary>
        /// Allows performing custom verification against all actual calls that match the 
        /// method invocation in <paramref name="function"/>.
        /// </summary>
        /// <param name="function">The method invocation to match against actual calls.</param>
        /// <param name="calls">Actual calls that match the given <paramref name="function"/>.</param>
        public static void Calls(Func<object> function, Action<IEnumerable<IMethodInvocation>> calls)
        {
            using (new SetupScope())
            {
                function();
                var setup = MockContext.CurrentSetup ?? CallContext.ThrowUnexpectedNull<IMockSetup>();
                var mock = (MockContext.CurrentInvocation ?? CallContext.ThrowUnexpectedNull<IMethodInvocation>()).Target.AsMock();
                calls.Invoke(mock.Invocations.Where(x => setup.AppliesTo(x)));
            }
        }

        /// <summary>
        /// Allows performing custom verification against all actual calls that match the 
        /// method invocation in <paramref name="action"/>.
        /// </summary>
        /// <param name="action">The method invocation to match against actual calls.</param>
        /// <param name="calls">Actual calls that match the given <paramref name="action"/>.</param>
        public static void Calls(Action action, Action<IEnumerable<IMethodInvocation>> calls)
        {
            using (new SetupScope())
            {
                action();
                var setup = MockContext.CurrentSetup ?? CallContext.ThrowUnexpectedNull<IMockSetup>();
                var mock = (MockContext.CurrentInvocation ?? CallContext.ThrowUnexpectedNull<IMethodInvocation>()).Target.AsMock();
                calls.Invoke(mock.Invocations.Where(x => setup.AppliesTo(x)));
            }
        }

        /// <summary>
        /// Gets the mock after verifying that all setups that specified occurrence 
        /// constraints have succeeded.
        /// </summary>
        static IMock<T> GetVerified<T>(T target) where T : class
        {
            var mock = target.AsMock();
            var failures = (from pipeline in mock.Setups
                            where pipeline.Setup.Occurrence != null
                            let times = pipeline.Setup.Occurrence!.Value
                            let calls = mock.Invocations.Where(x => pipeline.AppliesTo(x)).ToArray()
                            where !times.Validate(calls.Length)
                            select pipeline.Setup
                           ).ToArray();

            if (failures.Length > 0)
                throw new VerifyException(mock, failures);

            return mock;
        }

        /// <summary>
        /// Gets a clone of the original mock for verification purposes.
        /// </summary>
        /// <param name="mock">The mock to be cloned.</param>
        /// <param name="notCalled">Whether to add a behavior that verifies the invocations performed on 
        /// the clone were never performed on the original mock.
        /// </param>
        static T GetVerifier<T>(IMock<T> mock, bool notCalled = false) where T : class
        {
            // If the mock is already being verified, we don't need to clone again.
            if (mock.State.TryGetValue<bool>(typeof(Verify), out var verifying) && verifying)
                return mock.Object;

            // Otherwise, we create a verification copy that does not record invocations 
            // and has default behavior.
            var clone = mock.Clone();

            var recording = clone.Behaviors.OfType<MockRecordingBehavior>().FirstOrDefault();
            if (recording != null)
                clone.Behaviors.Remove(recording);

            if (notCalled)
                clone.Behaviors.Insert(
                    clone.Behaviors.IndexOf(clone.Behaviors.OfType<MockContextBehavior>().First()) + 1,
                    new NotCalledBehavior());

            // Sets up the right behaviors for a loose mock
            new Moq<T>(clone).Behavior = MockBehavior.Loose;

            clone.State.Set(typeof(Verify), true);

            return clone.Object;
        }

        class NotCalledBehavior : IAvatarBehavior
        {
            public bool AppliesTo(IMethodInvocation invocation) => true;

            public IMethodReturn Execute(IMethodInvocation invocation, ExecuteHandler next)
            {
                var mock = invocation.Target.AsMock();
                var setup = MockContext.CurrentSetup ?? CallContext.ThrowUnexpectedNull<IMockSetup>();
                if (mock.Invocations.Where(x => setup.AppliesTo(x)).Any())
                    throw new VerifyException(mock, setup);

                return next.Invoke(invocation, next);
            }
        }
    }
}
