// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Moq.Language;

#if FEATURE_ASYNC_ENUMERABLE
using System.Collections.Generic;
using System.Linq;
#endif

namespace Moq
{
	/// <summary>
	/// Helper for sequencing return values in the same method.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static partial class SequenceExtensions
	{
		/// <summary>
		/// Return a sequence of tasks, once per call.
		/// </summary>
		public static ISetupSequentialResult<Task<TResult>> ReturnsAsync<TResult>(this ISetupSequentialResult<Task<TResult>> setup, TResult value)
		{
			return setup.Returns(() => Task.FromResult(value));
		}

		/// <summary>
		/// Return a sequence of tasks, once per call.
		/// </summary>
		public static ISetupSequentialResult<Task<TResult>> ReturnsAsync<TResult>(this ISetupSequentialResult<Task<TResult>> setup, Func<TResult> valueFunction)
		{
			return setup.Returns(() => Task.FromResult(valueFunction()));
		}

		/// <summary>
		/// Return a sequence of tasks, once per call.
		/// </summary>
		public static ISetupSequentialResult<ValueTask<TResult>> ReturnsAsync<TResult>(this ISetupSequentialResult<ValueTask<TResult>> setup, TResult value)
		{
			return setup.Returns(() => new ValueTask<TResult>(value));
		}

		/// <summary>
		/// Return a sequence of tasks, once per call.
		/// </summary>
		public static ISetupSequentialResult<ValueTask<TResult>> ReturnsAsync<TResult>(this ISetupSequentialResult<ValueTask<TResult>> setup, Func<TResult> valueFunction)
		{
			return setup.Returns(() => new ValueTask<TResult>(valueFunction()));
		}

#if FEATURE_ASYNC_ENUMERABLE
		/// <summary>
		/// Return a sequence of tasks, once per call.
		/// </summary>
		public static ISetupSequentialResult<IAsyncEnumerable<TResult>> ReturnsAsync<TResult>(this ISetupSequentialResult<IAsyncEnumerable<TResult>> setup, TResult value)
		{
			return setup.Returns(() => new[] { value }.ToAsyncEnumerable());
		}

		/// <summary>
		/// Return a sequence of tasks, once per call.
		/// </summary>
		public static ISetupSequentialResult<IAsyncEnumerable<TResult>> ReturnsAsync<TResult>(this ISetupSequentialResult<IAsyncEnumerable<TResult>> setup, Func<TResult> valueFunction)
		{
			return setup.Returns(() => new[] { valueFunction() }.ToAsyncEnumerable());
		}

#endif

		/// <summary>
		/// Return a sequence of tasks, once per call.
		/// </summary>
		public static ISetupSequentialResult<Task> PassAsync(this ISetupSequentialResult<Task> setup)
		{
			return setup.Returns(() => Task.FromResult(0));
		}

		/// <summary>
		/// Return a sequence of tasks, once per call.
		/// </summary>
		public static ISetupSequentialResult<ValueTask> PassAsync(this ISetupSequentialResult<ValueTask> setup)
		{
			return setup.Returns(() => new ValueTask());
		}

		/// <summary>
		/// Throws a sequence of exceptions, once per call.
		/// </summary>
		public static ISetupSequentialResult<Task<TResult>> ThrowsAsync<TResult>(this ISetupSequentialResult<Task<TResult>> setup, Exception exception)
		{
			return setup.Returns(() =>
			{
				var tcs = new TaskCompletionSource<TResult>();
				tcs.SetException(exception);
				return tcs.Task;
			});
		}

		/// <summary>
		/// Throws a sequence of exceptions, once per call.
		/// </summary>
		public static ISetupSequentialResult<ValueTask<TResult>> ThrowsAsync<TResult>(this ISetupSequentialResult<ValueTask<TResult>> setup, Exception exception)
		{
			return setup.Returns(() =>
			{
				var tcs = new TaskCompletionSource<TResult>();
				tcs.SetException(exception);
				return new ValueTask<TResult>(tcs.Task);
			});
		}

#if FEATURE_ASYNC_ENUMERABLE
		/// <summary>
		/// Throws a sequence of exceptions, once per call.
		/// </summary>
		public static ISetupSequentialResult<IAsyncEnumerable<TResult>> ThrowsAsync<TResult>(this ISetupSequentialResult<IAsyncEnumerable<TResult>> setup, Exception exception)
		{
			return setup.Returns(() =>
			{
				var tcs = new TaskCompletionSource<TResult>();
				tcs.SetException(exception);
				return AsyncEnumerable.Empty<TResult>();
			});
		}
#endif

		/// <summary>
		/// Throws a sequence of exceptions, once per call.
		/// </summary>
		public static ISetupSequentialResult<Task> ThrowsAsync(this ISetupSequentialResult<Task> setup, Exception exception)
		{
			return setup.Returns(() =>
			{
				var tcs = new TaskCompletionSource<object>();
				tcs.SetException(exception);
				return tcs.Task;
			});
		}

		/// <summary>
		/// Throws a sequence of exceptions, once per call.
		/// </summary>
		public static ISetupSequentialResult<ValueTask> ThrowsAsync(this ISetupSequentialResult<ValueTask> setup, Exception exception)
		{
			return setup.Returns(() =>
			{
				var tcs = new TaskCompletionSource<object>();
				tcs.SetException(exception);
				return new ValueTask(tcs.Task);
			});
		}
	}
}
