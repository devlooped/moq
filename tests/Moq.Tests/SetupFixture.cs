// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Linq;
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

			Assert.False(setup.WasMatched);

			_ = mock.Object.ToString();

			Assert.True(setup.WasMatched);
		}

		[Fact]
		public void IsMatched_of_setup_implicitly_created_by_SetupAllProperties_becomes_true_as_soon_as_matching_invocation_is_made()
		{
			var mock = new Mock<IX>();
			mock.SetupAllProperties();

			_ = mock.Object.Property;
			var setup = mock.Setups.First();

			Assert.True(setup.WasMatched);
		}

		[Fact]
		public void IsMatched_of_setup_implicitly_created_by_multi_dot_expression_becomes_true_as_soon_as_matching_invocation_is_made()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner.Property);
			var setup = mock.Setups.First();

			Assert.False(setup.WasMatched);

			_ = mock.Object.Inner;

			Assert.True(setup.WasMatched);
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
		public void ReturnsMock_returns_null_if_return_value_cannot_be_determined_safely()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner).Returns(() => Mock.Of<IX>());
			var setup = mock.Setups.First();

			Assert.Null(setup.ReturnsMock(out _));
		}

		[Fact]
		public void ReturnsMock_returns_false_if_return_value_can_be_determined_but_is_not_a_mock()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner).Returns((IX)null);
			var setup = mock.Setups.First();

			Assert.False(setup.ReturnsMock(out _));
		}

		[Fact]
		public void ReturnsMock_returns_true_if_return_value_can_be_determined_and_is_a_mock()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner).Returns(Mock.Of<IX>());
			var setup = mock.Setups.First();

			Assert.True(setup.ReturnsMock(out var innerMock));
			Assert.IsAssignableFrom<Mock<IX>>(innerMock);
		}

		[Fact]
		public void ReturnsMock_returns_true_if_Task_async_return_value_can_be_determined_and_is_a_mock()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.GetInnerTaskAsync()).Returns(Task.FromResult<IX>(Mock.Of<IX>()));
			var setup = mock.Setups.First();

			Assert.True(setup.ReturnsMock(out var innerMock));
			Assert.IsAssignableFrom<Mock<IX>>(innerMock);
		}

		[Fact]
		public void ReturnsMock_returns_true_if_ValueTask_async_return_value_can_be_determined_and_is_a_mock()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.GetInnerValueTaskAsync()).Returns(new ValueTask<IX>(Mock.Of<IX>()));
			var setup = mock.Setups.First();

			Assert.True(setup.ReturnsMock(out var innerMock));
			Assert.IsAssignableFrom<Mock<IX>>(innerMock);
		}

		[Fact]
		public void ReturnsMock_returns_correct_inner_mock_explicitly_setup_up()
		{
			var expectedInnerMock = new Mock<IX>();
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner).Returns(expectedInnerMock.Object);
			var setup = mock.Setups.First();

			Assert.True(setup.ReturnsMock(out var actualInnerMock));
			Assert.Same(expectedInnerMock, actualInnerMock);
		}

		[Fact]
		public void ReturnsMock_returns_correct_inner_mock_implicitly_setup_up_via_multi_dot_expression()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inner.Property);
			var setup = mock.Setups.First();

			var expectedInnerMock = Mock.Get(mock.Object.Inner);

			Assert.True(setup.ReturnsMock(out var actualInnerMock));
			Assert.Same(expectedInnerMock, actualInnerMock);
		}

		[Fact]
		public void Verify_fails_on_unmatched_setup_even_when_not_flagged_as_verifiable()
		{
			var mock = new Mock<object>();
			mock.Setup(m => m.ToString());
			var setup = mock.Setups.First();

			Assert.False(setup.IsVerifiable);  // the root setup should be verified despite this
			Assert.False(setup.WasMatched);
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
			Assert.True(setup.WasMatched);
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

			Assert.True(setup.WasMatched);
			Assert.True(innerMockSetup.IsVerifiable);
			Assert.False(innerMockSetup.WasMatched);  // this should make recursive verification fail
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

			Assert.True(setup.WasMatched);
			Assert.False(innerMockSetup.IsVerifiable);  // which means that the inner mock setup will be ignored
			Assert.False(innerMockSetup.WasMatched);  // this would make verification fail if that setup were not ignored
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

			Assert.True(setup.WasMatched);
			Assert.True(innerMockSetup.IsVerifiable);
			Assert.False(innerMockSetup.WasMatched);
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

			Assert.True(setup.WasMatched);
			Assert.False(innerMockSetup.WasMatched);  // this will make verification fail only if it is recursive
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

			Assert.True(setup.WasMatched);
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

			Assert.All(mock.Invocations, i => Assert.False(i.WasVerified));

			setup.Verify();

			Assert.All(mock.Invocations, i => Assert.True(i.WasVerified));
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

			Assert.All(mock.Invocations, i => Assert.False(i.WasVerified));

			setup.VerifyAll();

			Assert.All(mock.Invocations, i => Assert.True(i.WasVerified));
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

			Assert.False(innerMockInvocation.WasVerified);

			setup.Verify();

			Assert.True(innerMockInvocation.WasVerified);
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

			Assert.False(innerMockInvocation.WasVerified);

			setup.VerifyAll();

			Assert.True(innerMockInvocation.WasVerified);
			mock.VerifyNoOtherCalls();
		}

		public interface IX
		{
			IX Inner { get; }
			object Property { get; set; }
			Task<IX> GetInnerTaskAsync();
			ValueTask<IX> GetInnerValueTaskAsync();
		}
	}
}
