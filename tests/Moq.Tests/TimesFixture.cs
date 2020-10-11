// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
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
			Assert.Equal(verifies, default(Times).Validate(count));
		}

		[Fact]
		public void AtLeastOnceRangesBetweenOneAndMaxValue()
		{
			var target = Times.AtLeastOnce();

			Assert.False(target.Validate(-1));
			Assert.False(target.Validate(0));
			Assert.True(target.Validate(1));
			Assert.True(target.Validate(5));
			Assert.True(target.Validate(int.MaxValue));
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

			Assert.False(target.Validate(-1));
			Assert.False(target.Validate(0));
			Assert.False(target.Validate(9));
			Assert.True(target.Validate(10));
			Assert.True(target.Validate(int.MaxValue));
		}

		[Fact]
		public void AtMostOnceRangesBetweenZeroAndOne()
		{
			var target = Times.AtMostOnce();

			Assert.False(target.Validate(-1));
			Assert.True(target.Validate(0));
			Assert.True(target.Validate(1));
			Assert.False(target.Validate(5));
			Assert.False(target.Validate(int.MaxValue));
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

			Assert.False(target.Validate(-1));
			Assert.True(target.Validate(0));
			Assert.True(target.Validate(6));
			Assert.True(target.Validate(10));
			Assert.False(target.Validate(11));
			Assert.False(target.Validate(int.MaxValue));
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

			Assert.False(target.Validate(0));
			Assert.False(target.Validate(9));
			Assert.True(target.Validate(10));
			Assert.True(target.Validate(14));
			Assert.True(target.Validate(20));
			Assert.False(target.Validate(21));
			Assert.False(target.Validate(int.MaxValue));
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

			Assert.False(target.Validate(0));
			Assert.False(target.Validate(10));
			Assert.True(target.Validate(11));
			Assert.True(target.Validate(14));
			Assert.True(target.Validate(19));
			Assert.False(target.Validate(20));
			Assert.False(target.Validate(int.MaxValue));
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

			Assert.False(target.Validate(-1));
			Assert.False(target.Validate(0));
			Assert.False(target.Validate(9));
			Assert.True(target.Validate(10));
			Assert.False(target.Validate(11));
			Assert.False(target.Validate(int.MaxValue));
		}

		[Fact]
		public void NeverChecksZeroTimes()
		{
			var target = Times.Never();

			Assert.False(target.Validate(-1));
			Assert.True(target.Validate(0));
			Assert.False(target.Validate(1));
			Assert.False(target.Validate(int.MaxValue));
		}

		[Fact]
		public void OnceChecksOneTime()
		{
			var target = Times.Once();

			Assert.False(target.Validate(-1));
			Assert.False(target.Validate(0));
			Assert.True(target.Validate(1));
			Assert.False(target.Validate(int.MaxValue));
		}

		public class Deconstruction
		{
			[Fact]
			public void AtLeast_n_deconstructs_to_n_MaxValue()
			{
				const int n = 42;

				var (from, to) = Times.AtLeast(n);
				Assert.Equal(n, from);
				Assert.Equal(int.MaxValue, to);
			}

			[Fact]
			public void AtLeastOnce_deconstructs_to_1_MaxValue()
			{
				var (from, to) = Times.AtLeastOnce();
				Assert.Equal(1, from);
				Assert.Equal(int.MaxValue, to);
			}

			[Fact]
			public void AtMost_n_deconstructs_to_0_n()
			{
				const int n = 42;

				var (from, to) = Times.AtMost(n);
				Assert.Equal(0, from);
				Assert.Equal(n, to);
			}

			[Fact]
			public void AtMostOnce_deconstructs_to_0_1()
			{
				var (from, to) = Times.AtMostOnce();
				Assert.Equal(0, from);
				Assert.Equal(1, to);
			}

			[Fact]
			public void BetweenExclusive_n_m_deconstructs_to__n_plus_1__m_minus_1()
			{
				const int n = 13;
				const int m = 42;
				var (from, to) = Times.Between(n, m, Range.Exclusive);
				Assert.Equal(n + 1, from);
				Assert.Equal(m - 1, to);
			}

			[Fact]
			public void BetweenInclusive_n_m_deconstructs_to_n_m()
			{
				const int n = 13;
				const int m = 42;
				var (from, to) = Times.Between(n, m, Range.Inclusive);
				Assert.Equal(n, from);
				Assert.Equal(m, to);
			}

			[Fact]
			public void Exactly_n_deconstructs_to_n_n()
			{
				const int n = 42;
				var (from, to) = Times.Exactly(n);
				Assert.Equal(n, from);
				Assert.Equal(n, to);
			}

			[Fact]
			public void Once_deconstructs_to_1_1()
			{
				var (from, to) = Times.Once();
				Assert.Equal(1, from);
				Assert.Equal(1, to);
			}

			[Fact]
			public void Never_deconstructs_to_0_0()
			{
				var (from, to) = Times.Never();
				Assert.Equal(0, from);
				Assert.Equal(0, to);
			}
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

		public class String_representation
		{
			[Fact]
			public void Default()
			{
				Assert.Equal("AtLeastOnce", default(Times).ToString());
			}

			[Fact]
			public void AtLeast()
			{
				Assert.Equal("AtLeast(123)", Times.AtLeast(123).ToString());
			}

			[Fact]
			public void AtLeastOnce()
			{
				Assert.Equal("AtLeastOnce", Times.AtLeastOnce().ToString());
			}

			[Fact]
			public void AtMost()
			{
				Assert.Equal("AtMost(123)", Times.AtMost(123).ToString());
			}

			[Fact]
			public void AtMostOnce()
			{
				Assert.Equal("AtMostOnce", Times.AtMostOnce().ToString());
			}

			[Fact]
			public void Between_Exclusive()
			{
				Assert.Equal("Between(123, 456, Exclusive)", Times.Between(123, 456, Range.Exclusive).ToString());
			}

			[Fact]
			public void Between_Inclusive()
			{
				Assert.Equal("Between(123, 456, Inclusive)", Times.Between(123, 456, Range.Inclusive).ToString());
			}

			[Fact]
			public void Exactly()
			{
				Assert.Equal("Exactly(123)", Times.Exactly(123).ToString());
			}

			[Fact]
			public void Never()
			{
				Assert.Equal("Never", Times.Never().ToString());
			}

			[Fact]
			public void Once()
			{
				Assert.Equal("Once", Times.Once().ToString());
			}
		}
	}
}
