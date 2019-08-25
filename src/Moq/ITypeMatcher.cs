using System;

namespace Moq
{
	/// <summary>
	///   A type matcher represents a criterion against which generic type arguments are matched.
	/// </summary>
	internal interface ITypeMatcher
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
