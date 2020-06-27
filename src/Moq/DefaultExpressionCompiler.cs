// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

namespace Moq
{
	internal sealed class DefaultExpressionCompiler : ExpressionCompiler
	{
		new public static readonly DefaultExpressionCompiler Instance = new DefaultExpressionCompiler();

		private DefaultExpressionCompiler()
		{
		}

		public override Delegate Compile(LambdaExpression expression)
		{
			return expression.Compile();
		}

		public override TDelegate Compile<TDelegate>(Expression<TDelegate> expression)
		{
			return expression.Compile();
		}
	}
}
