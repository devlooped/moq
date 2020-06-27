// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	/// <summary>
	/// Kind of range to use in a filter specified through 
	/// <see cref="It.IsInRange"/>.
	/// </summary>
	public enum Range
	{
		/// <summary>
		/// The range includes the <c>to</c> and 
		/// <c>from</c> values.
		/// </summary>
		Inclusive,

		/// <summary>
		/// The range does not include the <c>to</c> and 
		/// <c>from</c> values.
		/// </summary>
		Exclusive
	}
}
