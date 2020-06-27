// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
	/// <summary>
	/// Defines async extension methods on IReturns.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class GeneratedReturnsExtensions
	{
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <typeparam name="T">Type of the function parameter.</typeparam>
		/// <typeparam name="TMock">Mocked type.</typeparam>
		/// <typeparam name="TResult">Type of the return value.</typeparam>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T t) => Task.FromResult(valueFunction(t)));
		}	
		 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2) => Task.FromResult(valueFunction(t1, t2)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2, T3 t3) => Task.FromResult(valueFunction(t1, t2, t3)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4) => Task.FromResult(valueFunction(t1, t2, t3, t4)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => Task.FromResult(valueFunction(t1, t2, t3, t4, t5)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) => Task.FromResult(valueFunction(t1, t2, t3, t4, t5, t6)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) => Task.FromResult(valueFunction(t1, t2, t3, t4, t5, t6, t7)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) => Task.FromResult(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) => Task.FromResult(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10) => Task.FromResult(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11) => Task.FromResult(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12) => Task.FromResult(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13) => Task.FromResult(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14) => Task.FromResult(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> valueFunction) where TMock : class
		{
			if (ReturnsExtensions.IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15) => Task.FromResult(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15)));
		}

		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <typeparam name="T">Type of the function parameter.</typeparam>
		/// <typeparam name="TMock">Mocked type.</typeparam>
		/// <typeparam name="TResult">Type of the return value.</typeparam>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T t) => new ValueTask<TResult>(valueFunction(t)));
		}	
		 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2) => new ValueTask<TResult>(valueFunction(t1, t2)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, T3, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3) => new ValueTask<TResult>(valueFunction(t1, t2, t3)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, T3, T4, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4) => new ValueTask<TResult>(valueFunction(t1, t2, t3, t4)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, T3, T4, T5, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) => new ValueTask<TResult>(valueFunction(t1, t2, t3, t4, t5)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) => new ValueTask<TResult>(valueFunction(t1, t2, t3, t4, t5, t6)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) => new ValueTask<TResult>(valueFunction(t1, t2, t3, t4, t5, t6, t7)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) => new ValueTask<TResult>(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) => new ValueTask<TResult>(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10) => new ValueTask<TResult>(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11) => new ValueTask<TResult>(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12) => new ValueTask<TResult>(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13) => new ValueTask<TResult>(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14) => new ValueTask<TResult>(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14)));
		}
 
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> valueFunction) where TMock : class
		{
			return mock.Returns((T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9, T10 t10, T11 t11, T12 t12, T13 t13, T14 t14, T15 t15) => new ValueTask<TResult>(valueFunction(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15)));
		}
	}
}
