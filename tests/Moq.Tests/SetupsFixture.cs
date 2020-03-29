// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
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
		public void Mock_made_with_Mock_Of_without_an_expression_initially_has_no_setups()
		{
			var mockObject = Mock.Of<object>();
			var mock = Mock.Get(mockObject);
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
		public void Mock_Of_with_expression_for_a_single_member_adds_one_setup_with_same_but_only_partial_expression_to_Setups()
		{
			Expression<Func<object, bool>> mockSpecification = m => m.ToString() == default(string);
			Expression<Func<object, string>> setupExpression = m => m.ToString();

			var mockObject = Mock.Of<object>(mockSpecification);
			var mock = Mock.Get(mockObject);

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
		public void Setups_does_not_report_conditional_setups()
		{
			var mock = new Mock<object>();
			mock.When(() => true).Setup(m => m.ToString());
			Assert.Empty(mock.Setups);
		}

		[Fact]
		public void Setups_does_not_report_overridden_setups()
		{
			var mock = new Mock<object>();
			mock.Setup(m => m.ToString());
			var setupBeforeOverride = Assert.Single(mock.Setups);
			mock.Setup(m => m.ToString());
			var setupAfterOverride = Assert.Single(mock.Setups);
			Assert.NotSame(setupBeforeOverride, setupAfterOverride);
		}
	}
}
