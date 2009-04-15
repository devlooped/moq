using System;
using Xunit;

namespace Moq.Tests
{
	public class TimesFixture
	{
		[Fact]
		public void AtLeastOnceRangesBetweenOneAndMaxValue()
		{
			Times target = Times.AtLeastOnce();

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
			Times target = Times.AtLeast(10);

			Assert.False(target.Verify(-1));
			Assert.False(target.Verify(0));
			Assert.False(target.Verify(9));
			Assert.True(target.Verify(10));
			Assert.True(target.Verify(int.MaxValue));
		}

		[Fact]
		public void AtMostOnceRangesBetweenZeroAndOne()
		{
			Times target = Times.AtMostOnce();

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
			Times target = Times.AtMost(10);

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
			Times target = Times.Between(10, 20, Range.Inclusive);

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
			Times target = Times.Between(10, 20, Range.Exclusive);

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
			Times target = Times.Exactly(10);

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
			Times target = Times.Never();

			Assert.False(target.Verify(-1));
			Assert.True(target.Verify(0));
			Assert.False(target.Verify(1));
			Assert.False(target.Verify(int.MaxValue));
		}

		[Fact]
		public void OnceChecksOneTime()
		{
			Times target = Times.Once();

			Assert.False(target.Verify(-1));
			Assert.False(target.Verify(0));
			Assert.True(target.Verify(1));
			Assert.False(target.Verify(int.MaxValue));
		}
	}
}