// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
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

		public bool Matches(object argument, Type parameterType)
		{
			return argument is Expression valueExpression
				&& ExpressionComparer.Default.Equals(this.expression, valueExpression);
		}

		public void SetupEvaluatedSuccessfully(object argument, Type parameterType)
		{
			Debug.Assert(this.Matches(argument, parameterType));
		}
	}
}
