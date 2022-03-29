// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Moq.Language;
using Moq.Language.Flow;
using Moq.Properties;

#if FEATURE_ASYNC_ENUMERABLE
using System.Collections.Generic;
using System.Linq;
#endif


namespace Moq
{
	/// <summary>
	/// Defines async extension methods on IReturns.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static partial class ReturnsExtensions
	{
		/// <summary>
		/// Specifies the value to return from an asynchronous method.
		/// </summary>
		/// <typeparam name="TMock">Mocked type.</typeparam>
		/// <typeparam name="TResult">Type of the return value.</typeparam>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="value">The value to return, or <see longword="null"/>.</param>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, TResult value) where TMock : class
		{
			return mock.ReturnsAsync(() => value);
		}

		/// <summary>
		/// Specifies the value to return from an asynchronous method.
		/// </summary>
		/// <typeparam name="TMock">Mocked type.</typeparam>
		/// <typeparam name="TResult">Type of the return value.</typeparam>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="value">The value to return, or <see longword="null"/>.</param>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, TResult value) where TMock : class
		{
			return mock.ReturnsAsync(() => value);
		}

#if FEATURE_ASYNC_ENUMERABLE
		/// <summary>
		/// Specifies the value to return from an asynchronous enumerable method.
		/// </summary>
		/// <typeparam name="TMock">Mocked type.</typeparam>
		/// <typeparam name="TResult">Type of the return elements.</typeparam>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="value">The values to return, or <see longword="null"/>.</param>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, IAsyncEnumerable<TResult>> mock, IEnumerable<TResult> value) where TMock : class
		{
			return mock.ReturnsAsync(() => value);
		}
#endif

		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <typeparam name="TMock">Mocked type.</typeparam>
		/// <typeparam name="TResult">Type of the return value.</typeparam>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Func<TResult> valueFunction) where TMock : class
		{
			if (IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns(() => Task.FromResult(valueFunction()));
		}

		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous method.
		/// </summary>
		/// <typeparam name="TMock">Mocked type.</typeparam>
		/// <typeparam name="TResult">Type of the return value.</typeparam>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return value.</param>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Func<TResult> valueFunction) where TMock : class
		{
			if (IsNullResult(valueFunction, typeof(TResult)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns(() => new ValueTask<TResult>(valueFunction()));
		}

#if FEATURE_ASYNC_ENUMERABLE
		/// <summary>
		/// Specifies a function that will calculate the value to return from the asynchronous enumerable method.
		/// </summary>
		/// <typeparam name="TMock">Mocked type.</typeparam>
		/// <typeparam name="TResult">Type of the return elements.</typeparam>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="valueFunction">The function that will calculate the return values.</param>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, IAsyncEnumerable<TResult>> mock, Func<IEnumerable<TResult>> valueFunction) where TMock : class
		{
			if (IsNullResult(valueFunction, typeof(IEnumerable<TResult>)))
			{
				return mock.ReturnsAsync(() => default);
			}

			return mock.Returns(() => valueFunction()?.ToAsyncEnumerable());
		}
#endif

		/// <summary>
		/// Specifies the exception to throw when the asynchronous method is invoked.
		/// </summary>
		/// <typeparam name="TMock">Mocked type.</typeparam>
		/// <param name="mock">Returns verb which represents the mocked type and the task return type</param>
		/// <param name="exception">Exception instance to throw.</param>
		public static IReturnsResult<TMock> ThrowsAsync<TMock>(this IReturns<TMock, Task> mock, Exception exception) where TMock : class
		{
			return mock.Returns(() =>
			{
				var tcs = new TaskCompletionSource<bool>();
				tcs.SetException(exception);
				return tcs.Task;
			});
		}

		/// <summary>
		/// Specifies the exception to throw when the asynchronous method is invoked.
		/// </summary>
		/// <typeparam name="TMock">Mocked type.</typeparam>
		/// <typeparam name="TResult">Type of the return value.</typeparam>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="exception">Exception instance to throw.</param>
		public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock, Exception exception) where TMock : class
		{
			return mock.Returns(() =>
			{
				var tcs = new TaskCompletionSource<TResult>();
				tcs.SetException(exception);
				return tcs.Task;
			});
		}

		/// <summary>
		/// Specifies the exception to throw when the asynchronous method is invoked.
		/// </summary>
		/// <typeparam name="TMock">Mocked type.</typeparam>
		/// <typeparam name="TResult">Type of the return value.</typeparam>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="exception">Exception instance to throw.</param>
		public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock, Exception exception) where TMock : class
		{
			return mock.Returns(() =>
			{
				var tcs = new TaskCompletionSource<TResult>();
				tcs.SetException(exception);
				return new ValueTask<TResult>(tcs.Task);
			});
		}

#if FEATURE_ASYNC_ENUMERABLE
		/// <summary>
		/// Helper that implements ThrowsAsync behavior for asynchronous enumerable methods.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="exception"></param>
		/// <returns></returns>
		private static async IAsyncEnumerable<TResult> ThrowsAsync<TResult>(Exception exception)
		{
			var tcs = new TaskCompletionSource<TResult>();
			tcs.SetException(exception);
			await tcs.Task;
			yield break;
		}

		/// <summary>
		/// Specifies the exception to throw when the asynchronous method is invoked.
		/// </summary>
		/// <typeparam name="TMock">Mocked type.</typeparam>
		/// <typeparam name="TResult">Type of the return elements.</typeparam>
		/// <param name="mock">Returns verb which represents the mocked type and the task of return type</param>
		/// <param name="exception">Exception instance to throw.</param>
		public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, IAsyncEnumerable<TResult>> mock, Exception exception) where TMock : class
		{
			return mock.Returns(() => ThrowsAsync<TResult>(exception));
		}
#endif

		private static readonly Random Random = new Random();

		/// <summary>
		/// Allows to specify the delayed return value of an asynchronous method.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock,
			TResult value, TimeSpan delay) where TMock : class
		{
			return DelayedResult(mock, value, delay);
		}

		/// <summary>
		/// Allows to specify the delayed return value of an asynchronous method.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock,
			TResult value, TimeSpan delay) where TMock : class
		{
			return DelayedResult(mock, value, delay);
		}

#if FEATURE_ASYNC_ENUMERABLE
		/// <summary>
		/// Allows to specify the delayed return value of an asynchronous enumerable method.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, IAsyncEnumerable<TResult>> mock,
			IEnumerable<TResult> value, TimeSpan delay) where TMock : class
		{
			return DelayedResult(mock, value, delay);
		}
#endif

		/// <summary>
		/// Allows to specify the delayed return value of an asynchronous method.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock,
			TResult value, TimeSpan minDelay, TimeSpan maxDelay) where TMock : class
		{
			var delay = GetDelay(minDelay, maxDelay, Random);

			return DelayedResult(mock, value, delay);
		}

		/// <summary>
		/// Allows to specify the delayed return value of an asynchronous method.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock,
			TResult value, TimeSpan minDelay, TimeSpan maxDelay) where TMock : class
		{
			var delay = GetDelay(minDelay, maxDelay, Random);

			return DelayedResult(mock, value, delay);
		}

#if FEATURE_ASYNC_ENUMERABLE
		/// <summary>
		/// Allows to specify the delayed return value of an asynchronous enumerable method.
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, IAsyncEnumerable<TResult>> mock,
			IEnumerable<TResult> value, TimeSpan minDelay, TimeSpan maxDelay) where TMock : class
		{
			var delay = GetDelay(minDelay, maxDelay, Random);

			return DelayedResult(mock, value, delay);
		}
#endif

		/// <summary>
		/// <para>Allows to specify the delayed return value of an asynchronous method.</para>
		/// <para>Use the <see cref="Random"/> argument to pass in (seeded) random generators used across your unit test.</para>
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock,
			TResult value, TimeSpan minDelay, TimeSpan maxDelay, Random random) where TMock : class
		{
			if(random == null)
				throw new ArgumentNullException(nameof(random));

			var delay = GetDelay(minDelay, maxDelay, random);

			return DelayedResult(mock, value, delay);
		}

		/// <summary>
		/// <para>Allows to specify the delayed return value of an asynchronous method.</para>
		/// <para>Use the <see cref="Random"/> argument to pass in (seeded) random generators used across your unit test.</para>
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock,
			TResult value, TimeSpan minDelay, TimeSpan maxDelay, Random random) where TMock : class
		{
			if (random == null)
				throw new ArgumentNullException(nameof(random));

			var delay = GetDelay(minDelay, maxDelay, random);

			return DelayedResult(mock, value, delay);
		}

#if FEATURE_ASYNC_ENUMERABLE
		/// <summary>
		/// <para>Allows to specify the delayed return value of an asynchronous enumerable method.</para>
		/// <para>Use the <see cref="Random"/> argument to pass in (seeded) random generators used across your unit test.</para>
		/// </summary>
		public static IReturnsResult<TMock> ReturnsAsync<TMock, TResult>(this IReturns<TMock, IAsyncEnumerable<TResult>> mock,
			IEnumerable<TResult> value, TimeSpan minDelay, TimeSpan maxDelay, Random random) where TMock : class
		{
			if (random == null)
				throw new ArgumentNullException(nameof(random));

			var delay = GetDelay(minDelay, maxDelay, random);

			return DelayedResult(mock, value, delay);
		}
#endif
		/// <summary>
		/// Allows to specify the exception thrown by an asynchronous method.
		/// </summary>
		public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock,
			Exception exception, TimeSpan delay) where TMock : class
		{
			return DelayedException(mock, exception, delay);
		}

		/// <summary>
		/// Allows to specify the exception thrown by an asynchronous method.
		/// </summary>
		public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock,
			Exception exception, TimeSpan delay) where TMock : class
		{
			return DelayedException(mock, exception, delay);
		}

#if FEATURE_ASYNC_ENUMERABLE
		/// <summary>
		/// Allows to specify the exception thrown by an asynchronous enumerable method.
		/// </summary>
		public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, IAsyncEnumerable<TResult>> mock,
			Exception exception, TimeSpan delay) where TMock : class
		{
			return DelayedException(mock, exception, delay);
		}
#endif

		/// <summary>
		/// Allows to specify the exception thrown by an asynchronous method.
		/// </summary>
		public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock,
			Exception exception, TimeSpan minDelay, TimeSpan maxDelay) where TMock : class
		{
			var delay = GetDelay(minDelay, maxDelay, Random);

			return DelayedException(mock, exception, delay);
		}

		/// <summary>
		/// Allows to specify the exception thrown by an asynchronous method.
		/// </summary>
		public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock,
			Exception exception, TimeSpan minDelay, TimeSpan maxDelay) where TMock : class
		{
			var delay = GetDelay(minDelay, maxDelay, Random);

			return DelayedException(mock, exception, delay);
		}

#if FEATURE_ASYNC_ENUMERABLE
		/// <summary>
		/// Allows to specify the exception thrown by an asynchronous enumerable method.
		/// </summary>
		public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, IAsyncEnumerable<TResult>> mock,
			Exception exception, TimeSpan minDelay, TimeSpan maxDelay) where TMock : class
		{
			var delay = GetDelay(minDelay, maxDelay, Random);

			return DelayedException(mock, exception, delay);
		}
#endif

		/// <summary>
		/// <para>Allows to specify the exception thrown by an asynchronous method.</para> 
		/// <para>Use the <see cref="Random"/> argument to pass in (seeded) random generators used across your unit test.</para>
		/// </summary>
		public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, Task<TResult>> mock,
			Exception exception, TimeSpan minDelay, TimeSpan maxDelay, Random random) where TMock : class
		{
			if (random == null)
				throw new ArgumentNullException(nameof(random));

			var delay = GetDelay(minDelay, maxDelay, random);

			return DelayedException(mock, exception, delay);
		}

		/// <summary>
		/// <para>Allows to specify the exception thrown by an asynchronous method.</para> 
		/// <para>Use the <see cref="Random"/> argument to pass in (seeded) random generators used across your unit test.</para>
		/// </summary>
		public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, ValueTask<TResult>> mock,
			Exception exception, TimeSpan minDelay, TimeSpan maxDelay, Random random) where TMock : class
		{
			if (random == null)
				throw new ArgumentNullException(nameof(random));

			var delay = GetDelay(minDelay, maxDelay, random);

			return DelayedException(mock, exception, delay);
		}

#if FEATURE_ASYNC_ENUMERABLE
		/// <summary>
		/// <para>Allows to specify the exception thrown by an asynchronous method.</para> 
		/// <para>Use the <see cref="Random"/> argument to pass in (seeded) random generators used across your unit test.</para>
		/// </summary>
		public static IReturnsResult<TMock> ThrowsAsync<TMock, TResult>(this IReturns<TMock, IAsyncEnumerable<TResult>> mock,
			Exception exception, TimeSpan minDelay, TimeSpan maxDelay, Random random) where TMock : class
		{
			if (random == null)
				throw new ArgumentNullException(nameof(random));

			var delay = GetDelay(minDelay, maxDelay, random);

			return DelayedException(mock, exception, delay);
		}
#endif

		internal static bool IsNullResult(Delegate valueFunction, Type resultType)
		{
			if (valueFunction == null)
			{
				return !resultType.IsValueType || Nullable.GetUnderlyingType(resultType) != null;
			}
			else
			{
				return false;
			}
		}

		private static TimeSpan GetDelay(TimeSpan minDelay, TimeSpan maxDelay, Random random)
		{
			if (!(minDelay < maxDelay))
				throw new ArgumentException(Resources.MinDelayMustBeLessThanMaxDelay);

			var min = (int)minDelay.Ticks;
			var max = (int)maxDelay.Ticks;

			return new TimeSpan(random.Next(min, max));
		}

		private static IReturnsResult<TMock> DelayedResult<TMock, TResult>(IReturns<TMock, Task<TResult>> mock,
			TResult value, TimeSpan delay)
			where TMock : class
		{
			Guard.Positive(delay);

			return mock.Returns(() =>
			{
				return Task.Delay(delay).ContinueWith(t => value);
			});
		}

		private static IReturnsResult<TMock> DelayedResult<TMock, TResult>(IReturns<TMock, ValueTask<TResult>> mock,
			TResult value, TimeSpan delay)
			where TMock : class
		{
			Guard.Positive(delay);

			return mock.Returns(() =>
			{
				return new ValueTask<TResult>(Task.Delay(delay).ContinueWith(t => value));
			});
		}

#if FEATURE_ASYNC_ENUMERABLE
		private static IReturnsResult<TMock> DelayedResult<TMock, TResult>(IReturns<TMock, IAsyncEnumerable<TResult>> mock,
			IEnumerable<TResult> value, TimeSpan delay)
			where TMock : class
		{
			Guard.Positive(delay);

			async IAsyncEnumerable<TResult> DelayThenContinue()
			{
				await Task.Delay(delay);
				foreach (var item in value)
				{
					yield return item;
				}
			}

			return mock.Returns(DelayThenContinue);
		}

#endif

		private static IReturnsResult<TMock> DelayedException<TMock, TResult>(IReturns<TMock, Task<TResult>> mock,
			Exception exception, TimeSpan delay)
			where TMock : class
		{
			Guard.Positive(delay);

			return mock.Returns(() =>
			{
				var tcs = new TaskCompletionSource<TResult>();
				Task.Delay(delay).ContinueWith(task => tcs.SetException(exception));
				return tcs.Task;
			});
		}

		private static IReturnsResult<TMock> DelayedException<TMock, TResult>(IReturns<TMock, ValueTask<TResult>> mock,
			Exception exception, TimeSpan delay)
			where TMock : class
		{
			Guard.Positive(delay);

			return mock.Returns(() =>
			{
				var tcs = new TaskCompletionSource<TResult>();
				Task.Delay(delay).ContinueWith(task => tcs.SetException(exception));
				return new ValueTask<TResult>(tcs.Task);
			});
		}

#if FEATURE_ASYNC_ENUMERABLE
		private static IReturnsResult<TMock> DelayedException<TMock, TResult>(IReturns<TMock, IAsyncEnumerable<TResult>> mock,
			Exception exception, TimeSpan delay)
			where TMock : class
		{
			Guard.Positive(delay);

			return mock.Returns(() => ThrowsAsync<TResult>(exception));
		}

#endif
	}
}
