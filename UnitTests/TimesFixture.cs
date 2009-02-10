using System;
using Xunit;

namespace Moq.Tests
{
	public class TimesFixture
	{
		[Fact]
		public void AtLeastOnceGetExceptionMessage()
		{
			Times target = Times.AtLeastOnce();

			Assert.Equal(
				"param1\r\nInvocation was not performed on the mock: param2",
				target.GetExceptionMessage("param1", "param2"));
		}

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
		public void AtLeastGetExceptionMessage()
		{
			Times target = Times.AtLeast(3);

			Assert.Equal(
				"param1\r\nInvocation was performed on the mock less than 3 times: param2",
				target.GetExceptionMessage("param1", "param2"));
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
		public void AtMostOnceGetExceptionMessage()
		{
			Times target = Times.AtMostOnce();

			Assert.Equal(
				"param1\r\nInvocation was performed on the mock more than a time: param2",
				target.GetExceptionMessage("param1", "param2"));
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
		public void AtMostGetExceptionMessage()
		{
			Times target = Times.AtMost(5);

			Assert.Equal(
				"param1\r\nInvocation was performed on the mock more than 5 times: param2",
				target.GetExceptionMessage("param1", "param2"));
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
		public void BetweenInclusiveGetExceptionMessage()
		{
			Times target = Times.Between(3, 5, Range.Inclusive);

			Assert.Equal(
				"param1\r\nInvocation was performed on the mock less than 3 times or more than 5 times: param2",
				target.GetExceptionMessage("param1", "param2"));
		}

		[Fact]
		public void BetweenExclusiveGetExceptionMessage()
		{
			Times target = Times.Between(3, 8, Range.Exclusive);

			Assert.Equal(
				"param1\r\nInvocation was performed on the mock less or equal than 3 times or more or equal than 8 times: param2",
				target.GetExceptionMessage("param1", "param2"));
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
		public void ExactlyGetExceptionMessage()
		{
			Times target = Times.Exactly(6);

			Assert.Equal(
				"param1\r\nInvocation was not performed on the mock 6 times: param2",
				target.GetExceptionMessage("param1", "param2"));
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
		public void NeverGetExceptionMessage()
		{
			Times target = Times.Never();

			Assert.Equal(
				"param1\r\nInvocation was performed on the mock: param2",
				target.GetExceptionMessage("param1", "param2"));
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
	}
}