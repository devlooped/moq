// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Properties;

namespace Moq.Expressions.Visitors
{
	internal class ConstructorCallVisitor : ExpressionVisitor
	{
		/// <summary>
		/// Extracts the arguments from a lambda expression that calls a constructor.
		/// </summary>
		/// <param name="newExpression">The constructor expression.</param>
		/// <returns>Extracted argument values.</returns>
		public static object[] ExtractArgumentValues(LambdaExpression newExpression)
		{
			if (newExpression is null)
			{
				throw new ArgumentNullException(nameof(newExpression));
			}

			var visitor = new ConstructorCallVisitor();
			visitor.Visit(newExpression);

			if (visitor._constructor == null)
			{
				throw new NotSupportedException(Resources.NoConstructorCallFound);
			}

			return visitor._arguments;
		}

		private ConstructorInfo _constructor;
		private object[] _arguments;

		public override Expression Visit(Expression node)
		{
			switch (node)
			{
				case LambdaExpression _:
				case NewExpression _:
				case UnaryExpression unary when unary.NodeType == ExpressionType.Quote:
					return base.Visit(node);
				default:
					throw new NotSupportedException(
						string.Format(Resources.UnsupportedExpressionInConstructorCall, node.NodeType.ToString()));
			}
		}

		protected override Expression VisitNew(NewExpression node)
		{
			if (node != null)
			{
				_constructor = node.Constructor;

				// Creates a lambda which uses the same argument expressions as the
				// arguments contained in the NewExpression
				var argumentExtractor = Expression.Lambda<Func<object[]>>(
					Expression.NewArrayInit(
						typeof(object),
						node.Arguments.Select(a => Expression.Convert(a, typeof(object)))));
				_arguments = argumentExtractor.Compile().Invoke();
			}
			return node;
		}
	}
}
