using System;
using System.Linq.Expressions;

namespace Moq
{
	internal class LazyEvalMatcher : IMatcher
	{
		Expression matcherExpression;

		public void Initialize(Expression matcherExpression)
		{
			this.matcherExpression = matcherExpression;
		}

		public bool Matches(object value)
		{
			Expression eval = Evaluator.PartialEval(matcherExpression);
			if (eval.NodeType == ExpressionType.Constant)
				return Object.Equals(((ConstantExpression)eval).Value, value);
			else
				return false;
		}
	}
}
