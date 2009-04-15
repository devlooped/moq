using System;
using System.Linq.Expressions;
using Xunit;

namespace Moq.Tests
{
	public class ExpressionExtensionsFixture
	{
		[Fact]
		public void FixesGenericMethodName()
		{
			Expression<Action<ExpressionExtensionsFixture>> expr1 = f => f.Do("");
			Expression<Action<ExpressionExtensionsFixture>> expr2 = f => f.Do(5);

			Assert.NotEqual(expr1.ToStringFixed(), expr2.ToStringFixed());
		}

		[Fact]
		public void PrefixesStaticMethodWithClass()
		{
			Expression<Action> expr = () => DoStatic(5);

			var value = expr.ToStringFixed();

			Assert.Contains("ExpressionExtensionsFixture.DoStatic(5)", value);
		}

		[Fact]
		public void PrefixesStaticGenericMethodWithClass()
		{
			Expression<Action> expr = () => DoStaticGeneric(5);

			var value = expr.ToStringFixed();

			Assert.Contains("ExpressionExtensionsFixture.DoStaticGeneric<Int32>(5)", value);
		}

		[Fact]
		public void ToLambdaThrowsIfNullExpression()
		{
			Assert.Throws<ArgumentNullException>(() => ExpressionExtensions.ToLambda(null));
		}

		[Fact]
		public void ToLambdaThrowsIfExpressionNotLambda()
		{
			Assert.Throws<ArgumentException>(() => Expression.Constant(5).ToLambda());
		}

		[Fact]
		public void ToLambdaRemovesConvert()
		{
			var lambda = ToExpression<object>(() => (object)5);

			var result = lambda.ToLambda();

			Assert.Equal(typeof(int), result.Compile().Method.ReturnType);
		}

		[Fact]
		public void IsPropertyLambdaTrue()
		{
			var expr = ToExpression<IFoo, int>(f => f.Value).ToLambda();

			Assert.True(expr.IsProperty());
		}

		[Fact]
		public void IsPropertyLambdaFalse()
		{
			var expr = ToExpression<IFoo>(f => f.Do()).ToLambda();

			Assert.False(expr.IsProperty());
		}

		[Fact]
		public void IsPropertyExpressionTrue()
		{
			var expr = ToExpression<IFoo, int>(f => f.Value).ToLambda().Body;

			Assert.True(expr.IsProperty());
		}

		[Fact]
		public void IsPropertyExpressionFalse()
		{
			var expr = ToExpression<IFoo>(f => f.Do()).ToLambda().Body;

			Assert.False(expr.IsProperty());
		}

		[Fact]
		public void IsPropertyIndexerExpressionTrue()
		{
			var expr = ToExpression<IFoo, object>(f => f[5]).ToLambda().Body;

			Assert.True(expr.IsPropertyIndexer());
		}

		[Fact]
		public void ToMethodCallThrowsIfNotMethodCall()
		{
			var expr = ToExpression<IFoo, object>(f => f.Value).ToLambda();

			Assert.Throws<ArgumentException>(() => expr.ToMethodCall());
		}

		[Fact]
		public void ToMethodCallConvertsLambda()
		{
			var expr = ToExpression<IFoo>(f => f.Do()).ToLambda();

			Assert.Equal(typeof(IFoo).GetMethod("Do"), expr.ToMethodCall().Method);
		}

		[Fact]
		public void ToPropertyInfoConvertsExpression()
		{
			
		}

		private Expression ToExpression<T>(Expression<Func<T>> expression)
		{
			return expression;
		}

		private Expression ToExpression<T>(Expression<Action<T>> expression)
		{
			return expression;
		}

		private Expression ToExpression<T, TResult>(Expression<Func<T, TResult>> expression)
		{
			return expression;
		}

		private void Do<T>(T value) { }

		private static void DoStatic(int value) { }
		private static void DoStaticGeneric<T>(T value) { }

		public interface IFoo
		{
			int Value { get; set; }
			void Do();
			object this[int index] { get; set; }
		}
	}
}
