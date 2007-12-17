using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Moq
{
	internal class RangeMatcher : IMatcher
	{
		IMatcher predicateMatcher;

		public void Initialize(Expression matcherExpression)
		{
			var call = matcherExpression as MethodCallExpression;
			var rangeType = call.Method.GetGenericArguments()[0];

			var args = (from arg in call.Arguments
					   let evalArg = Evaluator.PartialEval(arg)
					   select (evalArg.NodeType == ExpressionType.Constant ?
							((ConstantExpression)evalArg).Value :
							evalArg)).ToArray();

			// TODO:
			// Instead of returning evalArg if it's not a constant, 
			// we could ask the MatcherFactory for an IMatcher and 
			// use that generate a lambda that will call the matcher 
			// instead of comparing the value directly.

			bool inclusive = (bool)args[2];
			var predicateType = typeof(Predicate<>).MakeGenericType(rangeType);

			ParameterExpression valueParam = Expression.Parameter(rangeType, "x");
			Expression body;

			if (inclusive)
			{
				// x => x >= from && x <= to
				body = Expression.AndAlso(
					Expression.GreaterThanOrEqual(
						valueParam, 
						Expression.Constant(args[0])), 
					Expression.LessThanOrEqual(
						valueParam, 
						Expression.Constant(args[1]))
					);
			}
			else
			{
				// x => x > from && x < to
				body = Expression.AndAlso(
					Expression.GreaterThan(
						valueParam, 
						Expression.Constant(args[0])), 
					Expression.LessThan(
						valueParam, 
						Expression.Constant(args[1]))
					);
			}

			var predicate = Expression.Lambda(predicateType, body, valueParam);
			var isexpr = Expression.Call(
				typeof(It).GetMethod("Is").MakeGenericMethod(rangeType),
				predicate);

			predicateMatcher = new PredicateMatcher();
			predicateMatcher.Initialize(isexpr);
		}

		public bool Matches(object value)
		{
			return predicateMatcher.Matches(value);
		}
	}
}
