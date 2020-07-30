// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Linq.Expressions;

using Moq.Protected;

using Xunit;

namespace Moq.Tests
{
	public class MatchExpressionFixture
	{
		[Fact]
		public void Prevents_compilation()
		{
			var ex = Assert.Throws<ArgumentException>(() =>
				GetExpression().CompileUsingExpressionCompiler());
			Assert.Contains("ReduceAndCheck", ex.StackTrace);
		}

		[Fact]
		public void Can_be_rendered_using_ToString()
		{
			Assert.Equal(
				"x => x.M(Is(arg => (arg == 5)))",
				GetExpression().ToString());
		}

		[Fact]
		public void Can_be_rendered_using_ToStringFixed()
		{
			Assert.Equal(
				"x => x.M(It.Is<int>(arg => arg == 5))",
				GetExpression().ToStringFixed());
		}

		[Fact]
		public void Is_not_evaluated_by_PartialEval()
		{
			var expression = GetExpression();
			var matchExpression = FindMatchExpression(expression);
			Assert.NotNull(matchExpression);
			var evaluatedExpression = expression.PartialEval();
			Assert.Same(matchExpression, FindMatchExpression(evaluatedExpression));
		}

		[Fact]
		public void Is_not_evaluated_by_PartialMatcherAwareEval()
		{
			var expression = GetExpression();
			var matchExpression = FindMatchExpression(expression);
			Debug.Assert(matchExpression != null);
			var evaluatedExpression = expression.PartialMatcherAwareEval();
			Assert.Same(matchExpression, FindMatchExpression(evaluatedExpression));
		}

		[Fact]
		public void Can_be_compared_by_ExpressionComparer()
		{
			var left = GetExpression();
			var right = GetExpression();
			Assert.Equal(left, right, ExpressionComparer.Default);
		}

		[Fact]
		public void Can_be_compared_by_ExpressionComparer_2()
		{
			var left = GetItIsAnyExpression();
			var right = GetItIsAnyExpression();
			Assert.Equal(left, right, ExpressionComparer.Default);
		}

		[Fact]
		public void Can_be_compared_by_ExpressionComparer_3()
		{
			var left = GetItIsAnyExpression();
			var right = GetItIsAnyMatchExpression();
			Assert.Equal(left, right, ExpressionComparer.Default);
		}

		[Fact]
		public void Can_be_compared_by_ExpressionComparer_4()
		{
			var left = GetItIsAnyMatchExpression();
			var right = GetItIsAnyExpression();
			Assert.Equal(left, right, ExpressionComparer.Default);
		}

		[Fact]
		public void Is_correctly_handled_by_MatcherFactory()
		{
			var expression = GetExpression();
			var matchExpression = FindMatchExpression(expression);
			Debug.Assert(matchExpression != null);
			var (matcher, _) = MatcherFactory.CreateMatcher(matchExpression);
			Assert.IsType<Match<int>>(matcher);
		}

		[Fact]
		public void Is_handled_correctly_in_setup_and_verification()
		{
			var mock = new Mock<IX>();
			mock.Setup(GetExpression()).Verifiable();
			Assert.Throws<MockException>(() => mock.Verify());
			mock.Object.M(1);
			Assert.Throws<MockException>(() => mock.Verify());
			mock.Object.M(5);
			mock.Verify();
		}

		private Expression<Action<IX>> GetExpression()
		{
			var x = Expression.Parameter(typeof(IX), "x");
			return Expression.Lambda<Action<IX>>(
				Expression.Call(
					x,
					typeof(IX).GetMethod(nameof(IX.M)),
					new MatchExpression(
						new Match<int>(arg => arg == 5, () => It.Is<int>(arg => arg == 5)))),
				x);
		}

		private Expression<Action<IX>> GetItIsAnyExpression()
		{
			var x = Expression.Parameter(typeof(IX), "x");
			return Expression.Lambda<Action<IX>>(
				Expression.Call(
					x,
					typeof(IX).GetMethod(nameof(IX.M)),
					ItExpr.IsAny<int>()),
				x);
		}

		private Expression<Action<IX>> GetItIsAnyMatchExpression()
		{
			Match itIsAnyMatch;
			using (var observer = MatcherObserver.Activate())
			{
				_ = It.IsAny<int>();
				_ = observer.TryGetLastMatch(out itIsAnyMatch);
			}

			var x = Expression.Parameter(typeof(IX), "x");
			return Expression.Lambda<Action<IX>>(
				Expression.Call(
					x,
					typeof(IX).GetMethod(nameof(IX.M)),
					new MatchExpression(itIsAnyMatch)),
				x);
		}

		private static MatchExpression FindMatchExpression(Expression expression)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Lambda:
					return FindMatchExpression(((LambdaExpression)expression).Body);

				case ExpressionType.Call:
					return FindMatchExpression(((MethodCallExpression)expression).Arguments[0]);

				case ExpressionType.Extension:
					return expression as MatchExpression;

				default:
					return null;
			}
		}

		public interface IX
		{
			void M(int arg);
		}
	}
}
