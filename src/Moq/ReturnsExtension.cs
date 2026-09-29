using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Stunts;

namespace Moq
{
    /// <summary>
    /// Extensions for configuring return values from mock invocations.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static partial class ReturnsExtension
    {
        /// <summary>
        /// Sets the return value for a property or non-void method.
        /// </summary>
        [OverloadResolutionPriority(1)]
        public static ISetup<TDelegate, TResult> Returns<TDelegate, TResult>(this ISetup<TDelegate, TResult> setup, TResult value)
        {
            setup.SetReturnValue(value);
            return setup;
        }

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every call.
        /// </summary>
        public static ISetup<TDelegate, TResult> Returns<TDelegate, TResult>(this ISetup<TDelegate, TResult> setup, Func<TResult> value)
        {
            setup.SetReturnValue(_ => value());
            return setup;
        }

        /// <summary>
        /// Sets the return value for a property or non-void method to be calculated 
        /// on every call by the given <paramref name="handler"/>, which receives the 
        /// invocation arguments and can set ref/out arguments.
        /// </summary>
        public static ISetup<TDelegate, TResult> Returns<TDelegate, TResult>(this ISetup<TDelegate, TResult> setup, TDelegate handler)
            where TDelegate : Delegate
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            setup.SetHandlerResult<TResult>(handler, result => result);
            return setup;
        }

        /// <summary>
        /// Sets the return value for a member set up with a custom delegate to be calculated 
        /// on every call by the given <paramref name="handler"/>, which receives the 
        /// invocation arguments and can set ref/out arguments.
        /// </summary>
        public static ISetupRef<TDelegate> Returns<TDelegate>(this ISetupRef<TDelegate> setup, TDelegate handler)
            where TDelegate : Delegate
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            setup.SetHandlerResult<object?>(handler, result => result);
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async property or method.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<TDelegate, Task<TResult>> Returns<TDelegate, TResult>(this ISetup<TDelegate, Task<TResult>> setup, TResult value)
        {
            setup.SetReturnValue(_ => Task.FromResult(value));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async property or method to be 
        /// evaluated dynamically using the given function on every call.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<TDelegate, Task<TResult>> Returns<TDelegate, TResult>(this ISetup<TDelegate, Task<TResult>> setup, Func<TResult> value)
        {
            setup.SetReturnValue(_ => Task.FromResult(value()));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<IArgumentCollection, Task<TResult>>, Task<TResult>> Returns<TResult>(this ISetup<Func<IArgumentCollection, Task<TResult>>, Task<TResult>> setup, Func<IArgumentCollection, TResult> handler)
        {
            setup.SetHandlerResult<TResult>(handler, result => Task.FromResult(result));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<IArgumentCollection, ValueTask<TResult>>, ValueTask<TResult>> Returns<TResult>(this ISetup<Func<IArgumentCollection, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<IArgumentCollection, TResult> handler)
        {
            setup.SetHandlerResult<TResult>(handler, result => new ValueTask<TResult>(result));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async property or method.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<TDelegate, ValueTask<TResult>> Returns<TDelegate, TResult>(this ISetup<TDelegate, ValueTask<TResult>> setup, TResult value)
        {
            setup.SetReturnValue(_ => new ValueTask<TResult>(value));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async property or method to be 
        /// evaluated dynamically using the given function on every call.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<TDelegate, ValueTask<TResult>> Returns<TDelegate, TResult>(this ISetup<TDelegate, ValueTask<TResult>> setup, Func<TResult> value)
        {
            setup.SetReturnValue(_ => new ValueTask<TResult>(value()));
            return setup;
        }
    }
}
