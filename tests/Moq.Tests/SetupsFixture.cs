// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq;
using System.Linq.Expressions;

using Xunit;

namespace Moq.Tests
{
	public class SetupsFixture
	{
		[Fact]
		public void Mock_made_with_new_operator_initially_has_no_setups()
		{
			var mock = new Mock<object>();
			Assert.Empty(mock.Setups);
		}

		[Fact]
		public void Setup_adds_one_setup_with_same_expression_to_Setups()
		{
			Expression<Func<object, string>> setupExpression = m => m.ToString();

			var mock = new Mock<object>();
			mock.Setup(setupExpression);

			var setup = Assert.Single(mock.Setups);
			Assert.Equal(setupExpression, setup.Expression, ExpressionComparer.Default);
		}

		[Fact]
		public void Mock_Reset_results_in_empty_Setups()
		{
			var mock = new Mock<object>();
			mock.Setup(m => m.ToString());
			Assert.NotEmpty(mock.Setups);
			mock.Reset();
			Assert.Empty(mock.Setups);
		}

		[Fact]
		public void Setups_includes_conditional_setups()
		{
			var mock = new Mock<object>();
			mock.When(() => true).Setup(m => m.ToString());

			var setup = Assert.Single(mock.Setups);
			Assert.True(setup.IsConditional);
		}

		[Fact]
		public void Setups_includes_overridden_setups()
		{
			var mock = new Mock<object>();
			mock.Setup(m => m.ToString());
			mock.Setup(m => m.ToString());

			var setups = mock.Setups.ToArray();
			Assert.Equal(2, setups.Length);
			Assert.True(setups[0].IsOverridden);
			Assert.False(setups[1].IsOverridden);
		}
	}
}
