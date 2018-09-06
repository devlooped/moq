// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Moq.Matchers
{
	internal class ParamArrayMatcher : IMatcher
	{
		private IMatcher[] matchers;

		public ParamArrayMatcher(NewArrayExpression expression)
		{
			if (expression != null)
			{
				this.matchers = expression.Expressions
					.Select(e => MatcherFactory.CreateMatcher(e)).ToArray();
			}
			else
			{
				this.matchers = new IMatcher[0];
			}
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
	}
}
