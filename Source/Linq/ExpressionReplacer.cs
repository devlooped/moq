// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System.Linq.Expressions;

namespace Moq.Linq
{
	/// <summary>
	/// Replaces references to one specific instance of an expression node with another node
	/// </summary>
	internal class ExpressionReplacer : ExpressionVisitor
	{
		private Expression searchFor;
		private Expression replaceWith;

		private ExpressionReplacer(Expression searchFor, Expression replaceWith)
		{
			this.searchFor = searchFor;
			this.replaceWith = replaceWith;
		}

		public static Expression Replace(Expression expression, Expression searchFor, Expression replaceWith)
		{
			return new ExpressionReplacer(searchFor, replaceWith).Visit(expression);
		}

		public static Expression ReplaceAll(Expression expression, Expression[] searchFor, Expression[] replaceWith)
		{
			for (int index = 0, n = searchFor.Length; index < n; index++)
			{
				expression = Replace(expression, searchFor[index], replaceWith[index]);
			}

			return expression;
		}

		public override Expression Visit(Expression node)
		{
			if (node == this.searchFor)
			{
				return this.replaceWith;
			}

			return base.Visit(node);
		}
	}
}
