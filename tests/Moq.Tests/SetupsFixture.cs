// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Linq;

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
		public void Setup_adds_one_item_to_Setups()
		{
			var mock = new Mock<object>();
			mock.Setup(m => m.ToString());
			Assert.Single(mock.Setups);
		}

		[Fact]
		public void Mock_Of_with_expression_for_a_single_member_adds_one_item_to_Setups()
		{
			var mockObject = Mock.Of<object>(m => m.ToString() == default(string));
			var mock = Mock.Get(mockObject);
			Assert.Single(mock.Setups);
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
