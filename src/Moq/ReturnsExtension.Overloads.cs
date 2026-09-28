using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Moq
{
    partial class ReturnsExtension
    {
        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, Task<TResult>>, Task<TResult>> Returns<T1, TResult>(this ISetup<Func<T1, Task<TResult>>, Task<TResult>> setup, Func<T1, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, Task<TResult>>, Task<TResult>> Returns<T1, T2, TResult>(this ISetup<Func<T1, T2, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, TResult>(this ISetup<Func<T1, T2, T3, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, T4, TResult>(this ISetup<Func<T1, T2, T3, T4, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, T4, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, T4, T5, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, T4, T5, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, T4, T5, T6, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!, (T13)args.GetValue(12)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!, (T13)args.GetValue(12)!, (T14)args.GetValue(13)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!, (T13)args.GetValue(12)!, (T14)args.GetValue(13)!, (T15)args.GetValue(14)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task<TResult>>, Task<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task<TResult>>, Task<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> handler)
        {
            setup.SetReturnValue(args => Task.FromResult(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!, (T13)args.GetValue(12)!, (T14)args.GetValue(13)!, (T15)args.GetValue(14)!, (T16)args.GetValue(15)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, TResult>(this ISetup<Func<T1, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, TResult>(this ISetup<Func<T1, T2, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, TResult>(this ISetup<Func<T1, T2, T3, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, T4, TResult>(this ISetup<Func<T1, T2, T3, T4, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, T4, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, T4, T5, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, T4, T5, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, T4, T5, T6, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!, (T13)args.GetValue(12)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!, (T13)args.GetValue(12)!, (T14)args.GetValue(13)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!, (T13)args.GetValue(12)!, (T14)args.GetValue(13)!, (T15)args.GetValue(14)!)));
            return setup;
        }

        /// <summary>
        /// Sets the result value for an async method to be calculated on every call 
        /// by the given <paramref name="handler"/>, which receives the invocation arguments.
        /// </summary>
        [OverloadResolutionPriority(2)]
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, ValueTask<TResult>>, ValueTask<TResult>> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, ValueTask<TResult>>, ValueTask<TResult>> setup, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> handler)
        {
            setup.SetReturnValue(args => new ValueTask<TResult>(handler((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!, (T13)args.GetValue(12)!, (T14)args.GetValue(13)!, (T15)args.GetValue(14)!, (T16)args.GetValue(15)!)));
            return setup;
        }
    }
}
