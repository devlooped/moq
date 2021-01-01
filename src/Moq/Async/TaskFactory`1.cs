// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Moq.Async
{
	internal sealed class TaskFactory<TResult> : AwaitableFactory<Task<TResult>, TResult>
	{
		public override Task<TResult> CreateCompleted(TResult result)
		{
			return Task.FromResult(result);
		}

		public override Task<TResult> CreateFaulted(Exception exception)
		{
			var tcs = new TaskCompletionSource<TResult>();
			tcs.SetException(exception);
			return tcs.Task;
		}

		public override Task<TResult> CreateFaulted(IEnumerable<Exception> exceptions)
		{
			var tcs = new TaskCompletionSource<TResult>();
			tcs.SetException(exceptions);
			return tcs.Task;
		}

		public override Expression CreateResultExpression(Expression awaitableExpression)
		{
			return Expression.MakeMemberAccess(
				awaitableExpression,
				typeof(Task<TResult>).GetProperty(nameof(Task<TResult>.Result)));
		}

		public override bool TryGetResult(Task<TResult> task, out TResult result)
		{
			if (task.Status == TaskStatus.RanToCompletion)
			{
				result = task.Result;
				return true;
			}

			result = default;
			return false;
		}
	}
}
