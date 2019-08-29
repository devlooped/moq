// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

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

		public bool Matches(object value)
		{
			return this.referenceIsValueType ? object.Equals(this.reference, value)
			                                 : object.ReferenceEquals(this.reference, value);
		}

		public void SetupEvaluatedSuccessfully(object value)
		{
			Debug.Assert(this.Matches(value));
		}
	}
}
