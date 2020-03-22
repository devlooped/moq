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

		public interface IX
		{
			IX GetX(int arg);
			string S { get; set; }
		}
	}
}
