// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using System.Threading.Tasks;

using Moq.Language;

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
		public static ISetupSequentialResult<ValueTask<TResult>> ReturnsAsync<TResult>(this ISetupSequentialResult<ValueTask<TResult>> setup, TResult value)
		{
			return setup.Returns(() => new ValueTask<TResult>(value));
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
	}
}
