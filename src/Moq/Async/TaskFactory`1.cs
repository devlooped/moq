// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Threading.Tasks;

namespace Moq.Async
{
	internal sealed class TaskFactory<TResult> : AwaitableFactory<Task<TResult>, TResult>
	{
		public override Task<TResult> CreateCompleted(TResult result)
		{
			return Task.FromResult(result);
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
