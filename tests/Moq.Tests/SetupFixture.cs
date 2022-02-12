// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Xunit;

namespace Moq.Tests
{
	public class SetupFixture
	{
		[Fact]
		public void IsMatched_becomes_true_as_soon_as_a_matching_invocation_is_made()
		{
			var mock = new Mock<object>();
			mock.Setup(m => m.ToString());
			var setup = mock.Setups.First();

			Assert.False(setup.IsMatched);

			_ = mock.Object.ToString();

			Assert.True(setup.IsMatched);
		}

		[Fact]
		public void IsMatched_of_setup_implicitly_created_by_SetupAllProperties_becomes_true_as_soon_as_matching_invocation_is_made()
		{
			var mock = new Mock<IX>();
			mock.SetupAllProperties();

			_ = mock.Object.Property;
			var setup = mock.Setups.First();

			Assert.True(setup.IsMatched);
		}

		[Fact]
		public void IsMatched_of_setup_implicitly_created_by_multi_dot_expression_becomes_true_as_soon_as_matching_invocation_is_made()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner.Property);
			var setup = mock.Setups.First();

			Assert.False(setup.IsMatched);

			_ = mock.Object.Inner;

			Assert.True(setup.IsMatched);
		}

		[Fact]
		public void IsOverridden_does_not_become_true_if_another_setup_with_a_different_expression_is_added_to_the_mock()
		{
			var mock = new Mock<object>();
			mock.Setup(m => m.Equals(1));
			var setup = mock.Setups.First();

			Assert.False(setup.IsOverridden);

			mock.Setup(m => m.Equals(2));

			Assert.False(setup.IsOverridden);
		}

		[Fact]
		public void IsOverridden_becomes_true_as_soon_as_another_setup_with_an_equal_expression_is_added_to_the_mock()
		{
			var mock = new Mock<object>();
			mock.Setup(m => m.Equals(1));
			var setup = mock.Setups.First();

			Assert.False(setup.IsOverridden);

			mock.Setup(m => m.Equals(1));

			Assert.True(setup.IsOverridden);
		}

		[Fact]
		public void IsVerifiable_becomes_true_if_parameterless_Verifiable_setup_method_is_called()
		{
			var mock = new Mock<object>();
			var setupBuilder = mock.Setup(m => m.ToString());
			var setup = mock.Setups.First();

			Assert.False(setup.IsVerifiable);

			setupBuilder.Verifiable();

			Assert.True(setup.IsVerifiable);
		}

		[Fact]
		public void IsVerifiable_becomes_true_if_parameterized_Verifiable_setup_method_is_called()
		{
			var mock = new Mock<object>();
			var setupBuilder = mock.Setup(m => m.ToString());
			var setup = mock.Setups.First();

			Assert.False(setup.IsVerifiable);

			setupBuilder.Verifiable(failMessage: "...");

			Assert.True(setup.IsVerifiable);
		}

		[Fact]
		public void OriginalExpression_equal_to_Expression_for_simple_property_access()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner);
			var setup = mock.Setups.First();

			Assert.Equal(setup.Expression, setup.OriginalExpression, ExpressionComparer.Default);
		}

		[Fact]
		public void OriginalExpression_equal_to_Expression_for_simple_indexer_access()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m[1]);
			var setup = mock.Setups.First();

			Assert.Equal(setup.Expression, setup.OriginalExpression, ExpressionComparer.Default);
		}

		[Fact]
		public void OriginalExpression_equal_to_Expression_for_simple_method_call()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.ToString());
			var setup = mock.Setups.First();

			Assert.Equal(setup.Expression, setup.OriginalExpression, ExpressionComparer.Default);
		}

		[Fact]
		public void OriginalExpression_returns_expression_different_from_Expression_for_multi_dot_expression()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner.ToString());
			var setup = mock.Setups.First();

			Assert.NotEqual(setup.Expression, setup.OriginalExpression, ExpressionComparer.Default);
		}

		[Fact]
		public void OriginalExpression_returns_expression_different_from_Expression_for_multi_dot_expression_in_Mock_Of()
		{
			var mockObject = Mock.Of<IX>(m => m.Inner.Property == null);
			var setup = Mock.Get(mockObject).Setups.First();

			Assert.NotEqual(setup.Expression, setup.OriginalExpression, ExpressionComparer.Default);
		}

		[Fact]
		public void OriginalExpression_returns_whole_multi_dot_expression()
		{
			Expression<Func<IX, string>> originalExpression = m => m.Inner[1].ToString();
			var mock = new Mock<IX>();
			mock.Setup(originalExpression);
			var setup = mock.Setups.First();

			Assert.Equal(originalExpression, setup.OriginalExpression, ExpressionComparer.Default);
		}

		[Fact]
		public void OriginalExpression_same_for_all_partial_setups_resulting_from_it()
		{
			Expression<Func<IX, string>> originalExpression = m => m.Inner[1].ToString();
			var mock = new Mock<IX>();
			mock.Setup(originalExpression);

			var setups = new List<ISetup>();
			for (var setup = mock.Setups.Single(); setup.InnerMock != null; setup = setup.InnerMock.Setups.Single())
			{
				setups.Add(setup);
			}

			// (using `HashSet` to automatically filter out duplicates:)
			var originalExpressions = new HashSet<Expression>(setups.Select(s => s.OriginalExpression));
			Assert.Single(originalExpressions);
		}

		[Fact]
		public void OriginalExpression_is_null_for_implicit_stubbed_property_accessor_setup()
		{
			var mock = new Mock<IX>();
			mock.SetupAllProperties();

			_ = mock.Object.Property;
			mock.Object.Property = null;

			Assert.All(mock.Setups, s => Assert.Null(s.OriginalExpression));
		}

		[Fact]
		public void OriginalExpression_is_null_for_implicit_DefaultValue_Mock_setup()
		{
			var mock = new Mock<IX>() { DefaultValue = DefaultValue.Mock };
			_ = mock.Object.Inner.Property;
			var setup = mock.Setups.First();

			Assert.Null(setup.OriginalExpression);
		}

		[Fact]
		public void InnerMock_is_null_if_return_value_cannot_be_determined_safely()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner).Returns(() => Mock.Of<IX>());
			var setup = mock.Setups.First();

			Assert.Null(setup.InnerMock);
		}

		[Fact]
		public void InnerMock_is_null_if_return_value_can_be_determined_but_is_not_a_mock()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner).Returns((IX)null);
			var setup = mock.Setups.First();

			Assert.Null(setup.InnerMock);
		}

		[Fact]
		public void InnerMock_is_set_if_return_value_can_be_determined_and_is_a_mock()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner).Returns(Mock.Of<IX>());
			var setup = mock.Setups.First();

			Assert.IsAssignableFrom<Mock<IX>>(setup.InnerMock);
		}

		[Fact]
		public void InnerMock_is_set_if_Task_async_return_value_can_be_determined_and_is_a_mock()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.GetInnerTaskAsync()).Returns(Task.FromResult<IX>(Mock.Of<IX>()));
			var setup = mock.Setups.First();

			Assert.IsAssignableFrom<Mock<IX>>(setup.InnerMock);
		}

		[Fact]
		public void InnerMock_is_set_if_ValueTask_async_return_value_can_be_determined_and_is_a_mock()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.GetInnerValueTaskAsync()).Returns(new ValueTask<IX>(Mock.Of<IX>()));
			var setup = mock.Setups.First();

			Assert.IsAssignableFrom<Mock<IX>>(setup.InnerMock);
		}

		[Fact]
		public void InnerMock_returns_correct_inner_mock_explicitly_setup_up()
		{
			var expectedInnerMock = new Mock<IX>();
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner).Returns(expectedInnerMock.Object);
			var setup = mock.Setups.First();

			Assert.Same(expectedInnerMock, setup.InnerMock);
		}

		[Fact]
		public void InnerMock_returns_correct_inner_mock_implicitly_setup_up_via_multi_dot_expression()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner.Property);
			var setup = mock.Setups.First();

			var expectedInnerMock = Mock.Get(mock.Object.Inner);

			Assert.Same(expectedInnerMock, setup.InnerMock);
		}

		[Fact]
		public void Verify_fails_on_unmatched_setup_even_when_not_flagged_as_verifiable()
		{
			var mock = new Mock<object>();
			mock.Setup(m => m.ToString());
			var setup = mock.Setups.First();

			Assert.False(setup.IsVerifiable);  // the root setup should be verified despite this
			Assert.False(setup.IsMatched);
			Assert.Throws<MockException>(() => setup.Verify());
		}

		[Fact]
		public void Verify_succeeds_on_matched_setup()
		{
			var mock = new Mock<object>();
			mock.Setup(m => m.ToString());
			var setup = mock.Setups.First();

			_ = mock.Object.ToString();

			Assert.False(setup.IsVerifiable);
			Assert.True(setup.IsMatched);
			setup.Verify();
		}

		[Fact]
		public void Verify_fails_on_matched_setup_having_unmatched_verifiable_setup_on_inner_mock()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner.Property).Verifiable();
			var setup = mock.Setups.First();

			var innerMock = mock.Object.Inner;
			var innerMockSetup = Mock.Get(innerMock).Setups.First();

			Assert.True(setup.IsMatched);
			Assert.True(innerMockSetup.IsVerifiable);
			Assert.False(innerMockSetup.IsMatched);  // this should make recursive verification fail
			Assert.Throws<MockException>(() => setup.Verify());
		}

		[Fact]
		public void Verify_succeeds_on_matched_setup_having_unmatched_setup_on_inner_mock()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner.Property);
			var setup = mock.Setups.First();

			var innerMock = mock.Object.Inner;
			var innerMockSetup = Mock.Get(innerMock).Setups.First();

			Assert.True(setup.IsMatched);
			Assert.False(innerMockSetup.IsVerifiable);  // which means that the inner mock setup will be ignored
			Assert.False(innerMockSetup.IsMatched);  // this would make verification fail if that setup were not ignored
			setup.Verify();
		}

		[Fact]
		public void Verify_succeeds_on_matched_setup_having_unmatched_verifiable_setup_on_inner_mock_if_recursive_set_to_false()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner.Property).Verifiable();
			var setup = mock.Setups.First();

			var innerMock = mock.Object.Inner;
			var innerMockSetup = Mock.Get(innerMock).Setups.First();

			Assert.True(setup.IsMatched);
			Assert.True(innerMockSetup.IsVerifiable);
			Assert.False(innerMockSetup.IsMatched);
			setup.Verify(recursive: false);  // which means that verification will never get to `innerMockSetup`
		}

		[Fact]
		public void VerifyAll_is_always_recursive()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner.Property).Verifiable();
			var setup = mock.Setups.First();

			var innerMock = mock.Object.Inner;
			var innerMockSetup = Mock.Get(innerMock).Setups.First();

			Assert.True(setup.IsMatched);
			Assert.False(innerMockSetup.IsMatched);  // this will make verification fail only if it is recursive
			Assert.Throws<MockException>(() => setup.VerifyAll());
		}

		[Fact]
		public void VerifyAll_includes_non_verifiable_setups()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner.Property);
			var setup = mock.Setups.First();

			var innerMock = mock.Object.Inner;
			var innerMockSetup = Mock.Get(innerMock).Setups.First();

			Assert.True(setup.IsMatched);
			Assert.False(innerMockSetup.IsVerifiable);  // this should not exclude the inner mock setup from verification
			Assert.Throws<MockException>(() => setup.VerifyAll());
		}

		[Fact]
		public void Verify_marks_invocations_matched_by_setup_as_verified()
		{
			var mock = new Mock<object>();
			mock.Setup(m => m.ToString());
			var setup = mock.Setups.First();

			_ = mock.Object.ToString();
			_ = mock.Object.ToString();

			Assert.All(mock.Invocations, i => Assert.False(i.IsVerified));

			setup.Verify();

			Assert.All(mock.Invocations, i => Assert.True(i.IsVerified));
			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyAll_marks_invocations_matched_by_setup_as_verified()
		{
			var mock = new Mock<object>();
			mock.Setup(m => m.ToString());
			var setup = mock.Setups.First();

			_ = mock.Object.ToString();
			_ = mock.Object.ToString();

			Assert.All(mock.Invocations, i => Assert.False(i.IsVerified));

			setup.VerifyAll();

			Assert.All(mock.Invocations, i => Assert.True(i.IsVerified));
			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void Verify_marks_invocation_matched_by_verifiable_inner_mock_setup_as_verified()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner.Property).Verifiable();
			var setup = mock.Setups.First();

			var innerMock = mock.Object.Inner;
			_ = innerMock.Property;
			var innerMockInvocation = Mock.Get(innerMock).Invocations.First();

			Assert.False(innerMockInvocation.IsVerified);

			setup.Verify();

			Assert.True(innerMockInvocation.IsVerified);
			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyAll_marks_invocation_matched_by_inner_mock_setup_as_verified()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner.Property);
			var setup = mock.Setups.First();

			var innerMock = mock.Object.Inner;
			_ = innerMock.Property;
			var innerMockInvocation = Mock.Get(innerMock).Invocations.First();

			Assert.False(innerMockInvocation.IsVerified);

			setup.VerifyAll();

			Assert.True(innerMockInvocation.IsVerified);
			mock.VerifyNoOtherCalls();
		}

		public interface IX
		{
			IX this[int index] { get; }
			IX Inner { get; }
			object Property { get; set; }
			object OtherProperty { get; set; }
			Task<IX> GetInnerTaskAsync();
			ValueTask<IX> GetInnerValueTaskAsync();
		}
	}
}
