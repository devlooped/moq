// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Matchers
{
	internal class RefMatcher : IMatcher
	{
		private readonly object reference;
		private readonly bool referenceIsValueType;

		public RefMatcher(object reference)
		{
			this.reference = reference;
			this.referenceIsValueType = reference?.GetType().IsValueType ?? false;
		}

		public bool Matches(object argument, Type parameterType)
		{
			return this.referenceIsValueType ? object.Equals(this.reference, argument)
			                                 : object.ReferenceEquals(this.reference, argument);
		}

		public void SetupEvaluatedSuccessfully(object value, Type parameterType)
		{
			Debug.Assert(this.Matches(value, parameterType));
		}
	}
}
