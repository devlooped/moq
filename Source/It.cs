using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Moq
{
	public static class It
	{
		[Matcher(typeof(AnyMatcher))]
		public static TValue IsAny<TValue>()
		{
			return default(TValue);
		}

		[Matcher(typeof(PredicateMatcher))]
		public static TValue Is<TValue>(Expression<Predicate<TValue>> match)
		{
			return default(TValue);
		}

		[Matcher(typeof(RangeMatcher))]
		public static TValue IsInRange<TValue>(TValue from, TValue to, Range rangeKind)
			where TValue : IComparable
		{
			return default(TValue);
		}
	}
}
