// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	/// <summary>
	///   Setups whose return value can be determined without running user-provided code
	///   (which could have side effects outside of Moq's control) should implement this interface.
	/// </summary>
	internal interface IReturnValueSetup : ISetup
	{
		object ReturnValue { get; }
	}
}
