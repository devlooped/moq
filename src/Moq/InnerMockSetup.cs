// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Linq.Expressions;
using System.Reflection;

using Moq.Matchers;

using ExpressionFactory = System.Linq.Expressions.Expression;

namespace Moq
{
	internal sealed class InnerMockSetup : Setup, IDeterministicReturnValueSetup
	{
		private readonly object returnValue;

		public InnerMockSetup(MethodInfo method, object returnValue)
			: base(new InvocationShape(method, GetArgumentMatchers(method)), GetExpression(method))
		{
			this.returnValue = returnValue;
		}

		public object ReturnValue => this.returnValue;

		public override void Execute(Invocation invocation)
		{
			invocation.Return(this.returnValue);
		}

		public override bool TryVerifyAll()
		{
			return true;
		}

		private static IMatcher[] GetArgumentMatchers(MethodInfo method)
		{
			var parameterTypes = method.GetParameterTypes();
			var parameterCount = parameterTypes.Count;

			var argumentMatchers = new IMatcher[parameterCount];
			for (int i = 0; i < parameterCount; ++i)
			{
				argumentMatchers[i] = AnyMatcher.Instance;
			}

			return argumentMatchers;
		}

		private static LambdaExpression GetExpression(MethodInfo method)
		{
			var parameterTypes = method.GetParameterTypes();
			var parameterCount = parameterTypes.Count;

			var arguments = new Expression[parameterCount];
			var itIsAnyMethod = typeof(It).GetMethod(nameof(It.IsAny), BindingFlags.Public | BindingFlags.Static);
			for (int i = 0; i < parameterCount; ++i)
			{
				arguments[i] = ExpressionFactory.Call(itIsAnyMethod.MakeGenericMethod(parameterTypes[i]));
			}

			var mock = ExpressionFactory.Parameter(method.DeclaringType, "mock");
			return ExpressionFactory.Lambda(ExpressionFactory.Call(mock, method, arguments), mock);
		}
	}
}
