using System;
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

			if (lambda == null)
				throw new MockException(MockException.ExceptionReason.ExpectedLambda,
					Properties.Resources.ExpectedLambda);

			predicate = lambda.Compile();
		}

		public bool Matches(object value)
		{
			return (bool)predicate.DynamicInvoke(value);
		}
	}
}
