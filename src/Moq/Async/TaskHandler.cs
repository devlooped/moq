// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Threading.Tasks;

namespace Moq.Async
{
	internal sealed class TaskHandler : AwaitableHandler
	{
		public static readonly TaskHandler Instance = new TaskHandler();

		private TaskHandler()
		{
		}

		public override Type ResultType => typeof(void);

		public object CreateCompleted()
		{
			var tcs = new TaskCompletionSource<bool>();
			tcs.SetResult(true);
			return tcs.Task;
		}

		public override object CreateCompleted(object _) => this.CreateCompleted();

		public override object CreateFaulted(Exception exception)
		{
			var tcs = new TaskCompletionSource<bool>();
			tcs.SetException(exception);
			return tcs.Task;
		}

		public override bool TryGetResult(object task, out object result)
		{
			result = null;
			return false;
		}
	}
}
