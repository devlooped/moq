// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Linq;

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

		public interface IX
		{
			IX Inner { get; }
			object Property { get; set; }
		}
	}
}
