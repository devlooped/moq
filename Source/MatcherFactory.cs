using System;
using System.Linq;
using System.Linq.Expressions;

namespace Moq
{
	internal static class MatcherFactory
	{
		public static IMatcher CreateMatcher(Expression expression)
		{
			// TODO: type inference on the call might 
			// be a smaller type and a Convert expression type 
			// might be the topmost instead.
			// i.e.: It.IsInRange(0, 100, Range.Inclusive)
			// the values are ints, but if the method to call 
			// expects, say, a double, a Convert node will be on 
			// the expression.
			if (expression.NodeType == ExpressionType.Call)
			{
				MethodCallExpression call = (MethodCallExpression)expression;
				AdvancedMatcherAttribute attr = call.Method.GetCustomAttribute<AdvancedMatcherAttribute>(true);
				MatcherAttribute staticMatcherMethodAttr = call.Method.GetCustomAttribute<MatcherAttribute>(true);
				if (attr != null)
				{
					IMatcher matcher = attr.CreateMatcher();
					matcher.Initialize(expression);
					return matcher;
				}
				else if (staticMatcherMethodAttr != null)
				{
					IMatcher matcher = new Moq.Matchers.MatcherAttributeMatcher();
					matcher.Initialize(expression);
					return matcher;
				}
				else
				{
					IMatcher matcher = new LazyEvalMatcher();
					matcher.Initialize(expression);
					return matcher;
				}
			}

			// Try reducing locals to get a constant.
			Expression reduced = expression.PartialEval();
			if (reduced.NodeType == ExpressionType.Constant)
			{
				return new ConstantMatcher(((ConstantExpression)reduced).Value);
			}

			throw new NotSupportedException(String.Format(
				Properties.Resources.UnsupportedExpression, 
				expression));
		}
	}
}
