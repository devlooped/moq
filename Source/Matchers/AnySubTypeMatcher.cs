using System;
using System.Linq.Expressions;

namespace Moq.Matchers
{
	internal class AnySubTypeMatcher : IMatcher
	{
		private Type baseType;

		public void Initialize(Expression matcherExpression)
		{
			// TODO: validate argument or trust the compiler?
			var call = matcherExpression as MethodCallExpression;
			
			if(call == null)
				throw new InvalidOperationException("AnySubTypeMatcher can only be used with MethodCallExpression expresions");

			baseType = call.Method.ReturnType;
		}

		public bool Matches(object value)
		{
			throw new InvalidOperationException("For AnySubTypeMatchers, use Matches(object value, Type methodCallArgumentType) overload");
		}

		public bool Matches(object value, Type methodCallArgumentType)
		{
			if (value != null && baseType.IsInstanceOfType(value))
			{
				return true;
			}

			return baseType.IsAssignableFrom(methodCallArgumentType);
		}
	}
}