// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq.Language;
using Moq.Properties;

namespace Moq.Linq
{
	internal class MockSetupsBuilder : ExpressionVisitor
	{
		private static readonly string[] queryableMethods = new[] { "First", "Where", "FirstOrDefault" };
		private static readonly string[] unsupportedMethods = new[] { "All", "Any", "Last", "LastOrDefault", "Single", "SingleOrDefault" };

		private int stackIndex;
		private MethodCallExpression underlyingCreateMocks;

		public MockSetupsBuilder(MethodCallExpression underlyingCreateMocks)
		{
			this.underlyingCreateMocks = underlyingCreateMocks;
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (node != null && this.stackIndex > 0)
			{
				if (node.NodeType != ExpressionType.Equal && node.NodeType != ExpressionType.AndAlso)
					throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Resources.LinqBinaryOperatorNotSupported, node.ToStringFixed()));

				if (node.NodeType == ExpressionType.Equal)
				{
					// TODO: throw if a matcher is used on either side of the expression.
					//ThrowIfMatcherIsUsed(

					// Account for the inverted assignment/querying like "false == foo.IsValid" scenario
					if (node.Left.NodeType == ExpressionType.Constant)
						// Invert left & right nodes in this case.
						return ConvertToSetup(node.Right, node.Left) ?? base.VisitBinary(node);
					else
						// Perform straight conversion where the right-hand side will be the setup return value.
						return ConvertToSetup(node.Left, node.Right) ?? base.VisitBinary(node);
				}
			}

			return base.VisitBinary(node);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (node != null && node.Type.GetTypeInfo().IsGenericType && node.Type.GetGenericTypeDefinition() == typeof(MockQueryable<>))
			{
				//var asQueryableMethod = createQueryableMethod.MakeGenericMethod(node.Type.GetGenericArguments()[0]);

				//return Expression.Call(null, asQueryableMethod);
				return this.underlyingCreateMocks;
			}

			return base.VisitConstant(node);
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node != null && this.stackIndex > 0 && node.Type == typeof(bool))
			{
				return ConvertToSetup(node.Expression, node, Expression.Constant(true));
			}

			return base.VisitMember(node);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node != null)
			{
				if (node.Method.DeclaringType == typeof(Queryable) && queryableMethods.Contains(node.Method.Name))
				{
					this.stackIndex++;
					var result = base.VisitMethodCall(node);
					this.stackIndex--;
					return result;
				}

				if (unsupportedMethods.Contains(node.Method.Name))
				{
					throw new NotSupportedException(string.Format(
						CultureInfo.CurrentCulture,
						Resources.LinqMethodNotSupported,
						node.Method.Name));
				}

				if (this.stackIndex > 0 && node.Type == typeof(bool))
				{
					return ConvertToSetup(node.Object, node, Expression.Constant(true));
				}
			}

			return base.VisitMethodCall(node);
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node != null && this.stackIndex > 0 && node.NodeType == ExpressionType.Not)
			{
				return ConvertToSetup(node.Operand, Expression.Constant(false)) ?? base.VisitUnary(node);
			}

			return base.VisitUnary(node);
		}

		private static Expression ConvertToSetup(Expression left, Expression right)
		{
			switch (left.NodeType)
			{
				case ExpressionType.MemberAccess:
					var member = (MemberExpression)left;
					member.ThrowIfNotMockeable();

					return ConvertToSetupProperty(member.Expression, member, right);

				case ExpressionType.Call:
					var method = (MethodCallExpression)left;

					if (!method.Method.CanOverride())
						throw new NotSupportedException(string.Format(
							CultureInfo.CurrentCulture,
							Resources.LinqMethodNotVirtual,
							method.ToStringFixed()));

					return ConvertToSetup(method.Object, method, right);

				case ExpressionType.Invoke:
					var invocation = (InvocationExpression)left;
					if (invocation.Expression is ParameterExpression && typeof(Delegate).IsAssignableFrom(invocation.Expression.Type))
					{
						return ConvertToSetup(invocation, right);
					}
					else
					{
						break;
					}

				case ExpressionType.Convert:
					var left1 = (UnaryExpression)left;
					return ConvertToSetup(left1.Operand, Expression.Convert(right, left1.Operand.Type));
			}

			return null;
		}

		private static Expression ConvertToSetup(InvocationExpression invocation, Expression right)
		{
			// transforms a delegate invocation expression such as `f(...) == x` (where `invocation` := `f(...)` and `right` := `x`)
			// to `Mock.Get(f).Setup(f' => f'(...)).Returns(x) != null` (which in turn will get incorporated into a query
			// `CreateMocks().First(f => ...)`.

			var delegateParameter = invocation.Expression;

			var mockGetMethod =
				typeof(Mock)
				.GetMethod("Get", BindingFlags.Public | BindingFlags.Static)
				.MakeGenericMethod(delegateParameter.Type);

			var mockGetCall = Expression.Call(mockGetMethod, delegateParameter);

			var setupMethod =
				typeof(Mock<>)
				.MakeGenericType(delegateParameter.Type)
				.GetMethods("Setup")
				.Single(m => m.IsGenericMethod)
				.MakeGenericMethod(right.Type);

			var setupCall = Expression.Call(
				mockGetCall,
				setupMethod,
				Expression.Lambda(invocation, invocation.Expression as ParameterExpression));

			var returnsMethod =
				typeof(IReturns<,>)
				.MakeGenericType(delegateParameter.Type, right.Type)
				.GetMethod("Returns", new[] { right.Type });

			var returnsCall = Expression.Call(
				setupCall,
				returnsMethod,
				right);

			return Expression.NotEqual(returnsCall, Expression.Constant(null));
		}

		private static Expression ConvertToSetupProperty(Expression targetObject, Expression left, Expression right)
		{
			// TODO: throw if target is a static class?
			var sourceType = targetObject.Type;
			var propertyInfo = (PropertyInfo)((MemberExpression)left).Member;
			var propertyType = propertyInfo.PropertyType;

			// where foo.Name == "bar"
			// becomes:	
			// where Mock.Get(foo).SetupProperty(mock => mock.Name, "bar") != null

			// if the property is readonly, we can only do a Setup(...) which is the same as a method setup.
			if (!propertyInfo.CanWrite || propertyInfo.GetSetMethod(true).IsPrivate)
				return ConvertToSetup(targetObject, left, right);

			// This will get up to and including the Mock.Get(foo).Setup(mock => mock.Name) call.
			var propertySetup = VisitFluent(left);
			// We need to go back one level, to the target expression of the Setup call, 
			// which would be the Mock.Get(foo), where we will actually invoke SetupProperty instead.
			if (propertySetup.NodeType != ExpressionType.Call)
				throw new NotSupportedException(string.Format(Resources.UnexpectedTranslationOfMemberAccess, propertySetup.ToStringFixed()));

			var propertyCall = (MethodCallExpression)propertySetup;
			var mockExpression = propertyCall.Object;
			var propertyExpression = propertyCall.Arguments.First().StripQuotes();

			// Because Mocks.CreateMocks (the underlying implementation of the IQueryable provider
			// already sets up all properties as stubs, we can safely just set the value here, 
			// which also allows the use of this querying capability against plain DTO even 
			// if their properties are not virtual.
			var setPropertyMethod = typeof(Mocks)
				.GetMethod("SetProperty", BindingFlags.Static | BindingFlags.NonPublic)
				.MakeGenericMethod(mockExpression.Type.GetGenericArguments().First(), propertyInfo.PropertyType);

			return Expression.Equal(
				Expression.Call(setPropertyMethod, mockExpression, propertyCall.Arguments.First(), right),
				Expression.Constant(true));
		}

		private static Expression ConvertToSetup(Expression targetObject, Expression left, Expression right)
		{
			// TODO: throw if target is a static class?
			var sourceType = targetObject.Type;
			var returnType = left.Type;

			var returnsMethod = typeof(IReturns<,>)
				.MakeGenericType(sourceType, returnType)
				.GetMethod("Returns", new[] { returnType });

			if (right is ConstantExpression constExpr && constExpr.Value == null)
			{
				right = Expression.Constant(null, left.Type);
			}

			return Expression.NotEqual(
				Expression.Call(VisitFluent(left), returnsMethod, right),
				Expression.Constant(null));
		}

		private static Expression VisitFluent(Expression expression)
		{
			return new FluentMockVisitor(resolveRoot: p => Expression.Call(null, Mock.GetMethod.MakeGenericMethod(p.Type), p),
			                             setupRightmost: true)
			       .Visit(expression);
		}
	}
}
