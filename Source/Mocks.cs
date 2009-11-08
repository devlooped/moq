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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Moq.Language;
using Moq.Linq;

namespace Moq
{
	/// <summary>
	/// Allows querying the universe of mocks for those that behave 
	/// according to the query specification.
	/// </summary>
	public static class Mocks
	{
		/// <summary>
		/// Creates a query for mocks of the given type.
		/// </summary>
		/// <typeparam name="T">The type of mocked object to query.</typeparam>
		public static IQueryable<T> CreateQuery<T>()
		{
			return new MockQueryable<T>();
		}

		private class MockQueryable<T> : Query<T>
		{
			public MockQueryable()
				: base(new MockQueryProvider())
			{
			}
		}

		/// <summary>
		/// Method that is turned into the actual call from .Query{T}, to 
		/// transform the queryable query into a normal enumerable query.
		/// This method should not be used by consumers.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IEnumerable<T> CreateReal<T>()
			where T : class
		{
			while (true)
			{
				yield return new Mock<T>().Object;
			}
		}

		private class MockQueryProvider : QueryProvider
		{
			private static readonly MethodInfo createMockMethod = typeof(Mocks).GetMethod("Create");
			private static readonly MethodInfo createRealMockMethod = typeof(Mocks).GetMethod("CreateReal");

			public override object Execute(Expression expression)
			{
				var createCalls = new ExpressionCollector().Collect(expression)
					.OfType<MethodCallExpression>()
					.Where(c => c.Method.IsGenericMethod && c.Method.GetGenericMethodDefinition() == createMockMethod)
					.ToArray();

				var replaceWith = createCalls
					.Select(call => Expression.Call(
						call.Object,
						createRealMockMethod.MakeGenericMethod(call.Method.GetGenericArguments()),
						call.Arguments.ToArray()))
					.ToArray();

				var replaced = ExpressionReplacer.ReplaceAll(expression, createCalls, replaceWith);
				replaced = MockSetupsReplacer.Accept(replaced);
				replaced = QueryableToEnumerableReplacer.ReplaceAll(replaced);

				var lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(replaced.Type), replaced);
				return lambda.Compile().DynamicInvoke(null);
			}

			public override string GetQueryText(Expression expression)
			{
				throw new NotImplementedException();
			}
		}

		private class ExpressionCollector : ExpressionVisitor
		{
			private List<Expression> expressions = new List<Expression>();

			public IEnumerable<Expression> Collect(Expression exp)
			{
				this.Visit(exp);
				return expressions;
			}

			public override Expression Visit(Expression exp)
			{
				this.expressions.Add(exp);
				return base.Visit(exp);
			}
		}

		private class MockSetupsReplacer : ExpressionVisitor
		{
			private Expression expression;
			private Stack<MethodCallExpression> whereCalls = new Stack<MethodCallExpression>();

			public MockSetupsReplacer(Expression expression)
			{
				this.expression = expression;
			}

			public static Expression Accept(Expression expression)
			{
				return new MockSetupsReplacer(expression).Accept();
			}

			public Expression Accept()
			{
				return this.Visit(expression);
			}

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				// We only translate Where for now.
				if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "Where")
				{
					whereCalls.Push(node);
					var result = base.VisitMethodCall(node);
					whereCalls.Pop();
					return result;
				}

				return base.VisitMethodCall(node);
			}

			protected override Expression VisitBinary(BinaryExpression node)
			{
				if (whereCalls.Count != 0 && node.NodeType == ExpressionType.Equal)
				{
					switch (node.Left.NodeType)
					{
						case ExpressionType.MemberAccess:
							var memberAccess = (MemberExpression)node.Left;
							return ConvertToSetup(memberAccess.Expression, memberAccess, node.Right);

						case ExpressionType.Call:
							var methodCall = (MethodCallExpression)node.Left;
							return ConvertToSetup(methodCall.Object, methodCall, node.Right);
					}
				}

				return base.VisitBinary(node);
			}

			protected override Expression VisitUnary(UnaryExpression node)
			{
				if (node.NodeType == ExpressionType.Quote && node.Operand.NodeType == ExpressionType.Lambda)
				{
					var oldLambda = (LambdaExpression)node.Operand;
					var isNotExpression = oldLambda.Body.NodeType == ExpressionType.Not;
					var memberAccess = isNotExpression ?
						((UnaryExpression)oldLambda.Body).Operand as MemberExpression :
						oldLambda.Body as MemberExpression;

					if (memberAccess != null)
					{
						// where f.IsValid becomes where f.IsValid == true
						var equal = Expression.Equal(memberAccess, Expression.Constant(!isNotExpression));
						var lambda = Expression.Lambda(equal, (ParameterExpression)memberAccess.Expression);
						var quote = Expression.Quote(lambda);
						return base.VisitUnary(quote);
					}
				}

				return base.VisitUnary(node);
			}

			private static Expression ConvertToSetup(Expression targetObject, Expression left, Expression right)
			{
				// TODO: throw if target is a static class?
				var sourceType = targetObject.Type;
				var returnType = left.Type;

				var mockType = typeof(Mock<>).MakeGenericType(sourceType);
				var actionType = typeof(Action<>).MakeGenericType(mockType);
				var funcType = typeof(Func<,>).MakeGenericType(sourceType, returnType);

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

		private class FluentMockVisitor : ExpressionVisitor
		{
			private static readonly MethodInfo FluentMockGenericMethod = ((Func<Mock<string>, Expression<Func<string, string>>, Mock<string>>)
				QueryableMockExtensions.FluentMock<string, string>).Method.GetGenericMethodDefinition();
			static readonly MethodInfo MockGetGenericMethod = ((Func<string, Mock<string>>)Moq.Mock.Get<string>)
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
				// the actual first object being used in a fluent expression.
				return Expression.Call(null, MockGetGenericMethod.MakeGenericMethod(node.Type), node);
			}

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
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
				// Translate differently member accesses over transparent
				// compiler-generated types as they are typically the 
				// anonymous types generated to build up the query expressions.
				if (node.Expression.NodeType == ExpressionType.Parameter &&
					node.Expression.Type.GetCustomAttribute<CompilerGeneratedAttribute>(false) != null)
				{
					var memberType = node.Member is FieldInfo ?
						((FieldInfo)node.Member).FieldType :
						((PropertyInfo)node.Member).PropertyType;

					// Generate a Mock.Get over the entire member access rather.
					// <anonymous_type>.foo => Mock.Get(<anonymous_type>.foo)
					return Expression.Call(null, MockGetGenericMethod.MakeGenericMethod(memberType), node);
				}

				// If member is not mock-able, actually, including being a sealed class, etc.?
				if (node.Member is FieldInfo)
					throw new NotSupportedException();

				var lambdaParam = Expression.Parameter(node.Expression.Type, "mock");
				Expression lambdaBody = Expression.MakeMemberAccess(lambdaParam, node.Member);
				var targetMethod = GetTargetMethod(node.Expression.Type, ((PropertyInfo)node.Member).PropertyType);
				if (isFirst) isFirst = false;

				return TranslateFluent(node.Expression.Type, ((PropertyInfo)node.Member).PropertyType, targetMethod, Visit(node.Expression), lambdaParam, lambdaBody);
			}

			// Args like: string IFoo (mock => mock.Value)
			private Expression TranslateFluent(Type objectType, Type returnType, MethodInfo targetMethod, Expression instance, ParameterExpression lambdaParam, Expression lambdaBody)
			{
				var funcType = typeof(Func<,>).MakeGenericType(objectType, returnType);

				// This is the fluent extension method one, so pass the instance as one more arg.
				if (targetMethod.IsStatic)
					return Expression.Call(
						targetMethod,
						instance,
						Expression.Lambda(
							funcType,
							lambdaBody,
							lambdaParam
						)
					);
				else
					return Expression.Call(
						instance,
						targetMethod,
						Expression.Lambda(
							funcType,
							lambdaBody,
							lambdaParam
						)
					);
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
					targetMethod = FluentMockGenericMethod.MakeGenericMethod(objectType, returnType);
				}
				return targetMethod;
			}

			private MethodInfo GetSetupMethod(Type objectType, Type returnType)
			{
				return typeof(Mock<>)
					.MakeGenericType(objectType)
					.GetMethods()
					.First(mi => mi.Name == "Setup" && mi.IsGenericMethod)
					.MakeGenericMethod(returnType);
			}
		}

		private class QueryableToEnumerableReplacer : ExpressionVisitor
		{
			private Expression expression;

			public QueryableToEnumerableReplacer(Expression expression)
			{
				this.expression = expression;
			}

			public static Expression ReplaceAll(Expression expression)
			{
				return new QueryableToEnumerableReplacer(expression).ReplaceAll();
			}

			public Expression ReplaceAll()
			{
				return this.Visit(expression);
			}

			protected override Expression VisitConstant(ConstantExpression node)
			{
				if (node.Type.IsGenericType && node.Type.GetGenericTypeDefinition() == typeof(MockQueryable<>))
				{
					var targetType = node.Type.GetGenericArguments()[0];
					var createRealMethod = typeof(Mocks).GetMethod("CreateReal").MakeGenericMethod(targetType);
					var createRealExpr = Expression.Call(null, createRealMethod);
					var asQueryableMethod = typeof(Queryable).GetMethods()
						.Where(mi => mi.Name == "AsQueryable" && mi.IsGenericMethodDefinition)
						.Single()
						.MakeGenericMethod(targetType);

					return Expression.Call(null, asQueryableMethod, createRealExpr);
				}

				return base.VisitConstant(node);
			}
		}
	}

	/// <summary>
	/// Helper extensions that are used by the query translator.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class QueryableMockExtensions
	{
		/// <summary>
		/// Retrieves a fluent mock from the given setup expression.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static Mock<TResult> FluentMock<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> setup)
			where T : class
			where TResult : class
		{
			typeof(TResult).ThrowIfNotMockeable();

			MethodInfo info;

			if (setup.Body.NodeType == ExpressionType.MemberAccess)
			{
				var property = ((MemberExpression)setup.Body).Member as PropertyInfo;
				if (property == null)
					throw new NotSupportedException("Fields are not supported");

				info = property.GetGetMethod();
			}
			else if (setup.Body.NodeType == ExpressionType.Call)
			{
				info = ((MethodCallExpression)setup.Body).Method;
			}
			else
			{
				throw new NotSupportedException("Unsupported expression: " + setup.ToString());
			}

			if (!info.ReturnType.IsMockeable())
				// We should have a type.ThrowIfNotMockeable() rather, so that we can reuse it.
				throw new NotSupportedException();

			Mock fluentMock;
			if (!mock.InnerMocks.TryGetValue(info, out fluentMock))
			{
				fluentMock = ((IMocked)new MockDefaultValueProvider(mock).ProvideDefault(info)).Mock;
			}

			var result = (TResult)fluentMock.Object;

			mock.Setup(setup).Returns(result);

			return (Mock<TResult>)fluentMock;
		}
	}
}