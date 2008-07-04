using System.Linq.Expressions;
using System;

namespace Moq
{
	internal class AnyMatcher : IMatcher
	{
		private Type matcherType;

		public void Initialize(Expression matcherExpression)
		{
			matcherType = matcherExpression.Type;
		}

		public bool Matches(object value)
		{
			// Fail the match if the type of the value is not the same 
			// as the type originally specified in the It.IsAny<T>
			if (!matcherType.IsAssignableFrom(value.GetType()))
			{
				return false;
			}

			return true;
		}
	}
}
