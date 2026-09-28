#nullable disable
// Disable nullable since this works from the Arguments collection which we know can have nulls
// A NRE here would be a mis-configuration from the user, which the compiler would have caught 
// already in the test setups anyway.
using System;
using Stunts;

namespace Moq
{
    public partial class CallbackExtension
    {
        /// <summary>
        /// Specifies a callback to invoke when the method is called.
        /// </summary>
        public static TResult Callback<TResult>(this TResult target, Action callback)
            => Callback(target, args => callback());

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T, TResult>(this TResult target, Action<T> callback)
            => Callback(target, args => callback(args.Get<T>(0)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, TResult>(this TResult target, Action<T1, T2> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, TResult>(this TResult target, Action<T1, T2, T3> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, T4, TResult>(this TResult target, Action<T1, T2, T3, T4> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2), args.Get<T4>(3)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, T4, T5, TResult>(this TResult target, Action<T1, T2, T3, T4, T5> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2), args.Get<T4>(3), args.Get<T5>(4)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, T4, T5, T6, TResult>(this TResult target, Action<T1, T2, T3, T4, T5, T6> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2), args.Get<T4>(3), args.Get<T5>(4), args.Get<T6>(5)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, T4, T5, T6, T7, TResult>(this TResult target, Action<T1, T2, T3, T4, T5, T6, T7> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2), args.Get<T4>(3), args.Get<T5>(4), args.Get<T6>(5), args.Get<T7>(6)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this TResult target, Action<T1, T2, T3, T4, T5, T6, T7, T8> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2), args.Get<T4>(3), args.Get<T5>(4), args.Get<T6>(5), args.Get<T7>(6), args.Get<T8>(7)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this TResult target, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2), args.Get<T4>(3), args.Get<T5>(4), args.Get<T6>(5), args.Get<T7>(6), args.Get<T8>(7), args.Get<T9>(8)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this TResult target, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2), args.Get<T4>(3), args.Get<T5>(4), args.Get<T6>(5), args.Get<T7>(6), args.Get<T8>(7), args.Get<T9>(8), args.Get<T10>(9)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this TResult target, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2), args.Get<T4>(3), args.Get<T5>(4), args.Get<T6>(5), args.Get<T7>(6), args.Get<T8>(7), args.Get<T9>(8), args.Get<T10>(9), args.Get<T11>(10)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this TResult target, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2), args.Get<T4>(3), args.Get<T5>(4), args.Get<T6>(5), args.Get<T7>(6), args.Get<T8>(7), args.Get<T9>(8), args.Get<T10>(9), args.Get<T11>(10), args.Get<T12>(11)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this TResult target, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2), args.Get<T4>(3), args.Get<T5>(4), args.Get<T6>(5), args.Get<T7>(6), args.Get<T8>(7), args.Get<T9>(8), args.Get<T10>(9), args.Get<T11>(10), args.Get<T12>(11), args.Get<T13>(12)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this TResult target, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2), args.Get<T4>(3), args.Get<T5>(4), args.Get<T6>(5), args.Get<T7>(6), args.Get<T8>(7), args.Get<T9>(8), args.Get<T10>(9), args.Get<T11>(10), args.Get<T12>(11), args.Get<T13>(12), args.Get<T14>(13)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this TResult target, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2), args.Get<T4>(3), args.Get<T5>(4), args.Get<T6>(5), args.Get<T7>(6), args.Get<T8>(7), args.Get<T9>(8), args.Get<T10>(9), args.Get<T11>(10), args.Get<T12>(11), args.Get<T13>(12), args.Get<T14>(13), args.Get<T15>(14)));

        /// <summary>
        /// Specifies a callback to invoke when the method is called that receives the original arguments.
        /// </summary>
        public static TResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this TResult target, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> callback)
            => Callback(target, args => callback(args.Get<T1>(0), args.Get<T2>(1), args.Get<T3>(2), args.Get<T4>(3), args.Get<T5>(4), args.Get<T6>(5), args.Get<T7>(6), args.Get<T8>(7), args.Get<T9>(8), args.Get<T10>(9), args.Get<T11>(10), args.Get<T12>(11), args.Get<T13>(12), args.Get<T14>(13), args.Get<T15>(14), args.Get<T16>(15)));
    }
}
