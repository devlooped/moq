using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Moq
{
	internal class Matcher : IMatcher
	{
		Match match;

		public Matcher(Match match)
		{
			this.match = match;
		}

		public void Initialize(Expression matcherExpression)
		{
		}

		public bool Matches(object value)
		{
			return match.Matches(value);
		}
	}
}
