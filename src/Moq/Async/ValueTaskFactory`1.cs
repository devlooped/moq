// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Moq.Async
{
	internal sealed class ValueTaskFactory<TResult> : AwaitableFactory<ValueTask<TResult>, TResult>
	{
		public override ValueTask<TResult> CreateCompleted(TResult result)
		{
			return new ValueTask<TResult>(result);
		}

		public override ValueTask<TResult> CreateFaulted(Exception exception)
		{
			var tcs = new TaskCompletionSource<TResult>();
			tcs.SetException(exception);
			return new ValueTask<TResult>(tcs.Task);
		}

		public override ValueTask<TResult> CreateFaulted(IEnumerable<Exception> exceptions)
		{
			var tcs = new TaskCompletionSource<TResult>();
			tcs.SetException(exceptions);
			return new ValueTask<TResult>(tcs.Task);
		}

		public override Expression CreateResultExpression(Expression awaitableExpression)
		{
			return Expression.MakeMemberAccess(
				awaitableExpression,
				typeof(ValueTask<TResult>).GetProperty(nameof(ValueTask<TResult>.Result)));
		}

		public override bool TryGetResult(ValueTask<TResult> valueTask, out TResult result)
		{
			if (valueTask.IsCompletedSuccessfully)
			{
				result = valueTask.Result;
				return true;
			}

			result = default;
			return false;
		}
	}
}
