// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Async
{
	internal static class Awaitable
	{
		/// <summary>
		///   Recursively gets the result of (i.e. "unwraps") completed awaitables
		///   until a value is found that isn't a successfully completed awaitable.
		/// </summary>
		/// <remarks>
		///   As an example, given <paramref name="obj"/> := <c>Task.FromResult(Task.FromResult(42))</c>,
		///   this method will return <c>42</c>.
		/// </remarks>
		/// <param name="obj">The (possibly awaitable) object to be "unwrapped".</param>
		public static object TryGetResultRecursive(object obj)
		{
			if (obj != null
				&& AwaitableFactory.TryGet(obj.GetType()) is { } awaitableFactory
				&& awaitableFactory.TryGetResult(obj, out var result))
			{
				return Awaitable.TryGetResultRecursive(result);
			}

			return obj;
		}
	}
}
