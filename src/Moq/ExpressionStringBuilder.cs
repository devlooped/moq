// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using Moq.Properties;

namespace Moq
{
	/// <summary>
	/// The intention of <see cref="ExpressionStringBuilder"/> is to create a more readable 
	/// string representation for the failure message.
	/// </summary>
	internal static class ExpressionStringBuilder
	{
		public static string ToString(Expression expression)
		{
			return new StringBuilder().ToString(expression).ToString();
		}

		private static StringBuilder ToString(this StringBuilder builder, Expression expression)
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
						return builder.AppendMatchExpression(me);
					}
					goto default;

				default:
					throw new Exception(string.Format(Resources.UnhandledExpressionType, expression.NodeType));
			}
		}

		private static StringBuilder AppendMemberBinding(this StringBuilder builder, MemberBinding binding)
		{
			switch (binding.BindingType)
			{
				case MemberBindingType.Assignment:
					return builder.AppendMemberAssignment((MemberAssignment)binding);

				case MemberBindingType.MemberBinding:
					return builder.AppendCommaSeparated(((MemberMemberBinding)binding).Bindings, AppendMemberBinding);

				case MemberBindingType.ListBinding:
					return builder.AppendElementInits(((MemberListBinding)binding).Initializers);

				default:
					throw new Exception(string.Format(Resources.UnhandledBindingType, binding.BindingType));
			}
		}

		private static StringBuilder AppendElementInit(this StringBuilder builder, ElementInit initializer)
		{
			return builder.AppendCommaSeparated("{ ", initializer.Arguments, ToString, " }");
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
					              .ToString(expression.Operand);

				case ExpressionType.ArrayLength:
					return builder.ToString(expression.Operand)
					              .Append(".Length");

				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
					return builder.Append('-')
					              .ToString(expression.Operand);

				case ExpressionType.Not:
					return builder.Append("!(")
					              .ToString(expression.Operand)
					              .Append(')');

				case ExpressionType.Quote:
					return builder.ToString(expression.Operand);

				case ExpressionType.TypeAs:
					return builder.Append('(')
					              .ToString(expression.Operand)
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
				builder.ToString(expression.Left)
				       .Append('[')
				       .ToString(expression.Right)
				       .Append(']');
			}
			else
			{
				string @operator = ToStringOperator(expression.NodeType);
				if (NeedEncloseInParen(expression.Left))
				{
					builder.Append('(')
					       .ToString(expression.Left)
					       .Append(')');
				}
				else
				{
					builder.ToString(expression.Left);
				}
				builder.Append(' ')
				       .Append(@operator)
				       .Append(' ');
				if (NeedEncloseInParen(expression.Right))
				{
					builder.Append('(')
					       .ToString(expression.Right)
					       .Append(')');
				}
				else
				{
					builder.ToString(expression.Right);
				}
			}

			return builder;
		}

		private static bool NeedEncloseInParen(Expression operand)
		{
			return operand.NodeType == ExpressionType.AndAlso || operand.NodeType == ExpressionType.OrElse;
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, TypeBinaryExpression expression)
		{
			return builder.ToString(expression.Expression);
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, ConditionalExpression expression)
		{
			return builder.ToString(expression.Test)
			              .Append(" ? ")
			              .ToString(expression.IfTrue)
			              .Append(" : ")
			              .ToString(expression.IfFalse);
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, ParameterExpression expression)
		{
			return builder.Append(expression.Name ?? "<param>");
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, MemberExpression expression)
		{
			if (expression.Expression != null)
			{
				builder.ToString(expression.Expression);
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
					builder.ToString(instance);
				}
				else // Method is static
				{
					builder.AppendNameOf(expression.Method.DeclaringType);
				}

				if (expression.Method.IsPropertyIndexerGetter())
				{
					builder.AppendCommaSeparated("[", expression.Arguments.Skip(paramFrom), ToString, "]");
				}
				else if (expression.Method.IsPropertyIndexerSetter())
				{
					builder.AppendCommaSeparated("[", expression.Arguments.Skip(paramFrom).Take(expression.Arguments.Count - paramFrom - 1), ToString, "] = ")
					       .ToString(expression.Arguments.Last());
				}
				else if (expression.Method.IsPropertyGetter())
				{
					builder.Append('.')
					       .Append(expression.Method.Name.Substring(4));
					if (expression.Arguments.Count > paramFrom)
					{
						builder.AppendCommaSeparated("[", expression.Arguments.Skip(paramFrom), ToString, "]");
					}
				}
				else if (expression.Method.IsPropertySetter())
				{
					builder.Append('.')
					       .Append(expression.Method.Name.Substring(4))
					       .Append(" = ")
					       .ToString(expression.Arguments.Last());
				}
				else if (expression.Method.LooksLikeEventAttach())
				{
					builder.Append('.')
					       .AppendNameOfAddEvent(expression.Method, includeGenericArgumentList: true)
					       .AppendCommaSeparated(expression.Arguments.Skip(paramFrom), ToString);
				}
				else if (expression.Method.LooksLikeEventDetach())
				{
					builder.Append('.')
					       .AppendNameOfRemoveEvent(expression.Method, includeGenericArgumentList: true)
					       .AppendCommaSeparated(expression.Arguments.Skip(paramFrom), ToString);
				}
				else
				{
					builder.Append('.')
					       .AppendNameOf(expression.Method, includeGenericArgumentList: true)
					       .AppendCommaSeparated("(", expression.Arguments.Skip(paramFrom), ToString, ")");
				}
			}

			return builder;
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, IndexExpression expression)
		{
			return builder.ToString(expression.Object)
			              .AppendCommaSeparated("[", expression.Arguments, ToString, "]");
		}

		private static StringBuilder AppendMemberAssignment(this StringBuilder builder, MemberAssignment assignment)
		{
			return builder.Append(assignment.Member.Name)
			              .Append("= ")
			              .ToString(assignment.Expression);
		}

		private static StringBuilder AppendElementInits(this StringBuilder builder, ReadOnlyCollection<ElementInit> original)
		{
			for (int i = 0, n = original.Count; i < n; i++)
			{
				builder.AppendElementInit(original[i]);
			}
			return builder;
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
			              .ToString(expression.Body);
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, NewExpression expression)
		{
			Type type = (expression.Constructor == null) ? expression.Type : expression.Constructor.DeclaringType;
			return builder.Append("new ")
			              .AppendNameOf(type)
			              .AppendCommaSeparated("(", expression.Arguments, ToString, ")");
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, MemberInitExpression expression)
		{
			return builder.AppendExpression(expression.NewExpression)
			              .AppendCommaSeparated(" { ", expression.Bindings, AppendMemberBinding, " }");
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, ListInitExpression expression)
		{
			return builder.AppendExpression(expression.NewExpression)
			              .AppendCommaSeparated(" { ", expression.Initializers, AppendElementInit, " }");
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, NewArrayExpression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.NewArrayInit:
					return builder.AppendCommaSeparated("new[] { ", expression.Expressions, ToString, " }");

				case ExpressionType.NewArrayBounds:
					return builder.Append("new ")
					              .AppendNameOf(expression.Type.GetElementType())
					              .AppendCommaSeparated("[", expression.Expressions, ToString, "]");
			}

			return builder;  // TODO: check whether this should be unreachable
		}

		private static StringBuilder AppendMatchExpression(this StringBuilder builder, MatchExpression expression)
		{
			return builder.ToString(expression.Match.RenderExpression);
		}

		private static StringBuilder AppendExpression(this StringBuilder builder, InvocationExpression expression)
		{
			return builder.ToString(expression.Expression)
			              .AppendCommaSeparated("(", expression.Arguments, ToString, ")");
		}

		internal static string ToStringOperator(ExpressionType nodeType)
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
}
