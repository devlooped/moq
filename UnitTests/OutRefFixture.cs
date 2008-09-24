using Xunit;

namespace Moq.Tests
{
	public class OutRefFixture
	{
		[Fact]
		public void ExpectsOutArgument()
		{
			var mock = new Mock<IFoo>();
			string expected = "ack";

			mock.Expect(m => m.Execute("ping", out expected)).Returns(true);

			string actual;
			bool ok = mock.Object.Execute("ping", out actual);

			Assert.True(ok);
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ExpectsRefArgument()
		{
			var mock = new Mock<IFoo>();
			string expected = "ack";

			mock.Expect(m => m.Echo(ref expected)).Returns<string>(s => s);

			string actual = mock.Object.Echo(ref expected);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public void RefOnlyMatchesSameInstance()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			string expected = "ack";

			mock.Expect(m => m.Echo(ref expected)).Returns<string>(s => s);

			string actual = null;
			Assert.Throws<MockException>(() => mock.Object.Echo(ref actual));
		}

		// ThrowsIfOutIsNotConstant
		// ThrowsIfRefIsNotConstant

		public interface IFoo
		{
			T Echo<T>(ref T value);
			bool Execute(string command, out string result);
			void Submit(string command, ref string result);
			int Value { get; set; }
		}
	}
}
