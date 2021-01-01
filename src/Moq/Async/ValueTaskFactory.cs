// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moq.Async
{
	internal sealed class ValueTaskFactory : AwaitableFactory<ValueTask>
	{
		public static readonly ValueTaskFactory Instance = new ValueTaskFactory();

		private ValueTaskFactory()
		{
		}

		public override ValueTask CreateCompleted()
		{
			return default;
		}

		public override ValueTask CreateFaulted(Exception exception)
		{
			var tcs = new TaskCompletionSource<object>();
			tcs.SetException(exception);
			return new ValueTask(tcs.Task);
		}

		public override ValueTask CreateFaulted(IEnumerable<Exception> exceptions)
		{
			var tcs = new TaskCompletionSource<object>();
			tcs.SetException(exceptions);
			return new ValueTask(tcs.Task);
		}
	}
}
