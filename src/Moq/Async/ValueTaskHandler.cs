// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Threading.Tasks;

namespace Moq.Async
{
	internal sealed class ValueTaskHandler : IAwaitableHandler
	{
		public static readonly ValueTaskHandler Instance = new ValueTaskHandler();

		private ValueTaskHandler()
		{
		}

		Type IAwaitableHandler.ResultType => typeof(void);

		public object CreateCompleted()
		{
			return new ValueTask();
		}

		object IAwaitableHandler.CreateCompleted(object _) => this.CreateCompleted();

		public object CreateFaulted(Exception exception)
		{
			var tcs = new TaskCompletionSource<bool>();
			tcs.SetException(exception);
			return new ValueTask(tcs.Task);
		}

		bool IAwaitableHandler.TryGetResult(object task, out object result)
		{
			result = null;
			return false;
		}
	}
}
