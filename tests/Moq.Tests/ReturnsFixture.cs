// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;

using Xunit;

namespace Moq.Tests
{
	public class ReturnsFixture
	{
		[Fact]
		public void ReturnsValue()
		{
			var mock = new Mock<IBar>();
			var clone = new object();

			mock.Setup(x => x.Clone()).Returns(clone);

			Assert.Equal(clone, mock.Object.Clone());
		}

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
		public void ReturnsNullValueIfNullDelegate()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(foo => foo.Execute("Whatever")).Returns((Delegate)null);
			Assert.Null(mock.Object.Execute("Whatever"));
			mock.VerifyAll();
		}

		[Fact]
		public void ReturnsNullValueIfSpecifiedForStrictMock()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(foo => foo.Execute("Whatever")).Returns((string)null);
			Assert.Null(mock.Object.Execute("Whatever"));
			mock.VerifyAll();
		}

		[Fact]
		public void ReturnsNullValueIfNullFuncForStrictMock()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(foo => foo.Execute("Whatever")).Returns((Func<string>)null);
			Assert.Null(mock.Object.Execute("Whatever"));
			mock.VerifyAll();
		}

		[Fact]
		public void ReturnsNullValueIfNullDelegateForStrictMock()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);
			mock.Setup(foo => foo.Execute("Whatever")).Returns((Delegate)null);
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

			Assert.True(mock.Object.ReturnBool());
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

		[Fact]
		public void ReturnsValueFromBaseMethod()
		{
			var mock = new Mock<Foo>();
			mock.Setup(foo => foo.Execute()).CallBase();

			Assert.Equal("Ok", mock.Object.Execute());
		}

		[Fact]
		public void ReturnsValueFromBaseProperty()
		{
			var mock = new Mock<Foo>();
			mock.SetupGet(foo => foo.StringProperty).CallBase();

			Assert.Equal("Text", mock.Object.StringProperty);
		}

		[Fact]
		public void ReturnsWithRefParameterReceivesArguments()
		{
			var input = "input";
			var received = default(string);

			var mock = new Mock<IFoo>();
			mock.Setup(f => f.Execute(ref input))
				.Returns(new ExecuteRHandler((ref string arg1) =>
				{
					received = arg1;
					return default(string);
				}));

			mock.Object.Execute(ref input);
			Assert.Equal("input", input);
			Assert.Equal(input, received);
		}

		[Fact]
		public void ReturnsWithRefParameterProducesReturnValue()
		{
			var input = default(string);

			var mock = new Mock<IFoo>();
			mock.Setup(f => f.Execute(ref input))
				.Returns(new ExecuteRHandler((ref string arg1) =>
				{
					return "result";
				}));

			var returnValue = mock.Object.Execute(ref input);
			Assert.Equal("result", returnValue);
		}

		[Fact]
		public void ReturnsWithRefParameterCanModifyRefParameter()
		{
			var value = "input";

			var mock = new Mock<IFoo>();
			mock.Setup(f => f.Execute(ref value))
				.Returns(new ExecuteRHandler((ref string arg1) =>
				{
					arg1 = "output";
					return default(string);
				}));

			Assert.Equal("input", value);
			mock.Object.Execute(ref value);
			Assert.Equal("output", value);
		}

		[Fact]
		public void ReturnsWithRefParameterCannotModifyNonRefParameter()
		{
			var _ = default(string);
			var value = "input";

			var mock = new Mock<IFoo>();
			mock.Setup(f => f.Execute(ref _, value))
				.Returns(new ExecuteRVHandler((ref string arg1, string arg2) =>
				{
					arg2 = "output";
					return default(string);
				}));

			Assert.Equal("input", value);
			mock.Object.Execute(ref _, value);
			Assert.Equal("input", value);
		}

		[Fact]
		public void Method_returning_a_Delegate_can_be_set_up_to_return_null()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(_ => _.ReturnDelegate()).Returns((Delegate)null);
			Assert.Null(mock.Object.ReturnDelegate());
		}

		[Fact]
		public void Setting_up_method_returning_a_Delegate_to_return_a_Delegate_does_not_invoke_that_Delegate()
		{
			Delegate expectedResult = new Func<Delegate>(() => null);
			var mock = new Mock<IFoo>();
			mock.Setup(_ => _.ReturnDelegate()).Returns(expectedResult);

			var actualResult = mock.Object.ReturnDelegate();

			Assert.NotNull(actualResult); // would be null if invoked
			Assert.Same(expectedResult, actualResult); // should be returned by method as is
		}

		[Fact]
		public void Given_a_loose_mock_Return_value_of_setup_without_Returns_nor_CallBase_equals_return_value_if_setup_werent_there_at_all()
		{
			const int expected = 42;

			var mock = new Mock<IFoo>(MockBehavior.Loose);
			mock.SetReturnsDefault<int>(expected);

			var actualWithoutSetup = mock.Object.Value;
			Assert.Equal(expected, actualWithoutSetup);

			mock.SetupGet(m => m.Value);

			var actualWithSetup = mock.Object.Value;
			Assert.Equal(actualWithoutSetup, actualWithSetup);
		}

		[Fact]
		public void Given_a_loose_mock_with_CallBase_Return_value_of_setup_without_Returns_nor_CallBase_equals_return_value_if_setup_werent_there_at_all()
		{
			var mock = new Mock<Foo>(MockBehavior.Loose) { CallBase = true };
			var expected = mock.Object.StringProperty;

			mock.SetupGet(m => m.StringProperty);
			var actual = mock.Object.StringProperty;

			Assert.Equal(expected, actual);
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

			string Execute(ref string arg1);
			string Execute(ref string arg1, string arg2);

			bool ReturnBool();
			IList<int> ReturnIntList();
			Delegate ReturnDelegate();

			int Value { get; set; }
		}

		public delegate string ExecuteRHandler(ref string arg1);
		public delegate string ExecuteRVHandler(ref string arg1, string arg2);
		public delegate Delegate ReturnDelegateHandler();

		public class Foo
		{
			public virtual string Execute()
			{
				return "Ok";
			}

			public virtual string StringProperty
			{
				get { return "Text"; }
			}
		}

		public interface IBar
		{
			object Clone();
		}
	}
}
