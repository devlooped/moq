// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Threading.Tasks;

namespace Moq.Async
{
	internal sealed class ValueTaskOfHandler : AwaitableHandler
	{
		private readonly Type resultType;
		private readonly Type taskType;
		private readonly Type tcsType;

		public ValueTaskOfHandler(Type taskType, Type resultType)
		{
			this.resultType = resultType;
			this.taskType = taskType;
			this.tcsType = typeof(TaskCompletionSource<>).MakeGenericType(this.resultType);
		}

		public override Type ResultType => this.resultType;

		public override object CreateCompleted(object result)
		{
			// `Activator.CreateInstance` could throw an `AmbiguousMatchException` in this use case,
			// so we're explicitly selecting and calling the constructor we want to use:
			var ctor = this.taskType.GetConstructor(new[] { resultType });
			var valueTask = ctor.Invoke(new object[] { result });
			return valueTask;

		}

		public override object CreateFaulted(Exception exception)
		{
			var tcs = Activator.CreateInstance(this.tcsType);
			this.tcsType.GetMethod("SetException", new Type[] { typeof(Exception) }).Invoke(tcs, new object[] { exception });
			var task = this.tcsType.GetProperty("Task").GetValue(tcs);

			// `Activator.CreateInstance` could throw an `AmbiguousMatchException` in this use case,
			// so we're explicitly selecting and calling the constructor we want to use:
			var ctor = this.taskType.GetConstructor(new[] { task.GetType() });
			var valueTask = ctor.Invoke(new object[] { task });
			return valueTask;
		}

		public override bool TryGetResult(object valueTask, out object result)
		{
			if (valueTask != null)
			{
				var type = valueTask.GetType();
				var isCompleted = (bool)type.GetProperty("IsCompleted").GetValue(valueTask);
				if (isCompleted)
				{
					try
					{
						result = type.GetProperty("Result").GetValue(valueTask);
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
