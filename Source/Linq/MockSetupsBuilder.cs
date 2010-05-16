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

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq.Language;
using System;

namespace Moq.Linq
{
	internal class MockSetupsBuilder : ExpressionVisitor
	{
		private int stackIndex;

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node.Method.DeclaringType == typeof(Queryable) &&
				(node.Method.Name == "Where" || node.Method.Name == "First"))
			{
				this.stackIndex++;
				var result = base.VisitMethodCall(node);
				this.stackIndex--;
				return result;
			}

			return base.VisitMethodCall(node);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (this.stackIndex > 0 && node.NodeType == ExpressionType.Equal)
			{
				switch (node.Left.NodeType)
				{
					case ExpressionType.MemberAccess:
						var member = (MemberExpression)node.Left;
						return ConvertToSetup(member.Expression, member, node.Right);

					case ExpressionType.Call:
						var method = (MethodCallExpression)node.Left;
						return ConvertToSetup(method.Object, method, node.Right);

					case ExpressionType.Convert:
						var left = (UnaryExpression)node.Left;

						switch (left.Operand.NodeType)
						{
							case ExpressionType.MemberAccess:
								var memberOperand = (MemberExpression)left.Operand;
								return ConvertToSetup(
									memberOperand.Expression,
									memberOperand,
									Expression.Convert(node.Right, memberOperand.Type));

							case ExpressionType.Call:
								var methodOperand = (MethodCallExpression)left.Operand;
								return ConvertToSetup(
									methodOperand.Object,
									methodOperand,
									Expression.Convert(node.Right, methodOperand.Type));
						}

						break;
				}
			}

			return base.VisitBinary(node);
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (this.stackIndex > 0 && node.Operand.NodeType == ExpressionType.MemberAccess && node.Type == typeof(bool))
			{
				var member = (MemberExpression)node.Operand;
				return ConvertToSetup(member.Expression, member, Expression.Constant(false));
			}

			return base.VisitUnary(node);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (node.Type.IsGenericType && node.Type.GetGenericTypeDefinition() == typeof(MockQueryable<>))
			{
				var targetType = node.Type.GetGenericArguments()[0];

				var createRealMethod = typeof(Mocks).GetMethod("CreateReal")
					.MakeGenericMethod(targetType);
				var createRealExpr = Expression.Call(null, createRealMethod);

				var asQueryableMethod = typeof(Queryable).GetMethods()
					.Where(mi => mi.Name == "AsQueryable" && mi.IsGenericMethodDefinition)
					.Single()
					.MakeGenericMethod(targetType);

				return Expression.Call(null, asQueryableMethod, createRealExpr);
			}

			return base.VisitConstant(node);
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (this.stackIndex > 0 && node.Type == typeof(bool))
			{
				return ConvertToSetup(node.Expression, node, Expression.Constant(true));
			}

			return base.VisitMember(node);
		}

		private static Expression ConvertToSetup(Expression targetObject, Expression left, Expression right)
		{
			// TODO: throw if target is a static class?
			var sourceType = targetObject.Type;
			var returnType = left.Type;

			// where dte.Solution == solution
			// becomes:	
			// where Mock.Get(dte).Setup(mock => mock.Solution).Returns(solution) != null

			var returnsMethod = typeof(IReturns<,>)
				.MakeGenericType(sourceType, returnType)
				.GetMethod("Returns", new[] { returnType });

			return Expression.NotEqual(
				Expression.Call(FluentMockVisitor.Accept(left), returnsMethod, right),
				Expression.Constant(null));
		}
	}
}