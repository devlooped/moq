using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Linq.Expressions;

namespace Moq.Tests
{
	[TestFixture]
	public class ExpressionExtensionsFixture
	{
		[Test]
		public void ShouldFixGenericMethodName()
		{
			Expression<Action<ExpressionExtensionsFixture>> expr1 = f => f.Do("");
			Expression<Action<ExpressionExtensionsFixture>> expr2 = f => f.Do(5);

			Assert.AreNotEqual(expr1.ToStringFixed(), expr2.ToStringFixed());
		}

		private void Do<T>(T value) { }
	}
}
