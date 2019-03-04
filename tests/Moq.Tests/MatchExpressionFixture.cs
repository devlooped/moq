// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

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
			Assert.Contains("reducible", ex.Message);
		}

		[Fact]
		public void Can_be_rendered_using_ToString()
		{
			Assert.Equal(
				"x => x.M(Is(arg => (arg == 5)))",
				GetExpression().ToString());
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

		public interface IX
		{
			void M(int arg);
		}
	}
}
