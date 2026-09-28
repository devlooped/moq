using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Stunts;

namespace Moq.Sdk
{
    /// <summary>
    /// Usability functions for working with mocks.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class MockExtensions
    {
        /// <summary>
        /// Gets the introspection information for a mocked object instance.
        /// </summary>
        public static IMock<T> AsMock<T>(this T instance) where T : class
            => (instance is MulticastDelegate @delegate ?
                @delegate.Target as IMocked :
                instance as IMocked)?.Mock.As(instance) ?? throw new ArgumentException(ThisAssembly.Strings.TargetNotMock, nameof(instance));

        /// <summary>
        /// Clones a mock by creating a new instance of the <see cref="IMock.Object"/> 
        /// from <paramref name="mock"/> and copying its behaviors, invocations and state.
        /// </summary>
        public static IMock<T> Clone<T>(this IMock<T> mock) where T : class
        {
            if (!mock.State.TryGetValue<object[]>(".ctor", out var ctor))
                throw new ArgumentException("No constructor state found for cloning.");

            var clone = ((IMocked)Activator.CreateInstance(mock.Object.GetType(), ctor)).Mock;
            clone.State = mock.State.Clone();

            var behaviors = clone.Behaviors;
            (behaviors as ISupportInitialize)?.BeginInit();
            try
            {
                behaviors.Clear();
                foreach (var behavior in mock.Behaviors)
                {
                    behaviors.Add(behavior);
                }
            }
            finally
            {
                (behaviors as ISupportInitialize)?.EndInit();
            }

            var invocations = clone.Invocations;
            invocations.Clear();
            foreach (var invocation in mock.Invocations)
            {
                invocations.Add(invocation);
            }

            return ((T)clone.Object).AsMock();
        }

        /// <summary>
        /// Gets the invocations performed on the mock so far that match the given 
        /// setup lambda.
        /// </summary>
        public static IEnumerable<IMethodInvocation> InvocationsFor<T>(this IMock<T> mock, Action<T> action) where T : class
        {
            using (new SetupScope())
            {
                action(mock.Object);
                var setup = MockContext.CurrentSetup ?? CallContext.ThrowUnexpectedNull<IMockSetup>();
                return mock.Invocations.Where(x => setup.AppliesTo(x));
            }
        }

        /// <summary>
        /// Gets the invocations performed on the mock so far that match the given 
        /// setup lambda.
        /// </summary>
        public static IEnumerable<IMethodInvocation> InvocationsFor<T, TResult>(this IMock<T> mock, Func<T, TResult> function) where T : class
        {
            using (new SetupScope())
            {
                function(mock.Object);
                var setup = MockContext.CurrentSetup ?? CallContext.ThrowUnexpectedNull<IMockSetup>();
                return mock.Invocations.Where(x => setup.AppliesTo(x));
            }
        }

        static IMock<T>? As<T>(this IMock? mock, T target) where T : class => mock == null ? null : new Mock<T>(mock, target);

        class Mock<T> : IMock<T> where T : class
        {
            readonly IMock mock;

            public Mock(IMock mock, T target)
            {
                this.mock = mock;
                Object = target;
            }

            public T Object { get; }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            object IMock.Object => mock.Object;

            public ICollection<IMethodInvocation> Invocations => mock.Invocations;

            public StateBag State
            {
                get => mock.State;
                set => mock.State = value;
            }

            public IEnumerable<IMockBehaviorPipeline> Setups => mock.Setups;

            public IList<IStuntBehavior> Behaviors => mock.Behaviors;

            public IMockBehaviorPipeline GetPipeline(IMockSetup setup) => mock.GetPipeline(setup);
        }
    }
}
