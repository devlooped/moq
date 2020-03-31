// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;
using System.Reflection;

namespace Moq
{
	/// <summary>
	/// Provides information about an invocation of a mock object.
	/// </summary>
	public interface IInvocation
	{
		/// <summary>
		/// Gets the method of the invocation.
		/// </summary>
		MethodInfo Method { get; }

		/// <summary>
		/// Gets the arguments of the invocation.
		/// </summary>
		IReadOnlyList<object> Arguments { get; }

		/// <summary>
		///   Gets whether this invocation was matched by a setup.
		///   If so, the matching setup is returned via the <see langword="out"/> parameter <paramref name="matchingSetup"/>.
		/// </summary>
		/// <param name="matchingSetup">
		///   If this invocation was matched by a setup,
		///   this <see langword="out"/> parameter will be set to the matching setup.
		/// </param>
		/// <returns>
		///   <see langword="true"/> if this invocation was matched by a setup;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		bool WasMatched(out ISetup matchingSetup);
	}
}