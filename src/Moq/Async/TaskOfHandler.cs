// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Threading.Tasks;

namespace Moq.Async
{
	internal sealed class TaskOfHandler : AwaitableHandler
	{
		private readonly Type resultType;
		private readonly Type tcsType;

		public TaskOfHandler(Type resultType)
		{
			this.resultType = resultType;
			this.tcsType = typeof(TaskCompletionSource<>).MakeGenericType(this.resultType);
		}

		public override Type ResultType => this.resultType;

		public override object CreateCompleted(object result)
		{
			var tcs = Activator.CreateInstance(this.tcsType);
			this.tcsType.GetMethod("SetResult").Invoke(tcs, new object[] { result });
			var task = this.tcsType.GetProperty("Task").GetValue(tcs);
			return task;
		}

		public override object CreateFaulted(Exception exception)
		{
			var tcs = Activator.CreateInstance(this.tcsType);
			this.tcsType.GetMethod("SetException", new Type[] { typeof(Exception) }).Invoke(tcs, new object[] { exception });
			var task = this.tcsType.GetProperty("Task").GetValue(tcs);
			return task;
		}

		public override bool TryGetResult(object task, out object result)
		{
			if (task != null)
			{
				var type = task.GetType();
				var isCompleted = (bool)type.GetProperty("IsCompleted").GetValue(task);
				if (isCompleted)
				{
					try
					{
						result = type.GetProperty("Result").GetValue(task);
						return true;
					}
					catch
					{
					}
				}
			}

			result = null;
			return false;
		}
	}
}
