//Copyright (c) 2007, Moq Team 
//http://code.google.com/p/moq/
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of the Moq Team nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq.Expressions;

namespace Moq
{
	/// <summary>
	/// Base class for visitors of expression trees.
	/// </summary>
	/// <remarks>
	/// <para>Provides the functionality of the internal visitor base class that 
	/// comes with Linq.</para>
	/// <para>Matt's comments on the implementation:</para>
	/// <para>
	/// In this variant there is only one visitor class that dispatches calls to the general 
	/// Visit function out to specific VisitXXX methods corresponding to different node types.  
	/// Note not every node type gets it own method, for example all binary operators are 
	/// treated in one VisitBinary method.  The nodes themselves do not directly participate 
	/// in the visitation process. They are treated as just data. 
	/// The reason for this is that the quantity of visitors is actually open ended. 
	/// You can write your own. Therefore no semantics of visiting is coupled into the node classes.  
	/// It’s all in the visitors.  The default visit behavior for node XXX is baked into the base 
	/// class’s version of VisitXXX.
	/// </para>
	/// <para>
	/// Another variant is that all VisitXXX methods return a node. 
	/// The Expression tree nodes are immutable. In order to change the tree you must construct 
	/// a new one. The default VisitXXX methods will construct a new node if any of its sub-trees change. 
	/// If no changes are made then the same node is returned. That way if you make a change 
	/// to a node (by making a new node) deep down in a tree, the rest of the tree is rebuilt 
	/// automatically for you.
	/// </para>
	/// See: http://blogs.msdn.com/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx.
	/// </remarks>
	/// <author>Matt Warren: http://blogs.msdn.com/mattwar</author>
	/// <contributor>Documented by InSTEDD: http://www.instedd.org</contributor>
	internal abstract class ExpressionVisitor
	{
		/// <summary>
		/// Default constructor used by derived visitors.
		/// </summary>
		protected ExpressionVisitor()
		{
		}

		/// <summary>
		/// Visits the <see cref="Expression"/>, determining which 
		/// of the concrete Visit methods to call.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		protected virtual Expression Visit(Expression exp)
		{
			if (exp == null)
				return exp;

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
					return this.VisitUnary((UnaryExpression)exp);
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
					return this.VisitBinary((BinaryExpression)exp);
				case ExpressionType.TypeIs:
					return this.VisitTypeIs((TypeBinaryExpression)exp);
				case ExpressionType.Conditional:
					return this.VisitConditional((ConditionalExpression)exp);
				case ExpressionType.Constant:
					return this.VisitConstant((ConstantExpression)exp);
				case ExpressionType.Parameter:
					return this.VisitParameter((ParameterExpression)exp);
				case ExpressionType.MemberAccess:
					return this.VisitMemberAccess((MemberExpression)exp);
				case ExpressionType.Call:
					return this.VisitMethodCall((MethodCallExpression)exp);
				case ExpressionType.Lambda:
					return this.VisitLambda((LambdaExpression)exp);
				case ExpressionType.New:
					return this.VisitNew((NewExpression)exp);
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
					return this.VisitNewArray((NewArrayExpression)exp);
				case ExpressionType.Invoke:
					return this.VisitInvocation((InvocationExpression)exp);
				case ExpressionType.MemberInit:
					return this.VisitMemberInit((MemberInitExpression)exp);
				case ExpressionType.ListInit:
					return this.VisitListInit((ListInitExpression)exp);
				default:
					throw new ArgumentException(String.Format(
						CultureInfo.CurrentCulture, 
						"Unhandled expression type: '{0}'", exp.NodeType), "exp");
			}
		}

		/// <summary>
		/// Visits the generic <see cref="MemberBinding"/>, determining and 
		/// calling the appropriate Visit method according to the 
		/// <see cref="MemberBinding.BindingType"/>, which will result 
		/// in calls to <see cref="VisitMemberAssignment"/>, 
		/// <see cref="VisitMemberMemberBinding"/> or <see cref="VisitMemberListBinding"/>.
		/// </summary>
		/// <param name="binding"></param>
		/// <returns></returns>
		protected virtual MemberBinding VisitBinding(MemberBinding binding)
		{
			switch (binding.BindingType)
			{
				case MemberBindingType.Assignment:
					return this.VisitMemberAssignment((MemberAssignment)binding);
				case MemberBindingType.MemberBinding:
					return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
				case MemberBindingType.ListBinding:
					return this.VisitMemberListBinding((MemberListBinding)binding);
				default:
					throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Unhandled binding type '{0}'", binding.BindingType), "binding");
			}
		}

		/// <summary>
		/// Visits the <see cref="ElementInit"/> initializer by 
		/// calling the <see cref="VisitExpressionList"/> for the 
		/// <see cref="ElementInit.Arguments"/>.
		/// </summary>
		protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
		{
			ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
			if (arguments != initializer.Arguments)
			{
				return Expression.ElementInit(initializer.AddMethod, arguments);
			}
			return initializer;
		}

		/// <summary>
		/// Visits the <see cref="UnaryExpression"/> expression by 
		/// calling <see cref="Visit"/> with the <see cref="UnaryExpression.Operand"/> expression.
		/// </summary>
		protected virtual Expression VisitUnary(UnaryExpression u)
		{
			Expression operand = this.Visit(u.Operand);
			if (operand != u.Operand)
			{
				return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
			}
			return u;
		}

		/// <summary>
		/// Visits the <see cref="BinaryExpression"/> by calling 
		/// <see cref="Visit"/> with the <see cref="BinaryExpression.Left"/>, 
		/// <see cref="BinaryExpression.Right"/> and <see cref="BinaryExpression.Conversion"/> 
		/// expressions.
		/// </summary>
		protected virtual Expression VisitBinary(BinaryExpression b)
		{
			Expression left = this.Visit(b.Left);
			Expression right = this.Visit(b.Right);
			Expression conversion = this.Visit(b.Conversion);
			if (left != b.Left || right != b.Right || conversion != b.Conversion)
			{
				if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
					return Expression.Coalesce(left, right, conversion as LambdaExpression);
				else
					return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
			}
			return b;
		}

		/// <summary>
		/// Visits the <see cref="TypeBinaryExpression"/> by calling 
		/// <see cref="Visit"/> with the <see cref="TypeBinaryExpression.Expression"/> 
		/// expression.
		/// </summary>
		protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
		{
			Expression expr = this.Visit(b.Expression);
			if (expr != b.Expression)
			{
				return Expression.TypeIs(expr, b.TypeOperand);
			}
			return b;
		}

		/// <summary>
		/// Visits the <see cref="ConstantExpression"/>, by default returning the
		/// same <see cref="ConstantExpression"/> without further behavior.
		/// </summary>
		protected virtual Expression VisitConstant(ConstantExpression c)
		{
			return c;
		}

		/// <summary>
		/// Visits the <see cref="ConditionalExpression"/> by calling 
		/// <see cref="Visit"/> with the <see cref="ConditionalExpression.Test"/>, 
		/// <see cref="ConditionalExpression.IfTrue"/> and <see cref="ConditionalExpression.IfFalse"/> 
		/// expressions.
		/// </summary>
		protected virtual Expression VisitConditional(ConditionalExpression c)
		{
			Expression test = this.Visit(c.Test);
			Expression ifTrue = this.Visit(c.IfTrue);
			Expression ifFalse = this.Visit(c.IfFalse);
			if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
			{
				return Expression.Condition(test, ifTrue, ifFalse);
			}
			return c;
		}

		/// <summary>
		/// Visits the <see cref="ParameterExpression"/> returning it 
		/// by default without further behavior.
		/// </summary>
		protected virtual Expression VisitParameter(ParameterExpression p)
		{
			return p;
		}

		/// <summary>
		/// Visits the <see cref="MemberExpression"/> by calling 
		/// <see cref="Visit"/> with the <see cref="MemberExpression.Expression"/> 
		/// expression.
		/// </summary>
		protected virtual Expression VisitMemberAccess(MemberExpression m)
		{
			Expression exp = this.Visit(m.Expression);
			if (exp != m.Expression)
			{
				return Expression.MakeMemberAccess(exp, m.Member);
			}
			return m;
		}

		/// <summary>
		/// Visits the <see cref="MethodCallExpression"/> by calling 
		/// <see cref="Visit"/> with the <see cref="MethodCallExpression.Object"/> expression, 
		/// and then <see cref="VisitExpressionList"/> with the <see cref="MethodCallExpression.Arguments"/>.
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		protected virtual Expression VisitMethodCall(MethodCallExpression m)
		{
			Expression obj = this.Visit(m.Object);
			IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
			if (obj != m.Object || args != m.Arguments)
			{
				return Expression.Call(obj, m.Method, args);
			}
			return m;
		}

		/// <summary>
		/// Visits the <see cref="ReadOnlyCollection{Expression}"/> by iterating 
		/// the list and visiting each <see cref="Expression"/> in it.
		/// </summary>
		/// <param name="original"></param>
		/// <returns></returns>
		protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
		{
			List<Expression> list = null;
			for (int i = 0, n = original.Count; i < n; i++)
			{
				Expression p = this.Visit(original[i]);
				if (list != null)
				{
					list.Add(p);
				}
				else if (p != original[i])
				{
					list = new List<Expression>(n);
					for (int j = 0; j < i; j++)
					{
						list.Add(original[j]);
					}
					list.Add(p);
				}
			}
			if (list != null)
			{
				return list.AsReadOnly();
			}
			return original;
		}

		/// <summary>
		/// Visits the <see cref="MemberAssignment"/> by calling 
		/// <see cref="Visit"/> with the <see cref="MemberAssignment.Expression"/> expression.
		/// </summary>
		/// <param name="assignment"></param>
		/// <returns></returns>
		protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
		{
			Expression e = this.Visit(assignment.Expression);
			if (e != assignment.Expression)
			{
				return Expression.Bind(assignment.Member, e);
			}
			return assignment;
		}
		
		/// <summary>
		/// Visits the <see cref="MemberMemberBinding"/> by calling 
		/// <see cref="VisitBindingList"/> with the <see cref="MemberMemberBinding.Bindings"/>.
		/// </summary>
		/// <param name="binding"></param>
		/// <returns></returns>
		protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
		{
			IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
			if (bindings != binding.Bindings)
			{
				return Expression.MemberBind(binding.Member, bindings);
			}
			return binding;
		}
		
		/// <summary>
		/// Visits the <see cref="MemberListBinding"/> by calling 
		/// <see cref="VisitElementInitializerList"/> with the 
		/// <see cref="MemberListBinding.Initializers"/>.
		/// </summary>
		/// <param name="binding"></param>
		/// <returns></returns>
		protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
		{
			IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
			if (initializers != binding.Initializers)
			{
				return Expression.ListBind(binding.Member, initializers);
			}
			return binding;
		}
		
		/// <summary>
		/// Visits the <see cref="ReadOnlyCollection{MemberBinding}"/> by 
		/// calling <see cref="VisitBinding"/> for each <see cref="MemberBinding"/> in the 
		/// collection.
		/// </summary>
		/// <param name="original"></param>
		/// <returns></returns>
		protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
		{
			List<MemberBinding> list = null;
			for (int i = 0, n = original.Count; i < n; i++)
			{
				MemberBinding b = this.VisitBinding(original[i]);
				if (list != null)
				{
					list.Add(b);
				}
				else if (b != original[i])
				{
					list = new List<MemberBinding>(n);
					for (int j = 0; j < i; j++)
					{
						list.Add(original[j]);
					}
					list.Add(b);
				}
			}
			if (list != null)
				return list;
			return original;
		}
		
		/// <summary>
		/// Visits the <see cref="ReadOnlyCollection{ElementInit}"/> by 
		/// calling <see cref="VisitElementInitializer"/> for each 
		/// <see cref="ElementInit"/> in the collection.
		/// </summary>
		/// <param name="original"></param>
		/// <returns></returns>
		protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
		{
			List<ElementInit> list = null;
			for (int i = 0, n = original.Count; i < n; i++)
			{
				ElementInit init = this.VisitElementInitializer(original[i]);
				if (list != null)
				{
					list.Add(init);
				}
				else if (init != original[i])
				{
					list = new List<ElementInit>(n);
					for (int j = 0; j < i; j++)
					{
						list.Add(original[j]);
					}
					list.Add(init);
				}
			}
			if (list != null)
				return list;
			return original;
		}
		
		/// <summary>
		/// Visits the <see cref="LambdaExpression"/> by calling 
		/// <see cref="Visit"/> with the <see cref="LambdaExpression.Body"/> expression.
		/// </summary>
		/// <param name="lambda"></param>
		/// <returns></returns>
		protected virtual Expression VisitLambda(LambdaExpression lambda)
		{
			Expression body = this.Visit(lambda.Body);
			if (body != lambda.Body)
			{
				return Expression.Lambda(lambda.Type, body, lambda.Parameters);
			}
			return lambda;
		}
		
		/// <summary>
		/// Visits the <see cref="NewExpression"/> by calling 
		/// <see cref="VisitExpressionList"/> with the <see cref="NewExpression.Arguments"/> 
		/// expressions.
		/// </summary>
		/// <param name="nex"></param>
		/// <returns></returns>
		protected virtual NewExpression VisitNew(NewExpression nex)
		{
			IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
			if (args != nex.Arguments)
			{
				if (nex.Members != null)
					return Expression.New(nex.Constructor, args, nex.Members);
				else
					return Expression.New(nex.Constructor, args);
			}
			return nex;
		}
		
		/// <summary>
		/// Visits the <see cref="MemberInitExpression"/> by calling 
		/// <see cref="VisitNew"/> with the <see cref="MemberInitExpression.NewExpression"/> 
		/// expression, then <see cref="VisitBindingList"/> with the 
		/// <see cref="MemberInitExpression.Bindings"/>.
		/// </summary>
		protected virtual Expression VisitMemberInit(MemberInitExpression init)
		{
			NewExpression n = this.VisitNew(init.NewExpression);
			IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
			if (n != init.NewExpression || bindings != init.Bindings)
			{
				return Expression.MemberInit(n, bindings);
			}
			return init;
		}

		/// <summary>
		/// Visits the <see cref="ListInitExpression"/> by calling 
		/// <see cref="VisitNew"/> with the <see cref="ListInitExpression.NewExpression"/> 
		/// expression, and then <see cref="VisitElementInitializerList"/> with the 
		/// <see cref="ListInitExpression.Initializers"/>.
		/// </summary>
		/// <param name="init"></param>
		/// <returns></returns>
		protected virtual Expression VisitListInit(ListInitExpression init)
		{
			NewExpression n = this.VisitNew(init.NewExpression);
			IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
			if (n != init.NewExpression || initializers != init.Initializers)
			{
				return Expression.ListInit(n, initializers);
			}
			return init;
		}
		
		/// <summary>
		/// Visits the <see cref="NewArrayExpression"/> by calling 
		/// <see cref="VisitExpressionList"/> with the <see cref="NewArrayExpression.Expressions"/> 
		/// expressions.
		/// </summary>
		/// <param name="na"></param>
		/// <returns></returns>
		protected virtual Expression VisitNewArray(NewArrayExpression na)
		{
			IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
			if (exprs != na.Expressions)
			{
				if (na.NodeType == ExpressionType.NewArrayInit)
				{
					return Expression.NewArrayInit(na.Type.GetElementType(), exprs);
				}
				else
				{
					return Expression.NewArrayBounds(na.Type.GetElementType(), exprs);
				}
			}
			return na;
		}
		
		/// <summary>
		/// Visits the <see cref="InvocationExpression"/> by calling 
		/// <see cref="VisitExpressionList"/> with the <see cref="InvocationExpression.Arguments"/> 
		/// expressions.
		/// </summary>
		/// <param name="iv"></param>
		/// <returns></returns>
		protected virtual Expression VisitInvocation(InvocationExpression iv)
		{
			IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
			Expression expr = this.Visit(iv.Expression);
			if (args != iv.Arguments || expr != iv.Expression)
			{
				return Expression.Invoke(expr, args);
			}
			return iv;
		}
	}
}
