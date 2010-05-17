using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Moq
{
	internal class ExpressionStringBuilder : ExpressionVisitor
	{
		private StringBuilder output = new StringBuilder();
		private Func<MethodInfo, string> getMethodName;

		private ExpressionStringBuilder(Func<MethodInfo, string> getMethodName)
		{
			this.getMethodName = getMethodName;
		}

		internal static string GetString(Expression expression)
		{
			return GetString(expression, method => method.GetName());
		}

		internal static string GetString(Expression expression, Func<MethodInfo, string> getMethodName)
		{
			var builder = new ExpressionStringBuilder(getMethodName);
			builder.Visit(expression);
			return builder.output.ToString();
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (node != null)
			{
				if (node.NodeType == ExpressionType.ArrayIndex)
				{
					this.Visit(node.Left);
					this.output.Append("[");
					this.Visit(node.Right);
					this.output.Append("]");
				}
				else
				{
					var @operator = GetStringOperator(node.NodeType);
					if (IsParentEnclosed(node.Left))
					{
						this.output.Append("(");
						this.Visit(node.Left);
						this.output.Append(")");
					}
					else
					{
						this.Visit(node.Left);
					}

					output.Append(" ").Append(@operator).Append(" ");
					if (IsParentEnclosed(node.Right))
					{
						this.output.Append("(");
						this.Visit(node.Right);
						this.output.Append(")");
					}
					else
					{
						this.Visit(node.Right);
					}
				}
			}

			return node;
		}

		protected override Expression VisitConditional(ConditionalExpression node)
		{
			if (node != null)
			{
				this.Visit(node.Test);
				this.Visit(node.IfTrue);
				this.Visit(node.IfFalse);
			}

			return node;
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (node != null)
			{
				var value = node.Value;
				if (value != null)
				{
					if (value is string)
					{
						this.output.Append('"').Append(value).Append('"');
					}
					else if (node.Type.IsEnum)
					{
						this.output.Append(node.Type.Name).Append(".").Append(value);
					}
					else if (value.ToString() != value.GetType().ToString())
					{
						this.output.Append(value);
					}
				}
				else
				{
					this.output.Append("null");
				}
			}

			return node;
		}

		protected override ElementInit VisitElementInit(ElementInit node)
		{
			if (node != null)
			{
				this.Visit(node.Arguments);
			}

			return node;
		}

		protected override Expression VisitInvocation(InvocationExpression node)
		{
			if (node != null)
			{
				this.Visit(node.Arguments);
			}

			return node;
		}

		protected override Expression VisitLambda<T>(Expression<T> node)
		{
			if (node != null)
			{
				if (node.Parameters.Count == 1)
				{
					this.VisitParameter(node.Parameters[0]);
				}
				else
				{
					this.output.Append("(");
					this.Visit(node.Parameters, n => this.VisitParameter(n), 0, node.Parameters.Count);
					this.output.Append(")");
				}

				output.Append(" => ");
				this.Visit(node.Body);
			}

			return node;
		}

		protected override Expression VisitListInit(ListInitExpression node)
		{
			if (node != null)
			{
				this.VisitNew(node.NewExpression);
				Visit(node.Initializers, n => this.VisitElementInit(n));
			}

			return node;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node != null)
			{
				if (node.Expression != null)
				{
					this.Visit(node.Expression);
				}
				else
				{
					this.output.Append(node.Member.DeclaringType.Name);
				}

				this.output.Append(".").Append(node.Member.Name);
			}

			return node;
		}

		protected override Expression VisitMemberInit(MemberInitExpression node)
		{
			if (node != null)
			{
				this.VisitNew(node.NewExpression);
				Visit(node.Bindings, n => this.VisitMemberBinding(n));
			}

			return node;
		}

		protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
		{
			if (node != null)
			{
				Visit(node.Initializers, n => this.VisitElementInit(n));
			}

			return node;
		}

		protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
		{
			if (node != null)
			{
				this.Visit(node.Expression);
			}

			return node;
		}

		protected override MemberBinding VisitMemberBinding(MemberBinding node)
		{
			if (node != null)
			{
				switch (node.BindingType)
				{
					case MemberBindingType.Assignment:
						this.VisitMemberAssignment((MemberAssignment)node);
						break;

					case MemberBindingType.MemberBinding:
						this.VisitMemberMemberBinding((MemberMemberBinding)node);
						break;

					case MemberBindingType.ListBinding:
						this.VisitMemberListBinding((MemberListBinding)node);
						break;

					default:
						throw new InvalidOperationException(string.Format(
							CultureInfo.CurrentCulture,
							"Unhandled binding type '{0}'",
							node.BindingType));
				}
			}

			return node;
		}

		protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
		{
			if (node != null)
			{
				Visit(node.Bindings, n => this.VisitMemberBinding(n));
			}

			return node;
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node != null)
			{
				var paramFrom = 0;
				var expression = node.Object;

				if (Attribute.GetCustomAttribute(node.Method, typeof(ExtensionAttribute)) != null)
				{
					paramFrom = 1;
					expression = node.Arguments[0];
				}

				if (expression != null)
				{
					this.Visit(expression);
				}
				else // Method is static
				{
					this.output.Append(node.Method.DeclaringType.Name);
				}

				if (node.Method.IsPropertyIndexerGetter())
				{
					this.output.Append("[");
					this.Visit(node.Arguments, n => this.Visit(n), paramFrom, node.Arguments.Count);
					this.output.Append("]");
				}
				else if (node.Method.IsPropertyIndexerSetter())
				{
					this.output.Append("[");
					this.Visit(node.Arguments, n => this.Visit(n), paramFrom, node.Arguments.Count - 1);
					this.output.Append("] = ");
					this.Visit(node.Arguments.Last());
				}
				else if (node.Method.IsPropertyGetter())
				{
					this.output.Append(".").Append(node.Method.Name.Substring(4));
				}
				else if (node.Method.IsPropertySetter())
				{
					this.output.Append(".").Append(node.Method.Name.Substring(4)).Append(" = ");
					this.Visit(node.Arguments.Last());
				}
				else
				{
					this.output.Append(".").Append(this.getMethodName(node.Method)).Append("(");
					this.Visit(node.Arguments, n => this.Visit(n), paramFrom, node.Arguments.Count);
					output.Append(")");
				}
			}

			return node;
		}

		protected override Expression VisitNew(NewExpression node)
		{
			if (node != null)
			{
				this.Visit(node.Arguments);
			}

			return node;
		}

		protected override Expression VisitNewArray(NewArrayExpression node)
		{
			if (node != null)
			{
				this.output.Append("new[] { ");
				this.Visit(node.Expressions);
				this.output.Append(" }");
			}

			return node;
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (node != null)
			{
				this.output.Append(node.Name != null ? node.Name : "<param>");
			}

			return node;
		}

		protected override Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			return node != null ? this.Visit(node.Expression) : node;
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node != null)
			{
				switch (node.NodeType)
				{
					case ExpressionType.Negate:
					case ExpressionType.NegateChecked:
						this.output.Append("-");
						this.Visit(node.Operand);
						break;

					case ExpressionType.Not:
						this.output.Append("!(");
						this.Visit(node.Operand);
						this.output.Append(")");
						break;

					case ExpressionType.Quote:
						this.Visit(node.Operand);
						break;

					case ExpressionType.TypeAs:
						this.output.Append("(");
						this.Visit(node.Operand);
						this.output.Append(" as ").Append(node.Type.Name).Append(")");
						break;
				}
			}

			return node;
		}

		private static string GetStringOperator(ExpressionType nodeType)
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

		private static bool IsParentEnclosed(Expression node)
		{
			return node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse;
		}

		private void Visit<T>(IList<T> arguments, Func<T, Expression> elementVisitor, int paramFrom, int paramTo)
		{
			var appendComma = false;
			for (var index = paramFrom; index < paramTo; index++)
			{
				if (appendComma)
				{
					this.output.Append(", ");
				}

				elementVisitor(arguments[index]);
				appendComma = true;
			}
		}
	}
}