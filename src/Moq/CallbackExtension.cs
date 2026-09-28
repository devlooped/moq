using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Stunts;

namespace Moq
{
    /// <summary>
    /// Extensions for configuring callbacks when invoking mocks.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial class CallbackExtension
    {
        /// <summary>
        /// Specifies a callback to invoke when the void member is called, which receives 
        /// the invocation arguments and can set ref/out arguments.
        /// </summary>
        public static ISetup<TDelegate> Callback<TDelegate>(this ISetup<TDelegate> setup, TDelegate callback)
            where TDelegate : Delegate
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            if (setup.IsUntyped() && callback is Action<IArgumentCollection> untyped)
                setup.AddCallback(untyped);
            else
                setup.AddCallback(args => callback.InvokeWith(args), callback.HasRefOut());

            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<IArgumentCollection, TResult>, TResult> Callback<TResult>(this ISetup<Func<IArgumentCollection, TResult>, TResult> setup, Action<IArgumentCollection> callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            if (setup.IsUntyped())
                setup.AddCallback(callback);
            else
                setup.AddCallback(args => callback((IArgumentCollection)args.GetValue(0)!));

            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called.
        /// </summary>
        [OverloadResolutionPriority(-1)]
        public static TSetup Callback<TSetup>(this TSetup setup, Action callback) where TSetup : ISetup
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            setup.AddCallback(_ => callback());
            return setup;
        }
    }
}
