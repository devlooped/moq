// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

namespace Moq.Language
{
	/// <summary>
	/// Defines occurrence members to constraint setups.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IOccurrence : IFluentInterface
	{
		/// <summary>
		/// The expected invocation can happen at most once.
		/// </summary>
		/// <example>
		/// <code>
		/// var mock = new Mock&lt;ICommand&gt;();
		/// mock.Setup(foo => foo.Execute("ping"))
		///     .AtMostOnce();
		/// </code>
		/// </example>
		[Obsolete("To verify this condition, use the overload to Verify that receives Times.AtMostOnce().")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		IVerifies AtMostOnce();
		/// <summary>
		/// The expected invocation can happen at most specified number of times.
		/// </summary>
		/// <param name="callCount">The number of times to accept calls.</param>
		/// <example>
		/// <code>
		/// var mock = new Mock&lt;ICommand&gt;();
		/// mock.Setup(foo => foo.Execute("ping"))
		///     .AtMost( 5 );
		/// </code>
		/// </example>
		[Obsolete("To verify this condition, use the overload to Verify that receives Times.AtMost(callCount).")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		IVerifies AtMost(int callCount);
	}
}
