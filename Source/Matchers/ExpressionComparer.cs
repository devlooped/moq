//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
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

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
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
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;

namespace Moq.Matchers
{
	internal class ExpressionComparer : IEqualityComparer<Expression>
	{
		public static readonly ExpressionComparer Default = new ExpressionComparer();

		private ExpressionComparer()
		{
		}

		public bool Equals(Expression x, Expression y)
		{
			if (x == null && y == null)
			{
				return true;
			}

			if (x == null || y == null)
			{
				return false;
			}

			if (x.NodeType == y.NodeType)
			{
				switch (x.NodeType)
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
						return this.EqualsUnary((UnaryExpression)x, (UnaryExpression)y);
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
						return this.EqualsBinary((BinaryExpression)x, (BinaryExpression)y);
					case ExpressionType.TypeIs:
						return this.EqualsTypeBinary((TypeBinaryExpression)x, (TypeBinaryExpression)y);
					case ExpressionType.Conditional:
						return this.EqualsConditional((ConditionalExpression)x, (ConditionalExpression)y);
					case ExpressionType.Constant:
						return EqualsConstant((ConstantExpression)x, (ConstantExpression)y);
					case ExpressionType.Parameter:
						return this.EqualsParameter((ParameterExpression)x, (ParameterExpression)y);
					case ExpressionType.MemberAccess:
						return this.EqualsMember((MemberExpression)x, (MemberExpression)y);
					case ExpressionType.Call:
						return this.EqualsMethodCall((MethodCallExpression)x, (MethodCallExpression)y);
					case ExpressionType.Lambda:
						return this.EqualsLambda((LambdaExpression)x, (LambdaExpression)y);
					case ExpressionType.New:
						return this.EqualsNew((NewExpression)x, (NewExpression)y);
					case ExpressionType.NewArrayInit:
					case ExpressionType.NewArrayBounds:
						return this.EqualsNewArray((NewArrayExpression)x, (NewArrayExpression)y);
					case ExpressionType.Invoke:
						return this.EqualsInvocation((InvocationExpression)x, (InvocationExpression)y);
					case ExpressionType.MemberInit:
						return this.EqualsMemberInit((MemberInitExpression)x, (MemberInitExpression)y);
					case ExpressionType.ListInit:
						return this.EqualsListInit((ListInitExpression)x, (ListInitExpression)y);
				}
			}

			return false;
		}

		public int GetHashCode(Expression obj)
		{
			return obj == null ? 0 : obj.GetHashCode();
		}

		private static bool Equals<T>(ReadOnlyCollection<T> x, ReadOnlyCollection<T> y, Func<T, T, bool> comparer)
		{
			if (x.Count != y.Count)
			{
				return false;
			}

			for (var index = 0; index < x.Count; index++)
			{
				if (!comparer(x[index], y[index]))
				{
					return false;
				}
			}

			return true;
		}

		private bool EqualsBinary(BinaryExpression x, BinaryExpression y)
		{
			return x.Method == x.Method && this.Equals(x.Left, y.Left) && this.Equals(x.Right, y.Right) &&
				this.Equals(x.Conversion, y.Conversion);
		}

		private bool EqualsConditional(ConditionalExpression x, ConditionalExpression y)
		{
			return this.Equals(x.Test, y.Test) && this.Equals(x.IfTrue, y.IfTrue) && this.Equals(x.IfFalse, y.IfFalse);
		}

		private static bool EqualsConstant(ConstantExpression x, ConstantExpression y)
		{
			return object.Equals(x.Value, y.Value);
		}

		private bool EqualsElementInit(ElementInit x, ElementInit y)
		{
			return x.AddMethod == y.AddMethod && Equals(x.Arguments, y.Arguments, this.Equals);
		}

		private bool EqualsInvocation(InvocationExpression x, InvocationExpression y)
		{
			return this.Equals(x.Expression, y.Expression) && Equals(x.Arguments, y.Arguments, this.Equals);
		}

		private bool EqualsLambda(LambdaExpression x, LambdaExpression y)
		{
			return x.GetType() == y.GetType() && this.Equals(x.Body, y.Body) &&
				Equals(x.Parameters, y.Parameters, this.EqualsParameter);
		}

		private bool EqualsListInit(ListInitExpression x, ListInitExpression y)
		{
			return this.EqualsNew(x.NewExpression, y.NewExpression) &&
				Equals(x.Initializers, y.Initializers, this.EqualsElementInit);
		}

		private bool EqualsMemberAssignment(MemberAssignment x, MemberAssignment y)
		{
			return this.Equals(x.Expression, y.Expression);
		}

		private bool EqualsMemberBinding(MemberBinding x, MemberBinding y)
		{
			if (x.BindingType == y.BindingType && x.Member == y.Member)
			{
				switch (x.BindingType)
				{
					case MemberBindingType.Assignment:
						return this.EqualsMemberAssignment((MemberAssignment)x, (MemberAssignment)y);
					case MemberBindingType.MemberBinding:
						return this.EqualsMemberMemberBinding((MemberMemberBinding)x, (MemberMemberBinding)y);
					case MemberBindingType.ListBinding:
						return this.EqualsMemberListBinding((MemberListBinding)x, (MemberListBinding)y);
				}
			}

			return false;
		}

		private bool EqualsMember(MemberExpression x, MemberExpression y)
		{
			return x.Member == y.Member && this.Equals(x.Expression, y.Expression);
		}

		private bool EqualsMemberInit(MemberInitExpression x, MemberInitExpression y)
		{
			return this.EqualsNew(x.NewExpression, y.NewExpression) &&
				Equals(x.Bindings, y.Bindings, this.EqualsMemberBinding);
		}

		private bool EqualsMemberListBinding(MemberListBinding x, MemberListBinding y)
		{
			return Equals(x.Initializers, y.Initializers, this.EqualsElementInit);
		}

		private bool EqualsMemberMemberBinding(MemberMemberBinding x, MemberMemberBinding y)
		{
			return Equals(x.Bindings, y.Bindings, this.EqualsMemberBinding);
		}

		private bool EqualsMethodCall(MethodCallExpression x, MethodCallExpression y)
		{
			return x.Method == y.Method && this.Equals(x.Object, y.Object) &&
				Equals(x.Arguments, y.Arguments, this.Equals);
		}

		private bool EqualsNewArray(NewArrayExpression x, NewArrayExpression y)
		{
			return x.Type == y.Type && Equals(x.Expressions, y.Expressions, this.Equals);
		}

		private bool EqualsNew(NewExpression x, NewExpression y)
		{
			return x.Constructor == y.Constructor && Equals(x.Arguments, y.Arguments, this.Equals);
		}

		private bool EqualsParameter(ParameterExpression x, ParameterExpression y)
		{
			return x.Type == y.Type;
		}

		private bool EqualsTypeBinary(TypeBinaryExpression x, TypeBinaryExpression y)
		{
			return x.TypeOperand == y.TypeOperand && this.Equals(x.Expression, y.Expression);
		}

		private bool EqualsUnary(UnaryExpression x, UnaryExpression y)
		{
			return x.Method == y.Method && this.Equals(x.Operand, y.Operand);
		}
	}
}