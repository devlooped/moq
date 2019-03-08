// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	/// <summary>
	///   Describes the "shape" of an invocation against which concrete <see cref="Invocation"/>s can be matched.
	/// </summary>
	internal readonly struct InvocationShape
	{
		private static readonly IReadOnlyList<Expression> noArguments = new Expression[0];
		private static readonly IMatcher[] noArgumentMatchers = new IMatcher[0];

		public readonly LambdaExpression Expression;
		public readonly MethodInfo Method;
		public readonly IReadOnlyList<Expression> Arguments;

		private readonly IMatcher[] argumentMatchers;

		public InvocationShape(LambdaExpression expression, MethodInfo method, IReadOnlyList<Expression> arguments = null)
		{
			this.Expression = expression;
			this.Method = method;
			this.Arguments = arguments ?? noArguments;

			this.argumentMatchers = arguments != null ? MatcherFactory.CreateMatchers(arguments, method.GetParameters())
			                                          : noArgumentMatchers;
		}

		public void Deconstruct(out LambdaExpression expression, out MethodInfo method, out IReadOnlyList<Expression> arguments)
		{
			expression = this.Expression;
			method = this.Method;
			arguments = this.Arguments;
		}

		public bool IsMatch(Invocation invocation)
		{
			var arguments = invocation.Arguments;
			if (this.argumentMatchers.Length != arguments.Length)
			{
				return false;
			}

			if (invocation.Method != this.Method && !this.IsOverride(invocation.Method))
			{
				return false;
			}

			for (int i = 0, n = this.argumentMatchers.Length; i < n; ++i)
			{
				if (this.argumentMatchers[i].Matches(arguments[i]) == false)
				{
					return false;
				}
			}

			return true;
		}

		private bool IsOverride(MethodInfo invocationMethod)
		{
			var method = this.Method;

			if (!method.DeclaringType.IsAssignableFrom(invocationMethod.DeclaringType))
			{
				return false;
			}

			if (!method.Name.Equals(invocationMethod.Name, StringComparison.Ordinal))
			{
				return false;
			}

			if (method.ReturnType != invocationMethod.ReturnType)
			{
				return false;
			}

			if (method.IsGenericMethod || invocationMethod.IsGenericMethod)
			{
				if (!method.GetGenericArguments().CompareTo(invocationMethod.GetGenericArguments(), exact: false))
				{
					return false;
				}
			}
			else
			{
				if (!invocationMethod.GetParameterTypes().CompareTo(method.GetParameterTypes(), exact: true))
				{
					return false;
				}
			}

			return true;
		}
	}
}
