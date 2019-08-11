using Moq.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Moq.Linq
{
	internal class ConstructorCallVisitor : ExpressionVisitor
	{
		/// <summary>
		/// Extracts the arguments from a lambda expression that calls a constructor.
		/// </summary>
		/// <param name="constructorExpression">The constructor expression.</param>
		/// <returns>Extracted argument values.</returns>
		public static object[] ExtractArgumentValues(LambdaExpression constructorExpression)
		{
			var visitor = new ConstructorCallVisitor();
			visitor.Visit(constructorExpression);

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
