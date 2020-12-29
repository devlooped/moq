// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Threading.Tasks;

namespace Moq
{
	/// <summary>
	///   Provides the <c>Await</c> group of methods that can serve as a substitute
	///   for the <see langword="await"/> keyword in setup expressions. Make <c>Await</c>
	///   available in your code by importing this type's static members:
	///   <code>
	///     using static Moq.AwaitOperator;
	///   </code>
	/// </summary>
	public static class AwaitOperator
	{
		/// <summary>
		///   Continues to set up what should happen when the given task completes.
		/// </summary>
		public static TResult Await<TResult>(Task<TResult> task)
		{
			return default(TResult);
		}

		/// <summary>
		///   Continues to set up what should happen when the given task completes.
		/// </summary>
		public static TResult Await<TResult>(ValueTask<TResult> task)
		{
			return default(TResult);
		}
	}
}
