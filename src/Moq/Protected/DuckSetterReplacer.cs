using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq.Protected
{
	internal sealed class DuckSetterReplacer<TMock, TAnalog> : ExpressionVisitor
	{
		private ParameterExpression parameterToReplace;
		private ParameterExpression mockParameter;
		private Type mockType = typeof(TMock);

		private PropertyInfo GetMockProperty(PropertyInfo property)
		{
			return mockType.GetProperty(
				property.Name,
				BindingFlags.NonPublic | BindingFlags.Instance,
				null,
				property.PropertyType,
				property.GetIndexParameters().Select(p => p.ParameterType).ToArray(),
				new ParameterModifier[] { }
				);
		}

		protected override Expression VisitIndex(IndexExpression node)
		{
			if (node.Object is ParameterExpression parameterExpression && parameterExpression == parameterToReplace)
			{
				return Expression.MakeIndex(mockParameter, GetMockProperty(node.Indexer), node.Arguments);
			}
			return base.VisitIndex(node);
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node.Expression is ParameterExpression parameterExpression && parameterExpression == parameterToReplace)
			{
				return Expression.MakeMemberAccess(mockParameter, GetMockProperty(node.Member as PropertyInfo));
			}
			return base.VisitMember(node);
		}

		public Expression<Action<TMock>> Replace(Expression<Action<TAnalog>> expression)
		{
			parameterToReplace = expression.Parameters[0];
			mockParameter = Expression.Parameter(typeof(TMock), parameterToReplace.Name);
			return Expression.Lambda<Action<TMock>>(expression.Body.Apply(this), mockParameter);
		}

	}
}
