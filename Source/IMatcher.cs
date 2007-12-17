using System.Linq.Expressions;

namespace Moq
{
	public interface IMatcher
	{
		void Initialize(Expression matcherExpression);
		bool Matches(object value);
	}
}
