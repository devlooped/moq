// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Async
{
	/// <summary>
	///   Abstract base class that facilitates type-safe implementation of <see cref="IAwaitableFactory"/>
	///   for awaitables that produce a result when awaited.
	/// </summary>
	internal abstract class AwaitableFactory<TAwaitable, TResult> : IAwaitableFactory
	{
		public Type ResultType => typeof(TResult);

		public abstract TAwaitable CreateCompleted(TResult result);

		object IAwaitableFactory.CreateCompleted(object result)
		{
			Debug.Assert(result is TResult || result == null);

			return this.CreateCompleted((TResult)result);
		}

		public abstract bool TryGetResult(TAwaitable awaitable, out TResult result);

		bool IAwaitableFactory.TryGetResult(object awaitable, out object result)
		{
			Debug.Assert(awaitable is TAwaitable);

			if (this.TryGetResult((TAwaitable)awaitable, out var r))
			{
				result = r;
				return true;
			}

			result = null;
			return false;
		}
	}
}
