// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq;
using System.Linq.Expressions;

using Xunit;

namespace Moq.Tests
{
	public class MockSetupsFixture
	{
		[Fact]
		public void New_Mock__initially_has_no_setups()
		{
			var mock = new Mock<IX>();

			Assert.Empty(mock.Setups);
		}

		[Fact]
		public void Mock_Of__initially_has_no_setups()
		{
			var mockObject = Mock.Of<IX>();
			var mock = Mock.Get(mockObject);

			Assert.Empty(mock.Setups);
		}

		[Fact]
		public void New_Mock__single_setup__can_be_discovered()
		{
			Expression<Func<IX, string>> setupExpression = x => x.S;

			var mock = new Mock<IX>();
			mock.Setup(setupExpression);

			Assert.Single(mock.Setups);
			Assert.Equal(setupExpression, mock.Setups.Single().Expression, ExpressionComparer.Default);
		}

		[Fact]
		public void Mock_Of__single_setup__can_be_discovered()
		{
			Expression<Func<IX, bool>> queryExpression = x => x.S == "something";
			Expression<Func<IX, string>> setupExpression = x => x.S;

			var mockObject = Mock.Of<IX>(queryExpression);
			var mock = Mock.Get(mockObject);

			Assert.Single(mock.Setups);
			Assert.Equal(setupExpression, mock.Setups.Single().Expression, ExpressionComparer.Default);
		}

		[Fact]
		public void New_Mock__single_composite_setup__can_be_discovered()
		{
			Expression<Func<IX, string>> originalSetupExpression = x => x.GetX(42).S;
			Expression<Func<IX, IX>> setupExpression = x => x.GetX(42);

			var mock = new Mock<IX>();
			mock.Setup(setupExpression);

			Assert.Single(mock.Setups);
			Assert.Equal(setupExpression, mock.Setups.Single().Expression, ExpressionComparer.Default);
		}

		[Fact]
		public void Mock_Of__single_composite_setup__can_be_discovered()
		{
			Expression<Func<IX, bool>> originalQueryExpression = x => x.GetX(42).S == "something";
			Expression<Func<IX, IX>> setupExpression = x => x.GetX(42);

			var mockObject = Mock.Of<IX>(originalQueryExpression);
			var mock = Mock.Get(mockObject);

			Assert.Single(mock.Setups);
			Assert.Equal(setupExpression, mock.Setups.Single().Expression, ExpressionComparer.Default);
		}

		[Fact]
		public void New_Mock__two_setups__listed_in_same_order_as_they_were_made()
		{
			Expression<Func<IX, IX>> setupExpression1 = x => x.GetX(1);
			Expression<Func<IX, IX>> setupExpression2 = x => x.GetX(2);

			var mock = new Mock<IX>();
			mock.Setup(setupExpression1);
			mock.Setup(setupExpression2);

			var setups = mock.Setups.ToArray();
			Assert.Equal(2, setups.Length);
			Assert.Equal(setupExpression1, setups[0].Expression, ExpressionComparer.Default);
			Assert.Equal(setupExpression2, setups[1].Expression, ExpressionComparer.Default);
		}

		[Fact]
		public void Mock_Of__two_setups__listed_in_left_to_right_order()
		{
			Expression<Func<IX, bool>> queryExpression = x => x.GetX(1) == null && x.GetX(2) == null;
			Expression<Func<IX, IX>> setupExpression1 = x => x.GetX(1);
			Expression<Func<IX, IX>> setupExpression2 = x => x.GetX(2);

			var mockObject = Mock.Of<IX>(queryExpression);
			var mock = Mock.Get(mockObject);

			var setups = mock.Setups.ToArray();
			Assert.Equal(2, setups.Length);
			Assert.Equal(setupExpression1, setups[0].Expression, ExpressionComparer.Default);
			Assert.Equal(setupExpression2, setups[1].Expression, ExpressionComparer.Default);
		}

		[Fact]
		public void Can_detect_non_conditional_setups()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.S);

			Assert.False(mock.Setups.First().IsConditional);
		}

		[Fact]
		public void Can_detect_conditional_setups()
		{
			var mock = new Mock<IX>();
			mock.When(() => true).Setup(m => m.S);

			Assert.True(mock.Setups.First().IsConditional);
		}

		[Fact]
		public void Can_detect_overridden_setups()
		{
			Expression<Func<IX, IX>> setupExpression = x => x.GetX(1);

			var mock = new Mock<IX>();
			mock.Setup(setupExpression);

			var setupsBefore = mock.Setups.ToArray();
			Assert.Single(setupsBefore);
			Assert.False(setupsBefore[0].IsOverridden);

			mock.Setup(setupExpression);

			var setupsAfter = mock.Setups.ToArray();
			Assert.Equal(2, setupsAfter.Length);
			Assert.Same(setupsAfter[0], setupsBefore[0]);
			Assert.True(setupsAfter[0].IsOverridden);
			Assert.False(setupsAfter[1].IsOverridden);
		}

		[Fact]
		public void Can_detect_nonverifiable_setups()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.S);

			Assert.False(mock.Setups.First().IsVerifiable);
		}

		[Fact]
		public void Can_detect_verifiable_setups()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.S).Verifiable();

			Assert.True(mock.Setups.First().IsVerifiable);
		}

		[Fact]
		public void Can_verify_nonverifiable_setups()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.S).Returns("something");

			var setup = mock.Setups.First();

			Assert.Throws<MockException>(() => setup.Verify());

			_ = mock.Object.S;

			setup.Verify();
		}

		[Fact]
		public void Can_verify_verifiable_setups()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.S).Returns("something").Verifiable();

			var setup = mock.Setups.First();

			Assert.Throws<MockException>(() => setup.Verify());

			_ = mock.Object.S;

			setup.Verify();
		}

		[Fact]
		public void Can_verify_conditional_setups()
		{
			var mock = new Mock<IX>();
			mock.When(() => true).Setup(m => m.S).Returns("something");

			var setup = mock.Setups.First();

			Assert.Throws<MockException>(() => setup.Verify());

			_ = mock.Object.S;

			setup.Verify();
		}

		[Fact]
		public void Setup_Verify_works_together_with_Mock_VerifyNoOtherCalls()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.S);

			_ = mock.Object.S;

			var setup = mock.Setups.First();
			setup.Verify();

			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void Can_retrieve_inner_mocks_manually_set_up_via_Returns()
		{
			Expression<Func<IX, IX>> outerMockSetupExpression = m => m.GetX(1);
			Expression<Func<IX, string>> innerMockSetupExpression = m => m.S;

			var innerMock = new Mock<IX>();
			innerMock.Setup(innerMockSetupExpression);

			var outerMock = new Mock<IX>();
			outerMock.Setup(outerMockSetupExpression).Returns(innerMock.Object);

			var outerMockSetup = outerMock.Setups.Single();
			Assert.Equal(outerMockSetupExpression, outerMockSetup.Expression, ExpressionComparer.Default);
			Assert.True(outerMockSetup.ReturnsInnerMock(out var reportedInnerMock));
			Assert.Same(innerMock, reportedInnerMock);

			var innerMockSetup = innerMock.Setups.Single();
			Assert.Equal(innerMockSetupExpression, innerMockSetup.Expression, ExpressionComparer.Default);
			Assert.False(innerMockSetup.ReturnsInnerMock(out _));
		}

		[Fact]
		public void Can_retrieve_inner_mocks_set_up_via_multidot_setup_expression()
		{
			Expression<Func<IX, string>> originalSetupExpression = m => m.GetX(1).S;
			Expression<Func<IX, IX>> outerMockSetupExpression = m => m.GetX(1);
			Expression<Func<IX, string>> innerMockSetupExpression = m => m.S;

			var outerMock = new Mock<IX>();
			outerMock.Setup(originalSetupExpression);

			var outerMockSetup = outerMock.Setups.Single();
			Assert.Equal(outerMockSetupExpression, outerMockSetup.Expression, ExpressionComparer.Default);
			Assert.True(outerMockSetup.ReturnsInnerMock(out var innerMock));

			var innerMockSetup = innerMock.Setups.Single();
			Assert.Equal(innerMockSetupExpression, innerMockSetup.Expression, ExpressionComparer.Default);
			Assert.False(innerMockSetup.ReturnsInnerMock(out _));
		}

		[Fact]
		public void IsPartOfCompositeSetup_returns_false_for_single_dot_setup()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.S);

			var setup = mock.Setups.First();
			Assert.False(setup.IsPartOfCompositeSetup(out _));
		}

		[Fact]
		public void IsPartOfCompositeSetup_returns_true_for_multi_dot_setup_created_by_Setup()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.GetX(It.IsAny<int>()).S);

			var setup = mock.Setups.First();
			Assert.True(setup.IsPartOfCompositeSetup(out _));
		}

		[Fact]
		public void IsPartOfCompositeSetup_returns_true_for_multi_dot_setup_created_by_Mock_Of()
		{
			var mockObject = Mock.Of<IX>(m => m.GetX(It.IsAny<int>()).S == "something");
			var mock = Mock.Get(mockObject);

			var setup = mock.Setups.First();
			Assert.True(setup.IsPartOfCompositeSetup(out _));
		}

		[Fact]
		public void Can_discover_composite_setup_parts()
		{
			Expression<Func<IX, string>> originalSetupExpression = m => m.GetX(1).S;
			Expression<Func<IX, IX>> mockSetupExpression = m => m.GetX(1);
			Expression<Func<IX, string>> innerMockSetupExpression = m => m.S;

			var mock = new Mock<IX>();
			mock.Setup(originalSetupExpression);

			var setup = mock.Setups.Single();

			Assert.True(setup.IsPartOfCompositeSetup(out var originalSetup));
			Assert.Equal(mockSetupExpression, originalSetup.Parts[0].Expression, ExpressionComparer.Default);
			Assert.Equal(innerMockSetupExpression, originalSetup.Parts[1].Expression, ExpressionComparer.Default);

			Assert.True(setup.ReturnsInnerMock(out var innerMock));
			var innerMockSetup = innerMock.Setups.Single();
			Assert.Same(setup, originalSetup.Parts[0]);
			Assert.Same(innerMockSetup, originalSetup.Parts[1]);
		}

		public interface IX
		{
			IX GetX(int arg);
			string S { get; set; }
		}
	}
}
