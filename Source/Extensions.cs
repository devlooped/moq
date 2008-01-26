using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Core.Interceptor;

namespace Moq
{
	internal static class Extensions
	{
		public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider source, bool inherit)
			where TAttribute : Attribute
		{
			object[] attrs = source.GetCustomAttributes(typeof(TAttribute), inherit);

			if (attrs.Length == 0)
			{
				return default(TAttribute);
			}
			else
			{
				return (TAttribute)attrs[0];
			}
		}

		public static void ForEach<TItem>(this IEnumerable<TItem> source, Action<TItem> action)
		{
			foreach (var item in source)
			{
				action(item);
			}
		}

		public static void ForEach(this IEnumerable source, Action<object> action)
		{
			ForEach<object>(source.OfType<object>(), action);
		}

		public static Expression StripQuotes(this Expression expression)
		{
			while (expression.NodeType == ExpressionType.Quote)
			{
				expression = ((UnaryExpression)expression).Operand;
			}

			return expression;
		}

		public static Expression PartialEval(this Expression expression)
		{
			return Evaluator.PartialEval(expression);
		}

		public static Expression CastTo<T>(this Expression expression)
		{
			return Expression.Convert(expression, typeof(T));
		}

		public static string Format(this IInvocation invocation)
		{
			return
				invocation.Method.DeclaringType.Name + "." +
				invocation.Method.Name + "(" +
				String.Join(", ",
					(from x in invocation.Arguments
					 select x == null ?
						"null" :
						x is string ?
							"\"" + (string)x + "\"" :
							x.ToString())
					.ToArray()
				) + ")";
		}
	}
}
