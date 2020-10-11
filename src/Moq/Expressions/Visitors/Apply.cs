// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Linq.Expressions;

namespace Moq
{
	static partial class ExpressionExtensions
	{
		/// <summary>
		///   Applies the specified <see cref="ExpressionVisitor"/> to this expression tree.
		/// </summary>
		/// <param name="expression">The <see cref="Expression"/> to which <paramref name="visitor"/> should be applied.</param>
		/// <param name="visitor">The <see cref="ExpressionVisitor"/> that should be applied to <paramref name="expression"/>.</param>
		public static Expression Apply(this Expression expression, ExpressionVisitor visitor)
		{
			return visitor.Visit(expression);
		}
	}
}
