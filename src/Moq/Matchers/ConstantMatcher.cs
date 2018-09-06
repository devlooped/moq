// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace Moq.Matchers
{
	internal class ConstantMatcher : IMatcher
	{
		private object constantValue;

		public ConstantMatcher(object constantValue)
		{
			this.constantValue = constantValue;
		}

		public bool Matches(object value)
		{
			if (object.Equals(constantValue, value))
			{
				return true;
			}

			if (this.constantValue is IEnumerable && value is IEnumerable enumerable &&
				!(this.constantValue is IMocked) && !(value is IMocked))
				// the above checks on the second line are necessary to ensure we have usable
				// implementations of IEnumerable, which might very well not be the case for
				// mocked objects.
			{
				return this.MatchesEnumerable(enumerable);
			}

			return false;
		}

		private bool MatchesEnumerable(IEnumerable enumerable)
		{
			var constValues = (IEnumerable)constantValue;
			return constValues.Cast<object>().SequenceEqual(enumerable.Cast<object>());
		}
	}
}
