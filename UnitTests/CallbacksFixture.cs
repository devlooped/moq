using System;
using Xunit;

namespace Moq.Tests
{
	public class CallbacksFixture
	{
		[Fact]
		public void ExecutesCallbackWhenVoidMethodIsCalled()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Setup(x => x.Submit()).Callback(() => called = true);

			mock.Object.Submit();
			Assert.True(called);
		}

		[Fact]
		public void ExecutesCallbackWhenNonVoidMethodIsCalled()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Setup(x => x.Execute("ping")).Callback(() => called = true).Returns("ack");

			Assert.Equal("ack", mock.Object.Execute("ping"));
			Assert.True(called);
		}

		[Fact]
		public void CallbackCalledWithoutArgumentsForMethodCallWithArguments()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Setup(x => x.Submit(It.IsAny<string>())).Callback(() => called = true);

			mock.Object.Submit("blah");
			Assert.True(called);
		}

		[Fact]
		public void FriendlyErrorWhenCallbackArgumentCountNotMatch()
		{
			var mock = new Mock<IFoo>();

			Assert.Throws<ArgumentException>(() => 
				mock.Setup(x => x.Submit(It.IsAny<string>()))
					.Callback((string s1, string s2) => System.Console.WriteLine(s1 + s2)));
		}

		[Fact]
		public void CallbackCalledWithOneArgument()
		{
			var mock = new Mock<IFoo>();
			string callbackArg = null;
			mock.Setup(x => x.Submit(It.IsAny<string>())).Callback((string s) => callbackArg = s);

			mock.Object.Submit("blah");
			Assert.Equal("blah", callbackArg);
		}

		[Fact]
		public void CallbackCalledWithTwoArguments()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			mock.Setup(x => x.Submit(It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2) => { callbackArg1 = s1; callbackArg2 = s2; });

			mock.Object.Submit("blah1", "blah2");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
		}

		[Fact]
		public void CallbackCalledWithThreeArguments()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			mock.Setup(x => x.Submit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; });

			mock.Object.Submit("blah1", "blah2", "blah3");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
			Assert.Equal("blah3", callbackArg3);
		}

		[Fact]
		public void CallbackCalledWithFourArguments()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			string callbackArg4 = null;
			mock.Setup(x => x.Submit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3, string s4) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; callbackArg4 = s4; });

			mock.Object.Submit("blah1", "blah2", "blah3", "blah4");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
			Assert.Equal("blah3", callbackArg3);
			Assert.Equal("blah4", callbackArg4);
		}

		[Fact]
		public void CallbackCalledWithOneArgumentForNonVoidMethod()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			mock.Setup(x => x.Execute(It.IsAny<string>()))
				.Callback((string s1) => callbackArg1 = s1)
				.Returns("foo");

			mock.Object.Execute("blah1");
			Assert.Equal("blah1", callbackArg1);
		}

		[Fact]
		public void CallbackCalledWithTwoArgumentsForNonVoidMethod()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2) => { callbackArg1 = s1; callbackArg2 = s2; })
				.Returns("foo");

			mock.Object.Execute("blah1", "blah2");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
		}

		[Fact]
		public void CallbackCalledWithThreeArgumentsForNonVoidMethod()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; })
				.Returns("foo");

			mock.Object.Execute("blah1", "blah2", "blah3");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
			Assert.Equal("blah3", callbackArg3);
		}

		[Fact]
		public void CallbackCalledWithFourArgumentsForNonVoidMethod()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			string callbackArg4 = null;
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3, string s4) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; callbackArg4 = s4; })
				.Returns("foo");

			mock.Object.Execute("blah1", "blah2", "blah3", "blah4");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
			Assert.Equal("blah3", callbackArg3);
			Assert.Equal("blah4", callbackArg4);
		}

		[Fact]
		public void CallbackCalledAfterReturnsCall()
		{
			var mock = new Mock<IFoo>();
			bool returnsCalled = false;
			bool beforeCalled = false;
			bool afterCalled = false;

			mock.Setup(foo => foo.Execute("ping"))
				.Callback(() => { Assert.False(returnsCalled); beforeCalled = true; })
				.Returns(() => { returnsCalled = true; return "ack"; })
				.Callback(() => { Assert.True(returnsCalled); afterCalled = true; });

			Assert.Equal("ack", mock.Object.Execute("ping"));

			Assert.True(beforeCalled);
			Assert.True(afterCalled);
		}

		[Fact]
		public void CallbackCalledAfterReturnsCallWithArg()
		{
			var mock = new Mock<IFoo>();
			bool returnsCalled = false;

			mock.Setup(foo => foo.Execute(It.IsAny<string>()))
				.Callback<string>(s => Assert.False(returnsCalled))
				.Returns(() => { returnsCalled = true; return "ack"; })
				.Callback<string>(s => Assert.True(returnsCalled));

			mock.Object.Execute("ping");

			Assert.True(returnsCalled);
		}

		public interface IFoo
		{
			void Submit();
			void Submit(string command);
			string Submit(string arg1, string arg2);
			string Submit(string arg1, string arg2, string arg3);
			string Submit(string arg1, string arg2, string arg3, string arg4);
			string Execute(string command);
			string Execute(string arg1, string arg2);
			string Execute(string arg1, string arg2, string arg3);
			string Execute(string arg1, string arg2, string arg3, string arg4);

			int Value { get; set; }
		}
	}
}
