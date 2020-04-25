// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Threading.Tasks;

namespace Moq.Linq.Expressions
{
	/// <todo/>
	public static class AwaitOperator
	{
		/// <todo/>
		public static TResult Await<TResult>(Task<TResult> task)
		{
			return Impl(task.Result);
		}

		/// <todo/>
		public static TResult Await<TResult>(ValueTask<TResult> task)
		{
			return Impl(task.Result);
		}

		/// <todo/>
		public static TResult Result<TResult>(this Task<TResult> task)
		{
			return Impl(task.Result);
		}

		/// <todo/>
		public static TResult Result<TResult>(this ValueTask<TResult> task)
		{
			return Impl(task.Result);
		}

		private static TResult Impl<TResult>(TResult result)
		{
			if (AwaitOperatorObserver.IsActive(out var o))
			{
				o.OnAwaitOperator();
			}

			return result;
		}
	}
}
