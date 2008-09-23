using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class OccurrenceFixture
	{
		[Fact]
		public void OnceThrowsOnSecondCall()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(foo => foo.Execute("ping"))
				.Returns("ack")
				.AtMostOnce();

			try
			{
				Assert.Equal("ack", mock.Object.Execute("ping"));

				mock.Object.Execute("ping");

				Assert.True(false, "should fail on two calls");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.MoreThanOneCall, mex.Reason);
			}
		}

		[Fact]
		public void ExpectsMethodNeverHappen()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(m => m.Execute("ping")).Returns("ack");
			mock.Expect(m => m.Execute("ack")).Never();

			Assert.Equal("ack", mock.Object.Execute("ping"));

			try
			{
				mock.Object.Execute("ack");
				Assert.False(true, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.ExpectedNever, mex.Reason);
			}
		}

		[Fact]
		public void ExpectsPropertyGetNeverHappen()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectGet(m => m.Value).Never();

			try
			{
				var value = mock.Object.Value;
				Assert.False(true, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.ExpectedNever, mex.Reason);
			}
		}

		[Fact]
		public void ExpectsPropertySetNeverHappen()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectSet(m => m.Value).Never();

			try
			{
				mock.Object.Value = 5;
				Assert.False(true, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.ExpectedNever, mex.Reason);
			}
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
