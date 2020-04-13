// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq
{
	internal static class SetupExtensions
	{
		public static Mock GetInnerMock(this ISetup setup)
		{
			return setup.ReturnsMock(out var innerMock) ? innerMock : throw new InvalidOperationException();
		}
	}
}
