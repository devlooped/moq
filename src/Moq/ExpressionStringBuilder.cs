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
	internal class ExpressionStringBuilder
	{
		private StringBuilder builder;

		public ExpressionStringBuilder()
		{
			this.builder = new StringBuilder();
		}

		public ExpressionStringBuilder Append(Expression expression)
		{
			this.ToString(expression);
			return this;
		}

		public override string ToString()
		{
			return this.builder.ToString();
		}

		private void ToString(Expression exp)
		{
			if (exp == null)
			{
				builder.Append("null");
				return;
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
					ToStringUnary((UnaryExpression)exp);
					return;
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
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
					ToStringBinary((BinaryExpression)exp);
					return;
				case ExpressionType.TypeIs:
					ToStringTypeIs((TypeBinaryExpression)exp);
					return;
				case ExpressionType.Conditional:
					ToStringConditional((ConditionalExpression)exp);
					return;
				case ExpressionType.Constant:
					ToStringConstant((ConstantExpression)exp);
					return;
				case ExpressionType.Parameter:
					ToStringParameter((ParameterExpression)exp);
					return;
				case ExpressionType.MemberAccess:
					ToStringMemberAccess((MemberExpression)exp);
					return;
				case ExpressionType.Call:
					ToStringMethodCall((MethodCallExpression)exp);
					return;
				case ExpressionType.Lambda:
					ToStringLambda((LambdaExpression)exp);
					return;
				case ExpressionType.New:
					ToStringNew((NewExpression)exp);
					return;
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
					ToStringNewArray((NewArrayExpression)exp);
					return;
				case ExpressionType.Invoke:
					ToStringInvocation((InvocationExpression)exp);
					return;
				case ExpressionType.MemberInit:
					ToStringMemberInit((MemberInitExpression)exp);
					return;
				case ExpressionType.ListInit:
					ToStringListInit((ListInitExpression)exp);
					return;
				default:
					throw new Exception(string.Format(Resources.UnhandledExpressionType, exp.NodeType));
			}
		}

		private void ToStringBinding(MemberBinding binding)
		{
			switch (binding.BindingType)
			{
				case MemberBindingType.Assignment:
					ToStringMemberAssignment((MemberAssignment)binding);
					return;
				case MemberBindingType.MemberBinding:
					ToStringMemberMemberBinding((MemberMemberBinding)binding);
					return;
				case MemberBindingType.ListBinding:
					ToStringMemberListBinding((MemberListBinding)binding);
					return;
				default:
					throw new Exception(string.Format(Resources.UnhandledBindingType, binding.BindingType));
			}
		}

		private void ToStringElementInitializer(ElementInit initializer)
		{
			builder.Append("{ ");
			ToStringExpressionList(initializer.Arguments);
			builder.Append(" }");
			return;
		}

		private void ToStringUnary(UnaryExpression u)
		{
			switch (u.NodeType)
			{
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
					builder.Append('(').AppendNameOf(u.Type).Append(')');
					ToString(u.Operand);
					return;

				case ExpressionType.ArrayLength:
					ToString(u.Operand);
					builder.Append(".Length");
					return;

				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
					builder.Append('-');
					ToString(u.Operand);
					return;

				case ExpressionType.Not:
					builder.Append("!(");
					ToString(u.Operand);
					builder.Append(')');
					return;

				case ExpressionType.Quote:
					ToString(u.Operand);
					return;

				case ExpressionType.TypeAs:
					builder.Append('(');
					ToString(u.Operand);
					builder.Append(" as ");
					builder.AppendNameOf(u.Type);
					builder.Append(')');
					return;
			}
			return;
		}

		private void ToStringBinary(BinaryExpression b)
		{
			if (b.NodeType == ExpressionType.ArrayIndex)
			{
				ToString(b.Left);
				builder.Append('[');
				ToString(b.Right);
				builder.Append(']');
			}
			else
			{
				string @operator = ToStringOperator(b.NodeType);
				if (NeedEncloseInParen(b.Left))
				{
					builder.Append('(');
					ToString(b.Left);
					builder.Append(')');
				}
				else
				{
					ToString(b.Left);
				}
				builder.Append(' ');
				builder.Append(@operator);
				builder.Append(' ');
				if (NeedEncloseInParen(b.Right))
				{
					builder.Append('(');
					ToString(b.Right);
					builder.Append(')');
				}
				else
				{
					ToString(b.Right);
				}
			}
		}

		private static bool NeedEncloseInParen(Expression operand)
		{
			return operand.NodeType == ExpressionType.AndAlso || operand.NodeType == ExpressionType.OrElse;
		}

		private void ToStringTypeIs(TypeBinaryExpression b)
		{
			ToString(b.Expression);
			return;
		}

		private void ToStringConstant(ConstantExpression c)
		{
			builder.AppendValueOf(c.Value);
		}

		private void ToStringConditional(ConditionalExpression c)
		{
			ToString(c.Test);
			ToString(c.IfTrue);
			ToString(c.IfFalse);
			return;
		}

		private void ToStringParameter(ParameterExpression p)
		{
			if (p.Name != null)
			{
				builder.Append(p.Name);
			}
			else
			{
				builder.Append("<param>");
			}
		}

		private void ToStringMemberAccess(MemberExpression m)
		{
			if (m.Expression != null)
			{
				ToString(m.Expression);
			}
			else
			{
				builder.AppendNameOf(m.Member.DeclaringType);
			}
			builder.Append('.');
			builder.Append(m.Member.Name);
			return;
		}

		private void ToStringMethodCall(MethodCallExpression node)
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
					ToString(expression);
				}
				else // Method is static
				{
					this.builder.AppendNameOf(node.Method.DeclaringType);
				}

				if (node.Method.IsPropertyIndexerGetter())
				{
					this.builder.Append('[');
					AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
					this.builder.Append(']');
				}
				else if (node.Method.IsPropertyIndexerSetter())
				{
					this.builder.Append('[');
					AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
					this.builder.Append("] = ");
					ToString(node.Arguments.Last());
				}
				else if (node.Method.IsPropertyGetter())
				{
					this.builder.Append('.').Append(node.Method.Name.Substring(4));
					if (node.Arguments.Count > paramFrom)
					{
						this.builder.Append('[');
						AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
						this.builder.Append(']');
					}
				}
				else if (node.Method.IsPropertySetter())
				{
					this.builder.Append('.').Append(node.Method.Name.Substring(4)).Append(" = ");
					ToString(node.Arguments.Last());
				}
				else
				{
					this.builder
						.Append('.')
						.AppendNameOf(node.Method, includeGenericArgumentList: true)
						.Append('(');
					AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
					this.builder.Append(')');
				}
			}
		}

		private void ToStringExpressionList(ReadOnlyCollection<Expression> original)
		{
			AsCommaSeparatedValues(original, ToString);
			return;
		}

		private void ToStringMemberAssignment(MemberAssignment assignment)
		{
			builder.Append(assignment.Member.Name);
			builder.Append("= ");
			ToString(assignment.Expression);
			return;
		}

		private void ToStringMemberMemberBinding(MemberMemberBinding binding)
		{
			ToStringBindingList(binding.Bindings);
			return;
		}

		private void ToStringMemberListBinding(MemberListBinding binding)
		{
			ToStringElementInitializerList(binding.Initializers);
			return;
		}

		private void ToStringBindingList(IEnumerable<MemberBinding> original)
		{
			bool appendComma = false;
			foreach (var exp in original)
			{
				if (appendComma)
				{
					builder.Append(", ");
				}
				ToStringBinding(exp);
				appendComma = true;
			}
			return;
		}

		private void ToStringElementInitializerList(ReadOnlyCollection<ElementInit> original)
		{
			for (int i = 0, n = original.Count; i < n; i++)
			{
				ToStringElementInitializer(original[i]);
			}
			return;
		}

		private void ToStringLambda(LambdaExpression lambda)
		{
			if (lambda.Parameters.Count == 1)
			{
				ToStringParameter(lambda.Parameters[0]);
			}
			else
			{
				builder.Append('(');
				AsCommaSeparatedValues(lambda.Parameters, ToStringParameter);
				builder.Append(')');
			}
			builder.Append(" => ");
			ToString(lambda.Body);
			return;
		}

		private void ToStringNew(NewExpression nex)
		{
			Type type = (nex.Constructor == null) ? nex.Type : nex.Constructor.DeclaringType;
			builder.Append("new ");
			builder.AppendNameOf(type);
			builder.Append('(');
			AsCommaSeparatedValues(nex.Arguments, ToString);
			builder.Append(')');
			return;
		}

		private void ToStringMemberInit(MemberInitExpression init)
		{
			ToStringNew(init.NewExpression);
			builder.Append(" { ");
			ToStringBindingList(init.Bindings);
			builder.Append(" }");
			return;
		}

		private void ToStringListInit(ListInitExpression init)
		{
			ToStringNew(init.NewExpression);
			builder.Append(" { ");
			bool appendComma = false;
			foreach (var initializer in init.Initializers)
			{
				if (appendComma)
				{
					builder.Append(", ");
				}
				ToStringElementInitializer(initializer);
				appendComma = true;
			}
			builder.Append(" }");
			return;
		}

		private void ToStringNewArray(NewArrayExpression na)
		{
			switch (na.NodeType)
			{
				case ExpressionType.NewArrayInit:
					builder.Append("new[] { ");
					AsCommaSeparatedValues(na.Expressions, ToString);
					builder.Append(" }");
					return;
				case ExpressionType.NewArrayBounds:
					builder.Append("new ");
					builder.AppendNameOf(na.Type.GetElementType());
					builder.Append('[');
					AsCommaSeparatedValues(na.Expressions, ToString);
					builder.Append(']');
					return;
			}
		}

		private void AsCommaSeparatedValues<T>(IEnumerable<T> source, Action<T> toStringAction) where T : Expression
		{
			bool appendComma = false;
			foreach (var exp in source)
			{
				if (appendComma)
				{
					builder.Append(", ");
				}
				toStringAction(exp);
				appendComma = true;
			}
		}

		private void ToStringInvocation(InvocationExpression iv)
		{
			ToString(iv.Expression);
			builder.Append('(');
			ToStringExpressionList(iv.Arguments);
			builder.Append(')');
			return;
		}

		internal static string ToStringOperator(ExpressionType nodeType)
		{
			switch (nodeType)
			{
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
					return "+";

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
			}
			return nodeType.ToString();
		}
	}
}
