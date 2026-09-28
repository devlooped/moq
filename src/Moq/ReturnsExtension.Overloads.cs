#nullable disable
// Disable nullable since this works from the Arguments collection which we know can have nulls
// A NRE here would be a mis-configuration from the user, which the compiler would have caught 
// already in the test setups anyway.
using System;
using Stunts;

namespace Moq
{
    public partial class ReturnsExtension
    {
        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T, TResult>(this TResult target, Func<T, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T>(0))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, TResult>(this TResult target, Func<T1, T2, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, TResult>(this TResult target, Func<T1, T2, T3, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, T4, TResult>(this TResult target, Func<T1, T2, T3, T4, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2), i.Arguments.Get<T4>(3))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, T4, T5, TResult>(this TResult target, Func<T1, T2, T3, T4, T5, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2), i.Arguments.Get<T4>(3), i.Arguments.Get<T5>(4))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, T4, T5, T6, TResult>(this TResult target, Func<T1, T2, T3, T4, T5, T6, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2), i.Arguments.Get<T4>(3), i.Arguments.Get<T5>(4), i.Arguments.Get<T6>(5))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, T4, T5, T6, T7, TResult>(this TResult target, Func<T1, T2, T3, T4, T5, T6, T7, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2), i.Arguments.Get<T4>(3), i.Arguments.Get<T5>(4), i.Arguments.Get<T6>(5), i.Arguments.Get<T7>(6))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this TResult target, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2), i.Arguments.Get<T4>(3), i.Arguments.Get<T5>(4), i.Arguments.Get<T6>(5), i.Arguments.Get<T7>(6), i.Arguments.Get<T8>(7))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this TResult target, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2), i.Arguments.Get<T4>(3), i.Arguments.Get<T5>(4), i.Arguments.Get<T6>(5), i.Arguments.Get<T7>(6), i.Arguments.Get<T8>(7), i.Arguments.Get<T9>(8))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this TResult target, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2), i.Arguments.Get<T4>(3), i.Arguments.Get<T5>(4), i.Arguments.Get<T6>(5), i.Arguments.Get<T7>(6), i.Arguments.Get<T8>(7), i.Arguments.Get<T9>(8), i.Arguments.Get<T10>(9))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this TResult target, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2), i.Arguments.Get<T4>(3), i.Arguments.Get<T5>(4), i.Arguments.Get<T6>(5), i.Arguments.Get<T7>(6), i.Arguments.Get<T8>(7), i.Arguments.Get<T9>(8), i.Arguments.Get<T10>(9), i.Arguments.Get<T11>(10))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this TResult target, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2), i.Arguments.Get<T4>(3), i.Arguments.Get<T5>(4), i.Arguments.Get<T6>(5), i.Arguments.Get<T7>(6), i.Arguments.Get<T8>(7), i.Arguments.Get<T9>(8), i.Arguments.Get<T10>(9), i.Arguments.Get<T11>(10), i.Arguments.Get<T12>(11))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this TResult target, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2), i.Arguments.Get<T4>(3), i.Arguments.Get<T5>(4), i.Arguments.Get<T6>(5), i.Arguments.Get<T7>(6), i.Arguments.Get<T8>(7), i.Arguments.Get<T9>(8), i.Arguments.Get<T10>(9), i.Arguments.Get<T11>(10), i.Arguments.Get<T12>(11), i.Arguments.Get<T13>(12))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this TResult target, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2), i.Arguments.Get<T4>(3), i.Arguments.Get<T5>(4), i.Arguments.Get<T6>(5), i.Arguments.Get<T7>(6), i.Arguments.Get<T8>(7), i.Arguments.Get<T9>(8), i.Arguments.Get<T10>(9), i.Arguments.Get<T11>(10), i.Arguments.Get<T12>(11), i.Arguments.Get<T13>(12), i.Arguments.Get<T14>(13))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this TResult target, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2), i.Arguments.Get<T4>(3), i.Arguments.Get<T5>(4), i.Arguments.Get<T6>(5), i.Arguments.Get<T7>(6), i.Arguments.Get<T8>(7), i.Arguments.Get<T9>(8), i.Arguments.Get<T10>(9), i.Arguments.Get<T11>(10), i.Arguments.Get<T12>(11), i.Arguments.Get<T13>(12), i.Arguments.Get<T14>(13), i.Arguments.Get<T15>(14))));

        /// <summary>
        /// Sets the return value for a property or non-void method to 
        /// be evaluated dynamically using the given function on every 
        /// call.
        /// </summary>
        public static TResult Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this TResult target, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> value)
            => Returns<TResult>(value, (m, i, next)
                => i.CreateValueReturn(value(i.Arguments.Get<T1>(0), i.Arguments.Get<T2>(1), i.Arguments.Get<T3>(2), i.Arguments.Get<T4>(3), i.Arguments.Get<T5>(4), i.Arguments.Get<T6>(5), i.Arguments.Get<T7>(6), i.Arguments.Get<T8>(7), i.Arguments.Get<T9>(8), i.Arguments.Get<T10>(9), i.Arguments.Get<T11>(10), i.Arguments.Get<T12>(11), i.Arguments.Get<T13>(12), i.Arguments.Get<T14>(13), i.Arguments.Get<T15>(14), i.Arguments.Get<T16>(15))));
    }
}
