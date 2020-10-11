// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
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

		public bool Matches(object argument, Type parameterType)
		{
			Array values = argument as Array;
			if (values == null || this.matchers.Length != values.Length)
			{
				return false;
			}

			var elementType = parameterType.GetElementType();

			for (int index = 0; index < values.Length; index++)
			{
				if (!this.matchers[index].Matches(values.GetValue(index), elementType))
				{
					return false;
				}
			}

			return true;
		}

		public void SetupEvaluatedSuccessfully(object argument, Type parameterType)
		{
			Debug.Assert(this.Matches(argument, parameterType));
			Debug.Assert(argument is Array array && array.Length == this.matchers.Length);

			var values = (Array)argument;
			var elementType = parameterType.GetElementType();
			for (int i = 0, n = this.matchers.Length; i < n; ++i)
			{
				this.matchers[i].SetupEvaluatedSuccessfully(values.GetValue(i), elementType);
			}
		}
	}
}
