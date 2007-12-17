using System.Linq.Expressions;
using System;

namespace Moq
{
	internal static class MatcherFactory
	{
		public static IMatcher CreateMatcher(Expression expression)
		{
			if (expression.NodeType == ExpressionType.Call)
			{
				MethodCallExpression call = (MethodCallExpression)expression;
				MatcherAttribute attr = call.Method.GetCustomAttribute<MatcherAttribute>(true);
				if (attr != null)
				{
					IMatcher matcher = attr.CreateMatcher();
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
