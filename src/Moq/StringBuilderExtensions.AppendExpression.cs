// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Moq.Properties;

namespace Moq
{
	// These methods are intended to create more readable string representations for use in failure messages.
	partial class StringBuilderExtensions
	{
		public static StringBuilder AppendExpression(this StringBuilder builder, Expression expression)
		{
			if (expression == null)
			{
				return builder.Append("null");
			}
			switch (expression.NodeType)
			{
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
				case ExpressionType.Not:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.ArrayLength:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
					return builder.AppendExpression((UnaryExpression)expression);

				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				case ExpressionType.AddAssign:
				case ExpressionType.Assign:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
				case ExpressionType.SubtractAssign:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
				case ExpressionType.Divide:
				case ExpressionType.Modulo:
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.Equal:
				case ExpressionType.NotEqual:
				case ExpressionType.Coalesce:
				case ExpressionType.ArrayIndex:
				case ExpressionType.RightShift:
				case ExpressionType.LeftShift:
				case ExpressionType.ExclusiveOr:
					return builder.AppendExpression((BinaryExpression)expression);

				case ExpressionType.TypeIs:
					return builder.AppendExpression((TypeBinaryExpression)expression);

				case ExpressionType.Conditional:
					return builder.AppendExpression((ConditionalExpression)expression);

				case ExpressionType.Constant:
					return builder.AppendValueOf(((ConstantExpression)expression).Value);

				case ExpressionType.Parameter:
					return builder.AppendExpression((ParameterExpression)expression);

				case ExpressionType.MemberAccess:
					return builder.AppendExpression((MemberExpression)expression);

				case ExpressionType.Call:
					return builder.AppendExpression((MethodCallExpression)expression);

				case ExpressionType.Index:
					return builder.AppendExpression((IndexExpression)expression);

				case ExpressionType.Lambda:
					return builder.AppendExpression((LambdaExpression)expression);

				case ExpressionType.New:
					return builder.AppendExpression((NewExpression)expression);

				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
					return builder.AppendExpression((NewArrayExpression)expression);

				case ExpressionType.Invoke:
					return builder.AppendExpression((InvocationExpression)expression);

				case ExpressionType.MemberInit:
					return builder.AppendExpression((MemberInitExpression)expression);

				case ExpressionType.ListInit:
					return builder.AppendExpression((ListInitExpression)expression);

				case ExpressionType.Extension:
					if (expression is MatchExpression me)
					{
						return builder.AppendExpression(me);
					}
					goto default;

				default:
					throw new Exception(string.Format(Resources.UnhandledExpressionType, expression.NodeType));
			}
		}

		private static StringBuilder AppendElementInit(this StringBuilder builder, ElementInit initializer)
		{
			return builder.AppendCommaSeparated("{ ", initializer.Arguments, AppendExpression, " }");
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, UnaryExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
					return builder.Append('(')
					              .AppendNameOf(expression.Type)
					              .Append(')')
					              .AppendExpression(expression.Operand);

				case ExpressionType.ArrayLength:
					return builder.AppendExpression(expression.Operand)
					              .Append(".Length");

				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
					return builder.Append('-')
					              .AppendExpression(expression.Operand);

				case ExpressionType.Not:
					return builder.Append("!(")
					              .AppendExpression(expression.Operand)
					              .Append(')');

				case ExpressionType.Quote:
					return builder.AppendExpression(expression.Operand);

				case ExpressionType.TypeAs:
					return builder.Append('(')
					              .AppendExpression(expression.Operand)
					              .Append(" as ")
					              .AppendNameOf(expression.Type)
					              .Append(')');
			}

			return builder;  // TODO: check whether this should be unreachable
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, BinaryExpression expression)
		{
			if (expression.NodeType == ExpressionType.ArrayIndex)
			{
				builder.AppendExpression(expression.Left)
				       .Append('[')
				       .AppendExpression(expression.Right)
				       .Append(']');
			}
			else
			{
				AppendMaybeParenthesized(expression.Left, builder);
				builder.Append(' ')
				       .Append(GetOperator(expression.NodeType))
				       .Append(' ');
				AppendMaybeParenthesized(expression.Right, builder);
			}

			return builder;

			void AppendMaybeParenthesized(Expression operand, StringBuilder b)
			{
				bool parenthesize = operand.NodeType == ExpressionType.AndAlso || operand.NodeType == ExpressionType.OrElse;
				if (parenthesize)
				{
					b.Append("(");
				}
				b.AppendExpression(operand);
				if (parenthesize)
				{
					b.Append(")");
				}
			}

			string GetOperator(ExpressionType nodeType)
			{
				switch (nodeType)
				{
					case ExpressionType.Add:
					case ExpressionType.AddChecked:
						return "+";

					case ExpressionType.AddAssign:
						return "+=";

					case ExpressionType.Assign:
						return "=";

					case ExpressionType.And:
						return "&";

					case ExpressionType.AndAlso:
						return "&&";

					case ExpressionType.Coalesce:
						return "??";

					case ExpressionType.Divide:
						return "/";

					case ExpressionType.Equal:
						return "==";

					case ExpressionType.ExclusiveOr:
						return "^";

					case ExpressionType.GreaterThan:
						return ">";

					case ExpressionType.GreaterThanOrEqual:
						return ">=";

					case ExpressionType.LeftShift:
						return "<<";

					case ExpressionType.LessThan:
						return "<";

					case ExpressionType.LessThanOrEqual:
						return "<=";

					case ExpressionType.Modulo:
						return "%";

					case ExpressionType.Multiply:
					case ExpressionType.MultiplyChecked:
						return "*";

					case ExpressionType.NotEqual:
						return "!=";

					case ExpressionType.Or:
						return "|";

					case ExpressionType.OrElse:
						return "||";

					case ExpressionType.Power:
						return "^";

					case ExpressionType.RightShift:
						return ">>";

					case ExpressionType.Subtract:
					case ExpressionType.SubtractChecked:
						return "-";

					case ExpressionType.SubtractAssign:
						return "-=";
				}
				return nodeType.ToString();
			}
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, TypeBinaryExpression expression)
		{
			return builder.AppendExpression(expression.Expression)
			              .Append(" is ")
			              .AppendNameOf(expression.TypeOperand);
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, ConditionalExpression expression)
		{
			return builder.AppendExpression(expression.Test)
			              .Append(" ? ")
			              .AppendExpression(expression.IfTrue)
			              .Append(" : ")
			              .AppendExpression(expression.IfFalse);
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, ParameterExpression expression)
		{
			return builder.Append(expression.Name ?? "<param>");
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, MemberExpression expression)
		{
			if (expression.Expression != null)
			{
				builder.AppendExpression(expression.Expression);
			}
			else
			{
				builder.AppendNameOf(expression.Member.DeclaringType);
			}

			return builder.Append('.')
			              .Append(expression.Member.Name);
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, MethodCallExpression expression)
		{
			if (expression != null)
			{
				var paramFrom = 0;
				var instance = expression.Object;

				if (expression.Method.IsExtensionMethod())
				{
					paramFrom = 1;
					instance = expression.Arguments[0];
				}

				if (instance != null)
				{
					builder.AppendExpression(instance);
				}
				else // Method is static
				{
					builder.AppendNameOf(expression.Method.DeclaringType);
				}

				if (expression.Method.IsPropertyIndexerGetter())
				{
					builder.AppendCommaSeparated("[", expression.Arguments.Skip(paramFrom), AppendExpression, "]");
				}
				else if (expression.Method.IsPropertyIndexerSetter())
				{
					builder.AppendCommaSeparated("[", expression.Arguments.Skip(paramFrom).Take(expression.Arguments.Count - paramFrom - 1), AppendExpression, "] = ")
					       .AppendExpression(expression.Arguments.Last());
				}
				else if (expression.Method.IsPropertyGetter())
				{
					builder.Append('.')
					       .Append(expression.Method.Name, 4);
					if (expression.Arguments.Count > paramFrom)
					{
						builder.AppendCommaSeparated("[", expression.Arguments.Skip(paramFrom), AppendExpression, "]");
					}
				}
				else if (expression.Method.IsPropertySetter())
				{
					builder.Append('.')
					       .Append(expression.Method.Name, 4)
					       .Append(" = ")
					       .AppendExpression(expression.Arguments.Last());
				}
				else if (expression.Method.LooksLikeEventAttach())
				{
					builder.Append('.')
					       .Append(expression.Method.Name, 4)
					       .Append(" += ")
					       .AppendCommaSeparated(expression.Arguments.Skip(paramFrom), AppendExpression);
				}
				else if (expression.Method.LooksLikeEventDetach())
				{
					builder.Append('.')
					       .Append(expression.Method.Name, 7)
					       .Append(" -= ")
					       .AppendCommaSeparated(expression.Arguments.Skip(paramFrom), AppendExpression);
				}
				else
				{
					builder.Append('.')
					       .AppendNameOf(expression.Method, includeGenericArgumentList: true)
					       .AppendCommaSeparated("(", expression.Arguments.Skip(paramFrom), AppendExpression, ")");
				}
			}

			return builder;
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, IndexExpression expression)
		{
			return builder.AppendExpression(expression.Object)
			              .AppendCommaSeparated("[", expression.Arguments, AppendExpression, "]");
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, LambdaExpression expression)
		{
			if (expression.Parameters.Count == 1)
			{
				builder.AppendExpression(expression.Parameters[0]);
			}
			else
			{
				builder.AppendCommaSeparated("(", expression.Parameters, AppendExpression, ")");
			}
			return builder.Append(" => ")
			              .AppendExpression(expression.Body);
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, NewExpression expression)
		{
			Type type = (expression.Constructor == null) ? expression.Type : expression.Constructor.DeclaringType;
			return builder.Append("new ")
			              .AppendNameOf(type)
			              .AppendCommaSeparated("(", expression.Arguments, AppendExpression, ")");
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, NewArrayExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.NewArrayInit:
					return builder.AppendCommaSeparated("new[] { ", expression.Expressions, AppendExpression, " }");

				case ExpressionType.NewArrayBounds:
					return builder.Append("new ")
					              .AppendNameOf(expression.Type.GetElementType())
					              .AppendCommaSeparated("[", expression.Expressions, AppendExpression, "]");
			}

			return builder;  // TODO: check whether this should be unreachable
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, InvocationExpression expression)
		{
			return builder.AppendExpression(expression.Expression)
			              .AppendCommaSeparated("(", expression.Arguments, AppendExpression, ")");
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, MemberInitExpression expression)
		{
			return builder.AppendExpression(expression.NewExpression)
			              .AppendCommaSeparated(" { ", expression.Bindings, AppendMemberBinding, " }");

			StringBuilder AppendMemberBinding(StringBuilder b, MemberBinding binding)
			{
				switch (binding.BindingType)
				{
					case MemberBindingType.Assignment:
						var assignment = (MemberAssignment)binding;
						return builder.Append(assignment.Member.Name)
						              .Append("= ")
						              .AppendExpression(assignment.Expression);

					case MemberBindingType.MemberBinding:
						return b.AppendCommaSeparated(((MemberMemberBinding)binding).Bindings, AppendMemberBinding);

					case MemberBindingType.ListBinding:
						var original = ((MemberListBinding)binding).Initializers;
						for (int i = 0, n = original.Count; i < n; i++)
						{
							builder.AppendElementInit(original[i]);
						}
						return builder;

					default:
						throw new Exception(string.Format(Resources.UnhandledBindingType, binding.BindingType));
				}
			}
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, ListInitExpression expression)
		{
			return builder.AppendExpression(expression.NewExpression)
			              .AppendCommaSeparated(" { ", expression.Initializers, AppendElementInit, " }");
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, MatchExpression expression)
		{
			return builder.AppendExpression(expression.Match.RenderExpression);
		}
	}
}
