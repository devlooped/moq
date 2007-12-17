using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	internal class RangeMatcher : IMatcher
	{
		IMatcher predicateMatcher;

		public void Initialize(Expression matcherExpression)
		{
			var call = matcherExpression as MethodCallExpression;
			//var rangeType = call.Method.GetGenericArguments()[0];
			var rangeType = typeof(IComparable);

			var predicateType = typeof(Predicate<>).MakeGenericType(rangeType);
			var isInRange = typeof(RangeMatcher).GetMethod("IsInRange");
			var value = Expression.Parameter(rangeType, "x");

			var body = Expression.Call(isInRange, value,
					call.Arguments[0].CastTo<IComparable>(),
					call.Arguments[1].CastTo<IComparable>(),
					call.Arguments[2]);
			var predicate = Expression.Lambda(predicateType, body, value);
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

		public static bool IsInRange(IComparable value, IComparable from, IComparable to, Range rangeKind)
		{
			if (rangeKind == Range.Exclusive)
			{
				return value.CompareTo(from) > 0 &&
					value.CompareTo(to) < 0;
			}
			else
			{
				return value.CompareTo(from) >= 0 &&
					value.CompareTo(to) <= 0;
			}
		}
	}
}
