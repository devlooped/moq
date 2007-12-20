using System.Linq.Expressions;

namespace Moq
{
	internal interface IMatcher
	{
		void Initialize(Expression matcherExpression);
		bool Matches(object value);
	}
}
