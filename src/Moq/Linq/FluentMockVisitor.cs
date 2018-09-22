// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

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
	internal class FluentMockVisitor : FluentMockVisitorBase
	{
		/// <summary>
		/// The first method call or member access will be the 
		/// last segment of the expression (depth-first traversal), 
		/// which is the one we have to Setup rather than FluentMock.
		/// And the last one is the one we have to Mock.Get rather 
		/// than FluentMock.
		/// </summary>
		private bool isFirst = true;

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (node == null)
			{
				return null;
			}

			// the actual first object being used in a fluent expression.
			return Expression.Call(null, Mock.GetMethod.MakeGenericMethod(node.Type), node);
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
				node.Expression.Type.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute), false))
			{
				var memberType = ((PropertyInfo)node.Member).PropertyType;

				// Generate a Mock.Get over the entire member access rather.
				// <anonymous_type>.foo => Mock.Get(<anonymous_type>.foo)
				return Expression.Call(null, Mock.GetMethod.MakeGenericMethod(memberType), node);
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
				targetMethod = QueryableMockExtensions.FluentMockMethod.MakeGenericMethod(objectType, returnType);
			}

			return targetMethod;
		}

		private static MethodInfo GetSetupMethod(Type objectType, Type returnType)
		{
			return typeof(Mock<>)
				.MakeGenericType(objectType)
				.GetMethods("Setup")
				.First(mi => mi.IsGenericMethod)
				.MakeGenericMethod(returnType);
		}
	}
}
