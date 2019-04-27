// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class TimesFixture
	{
		[Theory]
		[InlineData(0, false)]
		[InlineData(1, true)]
		[InlineData(int.MaxValue, true)]
		public void default_ranges_between_one_and_MaxValue(int count, bool verifies)
		{
			Assert.Equal(verifies, default(Times).Verify(count));
		}

		[Fact]
		public void AtLeastOnceRangesBetweenOneAndMaxValue()
		{
			var target = Times.AtLeastOnce();

			Assert.False(target.Verify(-1));
			Assert.False(target.Verify(0));
			Assert.True(target.Verify(1));
			Assert.True(target.Verify(5));
			Assert.True(target.Verify(int.MaxValue));
		}

		[Fact]
		public void AtLeastThrowsIfTimesLessThanOne()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.AtLeast(0));
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.AtLeast(-1));
		}

		[Fact]
		public void AtLeastRangesBetweenTimesAndMaxValue()
		{
			var target = Times.AtLeast(10);

			Assert.False(target.Verify(-1));
			Assert.False(target.Verify(0));
			Assert.False(target.Verify(9));
			Assert.True(target.Verify(10));
			Assert.True(target.Verify(int.MaxValue));
		}

		[Fact]
		public void AtMostOnceRangesBetweenZeroAndOne()
		{
			var target = Times.AtMostOnce();

			Assert.False(target.Verify(-1));
			Assert.True(target.Verify(0));
			Assert.True(target.Verify(1));
			Assert.False(target.Verify(5));
			Assert.False(target.Verify(int.MaxValue));
		}

		[Fact]
		public void AtMostThrowsIfTimesLessThanZero()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.AtMost(-1));
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.AtMost(-2));
		}

		[Fact]
		public void AtMostRangesBetweenZeroAndTimes()
		{
			var target = Times.AtMost(10);

			Assert.False(target.Verify(-1));
			Assert.True(target.Verify(0));
			Assert.True(target.Verify(6));
			Assert.True(target.Verify(10));
			Assert.False(target.Verify(11));
			Assert.False(target.Verify(int.MaxValue));
		}

		[Fact]
		public void BetweenInclusiveThrowsIfFromLessThanZero()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.Between(-1, 10, Range.Inclusive));
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.Between(-2, 3, Range.Inclusive));
		}

		[Fact]
		public void BetweenInclusiveThrowsIfFromGreaterThanTo()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.Between(3, 2, Range.Inclusive));
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.Between(-3, -2, Range.Inclusive));
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.Between(0, -2, Range.Inclusive));
		}

		[Fact]
		public void BetweenInclusiveRangesBetweenFromAndTo()
		{
			var target = Times.Between(10, 20, Range.Inclusive);

			Assert.False(target.Verify(0));
			Assert.False(target.Verify(9));
			Assert.True(target.Verify(10));
			Assert.True(target.Verify(14));
			Assert.True(target.Verify(20));
			Assert.False(target.Verify(21));
			Assert.False(target.Verify(int.MaxValue));
		}

		[Fact]
		public void BetweenExclusiveThrowsIfFromLessThanZero()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.Between(-1, 10, Range.Exclusive));
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.Between(-2, 3, Range.Exclusive));
		}

		[Fact]
		public void BetweenExclusiveThrowsIfFromPlusOneGreaterThanToMinusOne()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.Between(2, 3, Range.Exclusive));
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.Between(3, 2, Range.Exclusive));
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.Between(0, -2, Range.Exclusive));
		}

		[Fact]
		public void BetweenExclusiveRangesBetweenFromPlusOneAndToMinusOne()
		{
			var target = Times.Between(10, 20, Range.Exclusive);

			Assert.False(target.Verify(0));
			Assert.False(target.Verify(10));
			Assert.True(target.Verify(11));
			Assert.True(target.Verify(14));
			Assert.True(target.Verify(19));
			Assert.False(target.Verify(20));
			Assert.False(target.Verify(int.MaxValue));
		}

		[Fact]
		public void ExactlyThrowsIfTimesLessThanZero()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.Exactly(-1));
			Assert.Throws<ArgumentOutOfRangeException>(() => Times.Exactly(-2));
		}

		[Fact]
		public void ExactlyCheckExactTimes()
		{
			var target = Times.Exactly(10);

			Assert.False(target.Verify(-1));
			Assert.False(target.Verify(0));
			Assert.False(target.Verify(9));
			Assert.True(target.Verify(10));
			Assert.False(target.Verify(11));
			Assert.False(target.Verify(int.MaxValue));
		}

		[Fact]
		public void NeverChecksZeroTimes()
		{
			var target = Times.Never();

			Assert.False(target.Verify(-1));
			Assert.True(target.Verify(0));
			Assert.False(target.Verify(1));
			Assert.False(target.Verify(int.MaxValue));
		}

		[Fact]
		public void OnceChecksOneTime()
		{
			var target = Times.Once();

			Assert.False(target.Verify(-1));
			Assert.False(target.Verify(0));
			Assert.True(target.Verify(1));
			Assert.False(target.Verify(int.MaxValue));
		}

		public class Equality
		{
#pragma warning disable xUnit2000 // Constants and literals should be the expected argument
			[Fact]
			public void default_Equals_AtLeastOnce()
			{
				Assert.Equal(Times.AtLeastOnce(), default(Times));
			}
#pragma warning restore xUnit2000

			[Fact]
			public void default_GetHashCode_equals_AtLeastOnce_GetHashCode()
			{
				Assert.Equal(Times.AtLeastOnce().GetHashCode(), default(Times).GetHashCode());
			}

			[Fact]
			public void AtMostOnce_equals_Between_0_1_inclusive()
			{
				Assert.Equal(Times.AtMostOnce(), Times.Between(0, 1, Range.Inclusive));
			}

			[Fact]
			public void Between_1_2_inclusive_equals_Between_0_3_exclusive()
			{
				Assert.Equal(Times.Between(2, 3, Range.Inclusive), Times.Between(1, 4, Range.Exclusive));
			}

			[Fact]
			public void Once_equals_Once()
			{
				Assert.Equal(Times.Once(), Times.Once());
			}

			[Fact]
			public void Once_equals_Exactly_1()
			{
				Assert.Equal(Times.Once(), Times.Exactly(1));
			}

			[Fact]
			public void Between_x_y_inclusive_does_not_equal_Between_x_y_exclusive()
			{
				const int x = 1;
				const int y = 10;
				Assert.NotEqual(Times.Between(x, y, Range.Inclusive), Times.Between(x, y, Range.Exclusive));
			}
		}
	}
}
