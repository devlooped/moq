// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
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
		}
	}
}
