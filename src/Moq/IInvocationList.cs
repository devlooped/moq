// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;

namespace Moq
{
	/// <summary>
	/// A list of invocations which have been performed on a mock.
	/// </summary>
	public interface IInvocationList : IReadOnlyList<IInvocation>
	{
		/// <summary>
		/// Resets all invocations recorded for this mock.
		/// </summary>
		void Clear();
	}
}
