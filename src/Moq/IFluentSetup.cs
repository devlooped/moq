// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;

namespace Moq
{
	/// <summary>
	///   A composite setup that spans across more than one mock.
	///   <para>
	///     Fluent setups result from setup expressions that involve member chaining,
	///     such as <c>`parent => parent.Child.Member`</c>.
	///   </para>
	/// </summary>
	/// <example>
	///   The following statement will not create one, but two setups:
	///   <code>
	///     parentMock.Setup(parent => parent.Child.Member);
	///   </code>
	///   The setup expression is first split up such that each part refers to just one member:
	///   <list type="number">
	///     <item><c>`parent => parent.Child`</c>; and</item>
	///     <item><c>`(child) => (child).Member`</c>.</item>
	///   </list>
	///   The first expression is set up on <c>`parentMock`</c>.
	///   This setup is configured to return a <c>`Mock&lt;Child&gt;`</c> (<c>`parentMock`</c>'s so-called "inner mock").
	///   The second expression is then set up on that inner mock.
	/// </example>
	public interface IFluentSetup : ISetup
	{
		/// <summary>
		///   Gets the partial setups that together make up this fluent setup,
		///   in left-to-right order (that is, setup for leftmost sub-expression first, setup for rightmost sub-expression last).
		/// </summary>
		/// <remarks>
		///   Each setup in this collection will typically belong to a different mock.
		///   Given two adjacent setups, the former is configured to return the mock to which the latter setup belongs.
		/// </remarks>
		IReadOnlyList<ISetup> Parts { get; }
	}
}
