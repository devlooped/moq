// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	internal enum MockExceptionReason
	{
		Unknown = 0,
		MoreThanOneCall,
		MoreThanNCalls,
		NoMatchingCalls,
		NoSetup,
		ReturnValueRequired,
		UnmatchedSetups,
		UnverifiedInvocations,
	}
}
