using System;
using Xunit;

namespace Moq.Tests
{
	public class OccurrenceFixture
	{
		[Fact]
		[Obsolete]
		public void OnceThrowsOnSecondCall()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(foo => foo.Execute("ping"))
				.Returns("ack")
				.AtMostOnce();

			Assert.Equal("ack", mock.Object.Execute("ping"));
			MockException mex = Assert.Throws<MockException>(() => mock.Object.Execute("ping"));
			Assert.Equal(MockException.ExceptionReason.MoreThanOneCall, mex.Reason);
		}

		[Fact]
		[Obsolete]
		public void RepeatThrowsOnNPlusOneCall()
		{
			var repeat = 5;
			var mock = new Mock<IFoo>();
			mock.Setup(foo => foo.Execute("ping"))
				.Returns("ack")
				.AtMost(5);

			var calls = 0;
			MockException mex = Assert.Throws<MockException>(() =>
			{
				while (calls <= repeat + 1)
				{
					mock.Object.Execute("ping");
					calls++;
				}

				Assert.True(false, "should fail on two calls");
			});

			Assert.Equal(MockException.ExceptionReason.MoreThanNCalls, mex.Reason);
			Assert.Equal(calls, repeat);
		}

		public interface IFoo
		{
			int Value { get; set; }
			int Echo(int value);
			void Submit();
			string Execute(string command);
		}
	}
}