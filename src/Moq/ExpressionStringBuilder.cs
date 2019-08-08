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

		private static StringBuilder ToString(this StringBuilder builder, Expression exp)
		{
			if (exp == null)
			{
				return builder.Append("null");
			}
			switch (exp.NodeType)
			{
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
				case ExpressionType.Not:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.ArrayLength:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
					return builder.ToStringUnary((UnaryExpression)exp);

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
					return builder.ToStringBinary((BinaryExpression)exp);

				case ExpressionType.TypeIs:
					return builder.ToStringTypeIs((TypeBinaryExpression)exp);

				case ExpressionType.Conditional:
					return builder.ToStringConditional((ConditionalExpression)exp);

				case ExpressionType.Constant:
					return builder.ToStringConstant((ConstantExpression)exp);

				case ExpressionType.Parameter:
					return builder.ToStringParameter((ParameterExpression)exp);

				case ExpressionType.MemberAccess:
					return builder.ToStringMemberAccess((MemberExpression)exp);

				case ExpressionType.Call:
					return builder.ToStringMethodCall((MethodCallExpression)exp);

				case ExpressionType.Index:
					return builder.ToStringIndex((IndexExpression)exp);

				case ExpressionType.Lambda:
					return builder.ToStringLambda((LambdaExpression)exp);

				case ExpressionType.New:
					return builder.ToStringNew((NewExpression)exp);

				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
					return builder.ToStringNewArray((NewArrayExpression)exp);

				case ExpressionType.Invoke:
					return builder.ToStringInvocation((InvocationExpression)exp);

				case ExpressionType.MemberInit:
					return builder.ToStringMemberInit((MemberInitExpression)exp);

				case ExpressionType.ListInit:
					return builder.ToStringListInit((ListInitExpression)exp);

				case ExpressionType.Extension:
					if (exp is MatchExpression me)
					{
						return builder.ToStringMatch(me);
					}
					goto default;

				default:
					throw new Exception(string.Format(Resources.UnhandledExpressionType, exp.NodeType));
			}
		}

		private static StringBuilder ToStringBinding(this StringBuilder builder, MemberBinding binding)
		{
			switch (binding.BindingType)
			{
				case MemberBindingType.Assignment:
					return builder.ToStringMemberAssignment((MemberAssignment)binding);

				case MemberBindingType.MemberBinding:
					return builder.ToStringMemberMemberBinding((MemberMemberBinding)binding);

				case MemberBindingType.ListBinding:
					return builder.ToStringMemberListBinding((MemberListBinding)binding);

				default:
					throw new Exception(string.Format(Resources.UnhandledBindingType, binding.BindingType));
			}
		}

		private static StringBuilder ToStringElementInitializer(this StringBuilder builder, ElementInit initializer)
		{
			return builder.Append("{ ")
			              .ToStringExpressionList(initializer.Arguments)
			              .Append(" }");
		}

		private static StringBuilder ToStringUnary(this StringBuilder builder, UnaryExpression u)
		{
			switch (u.NodeType)
			{
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
					return builder.Append('(')
					              .AppendNameOf(u.Type)
					              .Append(')')
					              .ToString(u.Operand);

				case ExpressionType.ArrayLength:
					return builder.ToString(u.Operand)
					              .Append(".Length");

				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
					return builder.Append('-')
					              .ToString(u.Operand);

				case ExpressionType.Not:
					return builder.Append("!(")
					              .ToString(u.Operand)
					              .Append(')');

				case ExpressionType.Quote:
					return builder.ToString(u.Operand);

				case ExpressionType.TypeAs:
					return builder.Append('(')
					              .ToString(u.Operand)
					              .Append(" as ")
					              .AppendNameOf(u.Type)
					              .Append(')');
			}

			return builder;  // TODO: check whether this should be unreachable
		}

		private static StringBuilder ToStringBinary(this StringBuilder builder, BinaryExpression b)
		{
			if (b.NodeType == ExpressionType.ArrayIndex)
			{
				builder.ToString(b.Left)
				       .Append('[')
				       .ToString(b.Right)
				       .Append(']');
			}
			else
			{
				string @operator = ToStringOperator(b.NodeType);
				if (NeedEncloseInParen(b.Left))
				{
					builder.Append('(')
					       .ToString(b.Left)
					       .Append(')');
				}
				else
				{
					builder.ToString(b.Left);
				}
				builder.Append(' ')
				       .Append(@operator)
				       .Append(' ');
				if (NeedEncloseInParen(b.Right))
				{
					builder.Append('(')
					       .ToString(b.Right)
					       .Append(')');
				}
				else
				{
					builder.ToString(b.Right);
				}
			}

			return builder;
		}

		private static bool NeedEncloseInParen(Expression operand)
		{
			return operand.NodeType == ExpressionType.AndAlso || operand.NodeType == ExpressionType.OrElse;
		}

		private static StringBuilder ToStringTypeIs(this StringBuilder builder, TypeBinaryExpression b)
		{
			return builder.ToString(b.Expression);
		}

		private static StringBuilder ToStringConstant(this StringBuilder builder, ConstantExpression c)
		{
			return builder.AppendValueOf(c.Value);
		}

		private static StringBuilder ToStringConditional(this StringBuilder builder, ConditionalExpression c)
		{
			return builder.ToString(c.Test)
			              .Append(" ? ")
			              .ToString(c.IfTrue)
			              .Append(" : ")
			              .ToString(c.IfFalse);
		}

		private static StringBuilder ToStringParameter(this StringBuilder builder, ParameterExpression p)
		{
			return builder.Append(p.Name ?? "<param>");
		}

		private static StringBuilder ToStringMemberAccess(this StringBuilder builder, MemberExpression m)
		{
			if (m.Expression != null)
			{
				builder.ToString(m.Expression);
			}
			else
			{
				builder.AppendNameOf(m.Member.DeclaringType);
			}

			return builder.Append('.')
			              .Append(m.Member.Name);
		}

		private static StringBuilder ToStringMethodCall(this StringBuilder builder, MethodCallExpression node)
		{
			if (node != null)
			{
				var paramFrom = 0;
				var expression = node.Object;

				if (node.Method.IsExtensionMethod())
				{
					paramFrom = 1;
					expression = node.Arguments[0];
				}

				if (expression != null)
				{
					builder.ToString(expression);
				}
				else // Method is static
				{
					builder.AppendNameOf(node.Method.DeclaringType);
				}

				if (node.Method.IsPropertyIndexerGetter())
				{
					builder.Append('[')
					       .AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString)
					       .Append(']');
				}
				else if (node.Method.IsPropertyIndexerSetter())
				{
					builder.Append('[')
					       .AsCommaSeparatedValues(node.Arguments.Skip(paramFrom).Take(node.Arguments.Count - paramFrom - 1), ToString)
					       .Append("] = ")
					       .ToString(node.Arguments.Last());
				}
				else if (node.Method.IsPropertyGetter())
				{
					builder.Append('.')
					       .Append(node.Method.Name.Substring(4));
					if (node.Arguments.Count > paramFrom)
					{
						builder.Append('[')
						       .AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString)
						       .Append(']');
					}
				}
				else if (node.Method.IsPropertySetter())
				{
					builder.Append('.')
					       .Append(node.Method.Name.Substring(4))
					       .Append(" = ")
					       .ToString(node.Arguments.Last());
				}
				else if (node.Method.LooksLikeEventAttach())
				{
					builder.Append('.')
					       .AppendNameOfAddEvent(node.Method, includeGenericArgumentList: true)
					       .AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
				}
				else if (node.Method.LooksLikeEventDetach())
				{
					builder.Append('.')
					       .AppendNameOfRemoveEvent(node.Method, includeGenericArgumentList: true)
					       .AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
				}
				else
				{
					builder.Append('.')
					       .AppendNameOf(node.Method, includeGenericArgumentList: true)
					       .Append('(')
					       .AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString)
					       .Append(')');
				}
			}

			return builder;
		}

		private static StringBuilder ToStringIndex(this StringBuilder builder, IndexExpression expression)
		{
			return builder.ToString(expression.Object)
			              .Append('[')
			              .ToStringExpressionList(expression.Arguments)
			              .Append(']');
		}

		private static StringBuilder ToStringExpressionList(this StringBuilder builder, IEnumerable<Expression> original)
		{
			return builder.AsCommaSeparatedValues(original, ToString);
		}

		private static StringBuilder ToStringMemberAssignment(this StringBuilder builder, MemberAssignment assignment)
		{
			return builder.Append(assignment.Member.Name)
			              .Append("= ")
			              .ToString(assignment.Expression);
		}

		private static StringBuilder ToStringMemberMemberBinding(this StringBuilder builder, MemberMemberBinding binding)
		{
			return builder.ToStringBindingList(binding.Bindings);
		}

		private static StringBuilder ToStringMemberListBinding(this StringBuilder builder, MemberListBinding binding)
		{
			return builder.ToStringElementInitializerList(binding.Initializers);
		}

		private static StringBuilder ToStringBindingList(this StringBuilder builder, IEnumerable<MemberBinding> original)
		{
			bool appendComma = false;
			foreach (var exp in original)
			{
				if (appendComma)
				{
					builder.Append(", ");
				}
				builder.ToStringBinding(exp);
				appendComma = true;
			}
			return builder;
		}

		private static StringBuilder ToStringElementInitializerList(this StringBuilder builder, ReadOnlyCollection<ElementInit> original)
		{
			for (int i = 0, n = original.Count; i < n; i++)
			{
				builder.ToStringElementInitializer(original[i]);
			}
			return builder;
		}

		private static StringBuilder ToStringLambda(this StringBuilder builder, LambdaExpression lambda)
		{
			if (lambda.Parameters.Count == 1)
			{
				builder.ToStringParameter(lambda.Parameters[0]);
			}
			else
			{
				builder.Append('(')
				       .AsCommaSeparatedValues(lambda.Parameters, ToStringParameter)
				       .Append(')');
			}
			return builder.Append(" => ")
			              .ToString(lambda.Body);
		}

		private static StringBuilder ToStringNew(this StringBuilder builder, NewExpression nex)
		{
			Type type = (nex.Constructor == null) ? nex.Type : nex.Constructor.DeclaringType;
			return builder.Append("new ")
			              .AppendNameOf(type)
			              .Append('(')
			              .AsCommaSeparatedValues(nex.Arguments, ToString)
			              .Append(')');
		}

		private static StringBuilder ToStringMemberInit(this StringBuilder builder, MemberInitExpression init)
		{
			return builder.ToStringNew(init.NewExpression)
			              .Append(" { ")
			              .ToStringBindingList(init.Bindings)
			              .Append(" }");
		}

		private static StringBuilder ToStringListInit(this StringBuilder builder, ListInitExpression init)
		{
			builder.ToStringNew(init.NewExpression)
			       .Append(" { ");
			bool appendComma = false;
			foreach (var initializer in init.Initializers)
			{
				if (appendComma)
				{
					builder.Append(", ");
				}
				builder.ToStringElementInitializer(initializer);
				appendComma = true;
			}
			return builder.Append(" }");
		}

		private static StringBuilder ToStringNewArray(this StringBuilder builder, NewArrayExpression na)
		{
			switch (na.NodeType)
			{
				case ExpressionType.NewArrayInit:
					return builder.Append("new[] { ")
					              .AsCommaSeparatedValues(na.Expressions, ToString)
					              .Append(" }");

				case ExpressionType.NewArrayBounds:
					return builder.Append("new ")
					              .AppendNameOf(na.Type.GetElementType())
					              .Append('[')
					              .AsCommaSeparatedValues(na.Expressions, ToString)
					              .Append(']');
			}

			return builder;  // TODO: check whether this should be unreachable
		}

		private static StringBuilder ToStringMatch(this StringBuilder builder, MatchExpression node)
		{
			return builder.ToString(node.Match.RenderExpression);
		}

		private static StringBuilder AsCommaSeparatedValues<T>(this StringBuilder builder, IEnumerable<T> source, Func<StringBuilder, T, StringBuilder> append) where T : Expression
		{
			bool appendComma = false;
			foreach (var exp in source)
			{
				if (appendComma)
				{
					builder.Append(", ");
				}
				append(builder, exp);
				appendComma = true;
			}
			return builder;
		}

		private static StringBuilder ToStringInvocation(this StringBuilder builder, InvocationExpression iv)
		{
			return builder.ToString(iv.Expression)
			              .Append('(')
			              .ToStringExpressionList(iv.Arguments)
			              .Append(')');
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
