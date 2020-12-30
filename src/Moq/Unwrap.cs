// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using Moq.Async;

namespace Moq
{
	internal static class Unwrap
	{
		/// <summary>
		///   Recursively unwraps the result of successfully completed awaitable objects.
		///   If the given value is not a successfully completed awaitable, the value itself is returned.
		/// </summary>
		/// <param name="obj">The value to be unwrapped.</param>
		public static object ResultIfCompletedAwaitable(object obj)
		{
			if (obj != null
				&& AwaitableHandler.TryGet(obj.GetType()) is { } awaitableHandler
				&& awaitableHandler.TryGetResult(obj, out var innerObj))
			{
				return Unwrap.ResultIfCompletedAwaitable(innerObj);
			}
			else
			{
				return obj;
			}
		}
	}
}
