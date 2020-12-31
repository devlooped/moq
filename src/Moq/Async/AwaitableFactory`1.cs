// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Async
{
	/// <summary>
	///   Abstract base class that facilitates type-safe implementation of <see cref="IAwaitableFactory"/>
	///   for awaitables that do not produce a result when awaited.
	/// </summary>
	internal abstract class AwaitableFactory<TAwaitable> : IAwaitableFactory
	{
		Type IAwaitableFactory.ResultType => typeof(void);

		public abstract TAwaitable CreateCompleted();

		object IAwaitableFactory.CreateCompleted(object result)
		{
			Debug.Assert(result == null);

			return this.CreateCompleted();
		}

		bool IAwaitableFactory.TryGetResult(object awaitable, out object result)
		{
			Debug.Assert(awaitable is TAwaitable);

			result = null;
			return false;
		}
	}
}
