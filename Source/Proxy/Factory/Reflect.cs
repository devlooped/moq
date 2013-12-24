using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq.Proxy.Factory
{
	[DebuggerStepThrough]
	internal static class Reflect
	{
		public static FieldInfo Field<T>(Expression<Func<T>> expression)
		{
			var memberExpression = (expression.Body.NodeType == ExpressionType.Convert ?
				((UnaryExpression)expression.Body).Operand :
				expression.Body) as MemberExpression;

			if (memberExpression == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Resources.InvalidPropertyExpression", expression));
			}

			var field = memberExpression.Member as FieldInfo;
			if (field == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Resources.InvalidPropertyExpression", expression));
			}

			return field;
		}

		public static MethodInfo Method(Expression<Action> expression)
		{
			return GetMethod(expression);
		}

		public static MethodInfo Method<T>(Expression<Action<T>> expression)
		{
			return GetMethod(expression);
		}

		public static MethodInfo Method(Expression<Action> expression, Type type)
		{
			return GetMethod(expression).GetGenericMethodDefinition().MakeGenericMethod(type);
		}

		public static PropertyInfo Property<T>(Expression<Func<T, object>> expression)
		{
			var memberExpression = (expression.Body.NodeType == ExpressionType.Convert ?
				((UnaryExpression)expression.Body).Operand :
				expression.Body) as MemberExpression;

			if (memberExpression == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Resources.InvalidPropertyExpression", expression));
			}

			var property = memberExpression.Member as PropertyInfo;
			if (property == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Resources.InvalidPropertyExpression", expression));
			}

			return property;
		}

		public static ConstructorInfo Constructor<T>(Expression<Func<T>> expression)
		{
			var body = expression.Body as NewExpression;
			if (body == null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Resources.InvalidConstructorExpression", expression));
			}

			return body.Constructor;
		}

		private static MethodInfo GetMethod(LambdaExpression expression)
		{
			if (expression.Body.NodeType != ExpressionType.Call)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Resources.InvalidMethodExpression", expression));
			}

			return ((MethodCallExpression)expression.Body).Method;
		}
	}
}