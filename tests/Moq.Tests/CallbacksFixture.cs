// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

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
		public void CallbackCalledWithFiveArguments()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			string callbackArg4 = null;
			string callbackArg5 = null;
			mock.Setup(x => x.Submit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3, string s4, string s5) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; callbackArg4 = s4; callbackArg5 = s5; });

			mock.Object.Submit("blah1", "blah2", "blah3", "blah4", "blah5");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
			Assert.Equal("blah3", callbackArg3);
			Assert.Equal("blah4", callbackArg4);
			Assert.Equal("blah5", callbackArg5);
		}

		[Fact]
		public void CallbackCalledWithSixArguments()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			string callbackArg4 = null;
			string callbackArg5 = null;
			string callbackArg6 = null;
			mock.Setup(x => x.Submit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3, string s4, string s5, string s6) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; callbackArg4 = s4; callbackArg5 = s5; callbackArg6 = s6; });

			mock.Object.Submit("blah1", "blah2", "blah3", "blah4", "blah5", "blah6");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
			Assert.Equal("blah3", callbackArg3);
			Assert.Equal("blah4", callbackArg4);
			Assert.Equal("blah5", callbackArg5);
			Assert.Equal("blah6", callbackArg6);
		}

		[Fact]
		public void CallbackCalledWithSevenArguments()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			string callbackArg4 = null;
			string callbackArg5 = null;
			string callbackArg6 = null;
			string callbackArg7 = null;
			mock.Setup(x => x.Submit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3, string s4, string s5, string s6, string s7) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; callbackArg4 = s4; callbackArg5 = s5; callbackArg6 = s6; callbackArg7 = s7; });

			mock.Object.Submit("blah1", "blah2", "blah3", "blah4", "blah5", "blah6", "blah7");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
			Assert.Equal("blah3", callbackArg3);
			Assert.Equal("blah4", callbackArg4);
			Assert.Equal("blah5", callbackArg5);
			Assert.Equal("blah6", callbackArg6);
			Assert.Equal("blah7", callbackArg7);
		}

		[Fact]
		public void CallbackCalledWithEightArguments()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			string callbackArg4 = null;
			string callbackArg5 = null;
			string callbackArg6 = null;
			string callbackArg7 = null;
			string callbackArg8 = null;
			mock.Setup(x => x.Submit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; callbackArg4 = s4; callbackArg5 = s5; callbackArg6 = s6; callbackArg7 = s7; callbackArg8 = s8; });

			mock.Object.Submit("blah1", "blah2", "blah3", "blah4", "blah5", "blah6", "blah7", "blah8");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
			Assert.Equal("blah3", callbackArg3);
			Assert.Equal("blah4", callbackArg4);
			Assert.Equal("blah5", callbackArg5);
			Assert.Equal("blah6", callbackArg6);
			Assert.Equal("blah7", callbackArg7);
			Assert.Equal("blah8", callbackArg8);
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
		public void CallbackCalledWithFiveArgumentsForNonVoidMethod()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			string callbackArg4 = null;
			string callbackArg5 = null;
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3, string s4, string s5) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; callbackArg4 = s4; callbackArg5 = s5; })
				.Returns("foo");

			mock.Object.Execute("blah1", "blah2", "blah3", "blah4", "blah5");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
			Assert.Equal("blah3", callbackArg3);
			Assert.Equal("blah4", callbackArg4);
			Assert.Equal("blah5", callbackArg5);
		}

		[Fact]
		public void CallbackCalledWithSixArgumentsForNonVoidMethod()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			string callbackArg4 = null;
			string callbackArg5 = null;
			string callbackArg6 = null;
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3, string s4, string s5, string s6) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; callbackArg4 = s4; callbackArg5 = s5; callbackArg6 = s6; })
				.Returns("foo");

			mock.Object.Execute("blah1", "blah2", "blah3", "blah4", "blah5", "blah6");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
			Assert.Equal("blah3", callbackArg3);
			Assert.Equal("blah4", callbackArg4);
			Assert.Equal("blah5", callbackArg5);
			Assert.Equal("blah6", callbackArg6);
		}

		[Fact]
		public void CallbackCalledWithSevenArgumentsForNonVoidMethod()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			string callbackArg4 = null;
			string callbackArg5 = null;
			string callbackArg6 = null;
			string callbackArg7 = null;
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3, string s4, string s5, string s6, string s7) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; callbackArg4 = s4; callbackArg5 = s5; callbackArg6 = s6; callbackArg7 = s7; })
				.Returns("foo");

			mock.Object.Execute("blah1", "blah2", "blah3", "blah4", "blah5", "blah6", "blah7");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
			Assert.Equal("blah3", callbackArg3);
			Assert.Equal("blah4", callbackArg4);
			Assert.Equal("blah5", callbackArg5);
			Assert.Equal("blah6", callbackArg6);
			Assert.Equal("blah7", callbackArg7);
		}

		[Fact]
		public void CallbackCalledWithEightArgumentsForNonVoidMethod()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			string callbackArg4 = null;
			string callbackArg5 = null;
			string callbackArg6 = null;
			string callbackArg7 = null;
			string callbackArg8 = null;
			mock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3, string s4, string s5, string s6, string s7, string s8) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; callbackArg4 = s4; callbackArg5 = s5; callbackArg6 = s6; callbackArg7 = s7; callbackArg8 = s8; })
				.Returns("foo");

			mock.Object.Execute("blah1", "blah2", "blah3", "blah4", "blah5", "blah6", "blah7", "blah8");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
			Assert.Equal("blah3", callbackArg3);
			Assert.Equal("blah4", callbackArg4);
			Assert.Equal("blah5", callbackArg5);
			Assert.Equal("blah6", callbackArg6);
			Assert.Equal("blah7", callbackArg7);
			Assert.Equal("blah8", callbackArg8);
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

		[Fact]
		public void CallbackCanReceiveABaseClass()
		{
			var mock = new Mock<IInterface>(MockBehavior.Strict);
			mock.Setup(foo => foo.Method(It.IsAny<Derived>())).Callback<Derived>(TraceMe);

			mock.Object.Method(new Derived());
		}

		[Fact]
		public void CallbackCanBeImplementedByExtensionMethod()
		{
			var mock = new Mock<IFoo>();
			string receivedArgument = null;
			Action<string> innerCallback = param => { receivedArgument = param; };

			// Delegate parameter currying can confuse Moq (used by extension delegates)
			Action<string> callback = innerCallback.ExtensionCallbackHelper;
			mock.Setup(x => x.Submit(It.IsAny<string>())).Callback(callback);

			mock.Object.Submit("blah");
			Assert.Equal("blah extended", receivedArgument);
		}

		[Fact]
		public void CallbackWithRefParameterReceivesArguments()
		{
			var input = "input";
			var received = default(string);

			var mock = new Mock<IFoo>();
			mock.Setup(f => f.Execute(ref input))
				.Callback(new ExecuteRHandler((ref string arg1) =>
				{
					received = arg1;
				}));

			mock.Object.Execute(ref input);
			Assert.Equal("input", input);
			Assert.Equal(input, received);
		}

		[Fact]
		public void CallbackWithRefParameterCanModifyRefParameter()
		{
			var value = "input";

			var mock = new Mock<IFoo>();
			mock.Setup(f => f.Execute(ref value))
				.Callback(new ExecuteRHandler((ref string arg1) =>
				{
					arg1 = "output";
				}));

			Assert.Equal("input", value);
			mock.Object.Execute(ref value);
			Assert.Equal("output", value);
		}

		[Fact]
		public void CallbackWithRefParameterCannotModifyNonRefParameter()
		{
			var _ = default(string);
			var value = "input";

			var mock = new Mock<IFoo>();
			mock.Setup(f => f.Execute(ref _, value))
				.Callback(new ExecuteRVHandler((ref string arg1, string arg2) =>
				{
					arg2 = "output";
				}));

			Assert.Equal("input", value);
			mock.Object.Execute(ref _, value);
			Assert.Equal("input", value);
		}

		[Fact]
		public void CallbackWithIndexerSetter()
		{
			int x = default(int);
			int result = default(int);

			var mock = new Mock<IFoo>();
			mock.SetupSet(f => f[10] = It.IsAny<int>())
				.Callback(new Action<int, int>((x_, result_) =>
				{
					x = x_;
					result = result_;
				}));

			mock.Object[10] = 5;
			Assert.Equal(10, x);
			Assert.Equal(5, result);
		}

		[Fact]
		public void CallbackWithMultipleArgumentIndexerSetterWithoutAny()
		{
			int x = default(int);
			int y = default(int);
			int result = default(int);

			var mock = new Mock<IFoo>();
			mock.SetupSet(f => f[3, 13] = It.IsAny<int>())
				.Callback(new Action<int, int, int>((x_, y_, result_) =>
				{
					x = x_;
					y = y_;
					result = result_;
				}));

			mock.Object[3, 13] = 2;
			Assert.Equal(3, x);
			Assert.Equal(13, y);
			Assert.Equal(2, result);
		}

		[Fact]
		public void Type_of_exception_thrown_from_InvocationFunc_callback_should_be_preserved()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(m => m.Submit("good", "bad")).Returns(new InvocationFunc(invocation =>
			{
				throw new Exception("very bad");  // this used to be erroneously wrapped as a `TargetInvocationException`
			}));

			var ex = Assert.Throws<Exception>(() => mock.Object.Submit("good", "bad"));
			Assert.Equal("very bad", ex.Message);
		}

		public interface IInterface
		{
			void Method(Derived b);
		}

		public class Base
		{
		}

		public class Derived : Base
		{
		}

		private void TraceMe(Base b)
		{
		}
		
		public interface IFoo
		{
			void Submit();
			void Submit(string command);
			string Submit(string arg1, string arg2);
			string Submit(string arg1, string arg2, string arg3);
			string Submit(string arg1, string arg2, string arg3, string arg4);
			void Submit(string arg1, string arg2, string arg3, string arg4, string arg5);
			void Submit(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6);
			void Submit(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7);
			void Submit(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8);
			string Execute(string command);
			string Execute(string arg1, string arg2);
			string Execute(string arg1, string arg2, string arg3);
			string Execute(string arg1, string arg2, string arg3, string arg4);
			string Execute(string arg1, string arg2, string arg3, string arg4, string arg5);
			string Execute(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6);
			string Execute(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7);
			string Execute(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8);

			string Execute(ref string arg1);
			string Execute(ref string arg1, string arg2);

			int Value { get; set; }

			int this[int x] { get; set; }

			int this[int x, int y] { get; set; }
		}

		public delegate void ExecuteRHandler(ref string arg1);
		public delegate void ExecuteRVHandler(ref string arg1, string arg2);
	}

	static class Extensions
	{
		public static void ExtensionCallbackHelper(this Action<string> inner, string param)
		{
			inner.Invoke(param + " extended");
		}
	}
}
