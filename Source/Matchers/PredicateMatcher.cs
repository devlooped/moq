using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Moq
{
	internal class PredicateMatcher : IMatcher
	{
		Delegate predicate;

		public void Initialize(Expression matcherExpression)
		{
			// TODO: validate argument or trust the compiler?
			var call = matcherExpression as MethodCallExpression;
			var lambda = call.Arguments[0].StripQuotes() as LambdaExpression;
			predicate = lambda.Compile();
		}

		public bool Matches(object value)
		{
			return (bool)predicate.DynamicInvoke(value);
		}
	}
}
