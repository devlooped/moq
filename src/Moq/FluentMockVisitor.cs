// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Moq
{
	internal sealed class FluentMockVisitor : ExpressionVisitor
	{
		/// <summary>
		/// The first method call or member access will be the
		/// last segment of the expression (depth-first traversal),
		/// which is the one we have to Setup rather than FluentMock.
		/// And the last one is the one we have to Mock.Get rather
		/// than FluentMock.
		/// </summary>
		private bool isFirst;
		private readonly Func<ParameterExpression, Expression> resolveRoot;
		private readonly bool setupFirst;

		public FluentMockVisitor(Func<ParameterExpression, Expression> resolveRoot, bool setupFirst)
		{
			this.isFirst = true;
			this.resolveRoot = resolveRoot;
			this.setupFirst = setupFirst;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node == null)
			{
				return null;
			}

			// Translate differently member accesses over transparent
			// compiler-generated types as they are typically the
			// anonymous types generated to build up the query expressions.
			if (node.Expression.NodeType == ExpressionType.Parameter &&
				node.Expression.Type.GetTypeInfo().IsDefined(typeof(CompilerGeneratedAttribute), false))
			{
				var memberType = node.Member is FieldInfo ?
					((FieldInfo)node.Member).FieldType :
					((PropertyInfo)node.Member).PropertyType;

				// Generate a Mock.Get over the entire member access rather.
				// <anonymous_type>.foo => Mock.Get(<anonymous_type>.foo)
				return Expression.Call(null,
					Mock.GetMethod.MakeGenericMethod(memberType), node);
			}

			// If member is not mock-able, actually, including being a sealed class, etc.?
			if (node.Member is FieldInfo)
				throw new NotSupportedException();

			var lambdaParam = Expression.Parameter(node.Expression.Type, "mock");
			Expression lambdaBody = Expression.MakeMemberAccess(lambdaParam, node.Member);
			var targetMethod = GetTargetMethod(node.Expression.Type, ((PropertyInfo)node.Member).PropertyType);
			if (isFirst)
			{
				isFirst = false;
			}

			return TranslateFluent(node.Expression.Type, ((PropertyInfo)node.Member).PropertyType, targetMethod, Visit(node.Expression), lambdaParam, lambdaBody);
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

			return TranslateFluent(node.Object.Type, node.Method.ReturnType, targetMethod, this.Visit(node.Object), lambdaParam, lambdaBody);
		}

		protected override Expression VisitParameter(ParameterExpression node)
		{
			if (node == null)
			{
				return null;
			}

			return this.resolveRoot(node);
		}

		private MethodInfo GetTargetMethod(Type objectType, Type returnType)
		{
			// dte.Solution =>
			if (this.setupFirst && this.isFirst)
			{
				//.Setup(mock => mock.Solution)
				return typeof(Mock<>)
				       .MakeGenericType(objectType)
				       .GetMethods("Setup")
				       .First(m => m.IsGenericMethod)
				       .MakeGenericMethod(returnType);
			}
			else
			{
				//.FluentMock(mock => mock.Solution)
				Guard.Mockable(returnType);
				return QueryableMockExtensions.FluentMockMethod.MakeGenericMethod(objectType, returnType);
			}
		}

		// Args like: string IFoo (mock => mock.Value)
		private Expression TranslateFluent(Type objectType,
		                                   Type returnType,
		                                   MethodInfo targetMethod,
		                                   Expression instance,
		                                   ParameterExpression lambdaParam,
		                                   Expression lambdaBody)
		{
			var funcType = typeof(Func<,>).MakeGenericType(objectType, returnType);

			if (targetMethod.IsStatic)
			{
				// This is the fluent extension method one, so pass the instance as one more arg.
				return Expression.Call(targetMethod, instance, Expression.Lambda(funcType, lambdaBody, lambdaParam));
			}
			else
			{
				return Expression.Call(instance, targetMethod, Expression.Lambda(funcType, lambdaBody, lambdaParam));
			}
		}
	}
}
