using System;
using Xunit;

namespace Moq.Tests
{
	public class ReturnsFixture
	{
		[Fact]
		public void ReturnsValue()
		{
			var mock = new Mock<ICloneable>();
			var clone = new object();

			mock.Expect(x => x.Clone()).Returns(clone);

			Assert.Equal(clone, mock.Object.Clone());
		}

		[Fact]
		public void ReturnsNullValueIfSpecified()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(foo => foo.Execute("Whatever")).Returns((string)null);
			Assert.Null(mock.Object.Execute("Whatever"));
			mock.VerifyAll();
		}

		[Fact]
		public void ReturnsNullValueIfNullFunc()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(foo => foo.Execute("Whatever")).Returns((Func<string>)null);
			Assert.Null(mock.Object.Execute("Whatever"));
			mock.VerifyAll();
		}

		[Fact]
		public void DifferentMethodCallsReturnDifferentValues()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Execute("ping")).Returns("ack");
			mock.Expect(x => x.Execute("ping", "foo")).Returns("ack2");

			Assert.Equal("ack", mock.Object.Execute("ping"));
			Assert.Equal("ack2", mock.Object.Execute("ping", "foo"));
		}

		[Fact]
		public void DifferentArgumentsReturnDifferentValues()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute("ping")).Returns("ack");
			mock.Expect(x => x.Execute("send")).Returns("ok");

			Assert.Equal("ack", mock.Object.Execute("ping"));
			Assert.Equal("ok", mock.Object.Execute("send"));
		}

		[Fact]
		public void DifferentiatesCallWithNullArgument()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(null)).Returns("null");
			mock.Expect(x => x.Execute("ping")).Returns("ack");

			Assert.Equal("null", mock.Object.Execute(null));
			Assert.Equal("ack", mock.Object.Execute("ping"));
		}

		[Fact]
		public void ReturnsValueFromVariable()
		{
			var value = "ack";
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(null)).Returns(value);

			Assert.Equal(value, mock.Object.Execute(null));
		}

		[Fact]
		public void ReturnsValueFromLambdaLazyEvaluation()
		{
			var a = "25";
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(a.ToString())).Returns(() => a);
			a = "10";

			Assert.Equal("10", mock.Object.Execute("10"));

			a = "20";

			Assert.Equal("20", mock.Object.Execute("20"));
		}

		[Fact]
		public void PassesOneArgumentToReturns()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(It.IsAny<string>()))
				.Returns((string s) => s.ToLower());

			string result = mock.Object.Execute("blah1");
			Assert.Equal("blah1", result);
		}

		[Fact]
		public void PassesTwoArgumentsToReturns()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2) => s1 + s2);

			string result = mock.Object.Execute("blah1", "blah2");
			Assert.Equal("blah1blah2", result);
		}

		[Fact]
		public void PassesThreeArgumentsToReturns()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2, string s3) => s1 + s2 + s3);

			string result = mock.Object.Execute("blah1", "blah2", "blah3");
			Assert.Equal("blah1blah2blah3", result);
		}

		[Fact]
		public void PassesFourArgumentsToReturns()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2, string s3, string s4) => s1 + s2 + s3 + s4);

			string result = mock.Object.Execute("blah1", "blah2", "blah3", "blah4");
			Assert.Equal("blah1blah2blah3blah4", result);
		}

		public interface IFoo
		{
			void Execute();
			string Execute(string command);
			string Execute(string arg1, string arg2);
			string Execute(string arg1, string arg2, string arg3);
			string Execute(string arg1, string arg2, string arg3, string arg4);

			int Value { get; set; }
		}
	}
}
