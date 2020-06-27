// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq
{
	[Flags]
	internal enum MockExceptionReasons
	{
		MoreThanOneCall = 1,
		MoreThanNCalls = 2,
		NoMatchingCalls = 4,
		NoSetup = 8,
		ReturnValueRequired = 16,
		UnmatchedSetup = 32,
		UnverifiedInvocations = 64,
	}
}
