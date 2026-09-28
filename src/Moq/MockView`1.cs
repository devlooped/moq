using System;
using System.ComponentModel;
using System.Diagnostics;
using Moq.Sdk;
using Stunts;

namespace Moq
{
    /// <summary>
    /// A typed <see cref="IMock{T}"/> view over a mocked object instance, 
    /// storing its configuration in the underlying <see cref="IMockRuntime"/>.
    /// </summary>
    /// <remarks>
    /// Views hold no state of their own, so any number of them can be 
    /// created over the same mock instance.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class MockView<T> : IMock<T> where T : class
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly IMockRuntime runtime;

        /// <summary>
        /// Initializes the view over the given mocked <paramref name="instance"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The <paramref name="instance"/> is not a mock.</exception>
        public MockView(T instance)
        {
            Object = instance ?? throw new ArgumentNullException(nameof(instance));
            runtime = MockRuntime.Get(instance);
        }

        /// <inheritdoc />
        public T Object { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IMock.Object => Object;

        /// <inheritdoc />
        public MockBehavior Behavior
        {
            get => runtime.Behavior;
            set => runtime.Behavior = value;
        }

        /// <inheritdoc />
        public DefaultValueProvider DefaultValue
        {
            get => runtime.DefaultValue;
            set => runtime.DefaultValue = value;
        }

        /// <inheritdoc />
        public bool CallBase
        {
            get => MockRuntimeExtensions.GetCallBase(runtime);
            set => MockRuntimeExtensions.SetCallBase(runtime, value);
        }
    }
}
