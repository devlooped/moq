using System;
using System.ComponentModel;
using System.Linq;
using Stunts;
using Moq.Sdk;

namespace Moq
{
    /// <summary>
    /// Provides the Moq specific configurations on top of an <see cref="Sdk.IMock{T}"/> that 
    /// the Moq API provides beyond the SDK.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class Moq<T> : MockDecorator<T>, IMoq<T> where T : class
    {
        /// <summary>
        /// Decorates the given <see cref="IMock{T}"/> with Moq specific 
        /// properties.
        /// </summary>
        public Moq(IMock<T> mock) : base(mock) { }

        /// <summary>
        /// Gets or sets the <see cref="MockBehavior"/> for the mock.
        /// </summary>
        // NOTE: the setter is somewhat duplicating behavior in Initialize...
        public MockBehavior Behavior
        {
            get => State.GetOrAdd(nameof(IMoq) + "." + nameof(Behavior), () => MockBehavior.Default);
            set => State.Set(nameof(IMoq) + "." + nameof(Behavior), value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DefaultValueProvider"/> provider of 
        /// default values for the mock.
        /// </summary>
        public DefaultValueProvider DefaultValue
        {
            get => State.GetOrAdd(() => new DefaultValueProvider());
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (State.TryGetValue<DefaultValueProvider>(out var defaultValue) &&
                    value != defaultValue &&
                    Behaviors.OfType<DefaultValueBehavior>().FirstOrDefault() is DefaultValueBehavior behavior &&
                    behavior != null)
                {
                    behavior.Provider = value;
                }

                State.Set(value);
            }
        }

        /// <inheritdoc />
        public bool CallBase
        {
            get => State.GetOrAdd(nameof(IMoq) + "." + nameof(CallBase), () => false);
            set => State.Set(nameof(IMoq) + "." + nameof(CallBase), value);
        }
    }
}