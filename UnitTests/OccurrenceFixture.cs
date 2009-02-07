using Xunit;

namespace Moq.Tests
{
	public class OccurrenceFixture
	{
		[Fact]
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
		public void ExpectsMethodNeverHappen()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Execute("ping")).Returns("ack");
			mock.Setup(m => m.Execute("ack")).Never();

			Assert.Equal("ack", mock.Object.Execute("ping"));

			MockException mex = Assert.Throws<MockException>(() => mock.Object.Execute("ack"));
			Assert.Equal(MockException.ExceptionReason.SetupNever, mex.Reason);
		}

		[Fact]
		public void VerifiesMethodNeverHappenSucceeds()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Execute("ping")).Never();

			mock.VerifyAll();
		}

		[Fact]
		public void ExpectsPropertyGetNeverHappen()
		{
			var mock = new Mock<IFoo>();

			mock.SetupGet(m => m.Value).Never();

			int value;
			MockException mex = Assert.Throws<MockException>(() => value = mock.Object.Value);
			Assert.Equal(MockException.ExceptionReason.SetupNever, mex.Reason);
		}

		[Fact]
		public void ExpectsPropertySetNeverHappen()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet(m => m.Value).Never();

			MockException mex = Assert.Throws<MockException>(() => mock.Object.Value = 5);
			Assert.Equal(MockException.ExceptionReason.SetupNever, mex.Reason);
		}

		[Fact]
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
