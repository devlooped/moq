// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;

namespace Moq
{
	/// <summary>
	///   A "composite" setup is one with an expression where several member accesses or calls are chained together.
	///   They are a much more accurate reflection of the original setup expressions you will find in your own code,
	///   whereas the setups listed by <see cref="Mock.Setups"/> are more "technical" in nature:
	///   They only reflect the part of an original setup expression that is of immediate concern to that mock.
	/// </summary>
	public interface ICompositeSetup : ISetup
	{
		/// <summary>
		///   The "partial" setups that have resulted from splitting up the original composite setup expression.
		///   <para>
		///     Each of these partial setups will belong to a different mock.
		///     All of these mocks, except for that of the first partial setup,
		///     will be inner mocks returned by the preceding partial setup.
		///     That is, the partial setups are listed in order from the outermost to the innermost mock.
		///   </para>
		/// </summary>
		/// <seealso cref="ISetup.IsPartOfCompositeSetup"/>
		/// <seealso cref="ISetup.ReturnsInnerMock"/>
		IReadOnlyList<ISetup> Parts { get; }
	}
}
