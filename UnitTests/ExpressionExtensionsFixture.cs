using System;
using System.Linq.Expressions;
using Xunit;

namespace Moq.Tests
{
	public class ExpressionExtensionsFixture
	{
		[Fact]
		public void ShouldFixGenericMethodName()
		{
			Expression<Action<ExpressionExtensionsFixture>> expr1 = f => f.Do("");
			Expression<Action<ExpressionExtensionsFixture>> expr2 = f => f.Do(5);

			Assert.NotEqual(expr1.ToStringFixed(), expr2.ToStringFixed());
		}

		private void Do<T>(T value) { }
	}
}
