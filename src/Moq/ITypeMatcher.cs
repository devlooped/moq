// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq
{
	/// <summary>
	///   Types that implement this interface represent a criterion against which generic type arguments are matched.
	///   <para>
	///     To be used in combination with <see cref="TypeMatcherAttribute"/>.
	///   </para>
	/// </summary>
	public interface ITypeMatcher
	{
		/// <summary>
		///   Matches the provided type argument against the criterion represented by this type matcher.
		/// </summary>
		/// <param name="typeArgument">
		///   The generic type argument that should be matched.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if the provided type argument matched the criterion represented by this instance;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		bool Matches(Type typeArgument);
	}
}
