using System;
using System.ComponentModel;

namespace Moq.Sdk
{
    /// <summary>
    /// Creates the typed setups returned by the generated <see cref="IMock{T}"/> 
    /// extensions. Not intended to be used directly.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SetupFactory
    {
        /// <summary>
        /// Begins a setup scope for a single member invocation on a mock, 
        /// which can be retrieved afterwards with one of the <c>Create</c> overloads.
        /// </summary>
        public static IDisposable Begin()
        {
            MockContext.CurrentSetup = null;
            return new SetupScope();
        }

        /// <summary>
        /// Creates the setup for the void member just invoked within a <see cref="Begin"/> scope.
        /// </summary>
        public static ISetup<TDelegate> Create<TDelegate>() => new SetupHandle<TDelegate>(Current());

        /// <summary>
        /// Creates the setup for the non-void member just invoked within a <see cref="Begin"/> scope.
        /// </summary>
        public static ISetup<TDelegate, TResult> Create<TDelegate, TResult>() => new SetupHandle<TDelegate, TResult>(Current());

        /// <summary>
        /// Creates a lazy setup for a read-only property.
        /// </summary>
        public static IPropertySetup<TGetter, TValue> Property<T, TGetter, TValue>(IMock<T> mock, string name, Action<T> getter) where T : class
            => new PropertySetupHandle<T, TGetter, Action<TValue>, TValue>(mock, name, getter, null, false);

        /// <summary>
        /// Creates a lazy setup for a read-write property.
        /// </summary>
        public static IPropertySetup<TGetter, TSetter, TValue> Property<T, TGetter, TSetter, TValue>(IMock<T> mock, string name, Action<T> getter, Action<T, TValue> setter) where T : class
            => new PropertySetupHandle<T, TGetter, TSetter, TValue>(mock, name, getter, setter, false);

        /// <summary>
        /// Creates a lazy setup for a read-only indexer, capturing the argument 
        /// matchers used for its arguments.
        /// </summary>
        public static IPropertySetup<TGetter, TValue> Indexer<T, TGetter, TValue>(IMock<T> mock, string name, Action<T> getter) where T : class
            => new PropertySetupHandle<T, TGetter, Action<TValue>, TValue>(mock, name, getter, null, true);

        /// <summary>
        /// Creates a lazy setup for a read-write indexer, capturing the argument 
        /// matchers used for its arguments.
        /// </summary>
        public static IPropertySetup<TGetter, TSetter, TValue> Indexer<T, TGetter, TSetter, TValue>(IMock<T> mock, string name, Action<T> getter, Action<T, TValue> setter) where T : class
            => new PropertySetupHandle<T, TGetter, TSetter, TValue>(mock, name, getter, setter, true);

        /// <summary>
        /// Gets the handlers currently subscribed to the given event on the mock, if any.
        /// </summary>
        public static TDelegate? GetEventHandler<TDelegate>(IMock mock, string name) where TDelegate : Delegate
            => MockRuntime.Get(mock.Object).State.TryGetValue<Delegate>(name, out var handler) ? handler as TDelegate : null;

        internal static IMockSetup Current()
            => MockContext.CurrentSetup ?? throw new InvalidOperationException(ThisAssembly.Strings.SetupNotFound);
    }
}
