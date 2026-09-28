using System;

namespace Moq
{
    partial class CallbackExtension
    {
        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, TResult>, TResult> Callback<T1, TResult>(this ISetup<Func<T1, TResult>, TResult> setup, Action<T1> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, TResult>, TResult> Callback<T1, T2, TResult>(this ISetup<Func<T1, T2, TResult>, TResult> setup, Action<T1, T2> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, TResult>, TResult> Callback<T1, T2, T3, TResult>(this ISetup<Func<T1, T2, T3, TResult>, TResult> setup, Action<T1, T2, T3> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, T4, TResult>, TResult> Callback<T1, T2, T3, T4, TResult>(this ISetup<Func<T1, T2, T3, T4, TResult>, TResult> setup, Action<T1, T2, T3, T4> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, T4, T5, TResult>, TResult> Callback<T1, T2, T3, T4, T5, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, TResult>, TResult> setup, Action<T1, T2, T3, T4, T5> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, TResult>, TResult> Callback<T1, T2, T3, T4, T5, T6, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, TResult>, TResult> setup, Action<T1, T2, T3, T4, T5, T6> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, TResult>, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, TResult>, TResult> setup, Action<T1, T2, T3, T4, T5, T6, T7> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>, TResult> setup, Action<T1, T2, T3, T4, T5, T6, T7, T8> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>, TResult> setup, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>, TResult> setup, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>, TResult> setup, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>, TResult> setup, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>, TResult> setup, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!, (T13)args.GetValue(12)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>, TResult> setup, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!, (T13)args.GetValue(12)!, (T14)args.GetValue(13)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>, TResult> setup, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!, (T13)args.GetValue(12)!, (T14)args.GetValue(13)!, (T15)args.GetValue(14)!));
            return setup;
        }

        /// <summary>
        /// Specifies a callback to invoke when the member is called, which receives the invocation arguments.
        /// </summary>
        public static ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this ISetup<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>, TResult> setup, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> callback)
        {
            setup.AddCallback(args => callback((T1)args.GetValue(0)!, (T2)args.GetValue(1)!, (T3)args.GetValue(2)!, (T4)args.GetValue(3)!, (T5)args.GetValue(4)!, (T6)args.GetValue(5)!, (T7)args.GetValue(6)!, (T8)args.GetValue(7)!, (T9)args.GetValue(8)!, (T10)args.GetValue(9)!, (T11)args.GetValue(10)!, (T12)args.GetValue(11)!, (T13)args.GetValue(12)!, (T14)args.GetValue(13)!, (T15)args.GetValue(14)!, (T16)args.GetValue(15)!));
            return setup;
        }
    }
}
