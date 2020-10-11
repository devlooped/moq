// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Globalization;
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

			if (visitor.constructor == null)
			{
				throw new NotSupportedException(Resources.NoConstructorCallFound);
			}

			return visitor.arguments;
		}

		private ConstructorInfo constructor;
		private object[] arguments;

		public override Expression Visit(Expression node)
		{
			switch (node.NodeType)
			{
				case ExpressionType.Lambda:
				case ExpressionType.New:
				case ExpressionType.Quote:
					return base.Visit(node);
				default:
					throw new NotSupportedException(
						string.Format(
							CultureInfo.CurrentCulture,
							Resources.UnsupportedExpression,
							node.ToStringFixed()));
			}
		}

		protected override Expression VisitNew(NewExpression node)
		{
			if (node != null)
			{
				constructor = node.Constructor;

				// Creates a lambda which uses the same argument expressions as the
				// arguments contained in the NewExpression
				var argumentExtractor = Expression.Lambda<Func<object[]>>(
					Expression.NewArrayInit(
						typeof(object),
						node.Arguments.Select(a => Expression.Convert(a, typeof(object)))));
				arguments = ExpressionCompiler.Instance.Compile(argumentExtractor).Invoke();
			}
			return node;
		}
	}
}
