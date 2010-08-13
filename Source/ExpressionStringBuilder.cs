using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.ObjectModel;

namespace Moq
{
	/// <summary>
	/// The intention of <see cref="ExpressionStringBuilder"/> is to create a more readable 
	/// string representation for the failure message.
	/// </summary>
	internal class ExpressionStringBuilder
	{
		private readonly Expression expression;
		private StringBuilder builder;
		private Func<Type, string> getTypeName;

		internal static string GetString(Expression expression)
		{
			return GetString(expression, type => type.Name);
		}

		internal static string GetString(Expression expression, Func<Type, string> getTypeName)
		{
			var builder = new ExpressionStringBuilder(getTypeName, expression);
			return builder.ToString();
		}

		public ExpressionStringBuilder(Func<Type, string> getTypeName, Expression expression)
		{
			this.getTypeName = getTypeName;
			this.expression = expression;
		}

		public override string ToString()
		{
			builder = new StringBuilder();
			ToString(expression);
			return builder.ToString();
		}

		public void ToString(Expression exp)
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
					throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
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
					throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
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
					builder.Append("(").Append(this.getTypeName(u.Type)).Append(")");
					ToString(u.Operand);
					//builder.Append(")");
					return;

				case ExpressionType.ArrayLength:
					ToString(u.Operand);
					builder.Append(".Length");
					return;

				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
					builder.Append("-");
					ToString(u.Operand);
					return;

				case ExpressionType.Not:
					builder.Append("!(");
					ToString(u.Operand);
					builder.Append(")");
					return;

				case ExpressionType.Quote:
					ToString(u.Operand);
					return;

				case ExpressionType.TypeAs:
					builder.Append("(");
					ToString(u.Operand);
					builder.Append(" as ");
					builder.Append(u.Type.DisplayName(this.getTypeName));
					builder.Append(")");
					return;
			}
			return;
		}

		private void ToStringBinary(BinaryExpression b)
		{
			if (b.NodeType == ExpressionType.ArrayIndex)
			{
				ToString(b.Left);
				builder.Append("[");
				ToString(b.Right);
				builder.Append("]");
			}
			else
			{
				string @operator = ToStringOperator(b.NodeType);
				if (NeedEncloseInParen(b.Left))
				{
					builder.Append("(");
					ToString(b.Left);
					builder.Append(")");
				}
				else
				{
					ToString(b.Left);
				}
				builder.Append(" ");
				builder.Append(@operator);
				builder.Append(" ");
				if (NeedEncloseInParen(b.Right))
				{
					builder.Append("(");
					ToString(b.Right);
					builder.Append(")");
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
			var value = c.Value;
			if (value != null)
			{
				if (value is string)
				{
					builder.Append("\"").Append(value).Append("\"");
				}
				else if (value.ToString() == value.GetType().ToString())
				{
					// Perhaps is better without nothing (at least for local variables)
					//builder.Append("<value>");
				}
				else if (c.Type.IsEnum)
				{
					builder.Append(c.Type.DisplayName(this.getTypeName)).Append(".").Append(value);
				}
				else
				{
					builder.Append(value);
				}
			}
			else
			{
				builder.Append("null");
			}
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
				builder.Append(m.Member.DeclaringType.DisplayName(this.getTypeName));
			}
			builder.Append(".");
			builder.Append(m.Member.Name);
			return;
		}

		private void ToStringMethodCall(MethodCallExpression node)
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
					ToString(expression);
				}
				else // Method is static
				{
					this.builder.Append(this.getTypeName(node.Method.DeclaringType));
				}

				if (node.Method.IsPropertyIndexerGetter())
				{
					this.builder.Append("[");
					AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
					this.builder.Append("]");
				}
				else if (node.Method.IsPropertyIndexerSetter())
				{
					this.builder.Append("[");
					AsCommaSeparatedValues(node.Arguments
						.Skip(paramFrom)
						.Take(node.Arguments.Count - paramFrom), ToString);
					this.builder.Append("] = ");
					ToString(node.Arguments.Last());
				}
				else if (node.Method.IsPropertyGetter())
				{
					this.builder.Append(".").Append(node.Method.Name.Substring(4));
				}
				else if (node.Method.IsPropertySetter())
				{
					this.builder.Append(".").Append(node.Method.Name.Substring(4)).Append(" = ");
					ToString(node.Arguments.Last());
				}
				else if (node.Method.IsGenericMethod)
				{
					this.builder
						.Append(".")
						.Append(node.Method.Name)
						.Append("<")
						.Append(string.Join(",", node.Method.GetGenericArguments().Select(s => this.getTypeName(s)).ToArray()))
						.Append(">(");
					AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
					this.builder.Append(")");
				}
				else
				{
					this.builder
						.Append(".")
						.Append(node.Method.Name)
						.Append("(");
					AsCommaSeparatedValues(node.Arguments.Skip(paramFrom), ToString);
					this.builder.Append(")");
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
				builder.Append("(");
				AsCommaSeparatedValues(lambda.Parameters, ToStringParameter);
				builder.Append(")");
			}
			builder.Append(" => ");
			ToString(lambda.Body);
			return;
		}

		private void ToStringNew(NewExpression nex)
		{
			Type type = (nex.Constructor == null) ? nex.Type : nex.Constructor.DeclaringType;
			builder.Append("new ");
			builder.Append(type.DisplayName(this.getTypeName));
			builder.Append("(");
			AsCommaSeparatedValues(nex.Arguments, ToString);
			builder.Append(")");
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
					builder.Append(na.Type.GetElementType().DisplayName(this.getTypeName));
					builder.Append("[");
					AsCommaSeparatedValues(na.Expressions, ToString);
					builder.Append("]");
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
			ToStringExpressionList(iv.Arguments);
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

	internal static class StringExtensions
	{
		public static string[] Lines(this string source)
		{
			return source.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
		}

		public static string AsCommaSeparatedValues(this IEnumerable<string> source)
		{
			if (source == null)
			{
				return string.Empty;
			}
			var result = new StringBuilder(100);
			bool appendComma = false;
			foreach (var value in source)
			{
				if (appendComma)
				{
					result.Append(", ");
				}
				result.Append(value);
				appendComma = true;
			}
			return result.ToString();
		}
	}

	internal static class TypeExtensions
	{
		public static string DisplayName(this Type source, Func<Type, string> getName)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			var builder = new StringBuilder(100);
			builder.Append(getName(source).Split('`').First());
			if (source.IsGenericType)
			{
				builder.Append("<");
				builder.Append(source.GetGenericArguments().Select(t => getName(t)).AsCommaSeparatedValues());
				builder.Append(">");
			}
			return builder.ToString();
		}
	}

	//    internal class ExpressionStringBuilder : ExpressionVisitor
	//    {
	//        private StringBuilder output = new StringBuilder();
	//        private Func<MethodInfo, string> getMethodName;

	//        private ExpressionStringBuilder(Func<MethodInfo, string> getMethodName)
	//        {
	//            this.getMethodName = getMethodName;
	//        }

	//        internal static string GetString(Expression expression)
	//        {
	//            return GetString(expression, method => method.GetName());
	//        }

	//        internal static string GetString(Expression expression, Func<MethodInfo, string> getMethodName)
	//        {
	//            var builder = new ExpressionStringBuilder(getMethodName);
	//            builder.Visit(expression);
	//            return builder.output.ToString();
	//        }

	//        protected override Expression VisitBinary(BinaryExpression node)
	//        {
	//            if (node != null)
	//            {
	//                if (node.NodeType == ExpressionType.ArrayIndex)
	//                {
	//                    this.Visit(node.Left);
	//                    this.output.Append("[");
	//                    this.Visit(node.Right);
	//                    this.output.Append("]");
	//                }
	//                else
	//                {
	//                    var @operator = GetStringOperator(node.NodeType);
	//                    if (IsParentEnclosed(node.Left))
	//                    {
	//                        this.output.Append("(");
	//                        this.Visit(node.Left);
	//                        this.output.Append(")");
	//                    }
	//                    else
	//                    {
	//                        this.Visit(node.Left);
	//                    }

	//                    output.Append(" ").Append(@operator).Append(" ");
	//                    if (IsParentEnclosed(node.Right))
	//                    {
	//                        this.output.Append("(");
	//                        this.Visit(node.Right);
	//                        this.output.Append(")");
	//                    }
	//                    else
	//                    {
	//                        this.Visit(node.Right);
	//                    }
	//                }
	//            }

	//            return node;
	//        }

	//        protected override Expression VisitConditional(ConditionalExpression node)
	//        {
	//            if (node != null)
	//            {
	//                this.Visit(node.Test);
	//                this.Visit(node.IfTrue);
	//                this.Visit(node.IfFalse);
	//            }

	//            return node;
	//        }

	//        protected override Expression VisitConstant(ConstantExpression node)
	//        {
	//            if (node != null)
	//            {
	//                var value = node.Value;
	//                if (value != null)
	//                {
	//                    if (value is string)
	//                    {
	//                        this.output.Append('"').Append(value).Append('"');
	//                    }
	//                    else if (node.Type.IsEnum)
	//                    {
	//                        this.output.Append(node.Type.Name).Append(".").Append(value);
	//                    }
	//                    else if (value.ToString() != value.GetType().ToString())
	//                    {
	//                        this.output.Append(value);
	//                    }
	//                }
	//                else
	//                {
	//                    this.output.Append("null");
	//                }
	//            }

	//            return node;
	//        }

	//        protected override ElementInit VisitElementInit(ElementInit node)
	//        {
	//            if (node != null)
	//            {
	//                this.Visit(node.Arguments);
	//            }

	//            return node;
	//        }

	//        protected override Expression VisitInvocation(InvocationExpression node)
	//        {
	//            if (node != null)
	//            {
	//                this.Visit(node.Arguments);
	//            }

	//            return node;
	//        }

	//#if NET3x
	//        protected override Expression VisitLambda(LambdaExpression node)
	//#else
	//        protected override Expression VisitLambda<T>(Expression<T> node)
	//#endif
	//        {
	//            if (node != null)
	//            {
	//                if (node.Parameters.Count == 1)
	//                {
	//                    this.VisitParameter(node.Parameters[0]);
	//                }
	//                else
	//                {
	//                    this.output.Append("(");
	//                    this.Visit(node.Parameters, n => this.VisitParameter(n), 0, node.Parameters.Count);
	//                    this.output.Append(")");
	//                }

	//                output.Append(" => ");
	//                this.Visit(node.Body);
	//            }

	//            return node;
	//        }

	//        protected override Expression VisitListInit(ListInitExpression node)
	//        {
	//            if (node != null)
	//            {
	//                this.VisitNew(node.NewExpression);
	//                Visit(node.Initializers, n => this.VisitElementInit(n));
	//            }

	//            return node;
	//        }

	//        protected override Expression VisitMember(MemberExpression node)
	//        {
	//            if (node != null)
	//            {
	//                if (node.Expression != null)
	//                {
	//                    this.Visit(node.Expression);
	//                }
	//                else
	//                {
	//                    this.output.Append(node.Member.DeclaringType.Name);
	//                }

	//                this.output.Append(".").Append(node.Member.Name);
	//            }

	//            return node;
	//        }

	//        protected override Expression VisitMemberInit(MemberInitExpression node)
	//        {
	//            if (node != null)
	//            {
	//                this.VisitNew(node.NewExpression);
	//                Visit(node.Bindings, n => this.VisitMemberBinding(n));
	//            }

	//            return node;
	//        }

	//        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
	//        {
	//            if (node != null)
	//            {
	//                Visit(node.Initializers, n => this.VisitElementInit(n));
	//            }

	//            return node;
	//        }

	//        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
	//        {
	//            if (node != null)
	//            {
	//                this.Visit(node.Expression);
	//            }

	//            return node;
	//        }

	//        protected override MemberBinding VisitMemberBinding(MemberBinding node)
	//        {
	//            if (node != null)
	//            {
	//                switch (node.BindingType)
	//                {
	//                    case MemberBindingType.Assignment:
	//                        this.VisitMemberAssignment((MemberAssignment)node);
	//                        break;

	//                    case MemberBindingType.MemberBinding:
	//                        this.VisitMemberMemberBinding((MemberMemberBinding)node);
	//                        break;

	//                    case MemberBindingType.ListBinding:
	//                        this.VisitMemberListBinding((MemberListBinding)node);
	//                        break;

	//                    default:
	//                        throw new InvalidOperationException(string.Format(
	//                            CultureInfo.CurrentCulture,
	//                            "Unhandled binding type '{0}'",
	//                            node.BindingType));
	//                }
	//            }

	//            return node;
	//        }

	//        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
	//        {
	//            if (node != null)
	//            {
	//                Visit(node.Bindings, n => this.VisitMemberBinding(n));
	//            }

	//            return node;
	//        }

	//        protected override Expression VisitMethodCall(MethodCallExpression node)
	//        {
	//            if (node != null)
	//            {
	//                var paramFrom = 0;
	//                var expression = node.Object;

	//                if (Attribute.GetCustomAttribute(node.Method, typeof(ExtensionAttribute)) != null)
	//                {
	//                    paramFrom = 1;
	//                    expression = node.Arguments[0];
	//                }

	//                if (expression != null)
	//                {
	//                    this.Visit(expression);
	//                }
	//                else // Method is static
	//                {
	//                    this.output.Append(node.Method.DeclaringType.Name);
	//                }

	//                if (node.Method.IsPropertyIndexerGetter())
	//                {
	//                    this.output.Append("[");
	//                    this.Visit(node.Arguments, n => this.Visit(n), paramFrom, node.Arguments.Count);
	//                    this.output.Append("]");
	//                }
	//                else if (node.Method.IsPropertyIndexerSetter())
	//                {
	//                    this.output.Append("[");
	//                    this.Visit(node.Arguments, n => this.Visit(n), paramFrom, node.Arguments.Count - 1);
	//                    this.output.Append("] = ");
	//                    this.Visit(node.Arguments.Last());
	//                }
	//                else if (node.Method.IsPropertyGetter())
	//                {
	//                    this.output.Append(".").Append(node.Method.Name.Substring(4));
	//                }
	//                else if (node.Method.IsPropertySetter())
	//                {
	//                    this.output.Append(".").Append(node.Method.Name.Substring(4)).Append(" = ");
	//                    this.Visit(node.Arguments.Last());
	//                }
	//                else if (node.Method.IsGenericMethod)
	//                {
	//                    this.output
	//                        .Append(".")
	//                        .Append(this.getMethodName(node.Method))
	//                        .Append("<")
	//                        .Append(string.Join(",", node.Method.GetGenericArguments().Select(s => s.Name)))
	//                        .Append(">(");
	//                    this.Visit(node.Arguments, n => this.Visit(n), paramFrom, node.Arguments.Count);
	//                    output.Append(")");
	//                }
	//                else
	//                {
	//                    this.output.Append(".").Append(this.getMethodName(node.Method)).Append("(");
	//                    this.Visit(node.Arguments, n => this.Visit(n), paramFrom, node.Arguments.Count);
	//                    output.Append(")");
	//                }
	//            }

	//            return node;
	//        }

	//        protected override Expression VisitNew(NewExpression node)
	//        {
	//            if (node != null)
	//            {
	//                this.Visit(node.Arguments);
	//            }

	//            return node;
	//        }

	//        protected override Expression VisitNewArray(NewArrayExpression node)
	//        {
	//            if (node != null)
	//            {
	//                this.output.Append("new[] { ");
	//                this.Visit(node.Expressions);
	//                this.output.Append(" }");
	//            }

	//            return node;
	//        }

	//        protected override Expression VisitParameter(ParameterExpression node)
	//        {
	//            if (node != null)
	//            {
	//                this.output.Append(node.Name != null ? node.Name : "<param>");
	//            }

	//            return node;
	//        }

	//        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
	//        {
	//            return node != null ? this.Visit(node.Expression) : node;
	//        }

	//        protected override Expression VisitUnary(UnaryExpression node)
	//        {
	//            if (node != null)
	//            {
	//                switch (node.NodeType)
	//                {
	//                    case ExpressionType.Negate:
	//                    case ExpressionType.NegateChecked:
	//                        this.output.Append("-");
	//                        this.Visit(node.Operand);
	//                        break;

	//                    case ExpressionType.Not:
	//                        this.output.Append("!(");
	//                        this.Visit(node.Operand);
	//                        this.output.Append(")");
	//                        break;

	//                    case ExpressionType.Quote:
	//                        this.Visit(node.Operand);
	//                        break;

	//                    case ExpressionType.TypeAs:
	//                        this.output.Append("(");
	//                        this.Visit(node.Operand);
	//                        this.output.Append(" as ").Append(node.Type.Name).Append(")");
	//                        break;
	//                }
	//            }

	//            return node;
	//        }

	//        private static string GetStringOperator(ExpressionType nodeType)
	//        {
	//            switch (nodeType)
	//            {
	//                case ExpressionType.Add:
	//                case ExpressionType.AddChecked:
	//                    return "+";

	//                case ExpressionType.And:
	//                    return "&";

	//                case ExpressionType.AndAlso:
	//                    return "&&";

	//                case ExpressionType.Coalesce:
	//                    return "??";

	//                case ExpressionType.Divide:
	//                    return "/";

	//                case ExpressionType.Equal:
	//                    return "==";

	//                case ExpressionType.ExclusiveOr:
	//                    return "^";

	//                case ExpressionType.GreaterThan:
	//                    return ">";

	//                case ExpressionType.GreaterThanOrEqual:
	//                    return ">=";

	//                case ExpressionType.LeftShift:
	//                    return "<<";

	//                case ExpressionType.LessThan:
	//                    return "<";

	//                case ExpressionType.LessThanOrEqual:
	//                    return "<=";

	//                case ExpressionType.Modulo:
	//                    return "%";

	//                case ExpressionType.Multiply:
	//                case ExpressionType.MultiplyChecked:
	//                    return "*";

	//                case ExpressionType.NotEqual:
	//                    return "!=";

	//                case ExpressionType.Or:
	//                    return "|";

	//                case ExpressionType.OrElse:
	//                    return "||";

	//                case ExpressionType.Power:
	//                    return "^";

	//                case ExpressionType.RightShift:
	//                    return ">>";

	//                case ExpressionType.Subtract:
	//                case ExpressionType.SubtractChecked:
	//                    return "-";
	//            }

	//            return nodeType.ToString();
	//        }

	//        private static bool IsParentEnclosed(Expression node)
	//        {
	//            return node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse;
	//        }

	//        private void Visit<T>(IList<T> arguments, Func<T, Expression> elementVisitor, int paramFrom, int paramTo)
	//        {
	//            var appendComma = false;
	//            for (var index = paramFrom; index < paramTo; index++)
	//            {
	//                if (appendComma)
	//                {
	//                    this.output.Append(", ");
	//                }

	//                elementVisitor(arguments[index]);
	//                appendComma = true;
	//            }
	//        }
	//    }
}