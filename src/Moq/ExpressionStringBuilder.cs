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
					builder.ToStringUnary((UnaryExpression)exp);
					return builder;
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
					builder.ToStringBinary((BinaryExpression)exp);
					return builder;
				case ExpressionType.TypeIs:
					builder.ToStringTypeIs((TypeBinaryExpression)exp);
					return builder;
				case ExpressionType.Conditional:
					builder.ToStringConditional((ConditionalExpression)exp);
					return builder;
				case ExpressionType.Constant:
					builder.ToStringConstant((ConstantExpression)exp);
					return builder;
				case ExpressionType.Parameter:
					builder.ToStringParameter((ParameterExpression)exp);
					return builder;
				case ExpressionType.MemberAccess:
					builder.ToStringMemberAccess((MemberExpression)exp);
					return builder;
				case ExpressionType.Call:
					builder.ToStringMethodCall((MethodCallExpression)exp);
					return builder;
				case ExpressionType.Index:
					builder.ToStringIndex((IndexExpression)exp);
					return builder;
				case ExpressionType.Lambda:
					builder.ToStringLambda((LambdaExpression)exp);
					return builder;
				case ExpressionType.New:
					builder.ToStringNew((NewExpression)exp);
					return builder;
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
					builder.ToStringNewArray((NewArrayExpression)exp);
					return builder;
				case ExpressionType.Invoke:
					builder.ToStringInvocation((InvocationExpression)exp);
					return builder;
				case ExpressionType.MemberInit:
					builder.ToStringMemberInit((MemberInitExpression)exp);
					return builder;
				case ExpressionType.ListInit:
					builder.ToStringListInit((ListInitExpression)exp);
					return builder;
				case ExpressionType.Extension:
					if (exp is MatchExpression me)
					{
						builder.ToStringMatch(me);
						return builder;
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
					builder.ToStringMemberAssignment((MemberAssignment)binding);
					return builder;
				case MemberBindingType.MemberBinding:
					builder.ToStringMemberMemberBinding((MemberMemberBinding)binding);
					return builder;
				case MemberBindingType.ListBinding:
					builder.ToStringMemberListBinding((MemberListBinding)binding);
					return builder;
				default:
					throw new Exception(string.Format(Resources.UnhandledBindingType, binding.BindingType));
			}
		}

		private static StringBuilder ToStringElementInitializer(this StringBuilder builder, ElementInit initializer)
		{
			builder.Append("{ ");
			builder.ToStringExpressionList(initializer.Arguments);
			builder.Append(" }");
			return builder;
		}

		private static StringBuilder ToStringUnary(this StringBuilder builder, UnaryExpression u)
		{
			switch (u.NodeType)
			{
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
					builder.Append('(').AppendNameOf(u.Type).Append(')');
					builder.ToString(u.Operand);
					return builder;

				case ExpressionType.ArrayLength:
					builder.ToString(u.Operand);
					builder.Append(".Length");
					return builder;

				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
					builder.Append('-');
					builder.ToString(u.Operand);
					return builder;

				case ExpressionType.Not:
					builder.Append("!(");
					builder.ToString(u.Operand);
					builder.Append(')');
					return builder;

				case ExpressionType.Quote:
					builder.ToString(u.Operand);
					return builder;

				case ExpressionType.TypeAs:
					builder.Append('(');
					builder.ToString(u.Operand);
					builder.Append(" as ");
					builder.AppendNameOf(u.Type);
					builder.Append(')');
					return builder;
			}

			return builder;  // TODO: check whether this should be unreachable
		}

		private static StringBuilder ToStringBinary(this StringBuilder builder, BinaryExpression b)
		{
			if (b.NodeType == ExpressionType.ArrayIndex)
			{
				builder.ToString(b.Left);
				builder.Append('[');
				builder.ToString(b.Right);
				builder.Append(']');
			}
			else
			{
				string @operator = ToStringOperator(b.NodeType);
				if (NeedEncloseInParen(b.Left))
				{
					builder.Append('(');
					builder.ToString(b.Left);
					builder.Append(')');
				}
				else
				{
					builder.ToString(b.Left);
				}
				builder.Append(' ');
				builder.Append(@operator);
				builder.Append(' ');
				if (NeedEncloseInParen(b.Right))
				{
					builder.Append('(');
					builder.ToString(b.Right);
					builder.Append(')');
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
			builder.ToString(b.Expression);
			return builder;
		}

		private static StringBuilder ToStringConstant(this StringBuilder builder, ConstantExpression c)
		{
			builder.AppendValueOf(c.Value);
			return builder;
		}

		private static StringBuilder ToStringConditional(this StringBuilder builder, ConditionalExpression c)
		{
			builder.ToString(c.Test);
			builder.Append(" ? ");
			builder.ToString(c.IfTrue);
			builder.Append(" : ");
			builder.ToString(c.IfFalse);
			return builder;
		}

		private static StringBuilder ToStringParameter(this StringBuilder builder, ParameterExpression p)
		{
			if (p.Name != null)
			{
				builder.Append(p.Name);
			}
			else
			{
				builder.Append("<param>");
			}

			return builder;
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
			builder.Append('.');
			builder.Append(m.Member.Name);
			return builder;
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
					builder.Append('[');
					builder.AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
					builder.Append(']');
				}
				else if (node.Method.IsPropertyIndexerSetter())
				{
					builder.Append('[');
					builder.AsCommaSeparatedValues(node.Arguments.Skip(paramFrom).Take(node.Arguments.Count - paramFrom - 1), ToString);
					builder.Append("] = ");
					builder.ToString(node.Arguments.Last());
				}
				else if (node.Method.IsPropertyGetter())
				{
					builder.Append('.').Append(node.Method.Name.Substring(4));
					if (node.Arguments.Count > paramFrom)
					{
						builder.Append('[');
						builder.AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
						builder.Append(']');
					}
				}
				else if (node.Method.IsPropertySetter())
				{
					builder.Append('.').Append(node.Method.Name.Substring(4)).Append(" = ");
					builder.ToString(node.Arguments.Last());
				}
				else if (node.Method.LooksLikeEventAttach())
				{
					builder.Append('.')
					       .AppendNameOfAddEvent(node.Method, includeGenericArgumentList: true);
					builder.AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
				}
				else if (node.Method.LooksLikeEventDetach())
				{
					builder.Append('.')
					       .AppendNameOfRemoveEvent(node.Method, includeGenericArgumentList: true);
					builder.AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
				}
				else
				{
					builder
						.Append('.')
						.AppendNameOf(node.Method, includeGenericArgumentList: true)
						.Append('(');
					builder.AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
					builder.Append(')');
				}
			}

			return builder;
		}

		private static StringBuilder ToStringIndex(this StringBuilder builder, IndexExpression expression)
		{
			builder.ToString(expression.Object);
			builder.Append('[');
			builder.ToStringExpressionList(expression.Arguments);
			builder.Append(']');
			return builder;
		}

		private static StringBuilder ToStringExpressionList(this StringBuilder builder, IEnumerable<Expression> original)
		{
			builder.AsCommaSeparatedValues(original, ToString);
			return builder;
		}

		private static StringBuilder ToStringMemberAssignment(this StringBuilder builder, MemberAssignment assignment)
		{
			builder.Append(assignment.Member.Name);
			builder.Append("= ");
			builder.ToString(assignment.Expression);
			return builder;
		}

		private static StringBuilder ToStringMemberMemberBinding(this StringBuilder builder, MemberMemberBinding binding)
		{
			builder.ToStringBindingList(binding.Bindings);
			return builder;
		}

		private static StringBuilder ToStringMemberListBinding(this StringBuilder builder, MemberListBinding binding)
		{
			builder.ToStringElementInitializerList(binding.Initializers);
			return builder;
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
				builder.Append('(');
				builder.AsCommaSeparatedValues(lambda.Parameters, ToStringParameter);
				builder.Append(')');
			}
			builder.Append(" => ");
			builder.ToString(lambda.Body);
			return builder;
		}

		private static StringBuilder ToStringNew(this StringBuilder builder, NewExpression nex)
		{
			Type type = (nex.Constructor == null) ? nex.Type : nex.Constructor.DeclaringType;
			builder.Append("new ");
			builder.AppendNameOf(type);
			builder.Append('(');
			builder.AsCommaSeparatedValues(nex.Arguments, ToString);
			builder.Append(')');
			return builder;
		}

		private static StringBuilder ToStringMemberInit(this StringBuilder builder, MemberInitExpression init)
		{
			builder.ToStringNew(init.NewExpression);
			builder.Append(" { ");
			builder.ToStringBindingList(init.Bindings);
			builder.Append(" }");
			return builder;
		}

		private static StringBuilder ToStringListInit(this StringBuilder builder, ListInitExpression init)
		{
			builder.ToStringNew(init.NewExpression);
			builder.Append(" { ");
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
			builder.Append(" }");
			return builder;
		}

		private static StringBuilder ToStringNewArray(this StringBuilder builder, NewArrayExpression na)
		{
			switch (na.NodeType)
			{
				case ExpressionType.NewArrayInit:
					builder.Append("new[] { ");
					builder.AsCommaSeparatedValues(na.Expressions, ToString);
					builder.Append(" }");
					return builder;
				case ExpressionType.NewArrayBounds:
					builder.Append("new ");
					builder.AppendNameOf(na.Type.GetElementType());
					builder.Append('[');
					builder.AsCommaSeparatedValues(na.Expressions, ToString);
					builder.Append(']');
					return builder;
			}

			return builder;  // TODO: check whether this should be unreachable
		}

		private static StringBuilder ToStringMatch(this StringBuilder builder, MatchExpression node)
		{
			builder.ToString(node.Match.RenderExpression);
			return builder;
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
			builder.ToString(iv.Expression);
			builder.Append('(');
			builder.ToStringExpressionList(iv.Arguments);
			builder.Append(')');
			return builder;
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
