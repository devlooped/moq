using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Moq
{
	internal class RegexMatcher : IMatcher
	{
		Expression regexExpr, optionsExpr;

		public void Initialize(Expression matcherExpression)
		{
			var call = matcherExpression as MethodCallExpression;

			regexExpr = call.Arguments[0];

			if (call.Arguments.Count > 1)
				optionsExpr = call.Arguments[1];
		}

		public bool Matches(object value)
		{
			if (value == null && !(value is string))
				return false;

			Regex regex;

			string pattern = (string)((ConstantExpression)regexExpr.PartialEval()).Value;
			if (optionsExpr == null)
			{
				regex = new Regex(pattern);
			}
			else
			{
				RegexOptions options = (RegexOptions)((ConstantExpression)optionsExpr.PartialEval()).Value;
				regex = new Regex(pattern, options);
			}

			return regex.IsMatch((string)value);
		}
	}
}
