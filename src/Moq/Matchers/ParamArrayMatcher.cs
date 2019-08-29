// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Matchers
{
	internal class ParamArrayMatcher : IMatcher
	{
		private IMatcher[] matchers;

		public ParamArrayMatcher(IMatcher[] matchers)
		{
			Debug.Assert(matchers != null);

			this.matchers = matchers;
		}

		public bool Matches(object value)
		{
			Array values = value as Array;
			if (values == null || this.matchers.Length != values.Length)
			{
				return false;
			}

			for (int index = 0; index < values.Length; index++)
			{
				if (!this.matchers[index].Matches(values.GetValue(index)))
				{
					return false;
				}
			}

			return true;
		}

		public void SetupEvaluatedSuccessfully(object value)
		{
			Debug.Assert(value is Array array && array.Length == this.matchers.Length);

			var values = (Array)value;
			for (int i = 0, n = this.matchers.Length; i < n; ++i)
			{
				this.matchers[i].SetupEvaluatedSuccessfully(values.GetValue(i));
			}
		}
	}
}
