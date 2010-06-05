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
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace Moq.Linq
{
	internal class FluentMockVisitor : ExpressionVisitor
	{
		private static readonly MethodInfo fluentMockGenericMethod = ((Func<Mock<string>, Expression<Func<string, string>>, Mock<string>>)
			QueryableMockExtensions.FluentMock<string, string>).Method.GetGenericMethodDefinition();

		private static readonly MethodInfo mockGetGenericMethod = ((Func<string, Mock<string>>)Mock.Get<string>)
			.Method.GetGenericMethodDefinition();

		private Expression expression;

		/// <summary>
		/// The first method call or member access will be the 
		/// last segment of the expression (depth-first traversal), 
		/// which is the one we have to Setup rather than FluentMock.
		/// And the last one is the one we have to Mock.Get rather 
		/// than FluentMock.
		/// </summary>
		private bool isFirst = true;

		public FluentMockVisitor(Expression expression)
		{
			this.expression = expression;
		}

		public static Expression Accept(Expression expression)
		{
			return new FluentMockVisitor(expression).Accept();
		}

		public Expression Accept()
		{
			return this.Visit(expression);
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (node == null)
			{
				return null;
			}

			// the actual first object being used in a fluent expression.
			return Expression.Call(null, mockGetGenericMethod.MakeGenericMethod(node.Type), node);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node == null)
			{
				return null;
			}

			var lambdaParam = Expression.Parameter(node.Object.Type, "mock");
			var lambdaBody = Expression.Call(lambdaParam, node.Method, node.Arguments);
			var targetMethod = GetTargetMethod(node.Object.Type, node.Method.ReturnType);
			if (isFirst)
			{
				isFirst = false;
			}

			return TranslateFluent(node.Object.Type, node.Method.ReturnType, targetMethod, Visit(node.Object), lambdaParam, lambdaBody);
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node == null)
			{
				return null;
			}

			// If member is not mock-able, actually, including being a sealed class, etc.?
			if (node.Member is FieldInfo)
			{
				throw new NotSupportedException();
			}

			// Translate differently member accesses over transparent
			// compiler-generated types as they are typically the 
			// anonymous types generated to build up the query expressions.
			if (node.Expression.NodeType == ExpressionType.Parameter &&
				node.Expression.Type.GetCustomAttribute<CompilerGeneratedAttribute>(false) != null)
			{
				var memberType = ((PropertyInfo)node.Member).PropertyType;

				// Generate a Mock.Get over the entire member access rather.
				// <anonymous_type>.foo => Mock.Get(<anonymous_type>.foo)
				return Expression.Call(null, mockGetGenericMethod.MakeGenericMethod(memberType), node);
			}

			var lambdaParam = Expression.Parameter(node.Expression.Type, "mock");
			Expression lambdaBody = Expression.MakeMemberAccess(lambdaParam, node.Member);
			var targetMethod = GetTargetMethod(node.Expression.Type, ((PropertyInfo)node.Member).PropertyType);
			if (isFirst)
			{
				isFirst = false;
			}

			return TranslateFluent(node.Expression.Type, ((PropertyInfo)node.Member).PropertyType, targetMethod, Visit(node.Expression), lambdaParam, lambdaBody);
		}

		// Args like: string IFoo (mock => mock.Value)
		private static Expression TranslateFluent(
			Type objectType,
			Type returnType,
			MethodInfo targetMethod,
			Expression instance,
			ParameterExpression lambdaParam,
			Expression lambdaBody)
		{
			var funcType = typeof(Func<,>).MakeGenericType(objectType, returnType);

			// This is the fluent extension method one, so pass the instance as one more arg.
			if (targetMethod.IsStatic)
			{
				return Expression.Call(targetMethod, instance, Expression.Lambda(funcType, lambdaBody, lambdaParam));
			}

			return Expression.Call(instance, targetMethod, Expression.Lambda(funcType, lambdaBody, lambdaParam));
		}

		private MethodInfo GetTargetMethod(Type objectType, Type returnType)
		{
			MethodInfo targetMethod;
			// dte.Solution =>
			if (isFirst)
			{
				//.Setup(mock => mock.Solution)
				targetMethod = GetSetupMethod(objectType, returnType);
			}
			else
			{
				//.FluentMock(mock => mock.Solution)
				targetMethod = fluentMockGenericMethod.MakeGenericMethod(objectType, returnType);
			}

			return targetMethod;
		}

		private static MethodInfo GetSetupMethod(Type objectType, Type returnType)
		{
			return typeof(Mock<>)
				.MakeGenericType(objectType)
				.GetMethods()
				.First(mi => mi.Name == "Setup" && mi.IsGenericMethod)
				.MakeGenericMethod(returnType);
		}
	}
}