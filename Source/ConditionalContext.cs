using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language.Flow;
using Moq.Language;
using System.Linq.Expressions;

namespace Moq
{
	internal class ConditionalContext<TMock> : ISetupConditionResult<TMock>
		where TMock : class
	{
		Mock<TMock> mock;
		Func<bool> condition;

		public ConditionalContext(Mock<TMock> mock, Func<bool> condition)
		{
			this.mock = mock;
			this.condition = condition;
		}

		public ISetup<TMock> Setup(Expression<Action<TMock>> expression)
		{
			return Mock.Setup<TMock>(mock, expression, condition);
		}

		public ISetup<TMock, TResult> Setup<TResult>(Expression<Func<TMock, TResult>> expression)
		{
			return Mock.Setup<TMock, TResult>(mock, expression, condition);
		}
	}
}