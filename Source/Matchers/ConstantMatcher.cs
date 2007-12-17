using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Moq
{
	internal class ConstantMatcher : IMatcher
	{
		object constantValue;

		public ConstantMatcher(object constantValue)
		{
			this.constantValue = constantValue;
		}

		public void Initialize(Expression matcherExpression)
		{
			constantValue = ((ConstantExpression)matcherExpression).Value;
		}

		public bool Matches(object value)
		{
			return Object.Equals(constantValue, value);
		}
	}
}
