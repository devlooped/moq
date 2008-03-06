using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Moq
{
	internal static class ExpressionExtensions
	{
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

		/// <devdoc>
		/// TODO: remove this code when https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=331583 
		/// is fixed.
		/// </devdoc>
		public static string ToStringFixed(this Expression expression)
		{
			return new ToStringFixVisitor(expression).ExpressionString;
		}

		internal class ToStringFixVisitor : ExpressionVisitor
		{
			string expressionString;
			List<MethodCallExpression> calls = new List<MethodCallExpression>();

			public ToStringFixVisitor(Expression expression)
			{
				Visit(expression);

				string fullString = expression.ToString();

				foreach (var call in calls)
				{
					string properCallString = BuildCallExpressionString(call);

					fullString = fullString.Replace(call.ToString(), properCallString);
				}

				expressionString = fullString;
			}

			private string BuildCallExpressionString(MethodCallExpression call)
			{
				var builder = new StringBuilder();
				int startIndex = 0;
				Expression targetExpr = call.Object;

				if (Attribute.GetCustomAttribute(call.Method, typeof(ExtensionAttribute)) != null)
				{
					// We should start rendering the args for the invocation from the 
					// second argument.
					startIndex = 1;
					targetExpr = call.Arguments[0];
				}

				if (targetExpr != null)
				{
					builder.Append(targetExpr.ToString());
					builder.Append(".");
				}

				builder.Append(call.Method.Name);
				if (call.Method.IsGenericMethod)
				{
					builder.Append("<");
					builder.Append(String.Join(", ",
						(from arg in call.Method.GetGenericArguments()
						 select arg.Name).ToArray()));
					builder.Append(">");
				}

				builder.Append("(");

				builder.Append(String.Join(", ",
					(from c in call.Arguments
					 select c.ToString()).ToArray(),
					startIndex, call.Arguments.Count - startIndex));

				builder.Append(")");

				string properCallString = builder.ToString();
				return properCallString;
			}

			public string ExpressionString { get { return expressionString; } }

			protected override Expression VisitMethodCall(MethodCallExpression m)
			{
				// We only need to fix generic methods.
				if (m.Method.IsGenericMethod)
					calls.Add(m);

				return base.VisitMethodCall(m);
			}
		}
	}
}
