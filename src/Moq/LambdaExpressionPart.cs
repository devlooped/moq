// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	/// <summary>
	///   A thin wrapper around a <see cref="LambdaExpression"/> that has been split off
	///   a larger expression by <see cref="ExpressionExtensions.Split(LambdaExpression)"/>.
	///   It addition to the partial expression itself, it carries a <see cref="MethodInfo"/>
	///   and optional arguments associated with it.
	/// </summary>
	internal readonly struct LambdaExpressionPart
	{
		private static readonly IReadOnlyList<Expression> none = new Expression[0];

		public readonly LambdaExpression Expression;
		public readonly MethodInfo Method;
		public readonly IReadOnlyList<Expression> Arguments;

		public LambdaExpressionPart(LambdaExpression expression, MethodInfo method, IReadOnlyList<Expression> arguments = null)
		{
			Debug.Assert(expression != null);
			Debug.Assert(method != null);

			this.Expression = expression;
			this.Method = method;
			this.Arguments = arguments ?? none;
		}

		public void Deconstruct(out LambdaExpression expression, out MethodInfo method, out IReadOnlyList<Expression> arguments)
		{
			expression = this.Expression;
			method = this.Method;
			arguments = this.Arguments;
		}
	}
}
