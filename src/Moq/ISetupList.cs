// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;

namespace Moq
{
	/// <summary>
	///   A list of setups that have been configured on a mock,
	///   in chronological order (that is, oldest setup first, most recent setup last).
	/// </summary>
	public interface ISetupList : IReadOnlyList<ISetup>
	{
	}
}
