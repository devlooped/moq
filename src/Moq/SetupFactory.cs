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
        /// Invokes a typed setup member and returns the setup it produced.
        /// </summary>
        public static ISetup<TDelegate, TResult> Capture<TDelegate, TResult>(Action invoke)
        {
            using (Begin())
            {
                invoke();
                return Create<TDelegate, TResult>();
            }
        }

        /// <summary>
        /// Invokes a void typed setup member and returns the setup it produced.
        /// </summary>
        public static ISetup<TDelegate> Capture<TDelegate>(Action invoke)
        {
            using (Begin())
            {
                invoke();
                return Create<TDelegate>();
            }
        }

        /// <summary>
        /// Invokes a typed setup member through <paramref name="member"/> so ref and out arguments can be passed.
        /// </summary>
        public static ISetup<TDelegate, TResult> Capture<TDelegate, TResult>(TDelegate member, object? argument, params object?[] arguments)
            where TDelegate : Delegate
            => Invoke<TDelegate, TResult>(member, With(argument, arguments));

        /// <summary>
        /// Invokes a void typed setup member through <paramref name="member"/> so ref and out arguments can be passed.
        /// </summary>
        public static ISetup<TDelegate> Capture<TDelegate>(TDelegate member, object? argument, params object?[] arguments)
            where TDelegate : Delegate
            => Invoke<TDelegate>(member, With(argument, arguments));

        /// <summary>
        /// Begins a setup scope for a single member invocation on a mock,
        /// which can be retrieved afterwards with one of the <c>Create</c> overloads.
        /// </summary>
        internal static IDisposable Begin()
        {
            MockContext.CurrentSetup = null;
            return new SetupScope();
        }

        /// <summary>
        /// Creates the setup for the void member just invoked within a <see cref="Begin"/> scope.
        /// </summary>
        internal static ISetup<TDelegate> Create<TDelegate>() => new SetupHandle<TDelegate>(Current());

        /// <summary>
        /// Creates the setup for the non-void member just invoked within a <see cref="Begin"/> scope.
        /// </summary>
        internal static ISetup<TDelegate, TResult> Create<TDelegate, TResult>() => new SetupHandle<TDelegate, TResult>(Current());

        static ISetup<TDelegate, TResult> Invoke<TDelegate, TResult>(Delegate member, object?[] arguments)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            using (Begin())
            {
                member.DynamicInvoke(arguments);
                return Create<TDelegate, TResult>();
            }
        }

        static ISetup<TDelegate> Invoke<TDelegate>(Delegate member, object?[] arguments)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            using (Begin())
            {
                member.DynamicInvoke(arguments);
                return Create<TDelegate>();
            }
        }

        static object?[] With(object? argument, object?[] arguments)
        {
            var values = new object?[arguments.Length + 1];
            values[0] = argument;
            Array.Copy(arguments, 0, values, 1, arguments.Length);
            return values;
        }

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
