using System.Linq.Expressions;

namespace Moq
{
	internal class AnyMatcher : IMatcher
	{
		public void Initialize(Expression matcherExpression)
		{
		}

		public bool Matches(object value)
		{
			return true;
		}
	}
}
