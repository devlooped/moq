using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Stunts;

namespace Moq.Sdk
{
    /// <summary>
    /// Provides access to the <see cref="IMockRuntime"/> of mock instances.
    /// </summary>
    public static class MockRuntime
    {
        /// <summary>
        /// Gets the runtime for a mocked object instance (or mocked delegate).
        /// </summary>
        /// <exception cref="ArgumentException">The <paramref name="instance"/> is not a mock.</exception>
        public static IMockRuntime Get(object instance)
            => TryGet(instance, out var runtime) ? runtime : throw new ArgumentException(ThisAssembly.Strings.TargetNotMock, nameof(instance));

        /// <summary>
        /// Tries to get the runtime for a mocked object instance (or mocked delegate).
        /// </summary>
        public static bool TryGet(object? instance, out IMockRuntime runtime)
        {
            runtime = (instance is MulticastDelegate @delegate ?
                @delegate.Target as IMocked :
                instance as IMocked)?.Runtime!;

            return runtime != null;
        }

        /// <summary>
        /// Clones a mock by creating a new instance of the <see cref="IMockRuntime.Object"/> 
        /// from <paramref name="runtime"/> and copying its behaviors, invocations and state.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static IMockRuntime Clone(IMockRuntime runtime)
        {
            if (!runtime.State.TryGetValue<object[]>(".ctor", out var ctor))
                throw new ArgumentException("No constructor state found for cloning.");

            var clone = ((IMocked)Activator.CreateInstance(runtime.Object.GetType(), ctor)).Runtime;
            clone.State = runtime.State.Clone();

            var behaviors = clone.Behaviors;
            (behaviors as ISupportInitialize)?.BeginInit();
            try
            {
                behaviors.Clear();
                foreach (var behavior in runtime.Behaviors)
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
            foreach (var invocation in runtime.Invocations)
            {
                invocations.Add(invocation);
            }

            return clone;
        }

        /// <summary>
        /// Gets the invocations performed so far on the mock targeted by 
        /// the given <paramref name="action"/> that match the member invocation 
        /// it performs on it.
        /// </summary>
        public static IEnumerable<IMethodInvocation> InvocationsFor(Action action)
        {
            using (new SetupScope())
            {
                action();
                var setup = MockContext.CurrentSetup ?? CallContext.ThrowUnexpectedNull<IMockSetup>();
                return Get(setup.Invocation.Target).Invocations.Where(x => setup.AppliesTo(x)).ToArray();
            }
        }
    }
}
