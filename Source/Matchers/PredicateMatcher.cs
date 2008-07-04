using System;
using System.Linq.Expressions;

namespace Moq
{
	internal class PredicateMatcher : IMatcher
	{
		Delegate predicate;
		Type matcherType;

		public void Initialize(Expression matcherExpression)
		{
			// TODO: validate argument or trust the compiler?
			var call = matcherExpression as MethodCallExpression;
			var lambda = call.Arguments[0].StripQuotes() as LambdaExpression;
			matcherType = call.Type;

			if (lambda == null)
				throw new MockException(MockException.ExceptionReason.ExpectedLambda,
					Properties.Resources.ExpectedLambda);

			predicate = lambda.Compile();
		}

		public bool Matches(object value)
		{
			if (!matcherType.IsAssignableFrom(value.GetType()))
			{
				return false;
			}

			return (bool)predicate.InvokePreserveStack(value);
		}
	}
}
