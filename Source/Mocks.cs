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
using System.ComponentModel;
using Moq;
using IQToolkit;
using System.Linq.Expressions;
using System.Reflection;
using Moq.Language.Flow;
using Moq.Language;
using System.Collections;
using System.Runtime.CompilerServices;

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

		class MockQueryable<T> : Query<T>
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

		class MockQueryProvider : QueryProvider
		{
			static readonly MethodInfo createMockMethod = typeof(Mocks).GetMethod("Create");
			static readonly MethodInfo createRealMockMethod = typeof(Mocks).GetMethod("CreateReal");

			public override object Execute(Expression expression)
			{
				var collector = new ExpressionCollector();
				var createCalls = collector
					.Collect(expression)
					.OfType<MethodCallExpression>()
					.Where(call => call.Method.IsGenericMethod &&
						call.Method.GetGenericMethodDefinition() == createMockMethod)
					.ToArray();
				var replaceWith = createCalls
					.Select(call => Expression.Call(
						call.Object,
						createRealMockMethod.MakeGenericMethod(
							call.Method.GetGenericArguments()),
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

		class ExpressionCollector : ExpressionVisitor
		{
			List<Expression> expressions = new List<Expression>();

			public IEnumerable<Expression> Collect(Expression exp)
			{
				Visit(exp);
				return expressions;
			}

			protected override Expression Visit(Expression exp)
			{
				expressions.Add(exp);
				return base.Visit(exp);
			}
		}

		class MockSetupsReplacer : ExpressionVisitor
		{
			Expression expression;
			Stack<MethodCallExpression> whereCalls = new Stack<MethodCallExpression>();

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
				return Visit(expression);
			}

			protected override Expression VisitMethodCall(MethodCallExpression m)
			{
				// We only translate Where for now.
				if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
				{
					whereCalls.Push(m);
					var result = base.VisitMethodCall(m);
					whereCalls.Pop();

					return result;
				}
				else
				{
					return base.VisitMethodCall(m);
				}
			}

			protected override Expression VisitBinary(BinaryExpression b)
			{
				if (whereCalls.Count != 0 && b.NodeType == ExpressionType.Equal &&
					(b.Left.NodeType == ExpressionType.MemberAccess || b.Left.NodeType == ExpressionType.Call))
				{
					var isMember = b.Left.NodeType == ExpressionType.MemberAccess;
					var methodCall = b.Left as MethodCallExpression;
					var memberAccess = b.Left as MemberExpression;

					// TODO: throw if target is a static class?
					var targetObject = isMember ? memberAccess.Expression : methodCall.Object;
					var sourceType = isMember ? memberAccess.Expression.Type : methodCall.Object.Type;
					var returnType = isMember ? memberAccess.Type : methodCall.Method.ReturnType;

					var mockType = typeof(Mock<>).MakeGenericType(sourceType);
					var actionType = typeof(Action<>).MakeGenericType(mockType);
					var funcType = typeof(Func<,>).MakeGenericType(sourceType, returnType);

					// where dte.Solution == solution
					// becomes:	
					// where Mock.Get(dte).Setup(mock => mock.Solution).Returns(solution) != null

					var returnsMethod = typeof(IReturns<,>)
						.MakeGenericType(sourceType, returnType)
						.GetMethod("Returns", new[] { returnType });

					var notNullBinary = Expression.NotEqual(
						Expression.Call(
							FluentMockVisitor.Accept(b.Left),
						// .Returns(solution)
							returnsMethod,
							b.Right
						),
						// != null
						Expression.Constant(null)
					);

					return notNullBinary;
				}

				return base.VisitBinary(b);
			}
		}

		class FluentMockVisitor : ExpressionVisitor
		{
			static readonly MethodInfo FluentMockGenericMethod = ((Func<Mock<string>, Expression<Func<string, string>>, Mock<string>>)
				QueryableMockExtensions.FluentMock<string, string>).Method.GetGenericMethodDefinition();
			static readonly MethodInfo MockGetGenericMethod = ((Func<string, Mock<string>>)Moq.Mock.Get<string>)
				.Method.GetGenericMethodDefinition();

			Expression expression;

			/// <summary>
			/// The first method call or member access will be the 
			/// last segment of the expression (depth-first traversal), 
			/// which is the one we have to Setup rather than FluentMock.
			/// And the last one is the one we have to Mock.Get rather 
			/// than FluentMock.
			/// </summary>
			bool isFirst = true;

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
				return Visit(expression);
			}

			protected override Expression VisitParameter(ParameterExpression p)
			{
				// the actual first object being used in a fluent expression.
				return Expression.Call(null,
					MockGetGenericMethod.MakeGenericMethod(p.Type),
					p);
			}

			protected override Expression VisitMethodCall(MethodCallExpression m)
			{
				var lambdaParam = Expression.Parameter(m.Object.Type, "mock");
				Expression lambdaBody = Expression.Call(lambdaParam, m.Method, m.Arguments);
				var targetMethod = GetTargetMethod(m.Object.Type, m.Method.ReturnType);
				if (isFirst) isFirst = false;

				return TranslateFluent(m.Object.Type, m.Method.ReturnType, targetMethod, Visit(m.Object), lambdaParam, lambdaBody);
			}

			protected override Expression VisitMemberAccess(MemberExpression m)
			{
				// Translate differently member accesses over transparent
				// compiler-generated types as they are typically the 
				// anonymous types generated to build up the query expressions.
				if (m.Expression.NodeType == ExpressionType.Parameter &&
					m.Expression.Type.GetCustomAttribute<CompilerGeneratedAttribute>(false) != null)
				{
					var memberType = m.Member is FieldInfo ? 
						((FieldInfo)m.Member).FieldType : 
						((PropertyInfo)m.Member).PropertyType;

					// Generate a Mock.Get over the entire member access rather.
					// <anonymous_type>.foo => Mock.Get(<anonymous_type>.foo)
					return Expression.Call(null,
						MockGetGenericMethod.MakeGenericMethod(memberType), m);
				}

				// If member is not mock-able, actually, including being a sealed class, etc.?
				if (m.Member is FieldInfo)
					throw new NotSupportedException();

				var lambdaParam = Expression.Parameter(m.Expression.Type, "mock");
				Expression lambdaBody = Expression.MakeMemberAccess(lambdaParam, m.Member);
				var targetMethod = GetTargetMethod(m.Expression.Type, ((PropertyInfo)m.Member).PropertyType);
				if (isFirst) isFirst = false;

				return TranslateFluent(m.Expression.Type, ((PropertyInfo)m.Member).PropertyType, targetMethod, Visit(m.Expression), lambdaParam, lambdaBody);
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

		class QueryableToEnumerableReplacer : ExpressionVisitor
		{
			Expression expression;

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

			protected override Expression VisitConstant(ConstantExpression c)
			{
				if (c.Type.IsGenericType &&
					c.Type.GetGenericTypeDefinition() == (typeof(MockQueryable<>)))
				{
					var targetType = c.Type.GetGenericArguments()[0];
					var createRealMethod = typeof(Mocks).GetMethod("CreateReal").MakeGenericMethod(targetType);
					var createRealExpr = Expression.Call(null, createRealMethod);
					var asQueryableMethod = typeof(Queryable).GetMethods()
						.Where(mi => mi.Name == "AsQueryable" && mi.IsGenericMethodDefinition)
						.Single()
						.MakeGenericMethod(targetType);
					var asQueryable = Expression.Call(null, asQueryableMethod, createRealExpr);

					return asQueryable;
				}

				return base.VisitConstant(c);
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
