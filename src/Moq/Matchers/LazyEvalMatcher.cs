// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

namespace Moq.Matchers
{
	internal class LazyEvalMatcher : IMatcher
	{
		private Expression expression;

		public LazyEvalMatcher(Expression expression)
		{
			this.expression = expression;
		}

		public bool Matches(object value)
		{
			var eval = Evaluator.PartialEval(this.expression);
			if (eval.NodeType == ExpressionType.Constant)
			{
				return object.Equals(((ConstantExpression)eval).Value, value);
			}

			return false;
		}
	}
}
