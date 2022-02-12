// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
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

		public bool Matches(object argument, Type parameterType)
		{
			var eval = Evaluator.PartialEval(this.expression);
			return eval is ConstantExpression ce && new ConstantMatcher(ce.Value).Matches(argument, parameterType);
		}

		public void SetupEvaluatedSuccessfully(object argument, Type parameterType)
		{
			Debug.Assert(this.Matches(argument, parameterType));
		}
	}
}
