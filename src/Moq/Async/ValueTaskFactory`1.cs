// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Threading.Tasks;

namespace Moq.Async
{
	internal sealed class ValueTaskFactory<TResult> : AwaitableFactory<ValueTask<TResult>, TResult>
	{
		public override ValueTask<TResult> CreateCompleted(TResult result)
		{
			return new ValueTask<TResult>(result);
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
