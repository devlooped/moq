// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Matchers
{
    internal sealed class AnyMatcher : IMatcher
    {
		public static AnyMatcher Instance { get; } = new AnyMatcher();

		private AnyMatcher()
		{
		}

		public bool Matches(object value) => true;
	}
}
