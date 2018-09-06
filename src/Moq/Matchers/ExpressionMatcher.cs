// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

namespace Moq.Matchers
{
	internal class ExpressionMatcher : IMatcher
	{
		private Expression expression;

		public ExpressionMatcher(Expression expression)
		{
			this.expression = expression;
		}

		public bool Matches(object value)
		{
			return value is Expression valueExpression
				&& ExpressionComparer.Default.Equals(this.expression, valueExpression);
		}
	}
}
