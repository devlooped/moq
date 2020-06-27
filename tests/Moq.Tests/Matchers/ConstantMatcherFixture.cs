// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using Xunit;

namespace Moq.Tests.Matchers
{
	public class ConstantMatcherFixture
	{
		public class No_infinite_recursion
		{
			[Fact]
			public void Can_match_some_object_against_mock_object_if_mock_has_setup_for_Equals_its_object()
			{
				// This test is here to ensure that `ConstantMatcher` does not kick off infinite recursion
				// by a call equivalent to `object.Equals(mock.Object, new object())`. (This is what it
				// used to do in an earlier implementation.)

				var mock = new Mock<object>();
				mock.Setup(m => m.Equals(mock.Object)).Returns(true);
				Assert.False(mock.Object.Equals(new object()));
			}

			[Fact]
			public void Can_match_mock_object_against_itself_if_mock_has_setup_for_Equals_its_object()
			{
				// This test is here to ensure that `ConstantMatcher` also won't kick off infinite recursion
				// by a call equivalent to `object.Equals(mock.Object, mock.Object)`. (Doing the comparison
				// this way should succeed because the static `object.Equals` method starts off with a
				// reference equality check before calling into non-static `object.Equals` on the first arg.)

				var mock = new Mock<object>();
				mock.Setup(m => m.Equals(mock.Object)).Returns(true);
				Assert.True(mock.Object.Equals(mock.Object));
			}

			[Fact(Skip = "Supporting this very specific corner case would probably be more work than it is worth.")]
			public void Can_match_mock_object_against_another_mock_object_if_other_mock_has_setup_for_Equals_its_object()
			{
				// This unit test would currently trigger a `StackOverflowException`. Fixing it would
				// involve adding a guard in `ConstantMatcher` against triggering infinite recursion:
				//
				//  1. Is the first argument passed to `object.Equals` a mocked object?
				//  2. If so, does it have a setup for the `object.Equals` method?
				//  3. If so, is the mocked object specified for the setup expression's first argument?
				//
				// This seems so specific a use case that supporting it probably isn't worth the work,
				// especially given that workarounds in user code are possible (see tests below).

				var mockA = new Mock<object>();
				var mockB = new Mock<object>();
				mockA.Setup(m => m.Equals(mockA.Object)).Returns(true);
				mockB.Setup(m => m.Equals(mockB.Object)).Returns(true);
				//Assert.True(mockA.Object.Equals(mockA.Object));
				Assert.False(mockA.Object.Equals(mockB.Object));
				//Assert.False(mockB.Object.Equals(mockA.Object));
				//Assert.True(mockB.Object.Equals(mockB.Object));
			}

			[Fact]
			public void Workaround_1()
			{
				// Demonstrates one possible workaround for the limitation documented in the above test.
				// This works because Moq already auto-implements `object.Equals` for you:

				var mockA = new Mock<object>();
				var mockB = new Mock<object>();
				Assert.True(mockA.Object.Equals(mockA.Object));
				Assert.False(mockA.Object.Equals(mockB.Object));
				Assert.False(mockB.Object.Equals(mockA.Object));
				Assert.True(mockB.Object.Equals(mockB.Object));
			}

			[Fact]
			public void Workaround_2()
			{
				// Demonstrates one possible workaround for the limitation documented in the above test.
				// This works because it avoids `ConstantMatcher` and re-invoking `object.Equals`:

				var mockA = new Mock<object>();
				var mockB = new Mock<object>();
				mockA.Setup(m => m.Equals(It.Is<object>(x => object.ReferenceEquals(x, mockA.Object)))).Returns(true);
				mockB.Setup(m => m.Equals(It.Is<object>(x => object.ReferenceEquals(x, mockB.Object)))).Returns(true);
				Assert.True(mockA.Object.Equals(mockA.Object));
				Assert.False(mockA.Object.Equals(mockB.Object));
				Assert.False(mockB.Object.Equals(mockA.Object));
				Assert.True(mockB.Object.Equals(mockB.Object));
			}
		}
	}
}
