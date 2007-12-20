using System;
using System.Linq.Expressions;

namespace Moq
{
	internal class RangeMatcher : IMatcher
	{
		Expression from, to, rangeKind;

		public void Initialize(Expression matcherExpression)
		{
			var call = matcherExpression as MethodCallExpression;

			from = call.Arguments[0];
			to = call.Arguments[1];
			rangeKind = call.Arguments[2];
		}

		public bool Matches(object value)
		{
			if (value == null)
			{
				return false;
			}

			return IsInRange((IComparable)value,
				(IComparable)((ConstantExpression)from.PartialEval()).Value,
				(IComparable)((ConstantExpression)to.PartialEval()).Value,
				(Range)((ConstantExpression)rangeKind.PartialEval()).Value);
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
