// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Matchers
{
	internal sealed class AnyMatcher : IMatcher
	{
		public static AnyMatcher Instance { get; } = new AnyMatcher();

		private AnyMatcher()
		{
		}

		public bool Matches(object argument, Type parameterType) => true;

		public void SetupEvaluatedSuccessfully(object argument, Type parameterType)
		{
			Debug.Assert(this.Matches(argument, parameterType));
		}
	}
}
