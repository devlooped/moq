#if !NET3x
using System;
using System.Threading.Tasks;
using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
    /// <summary>
    /// Defines async extension methods on IReturns.
    /// </summary>
	public static class GeneratedReturnsExtensions
	{
        /// <summary>
		/// Allows to specify the return value of an asynchronous method.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T, TResult> value) where TMock : class
		{
			return mock.Returns((T t) => Task.FromResult(value(t)));
		}	
		 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2) => Task.FromResult(value(t1, t2)));
		}
 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3) => Task.FromResult(value(t1, t2, t3)));
		}
 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4) => Task.FromResult(value(t1, t2, t3, t4)));
		}
 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => Task.FromResult(value(t1, t2, t3, t4, t5)));
		}
 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) => Task.FromResult(value(t1, t2, t3, t4, t5, t6)));
		}
 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) => Task.FromResult(value(t1, t2, t3, t4, t5, t6, t7)));
		}
 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) => Task.FromResult(value(t1, t2, t3, t4, t5, t6, t7, t8)));
		}
 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) => Task.FromResult(value(t1, t2, t3, t4, t5, t6, t7, t8, t9)));
		}
 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10) => Task.FromResult(value(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10)));
		}
 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11) => Task.FromResult(value(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11)));
		}
 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12) => Task.FromResult(value(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12)));
		}
 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13) => Task.FromResult(value(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13)));
		}
 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14) => Task.FromResult(value(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14)));
		}
 
        /// <summary>
		/// Specifies a function that will calculate the value to return from the method, 
		/// retrieving the arguments for the invocation.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> value) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15) => Task.FromResult(value(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15)));
		}
	}
}
#endif