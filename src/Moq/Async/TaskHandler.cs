// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Threading.Tasks;

namespace Moq.Async
{
	internal sealed class TaskHandler : IAwaitableHandler
	{
		public static readonly TaskHandler Instance = new TaskHandler();

		private TaskHandler()
		{
		}

		Type IAwaitableHandler.ResultType => typeof(void);

		public object CreateCompleted()
		{
			var tcs = new TaskCompletionSource<bool>();
			tcs.SetResult(true);
			return tcs.Task;
		}

		object IAwaitableHandler.CreateCompleted(object _) => this.CreateCompleted();

		public object CreateFaulted(Exception exception)
		{
			var tcs = new TaskCompletionSource<bool>();
			tcs.SetException(exception);
			return tcs.Task;
		}

		bool IAwaitableHandler.TryGetResult(object task, out object result)
		{
			result = null;
			return false;
		}
	}
}
