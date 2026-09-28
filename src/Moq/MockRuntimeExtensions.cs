using System;
using System.Linq;
using Moq.Sdk;
using Stunts;

namespace Moq
{
    /// <summary>
    /// Provides the Moq specific configuration on top of an <see cref="IMockRuntime"/>, 
    /// and access to the runtime from an <see cref="IMock"/>.
    /// </summary>
    public static class MockRuntimeExtensions
    {
        const string BehaviorKey = nameof(IMock) + "." + nameof(IMock.Behavior);
        const string CallBaseKey = nameof(IMock) + "." + nameof(IMock.CallBase);
        const string NameKey = "Name";

        extension(IMock mock)
        {
            /// <summary>
            /// Gets the low-level <see cref="IMockRuntime"/> of the mock.
            /// </summary>
            public IMockRuntime Sdk => MockRuntime.Get(mock.Object);
        }

        extension(IMockRuntime runtime)
        {
            /// <summary>
            /// Gets or sets the name of the mock, used in diagnostics.
            /// </summary>
            public string? Name
            {
                get => runtime.State.TryGetValue<string>(NameKey, out var name) ? name : null;
                set
                {
                    if (value == null)
                        runtime.State.TryRemove<string>(NameKey, out _);
                    else
                        runtime.State.Set(NameKey, value);
                }
            }

            /// <summary>
            /// Gets or sets the <see cref="MockBehavior"/> for the mock.
            /// </summary>
            public MockBehavior Behavior
            {
                get => runtime.State.GetOrAdd(BehaviorKey, () => MockBehavior.Default);
                set => runtime.State.Set(BehaviorKey, value);
            }

            /// <summary>
            /// Gets or sets the <see cref="DefaultValueProvider"/> provider of 
            /// default values for the mock.
            /// </summary>
            public DefaultValueProvider DefaultValue
            {
                get => runtime.State.GetOrAdd(() => new DefaultValueProvider());
                set
                {
                    if (value == null)
                        throw new ArgumentNullException(nameof(value));

                    if (runtime.State.TryGetValue<DefaultValueProvider>(out var defaultValue) &&
                        value != defaultValue &&
                        runtime.Behaviors.OfType<DefaultValueBehavior>().FirstOrDefault() is DefaultValueBehavior behavior)
                    {
                        behavior.Provider = value;
                    }

                    runtime.State.Set(value);
                }
            }

            /// <summary>
            /// Whether the base member virtual implementation will be called for mocked classes if no setup is matched.
            /// </summary>
            public bool CallBase
            {
                get => GetCallBase(runtime);
                set => SetCallBase(runtime, value);
            }
        }

        internal static bool GetCallBase(IMockRuntime runtime) => runtime.State.GetOrAdd(CallBaseKey, () => false);

        internal static void SetCallBase(IMockRuntime runtime, bool value) => runtime.State.Set(CallBaseKey, value);
    }
}
