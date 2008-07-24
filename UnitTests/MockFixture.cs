using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Xunit;

namespace Moq.Tests
{
	public class MockFixture
	{
		[Fact]
		public void CreatesMockAndExposesInterface()
		{
			var mock = new Mock<ICloneable>();

			ICloneable cloneable = mock.Object;

			Assert.NotNull(cloneable);
		}

		[Fact]
		public void ThrowsIfNullExpectAction()
		{
			var mock = new Mock<ICloneable>();

			Assert.Throws<ArgumentNullException>(() => mock.Expect((Expression<Action<ICloneable>>)null));
		}

		[Fact]
		public void ThrowsIfNullExpectFunction()
		{
			var mock = new Mock<ICloneable>();

			Assert.Throws<ArgumentNullException>(() => mock.Expect((Expression<Func<ICloneable, string>>)null));
		}

		[Fact]
		public void ReturnsValue()
		{
			var mock = new Mock<ICloneable>();
			var clone = new object();

			mock.Expect(x => x.Clone()).Returns(clone);

			Assert.Equal(clone, mock.Object.Clone());
		}

		[Fact]
		public void DifferentMethodCallsReturnDifferentValues()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoReturnInt()).Returns(1);
			mock.Expect(x => x.DoReturnString()).Returns("foo");

			Assert.Equal(1, mock.Object.DoReturnInt());
			Assert.Equal("foo", mock.Object.DoReturnString());
		}

		[Fact]
		public void MethodCallsWithDifferentArgumentsReturnDifferentValues()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoIntArgReturnInt(1)).Returns(11);
			mock.Expect(x => x.DoIntArgReturnInt(2)).Returns(22);

			Assert.Equal(11, mock.Object.DoIntArgReturnInt(1));
			Assert.Equal(22, mock.Object.DoIntArgReturnInt(2));
		}

		[Fact]
		public void DifferentiatesCallWithNullArgument()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoStringArgReturnInt(null)).Returns(1);
			mock.Expect(x => x.DoStringArgReturnInt("foo")).Returns(2);

			Assert.Equal(1, mock.Object.DoStringArgReturnInt(null));
			Assert.Equal(2, mock.Object.DoStringArgReturnInt("foo"));
		}

		[Fact]
		public void ReturnsValueFromVariable()
		{
			int value = 25;
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoStringArgReturnInt(null)).Returns(value);

			Assert.Equal(value, mock.Object.DoStringArgReturnInt(null));
		}

		[Fact]
		public void ReturnsValueFromLambdaLazyEvaluation()
		{
			int a = 25;
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoStringArgReturnInt(a.ToString())).Returns(() => a);
			a = 10;

			Assert.Equal(10, mock.Object.DoStringArgReturnInt("10"));

			a = 20;

			Assert.Equal(20, mock.Object.DoStringArgReturnInt("20"));
		}

		[Fact]
		public void ThrowsIfExpectationSetForField()
		{
			var mock = new Mock<FooBase>();

			try
			{
				mock.Expect(x => x.ValueField);
				Assert.True(false, "Shouldn't have reached here");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.ExpectedMethodOrProperty, mex.Reason);
			}
		}

		[Fact]
		public void CallParameterCanBeVariable()
		{
			int value = 5;
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Duplicate(value)).Returns(() => value * 2);

			Assert.Equal(value * 2, mock.Object.Duplicate(value));
		}

		[Fact]
		public void CallParameterCanBeMethodCall()
		{
			int value = 5;
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Duplicate(GetValue(value))).Returns(() => value * 2);

			Assert.Equal(value * 2, mock.Object.Duplicate(value * 2));
		}

		private int GetValue(int value)
		{
			return value * 2;
		}

		[Fact]
		public void MatchesAnyParameterValue()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Duplicate(It.IsAny<int>())).Returns(5);
			mock.Expect(x => x.Execute(It.IsAny<string>())).Returns("foo");

			Assert.Equal(5, mock.Object.Duplicate(5));
			Assert.Equal(5, mock.Object.Duplicate(25));
			Assert.Equal("foo", mock.Object.Execute("hello"));
			Assert.Equal("foo", mock.Object.Execute((string)null));
		}

		[Fact]
		public void MatchesPredicateParameter()
		{
			var mock = new Mock<IFoo>();

			mock.
				Expect(x => x.Duplicate(It.Is<int>(value => value < 5 && value > 0))).
				Returns(1);
			mock.
				Expect(x => x.Duplicate(It.Is<int>(value => value <= 0))).
				Returns(0);
			mock.
				Expect(x => x.Duplicate(It.Is<int>(value => value >= 5))).
				Returns(2);

			Assert.Equal(1, mock.Object.Duplicate(3));
			Assert.Equal(0, mock.Object.Duplicate(0));
			Assert.Equal(0, mock.Object.Duplicate(-5));
			Assert.Equal(2, mock.Object.Duplicate(5));
			Assert.Equal(2, mock.Object.Duplicate(6));
		}

		[Fact]
		public void ExpectsVoidCall()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Execute());

			mock.Object.Execute();
		}

		[Fact]
		public void ThrowsIfExpectationThrows()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoReturnInt()).Throws(new FormatException());

			Assert.Throws<FormatException>(() => mock.Object.DoReturnInt());
		}

		[Fact]
		public void ThrowsIfExpectationThrowsWithGenericsExceptionType()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoReturnInt()).Throws<FormatException>();

			Assert.Throws<FormatException>(() => mock.Object.DoReturnInt());
		}

		[Fact]
		public void ExecutesCallbackWhenVoidMethodIsCalled()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Expect(x => x.Execute()).Callback(() => called = true);

			mock.Object.Execute();
			Assert.True(called);
		}

		[Fact]
		public void ExecutesCallbackWhenNonVoidMethodIsCalled()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Expect(x => x.DoReturnInt()).Callback(() => called = true).Returns(1);

			Assert.Equal(1, mock.Object.DoReturnInt());
			Assert.True(called);
		}

		[Fact]
		public void MatchesRanges()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoIntArgReturnInt(It.IsInRange(1, 5, Range.Inclusive))).Returns(1);
			mock.Expect(x => x.DoIntArgReturnInt(It.IsInRange(6, 10, Range.Exclusive))).Returns(2);

			Assert.Equal(1, mock.Object.DoIntArgReturnInt(1));
			Assert.Equal(1, mock.Object.DoIntArgReturnInt(2));
			Assert.Equal(1, mock.Object.DoIntArgReturnInt(5));

			Assert.Equal(2, mock.Object.DoIntArgReturnInt(7));
			Assert.Equal(2, mock.Object.DoIntArgReturnInt(9));
		}

		[Fact]
		public void DoesNotMatchOutOfRange()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);

			mock.Expect(x => x.DoIntArgReturnInt(It.IsInRange(1, 5, Range.Exclusive))).Returns(1);

			Assert.Equal(1, mock.Object.DoIntArgReturnInt(2));

			try
			{
				int throwHere = mock.Object.DoIntArgReturnInt(1);
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.NoExpectation, mex.Reason);
			}
		}

		[Fact]
		public void RangesCanIncludeVariableAndMethodInvocation()
		{
			var mock = new Mock<IFoo>();
			var from = 1;

			mock.Expect(x => x.DoIntArgReturnInt(It.IsInRange(from, GetToRange(), Range.Inclusive))).Returns(1);

			Assert.Equal(1, mock.Object.DoIntArgReturnInt(1));
		}

		[Fact]
		public void RangesAreLazyEvaluated()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);
			var from = "a";
			var to = "d";

			mock.Expect(x => x.DoStringArgReturnInt(It.IsInRange(from, to, Range.Inclusive))).Returns(1);

			Assert.Equal(1, mock.Object.DoStringArgReturnInt("b"));

			from = "c";

			Assert.Equal(default(int), mock.Object.DoStringArgReturnInt("b"));
		}

		[Fact]
		public void RegexMatchesAndLazyEvaluates()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);
			var reg = "[a-d]+";

			mock.Expect(x => x.DoStringArgReturnInt(It.IsRegex(reg, RegexOptions.IgnoreCase))).Returns(2);
			mock.Expect(x => x.DoStringArgReturnInt(It.IsRegex(reg))).Returns(1);

			Assert.Equal(1, mock.Object.DoStringArgReturnInt("b"));
			Assert.Equal(1, mock.Object.DoStringArgReturnInt("abc"));
			Assert.Equal(2, mock.Object.DoStringArgReturnInt("B"));
			Assert.Equal(2, mock.Object.DoStringArgReturnInt("BC"));

			reg = "[c-d]+";

			// Will not match neither the 1 and 2 return values we had.
			Assert.Equal(default(int), mock.Object.DoStringArgReturnInt("b"));
		}

		[Fact]
		public void ReturnsServiceFromServiceProvider()
		{
			var provider = new Mock<IServiceProvider>();

			provider.Expect(x => x.GetService(typeof(IFooService))).Returns(new FooService());

			Assert.True(provider.Object.GetService(typeof(IFooService)) is FooService);
		}

		[Fact]
		public void InvokesLastExpectationThatMatches()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(It.IsAny<string>())).Throws(new ArgumentException());
			mock.Expect(x => x.Execute("ping")).Returns("I'm alive!");

			Assert.Equal("I'm alive!", mock.Object.Execute("ping"));

			Assert.Throws<ArgumentException>(() => mock.Object.Execute("asdf"));
		}

		[Fact]
		public void MatchesEvenNumbersWithLambdaMatching()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoIntArgReturnInt(It.Is<int>(i => i % 2 == 0))).Returns(1);

			Assert.Equal(1, mock.Object.DoIntArgReturnInt(2));
		}

		[Fact]
		public void MockObjectIsAssignableToMockedInterface()
		{
			var mock = new Mock<IFoo>();
			Assert.True(typeof(IFoo).IsAssignableFrom(mock.Object.GetType()));
		}

		[Fact]
		public void MockObjectsEqualityIsReferenceEquals()
		{
			var mock1 = new Mock<IFoo>();
			var mock2 = new Mock<IFoo>();

			Assert.True(mock1.Object.Equals(mock1.Object));
			Assert.False(mock1.Object.Equals(mock2.Object));
		}

		[Fact]
		public void HashCodeIsDifferentForEachMock()
		{
			var mock1 = new Mock<IFoo>();
			var mock2 = new Mock<IFoo>();

			Assert.Equal(mock1.Object.GetHashCode(), mock1.Object.GetHashCode());
			Assert.Equal(mock2.Object.GetHashCode(), mock2.Object.GetHashCode());
			Assert.NotEqual(mock1.Object.GetHashCode(), mock2.Object.GetHashCode());
		}

		[Fact]
		public void ToStringIsNullOrEmpty()
		{
			var mock = new Mock<IFoo>();
			Assert.False(String.IsNullOrEmpty(mock.Object.ToString()));
		}

		[Fact(Skip = "Castle.DynamicProxy2 doesn't seem to call interceptors for ToString, GetHashCode")]
		public void OverridesObjectMethods()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.GetHashCode()).Returns(1);
			mock.Expect(x => x.ToString()).Returns("foo");

			Assert.Equal("foo", mock.Object.ToString());
			Assert.Equal(1, mock.Object.GetHashCode());
		}

		[Fact]
		public void OverridesBehaviorFromAbstractClass()
		{
			var mock = new Mock<FooBase>();
			mock.CallBase = true;

			mock.Expect(x => x.Check("foo")).Returns(false);

			Assert.False(mock.Object.Check("foo"));
			Assert.True(mock.Object.Check("bar"));
		}

		[Fact]
		public void CallsUnderlyingClassEquals()
		{
			var mock = new Mock<FooOverrideEquals>();
			var mock2 = new Mock<FooOverrideEquals>();

			mock.CallBase = true;

			mock.Object.Name = "Foo";
			mock2.Object.Name = "Foo";

			Assert.True(mock.Object.Equals(mock2.Object));
		}

		[Fact]
		public void ThrowsIfSealedClass()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooSealed>());
		}

		[Fact]
		public void ThrowsIfVerifiableExpectationNotCalled()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoReturnInt())
				.Returns(5)
				.Verifiable();

			try
			{
				mock.Verify();
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void ThrowsWithEvaluatedExpressionsIfVerifiableExpectationNotCalled()
		{
			var expectedArg = "lorem,ipsum";
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoStringArgReturnInt(expectedArg.Substring(0, 5)))
				.Returns(5)
				.Verifiable();

			try
			{
				mock.Verify();
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
				Assert.True(mex.Message.Contains(@".DoStringArgReturnInt(""lorem"")"), "Contains evaluated expected argument.");
			}
		}

		[Fact]
		public void ThrowsWithExpressionIfVerifiableExpectationWithLambdaMatcherNotCalled()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoStringArgReturnInt(It.Is<string>(s => string.IsNullOrEmpty(s))))
				.Returns(5)
				.Verifiable();

			try
			{
				mock.Verify();
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
				Assert.True(mex.Message.Contains(@".DoStringArgReturnInt(Is<String>(s => IsNullOrEmpty(s)))"), "Contains evaluated expected argument.");
			}
		}

		[Fact]
		public void VerifiesNoOpIfNoVerifiableExpectations()
		{
			var mock = new Mock<IFoo>();

			mock.Verify();
		}

		[Fact]
		public void ThrowsIfVerifyAllNotMet()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoReturnInt())
				.Returns(1);

			try
			{
				mock.VerifyAll();
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void RendersReadableMessageForVerifyFailures()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoReturnInt());
			mock.Expect(x => x.DoReturnString());
			mock.Expect(x => x.DoStringArgReturnInt("Hello World"));

			try
			{
				mock.VerifyAll();
				Assert.True(false, "Should have thrown");
			}
			catch (Exception ex)
			{
				Assert.True(ex.Message.Contains("x => x.DoReturnInt()"));
				Assert.True(ex.Message.Contains("x => x.DoReturnString()"));
				Assert.True(ex.Message.Contains("x => x.DoStringArgReturnInt(\"Hello World\")"));
			}
		}

		[Fact]
		public void ThrowsIfExpectOnNonVirtual()
		{
			var mock = new Mock<FooBase>();

			Assert.Throws<ArgumentException>(() => mock.Expect(x => x.True()).Returns(false));
		}

		[Fact]
		public void OverridesPreviousExpectation()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoReturnInt()).Returns(5);

			Assert.Equal(5, mock.Object.DoReturnInt());

			mock.Expect(x => x.DoReturnInt()).Returns(10);

			Assert.Equal(10, mock.Object.DoReturnInt());
		}

		[Fact]
		public void ConstructsObjectsWithCtorArguments()
		{
			var mock = new Mock<FooWithConstructors>(MockBehavior.Default, "Hello", 26);

			Assert.Equal("Hello", mock.Object.StringValue);
			Assert.Equal(26, mock.Object.IntValue);

			// Should also construct without args.
			mock = new Mock<FooWithConstructors>(MockBehavior.Default);

			Assert.Equal(null, mock.Object.StringValue);
			Assert.Equal(0, mock.Object.IntValue);
		}

		[Fact]
		public void ConstructsClassWithNoDefaultConstructor()
		{
			var mock = new Mock<ClassWithNoDefaultConstructor>(MockBehavior.Default, "Hello", 26);

			Assert.Equal("Hello", mock.Object.StringValue);
			Assert.Equal(26, mock.Object.IntValue);
		}

		[Fact]
		public void ConstructsClassWithNoDefaultConstructorAndNullValue()
		{
			var mock = new Mock<ClassWithNoDefaultConstructor>(MockBehavior.Default, null, 26);

			Assert.Equal(null, mock.Object.StringValue);
			Assert.Equal(26, mock.Object.IntValue);
		}

		[Fact]
		public void ThrowsIfNoMatchingConstructorFound()
		{
			Assert.Throws<ArgumentException>(() => 
			{
				Console.WriteLine(new Mock<ClassWithNoDefaultConstructor>(25, true).Object);
			});
		}

		[Fact]
		public void ThrowsIfArgumentsPassedForInterface()
		{
			Assert.Throws<ArgumentException>(() => new Mock<IFoo>(25, true));
		}

		[Fact]
		public void CallbackCalledWithoutArgumentsForMethodCallWithArguments()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Expect(x => x.DoVoidArgs(It.IsAny<string>())).Callback(() => called = true);

			mock.Object.DoVoidArgs("blah");
			Assert.True(called);
		}

		[Fact]
		public void CallbackCalledWithOneArgument()
		{
			var mock = new Mock<IFoo>();
			string callbackArg = null;
			mock.Expect(x => x.DoVoidArgs(It.IsAny<string>())).Callback((string s) => callbackArg = s);

			mock.Object.DoVoidArgs("blah");
			Assert.Equal("blah", callbackArg);
		}

		[Fact]
		public void CallbackCalledWithTwoArguments()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			mock.Expect(x => x.DoVoidArgs(It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2) => { callbackArg1 = s1; callbackArg2 = s2; });

			mock.Object.DoVoidArgs("blah1", "blah2");
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
			mock.Expect(x => x.DoVoidArgs(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; });

			mock.Object.DoVoidArgs("blah1", "blah2", "blah3");
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
			mock.Expect(x => x.DoVoidArgs(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3, string s4) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; callbackArg4 = s4; });

			mock.Object.DoVoidArgs("blah1", "blah2", "blah3", "blah4");
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
			mock.Expect(x => x.Execute(It.IsAny<string>()))
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
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>()))
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
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3, string s4) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; callbackArg4 = s4; })
				.Returns("foo");

			mock.Object.Execute("blah1", "blah2", "blah3", "blah4");
			Assert.Equal("blah1", callbackArg1);
			Assert.Equal("blah2", callbackArg2);
			Assert.Equal("blah3", callbackArg3);
			Assert.Equal("blah4", callbackArg4);
		}

		[Fact]
		public void ReturnsCalledUsingOneArgument()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(It.IsAny<string>()))
				.Returns((string s) => s.ToLower());

			string result = mock.Object.Execute("blah1");
			Assert.Equal("blah1", result);
		}

		[Fact]
		public void ReturnsCalledUsingTwoArguments()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2) => s1 + s2);

			string result = mock.Object.Execute("blah1", "blah2");
			Assert.Equal("blah1blah2", result);
		}

		[Fact]
		public void ReturnsCalledUsingThreeArguments()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2, string s3) => s1 + s2 + s3);

			string result = mock.Object.Execute("blah1", "blah2", "blah3");
			Assert.Equal("blah1blah2blah3", result);
		}

		[Fact]
		public void ReturnsCalledUsingFourArguments()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2, string s3, string s4) => s1 + s2 + s3 + s4);

			string result = mock.Object.Execute("blah1", "blah2", "blah3", "blah4");
			Assert.Equal("blah1blah2blah3blah4", result);
		}

		[Fact]
		public void MatchsDifferentOverloadsWithItIsAny()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(foo => foo.DoTypeOverload(It.IsAny<Bar>()))
				.Returns(true);
			mock.Expect(foo => foo.DoTypeOverload(It.IsAny<Baz>()))
				.Returns(false);

			bool bar = mock.Object.DoTypeOverload(new Bar());
			bool baz = mock.Object.DoTypeOverload(new Baz());

			Assert.True(bar);
			Assert.False(baz);
		}

		[Fact]
		public void OnceThrowsOnSecondCall()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(foo => foo.DoVoidArgs("foo"))
				.AtMostOnce();

			try
			{
				mock.Object.DoVoidArgs("foo");
				mock.Object.DoVoidArgs("foo");

				Assert.True(false, "should fail on two calls");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.MoreThanOneCall, mex.Reason);
			}
		}

		[Fact]
		public void ThrowsOnStrictWithExpectButNoReturns()
		{
			var mock = new Mock<IFoo>(MockBehavior.Strict);

			mock.Expect(x => x.DoReturnString());

			try
			{
				mock.Object.DoReturnString();
				Assert.True(false, "SHould throw");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.ReturnValueRequired, mex.Reason);
			}
		}

		[Fact]
		public void ReturnsNullValueIfSpecified()
		{
			var mock = new Mock<IBag>();
			mock.Expect(bar => bar.Get("Whatever")).Returns(null);
			Assert.Null(mock.Object.Get("Whatever"));
			mock.VerifyAll();
		}

		[Fact]
		public void ReturnsNullValueIfNullFunc()
		{
			var mock = new Mock<IBag>();
			mock.Expect(bar => bar.Get("Whatever")).Returns((Func<object>)null);
			Assert.Null(mock.Object.Get("Whatever"));
			mock.VerifyAll();
		}

		[Fact]
		public void ExpectsPropertySetter()
		{
			var mock = new Mock<IFoo>();

			int value = 0;

			mock.ExpectSet(foo => foo.ValueProperty)
				.Callback(i => value = i);

			mock.Object.ValueProperty = 5;

			Assert.Equal(5, value);
		}

		[Fact]
		public void ExpectsPropertyGetter()
		{
			var mock = new Mock<IFoo>();

			bool called = false;

			mock.ExpectGet(x => x.ValueProperty)
				.Callback(() => called = true)
				.Returns(25);

			Assert.Equal(25, mock.Object.ValueProperty);
			Assert.True(called);
		}

		[Fact]
		public void ThrowsIfExpectPropertySetterOnMethod()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.ExpectSet(foo => foo.DoIntArgReturnInt(5));
				Assert.True(false, "Should throw on ExpectSet on method instead of property.");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.ExpectedProperty, mex.Reason);
			}
		}

		[Fact]
		public void ThrowsIfExpectPropertyGetterOnMethod()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.ExpectGet(foo => foo.DoIntArgReturnInt(5));
				Assert.True(false, "Should throw on ExpectGet on method instead of property.");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.ExpectedProperty, mex.Reason);
			}
		}

		[Fact]
		public void DoesNotCallBaseClassVirtualImplementationByDefault()
		{
			var mock = new Mock<FooBase>();

			Assert.False(mock.Object.BaseCalled);
			mock.Object.BaseCall();

			Assert.False(mock.Object.BaseCalled);
		}

		[Fact]
		public void DoesNotCallBaseClassVirtualImplementationIfSpecified()
		{
			var mock = new Mock<FooBase>();

			mock.CallBase = false;

			Assert.False(mock.Object.BaseCalled);
			mock.Object.BaseCall();

			Assert.Equal(default(bool), mock.Object.BaseCall("foo"));
			Assert.False(mock.Object.BaseCalled);
		}

		[Fact]
		public void ExpectsGetIndexedProperty()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectGet(foo => foo[0])
				.Returns(1);
			mock.ExpectGet(foo => foo[1])
				.Returns(2);

			Assert.Equal(1, mock.Object[0]);
			Assert.Equal(2, mock.Object[1]);
		}

		[Fact]
		public void ExpectAndExpectGetAreSynonyms()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectGet(foo => foo.ValueProperty)
				.Returns(1);
			mock.Expect(foo => foo.ValueProperty)
				.Returns(2);

			Assert.Equal(2, mock.Object.ValueProperty);
		}

		[Fact]
		public void ThrowsIfItIsWithoutLambda()
		{
			var foo = new Mock<IFoo>();

			Expression<Predicate<int>> isSix = (arg) => arg == 6;

			try
			{
				foo.Expect((f) => f.DoIntArgReturnInt(It.Is(isSix))).Returns(12);
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.ExpectedLambda, mex.Reason);
			}
		}

		[Fact]
		public void ThrowsIfExpecationSetForNotOverridableMember()
		{
			var target = new Mock<Doer>();

			Assert.Throws<ArgumentException>(() => target.Expect(t => t.Do()));
		}

		[Fact]
		public void ThrowsIfExpectGetOnPropertyWithPrivateSetter()
		{
			var mock = new Mock<FooWithPrivateSetter>();

			Assert.Throws<ArgumentException>(() => mock.ExpectSet(m => m.Foo));
		}

		[Fact]
		public void CallbackCalledAfterReturnsCall()
		{
			var mock = new Mock<IFoo>();
			bool returnsCalled = false;
			bool beforeCalled = false;
			bool afterCalled = false;

			mock.Expect(foo => foo.DoReturnInt())
				.Callback(() => { Assert.False(returnsCalled); beforeCalled = true; })
				.Returns(() => { returnsCalled = true; return 5; })
				.Callback(() => { Assert.True(returnsCalled); afterCalled = true; });

			mock.Object.DoReturnInt();

			Assert.True(beforeCalled);
			Assert.True(afterCalled);
		}

		[Fact]
		public void CallbackCalledAfterReturnsCallWithArg()
		{
			var mock = new Mock<IFoo>();
			bool returnsCalled = false;

			mock.Expect(foo => foo.DoStringArgReturnInt(It.IsAny<string>()))
				.Callback<string>(s => Assert.False(returnsCalled))
				.Returns(() => { returnsCalled = true; return 5; })
				.Callback<string>(s => Assert.True(returnsCalled));

			mock.Object.DoStringArgReturnInt("foo");
		}

		[Fact]
		public void ThrowsIfVerifyVoidMethodWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.Verify(f => f.Execute());
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void VerifiesVoidMethodWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Execute();

			mock.Verify(f => f.Execute());
		}

		[Fact]
		public void ThrowsIfVerifyReturningMethodWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.Verify(f => f.Execute("ping"));
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void VerifiesReturningMethodWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Execute("ping");

			mock.Verify(f => f.Execute("ping"));
		}

		[Fact]
		public void VerifiesPropertyGetWithExpression()
		{
			var mock = new Mock<IFoo>();

			var v = mock.Object.ValueProperty;

			mock.VerifyGet(f => f.ValueProperty);
		}

		[Fact]
		public void ThrowsIfVerifyPropertyGetWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.VerifyGet(f => f.ValueProperty);
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void VerifiesPropertySetWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.Object.ValueProperty = 5;

			mock.VerifySet(f => f.ValueProperty);
		}

		[Fact]
		public void ThrowsIfVerifyPropertySetWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.VerifySet(f => f.ValueProperty);
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void ShouldThrowIfAsIsInvokedAfterInstanceIsRetrieved()
		{
			var mock = new Mock<IBag>();

			var instance = mock.Object;

			Assert.Throws<InvalidOperationException>(() => mock.As<IFoo>());
		}

		[Fact]
		public void ShouldThrowIfAsIsInvokedWithANonInterfaceTypeParameter()
		{
			var mock = new Mock<IBag>();

			Assert.Throws<ArgumentException>(() => mock.As<object>());
		}

		[Fact]
		public void ShouldExpectGetOnANewInterface()
		{
			var mock = new Mock<IBag>();

			bool called = false;

			mock.As<IFoo>().ExpectGet(x => x.ValueProperty)
				.Callback(() => called = true)
				.Returns(25);

			Assert.Equal(25, ((IFoo)mock.Object).ValueProperty);
			Assert.True(called);
		}

		[Fact]
		public void ShouldExpectCallWithArgumentOnNewInterface()
		{
			var mock = new Mock<IBag>();
			mock.As<IFoo>().Expect(x => x.DoIntArgReturnInt(100)).Returns(10);

			Assert.Equal(10, ((IFoo)mock.Object).DoIntArgReturnInt(100));
		}

		[Fact]
		public void ShouldExpectPropertySetterOnNewInterface()
		{
			bool called = false;
			int value = 0;
			var mock = new Mock<IBag>();
			mock.As<IFoo>().ExpectSet(x => x.ValueProperty).Callback(i => { value = i; called = true; });

			((IFoo)mock.Object).ValueProperty = 100;

			Assert.Equal(100, value);
			Assert.True(called);
		}

		[Fact]
		public void MockAsExistingInterfaceAfterObjectSucceedsIfNotNew()
		{
			var mock = new Mock<IBag>();

			mock.As<IFoo>().ExpectGet(x => x.ValueProperty).Returns(25);

			Assert.Equal(25, ((IFoo)mock.Object).ValueProperty);

			var fm = mock.As<IFoo>();

			fm.Expect(f => f.Execute());
		}

		[Fact]
		public void GetMockFromAddedInterfaceWorks()
		{
			var bag = new Mock<IBag>();
			var foo = bag.As<IFoo>();

			foo.ExpectGet(x => x.ValueProperty).Returns(25);

			IFoo f = bag.Object as IFoo;

			var foomock = Mock.Get(f);

			Assert.NotNull(foomock);
		}

		[Fact]
		public void GetMockFromNonAddedInterfaceThrows()
		{
			var bag = new Mock<IBag>();
			bag.As<IFoo>();
			bag.As<ICloneable>();
			object b = bag.Object;

			Assert.Throws<ArgumentException>(() => Mock.Get(b));
		}

		[Fact]
		public void VerifiesExpectationOnAddedInterface()
		{
			var bag = new Mock<IBag>();
			var foo = bag.As<IFoo>();

			foo.Expect(f => f.Execute()).Verifiable();

			Assert.Throws<MockVerificationException>(() => foo.Verify());
			Assert.Throws<MockVerificationException>(() => foo.VerifyAll());
			Assert.Throws<MockException>(() => foo.Verify(f => f.Execute()));

			foo.Object.Execute();

			foo.Verify();
			foo.VerifyAll();
			foo.Verify(f => f.Execute());
		}

		// ShouldSupportByRefArguments?
		// ShouldSupportOutArguments?

		interface IDo { void Do(); }

		public class Doer : IDo
		{
			public void Do()
			{
			}
		}

		public sealed class FooSealed { }
		class FooService : IFooService { }
		interface IFooService { }

		private int GetToRange()
		{
			return 5;
		}

		public class FooWithPrivateSetter
		{
			public virtual string Foo { get; private set; }
		}

		public class ClassWithNoDefaultConstructor
		{
			public ClassWithNoDefaultConstructor(string stringValue, int intValue)
			{
				this.StringValue = stringValue;
				this.IntValue = intValue;
			}

			public string StringValue { get; set; }
			public int IntValue { get; set; }
		}

		public abstract class FooWithConstructors
		{
			public FooWithConstructors(string stringValue, int intValue)
			{
				this.StringValue = stringValue;
				this.IntValue = intValue;
			}

			public FooWithConstructors()
			{
			}

			public override string ToString()
			{
				return base.ToString();
			}

			public string StringValue { get; set; }
			public int IntValue { get; set; }
		}

		public class FooOverrideEquals
		{
			public string Name { get; set; }

			public override bool Equals(object obj)
			{
				return (obj is FooOverrideEquals) &&
					((FooOverrideEquals)obj).Name == this.Name;
			}

			public override int GetHashCode()
			{
				return Name.GetHashCode();
			}
		}

		public interface IFoo
		{
			int DoIntArgReturnInt(int arg);
			int DoReturnInt();
			string DoReturnString();
			void DoVoidArgs(string arg);
			void DoVoidArgs(string arg1, string arg2);
			void DoVoidArgs(string arg1, string arg2, string arg3);
			void DoVoidArgs(string arg1, string arg2, string arg3, string arg4);

			int DoStringArgReturnInt(string arg);

			int Duplicate(int value);
			AttributeTargets GetTargets();

			void Execute();
			string Execute(string command);
			string Execute(string arg1, string arg2);
			string Execute(string arg1, string arg2, string arg3);
			string Execute(string arg1, string arg2, string arg3, string arg4);
			void Execute(string arg1, int arg2);

			int ValueProperty { get; set; }
			int WriteOnlyValue { set; }

			bool DoTypeOverload(Bar bar);
			bool DoTypeOverload(Baz bar);

			int this[int index] { get; set; }
		}

		public class Bar { }
		public class Baz { }

		public abstract class FooBase
		{
			public int ValueField;
			public abstract void Do(int value);

			public virtual bool Check(string value)
			{
				return true;
			}

			public bool GetIsProtected()
			{
				return IsProtected();
			}

			protected virtual bool IsProtected()
			{
				return true;
			}

			public bool True()
			{
				return true;
			}

			public bool BaseCalled = false;

			public virtual void BaseCall()
			{
				BaseCalled = true;
			}

			public bool BaseReturnCalled = false;

			public virtual bool BaseCall(string value)
			{
				BaseReturnCalled = true;
				return default(bool);
			}
		}

		public interface IBag
		{
			void Add(string key, object o);
			object Get(string key);
		}
	}
}
