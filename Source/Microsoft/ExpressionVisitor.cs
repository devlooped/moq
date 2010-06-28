// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)
#if NET3x
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace System.Linq.Expressions
{
	[DebuggerStepThrough]
	internal abstract class ExpressionVisitor
	{
		protected ExpressionVisitor()
		{
		}

		public static ReadOnlyCollection<T> Visit<T>(ReadOnlyCollection<T> nodes, Func<T, T> elementVisitor)
		{
			List<T> list = null;
			for (var index = 0; index < nodes.Count; index++)
			{
				var expression = elementVisitor(nodes[index]);
				if (list != null)
				{
					list.Add(expression);
				}
				else if (!object.ReferenceEquals(expression, nodes[index]))
				{
					list = new List<T>(nodes.Count);
					for (var j = 0; j < index; j++)
					{
						list.Add(nodes[j]);
					}

					list.Add(expression);
				}
			}

			return list != null ? list.AsReadOnly() : nodes;
		}

		public virtual Expression Visit(Expression node)
		{
			if (node == null)
			{
				return node;
			}

			switch (node.NodeType)
			{
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
				case ExpressionType.Not:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.ArrayLength:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
				case ExpressionType.UnaryPlus:
					return this.VisitUnary((UnaryExpression)node);
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
				case ExpressionType.Power:
					return this.VisitBinary((BinaryExpression)node);
				case ExpressionType.TypeIs:
					return this.VisitTypeBinary((TypeBinaryExpression)node);
				case ExpressionType.Conditional:
					return this.VisitConditional((ConditionalExpression)node);
				case ExpressionType.Constant:
					return this.VisitConstant((ConstantExpression)node);
				case ExpressionType.Parameter:
					return this.VisitParameter((ParameterExpression)node);
				case ExpressionType.MemberAccess:
					return this.VisitMember((MemberExpression)node);
				case ExpressionType.Call:
					return this.VisitMethodCall((MethodCallExpression)node);
				case ExpressionType.Lambda:
					return this.VisitLambda((LambdaExpression)node);
				case ExpressionType.New:
					return this.VisitNew((NewExpression)node);
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
					return this.VisitNewArray((NewArrayExpression)node);
				case ExpressionType.Invoke:
					return this.VisitInvocation((InvocationExpression)node);
				case ExpressionType.MemberInit:
					return this.VisitMemberInit((MemberInitExpression)node);
				case ExpressionType.ListInit:
					return this.VisitListInit((ListInitExpression)node);
			}

			return VisitUnknown(node);
		}

		public ReadOnlyCollection<Expression> Visit(ReadOnlyCollection<Expression> nodes)
		{
			if (nodes != null)
			{
				List<Expression> list = null;
				for (var index = 0; index < nodes.Count; index++)
				{
					var node = this.Visit(nodes[index]);
					if (list != null)
					{
						list.Add(node);
					}
					else if (node != nodes[index])
					{
						list = new List<Expression>(nodes.Count);
						for (int j = 0; j < index; j++)
						{
							list.Add(nodes[j]);
						}

						list.Add(node);
					}
				}

				if (list != null)
				{
					return list.AsReadOnly();
				}
			}

			return nodes;
		}

		protected virtual Expression VisitBinary(BinaryExpression node)
		{
			Expression left = this.Visit(node.Left);
			Expression right = this.Visit(node.Right);
			Expression conversion = this.Visit(node.Conversion);
			return UpdateBinary(node, left, right, conversion, node.IsLiftedToNull, node.Method);
		}

		protected virtual Expression VisitConditional(ConditionalExpression node)
		{
			var test = this.Visit(node.Test);
			var ifTrue = this.Visit(node.IfTrue);
			var ifFalse = this.Visit(node.IfFalse);
			return UpdateConditional(node, test, ifTrue, ifFalse);
		}

		protected virtual Expression VisitConstant(ConstantExpression node)
		{
			return node;
		}

		protected virtual ElementInit VisitElementInit(ElementInit node)
		{
			var arguments = this.Visit(node.Arguments);
			if (arguments != node.Arguments)
			{
				return Expression.ElementInit(node.AddMethod, arguments);
			}

			return node;
		}

		protected virtual Expression VisitInvocation(InvocationExpression node)
		{
			var args = this.Visit(node.Arguments);
			var expression = this.Visit(node.Expression);
			return UpdateInvocation(node, expression, args);
		}

		protected virtual Expression VisitLambda(LambdaExpression node)
		{
			var body = this.Visit(node.Body);
			return UpdateLambda(node, node.Type, body, node.Parameters);
		}

		protected virtual Expression VisitListInit(ListInitExpression node)
		{
			var newExpression = (NewExpression)this.VisitNew(node.NewExpression);
			var initializers = Visit<ElementInit>(node.Initializers, n => this.VisitElementInit(n));
			return UpdateListInit(node, newExpression, initializers);
		}

		protected virtual Expression VisitMember(MemberExpression node)
		{
			var expression = this.Visit(node.Expression);
			return UpdateMember(node, expression, node.Member);
		}

		protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment node)
		{
			var expression = this.Visit(node.Expression);
			return UpdateMemberAssignment(node, node.Member, expression);
		}

		protected virtual MemberBinding VisitMemberBinding(MemberBinding node)
		{
			switch (node.BindingType)
			{
				case MemberBindingType.Assignment:
					return this.VisitMemberAssignment((MemberAssignment)node);
				case MemberBindingType.MemberBinding:
					return this.VisitMemberMemberBinding((MemberMemberBinding)node);
				case MemberBindingType.ListBinding:
					return this.VisitMemberListBinding((MemberListBinding)node);
			}

			throw new InvalidOperationException(string.Format(
				CultureInfo.CurrentCulture,
				"Unhandled binding type '{0}'",
				node.BindingType));
		}

		protected virtual Expression VisitMemberInit(MemberInitExpression node)
		{
			var newExpression = (NewExpression)this.VisitNew(node.NewExpression);
			var bindings = Visit<MemberBinding>(node.Bindings, n => this.VisitMemberBinding(n));
			return UpdateMemberInit(node, newExpression, bindings);
		}

		protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding node)
		{
			var initializers = Visit<ElementInit>(node.Initializers, n => this.VisitElementInit(n));
			return UpdateMemberListBinding(node, node.Member, initializers);
		}

		protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
		{
			var bindings = Visit<MemberBinding>(node.Bindings, n => this.VisitMemberBinding(n));
			return UpdateMemberMemberBinding(node, node.Member, bindings);
		}

		protected virtual Expression VisitMethodCall(MethodCallExpression node)
		{
			var obj = this.Visit(node.Object);
			var args = this.Visit(node.Arguments);
			return UpdateMethodCall(node, obj, node.Method, args);
		}

		protected virtual Expression VisitNew(NewExpression node)
		{
			var args = this.Visit(node.Arguments);
			return UpdateNew(node, node.Constructor, args, node.Members);
		}

		protected virtual Expression VisitNewArray(NewArrayExpression node)
		{
			var expressions = this.Visit(node.Expressions);
			return UpdateNewArray(node, node.Type, expressions);
		}

		protected virtual Expression VisitParameter(ParameterExpression node)
		{
			return node;
		}

		protected virtual Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			var expression = this.Visit(node.Expression);
			return UpdateTypeBinary(node, expression, node.TypeOperand);
		}

		protected virtual Expression VisitUnary(UnaryExpression node)
		{
			var operand = this.Visit(node.Operand);
			return UpdateUnary(node, operand, node.Type, node.Method);
		}

		private static BinaryExpression UpdateBinary(
			BinaryExpression node,
			Expression left,
			Expression right,
			Expression conversion,
			bool isLiftedToNull,
			MethodInfo method)
		{
			if (left != node.Left || right != node.Right || conversion != node.Conversion ||
				method != node.Method || isLiftedToNull != node.IsLiftedToNull)
			{
				if (node.NodeType == ExpressionType.Coalesce && node.Conversion != null)
				{
					return Expression.Coalesce(left, right, conversion as LambdaExpression);
				}

				return Expression.MakeBinary(node.NodeType, left, right, isLiftedToNull, method);
			}

			return node;
		}

		private static ConditionalExpression UpdateConditional(
			ConditionalExpression node,
			Expression test,
			Expression ifTrue,
			Expression ifFalse)
		{
			if (test != node.Test || ifTrue != node.IfTrue || ifFalse != node.IfFalse)
			{
				return Expression.Condition(test, ifTrue, ifFalse);
			}

			return node;
		}

		private static InvocationExpression UpdateInvocation(
			InvocationExpression node,
			Expression expression,
			IEnumerable<Expression> args)
		{
			if (args != node.Arguments || expression != node.Expression)
			{
				return Expression.Invoke(expression, args);
			}

			return node;
		}

		private static LambdaExpression UpdateLambda(
			LambdaExpression node,
			Type delegateType,
			Expression body,
			IEnumerable<ParameterExpression> parameters)
		{
			if (body != node.Body || parameters != node.Parameters || delegateType != node.Type)
			{
				return Expression.Lambda(delegateType, body, parameters);
			}

			return node;
		}

		private static ListInitExpression UpdateListInit(
			ListInitExpression node,
			NewExpression newExpression,
			IEnumerable<ElementInit> initializers)
		{
			if (newExpression != node.NewExpression || initializers != node.Initializers)
			{
				return Expression.ListInit(newExpression, initializers);
			}

			return node;
		}

		private static MemberExpression UpdateMember(MemberExpression node, Expression expression, MemberInfo member)
		{
			if (expression != node.Expression || member != node.Member)
			{
				return Expression.MakeMemberAccess(expression, member);
			}

			return node;
		}

		private static MemberAssignment UpdateMemberAssignment(MemberAssignment node, MemberInfo member, Expression expression)
		{
			if (expression != node.Expression || member != node.Member)
			{
				return Expression.Bind(member, expression);
			}

			return node;
		}

		private static MemberInitExpression UpdateMemberInit(
			MemberInitExpression node,
			NewExpression newExpression,
			IEnumerable<MemberBinding> bindings)
		{
			if (newExpression != node.NewExpression || bindings != node.Bindings)
			{
				return Expression.MemberInit(newExpression, bindings);
			}

			return node;
		}

		private static MemberListBinding UpdateMemberListBinding(
			MemberListBinding node,
			MemberInfo member,
			IEnumerable<ElementInit> initializers)
		{
			if (initializers != node.Initializers || member != node.Member)
			{
				return Expression.ListBind(member, initializers);
			}

			return node;
		}

		private static MemberMemberBinding UpdateMemberMemberBinding(
			MemberMemberBinding node,
			MemberInfo member,
			IEnumerable<MemberBinding> bindings)
		{
			if (bindings != node.Bindings || member != node.Member)
			{
				return Expression.MemberBind(member, bindings);
			}

			return node;
		}

		private static MethodCallExpression UpdateMethodCall(
			MethodCallExpression node,
			Expression obj,
			MethodInfo method,
			IEnumerable<Expression> args)
		{
			if (obj != node.Object || method != node.Method || args != node.Arguments)
			{
				return Expression.Call(obj, method, args);
			}

			return node;
		}

		private static NewExpression UpdateNew(
			NewExpression node,
			ConstructorInfo constructor,
			IEnumerable<Expression> args,
			IEnumerable<MemberInfo> members)
		{
			if (args != node.Arguments || constructor != node.Constructor || members != node.Members)
			{
				if (node.Members != null)
				{
					return Expression.New(constructor, args, members);
				}

				return Expression.New(constructor, args);
			}

			return node;
		}

		private static NewArrayExpression UpdateNewArray(
			NewArrayExpression node,
			Type arrayType,
			IEnumerable<Expression> expressions)
		{
			if (expressions != node.Expressions || node.Type != arrayType)
			{
				if (node.NodeType == ExpressionType.NewArrayInit)
				{
					return Expression.NewArrayInit(arrayType.GetElementType(), expressions);
				}

				return Expression.NewArrayBounds(arrayType.GetElementType(), expressions);
			}

			return node;
		}

		private static TypeBinaryExpression UpdateTypeBinary(TypeBinaryExpression node, Expression expression, Type typeOperand)
		{
			if (expression != node.Expression || typeOperand != node.TypeOperand)
			{
				return Expression.TypeIs(expression, typeOperand);
			}

			return node;
		}

		private static UnaryExpression UpdateUnary(UnaryExpression node, Expression operand, Type resultType, MethodInfo method)
		{
			if (node.Operand != operand || node.Type != resultType || node.Method != method)
			{
				return Expression.MakeUnary(node.NodeType, operand, resultType, method);
			}

			return node;
		}

		private static Expression VisitUnknown(Expression node)
		{
			throw new InvalidOperationException(string.Format(
				CultureInfo.CurrentCulture,
				"Unhandled expression type: '{0}'",
				node.NodeType));
		}
	}
}
#endif