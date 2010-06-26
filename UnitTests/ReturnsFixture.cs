using System;
using Xunit;
using System.Collections.Generic;

namespace Moq.Tests
{
	public class ReturnsFixture
	{
#if !SILVERLIGHT
		[Fact]
		public void ReturnsValue()
		{
			var mock = new Mock<ICloneable>();
			var clone = new object();

			mock.Setup(x => x.Clone()).Returns(clone);

			Assert.Equal(clone, mock.Object.Clone());
		}
#endif

		[Fact]
		public void ReturnsNullValueIfSpecified()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(foo => foo.Execute("Whatever")).Returns((string)null);
			Assert.Null(mock.Object.Execute("Whatever"));
			mock.VerifyAll();
		}

		[Fact]
		public void ReturnsNullValueIfNullFunc()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(foo => foo.Execute("Whatever")).Returns((Func<string>)null);
			Assert.Null(mock.Object.Execute("Whatever"));
			mock.VerifyAll();
		}

		[Fact]
		public void DifferentMethodCallsReturnDifferentValues()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Execute("ping")).Returns("ack");
			mock.Setup(x => x.Execute("ping", "foo")).Returns("ack2");

			Assert.Equal("ack", mock.Object.Execute("ping"));
			Assert.Equal("ack2", mock.Object.Execute("ping", "foo"));
		}

		[Fact]
		public void DifferentArgumentsReturnDifferentValues()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute("ping")).Returns("ack");
			mock.Setup(x => x.Execute("send")).Returns("ok");

			Assert.Equal("ack", mock.Object.Execute("ping"));
			Assert.Equal("ok", mock.Object.Execute("send"));
		}

		[Fact]
		public void DifferentiatesCallWithNullArgument()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(null)).Returns("null");
			mock.Setup(x => x.Execute("ping")).Returns("ack");

			Assert.Equal("null", mock.Object.Execute(null));
			Assert.Equal("ack", mock.Object.Execute("ping"));
		}

		[Fact]
		public void ReturnsValueFromVariable()
		{
			var value = "ack";
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(null)).Returns(value);

			Assert.Equal(value, mock.Object.Execute(null));
		}

		[Fact]
		public void ReturnsValueFromLambdaLazyEvaluation()
		{
			var a = "25";
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(a.ToString())).Returns(() => a);
			a = "10";

			Assert.Equal("10", mock.Object.Execute("10"));

			a = "20";

			Assert.Equal("20", mock.Object.Execute("20"));
		}

		[Fact]
		public void PassesOneArgumentToReturns()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>()))
				.Returns((string s) => s.ToLower());

			string result = mock.Object.Execute("blah1");
			Assert.Equal("blah1", result);
		}

		[Fact]
		public void PassesTwoArgumentsToReturns()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2) => s1 + s2);

			string result = mock.Object.Execute("blah1", "blah2");
			Assert.Equal("blah1blah2", result);
		}

		[Fact]
		public void PassesThreeArgumentsToReturns()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2, string s3) => s1 + s2 + s3);

			string result = mock.Object.Execute("blah1", "blah2", "blah3");
			Assert.Equal("blah1blah2blah3", result);
		}

		[Fact]
		public void PassesFourArgumentsToReturns()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2, string s3, string s4) => s1 + s2 + s3 + s4);

			string result = mock.Object.Execute("blah1", "blah2", "blah3", "blah4");
			Assert.Equal("blah1blah2blah3blah4", result);
		}

		[Fact]
		public void PassesFiveArgumentsToReturns()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2, string s3, string s4, string s5) => s1 + s2 + s3 + s4 + s5);

			string result = mock.Object.Execute("blah1", "blah2", "blah3", "blah4", "blah5");
			Assert.Equal("blah1blah2blah3blah4blah5", result);
		}

		[Fact]
		public void PassesSixArgumentsToReturns()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2, string s3, string s4, string s5, string s6) => s1 + s2 + s3 + s4 + s5 + s6);

			string result = mock.Object.Execute("blah1", "blah2", "blah3", "blah4", "blah5", "blah6");
			Assert.Equal("blah1blah2blah3blah4blah5blah6", result);
		}

		[Fact]
		public void PassesSevenArgumentsToReturns()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2, string s3, string s4, string s5, string s6, string s7) => s1 + s2 + s3 + s4 + s5 + s6 + s7);

			string result = mock.Object.Execute("blah1", "blah2", "blah3", "blah4", "blah5", "blah6", "blah7");
			Assert.Equal("blah1blah2blah3blah4blah5blah6blah7", result);
		}

		[Fact]
		public void PassesEightArgumentsToReturns()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8) => s1 + s2 + s3 + s4 + s5 + s6 + s7 + s8);

			string result = mock.Object.Execute("blah1", "blah2", "blah3", "blah4", "blah5", "blah6", "blah7", "blah8");
			Assert.Equal("blah1blah2blah3blah4blah5blah6blah7blah8", result);
		}

		[Fact]
		public void ReturnsDefaultValueType()
		{
			var mock = new Mock<IFoo>();
			mock.SetReturnsDefault(true);

			Assert.Equal(true, mock.Object.ReturnBool());
		}

		[Fact]
		public void ReturnsDefaultReferenceValue()
		{
			var mock = new Mock<IFoo>();
			mock.SetReturnsDefault<IList<int>>(new List<int>());

			Assert.NotNull(mock.Object.ReturnIntList());
		}

		[Fact]
		public void ReturnsDefaultValueOnProperty()
		{
			var mock = new Mock<IFoo>();
			mock.SetReturnsDefault(int.MinValue);

			Assert.Equal(int.MinValue, mock.Object.Value);
		}

		public interface IFoo
		{
			void Execute();
			string Execute(string command);
			string Execute(string arg1, string arg2);
			string Execute(string arg1, string arg2, string arg3);
			string Execute(string arg1, string arg2, string arg3, string arg4);
			string Execute(string arg1, string arg2, string arg3, string arg4, string arg5);
			string Execute(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6);
			string Execute(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7);
			string Execute(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8);
			bool ReturnBool();
			IList<int> ReturnIntList();

			int Value { get; set; }
		}
	}
}
