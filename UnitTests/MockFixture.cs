using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Moq.Tests
{
	[TestFixture]
	public class MockFixture
	{
		[Test]
		public void ShouldCreateMockAndExposeInterface()
		{
			var mock = new Mock<ICloneable>();

			ICloneable cloneable = mock.Object;

			Assert.IsNotNull(cloneable);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfNullExpectAction()
		{
			var mock = new Mock<ICloneable>();

			mock.Expect((Expression<Action<ICloneable>>)null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfNullExpectFunction()
		{
			var mock = new Mock<ICloneable>();

			mock.Expect((Expression<Func<ICloneable, string>>)null);
		}

		[Test]
		public void ShouldExpectCallReturn()
		{
			var mock = new Mock<ICloneable>();
			var clone = new object();

			mock.Expect(x => x.Clone()).Returns(clone);

			Assert.AreEqual(clone, mock.Object.Clone());
		}

		[Test]
		public void ShouldExpectDifferentMethodCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoReturnInt()).Returns(1);
			mock.Expect(x => x.DoReturnString()).Returns("foo");

			Assert.AreEqual(1, mock.Object.DoReturnInt());
			Assert.AreEqual("foo", mock.Object.DoReturnString());
		}

		[Test]
		public void ShouldExpectCallWithArgument()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoIntArgReturnInt(1)).Returns(11);
			mock.Expect(x => x.DoIntArgReturnInt(2)).Returns(22);

			Assert.AreEqual(11, mock.Object.DoIntArgReturnInt(1));
			Assert.AreEqual(22, mock.Object.DoIntArgReturnInt(2));
		}

		[Test]
		public void ShouldExpectCallWithNullArgument()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoStringArgReturnInt(null)).Returns(1);
			mock.Expect(x => x.DoStringArgReturnInt("foo")).Returns(2);

			Assert.AreEqual(1, mock.Object.DoStringArgReturnInt(null));
			Assert.AreEqual(2, mock.Object.DoStringArgReturnInt("foo"));
		}

		[Test]
		public void ShouldExpectCallWithVariable()
		{
			int value = 25;
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoStringArgReturnInt(null)).Returns(value);

			Assert.AreEqual(value, mock.Object.DoStringArgReturnInt(null));
		}

		[Test]
		public void ShouldExpectCallWithReferenceLazyEvaluate()
		{
			int a = 25;
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoStringArgReturnInt(a.ToString())).Returns(() => a);
			a = 10;

			Assert.AreEqual(10, mock.Object.DoStringArgReturnInt("10"));

			a = 20;

			Assert.AreEqual(20, mock.Object.DoStringArgReturnInt("20"));
		}

		[Test]
		public void ShouldExpectReturnPropertyValue()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.ValueProperty).Returns(25);

			Assert.AreEqual(25, mock.Object.ValueProperty);
		}

		[ExpectedException(typeof(NotSupportedException))]
		[Test]
		public void ShouldThrowIfExpectFieldValue()
		{
			var mock = new Mock<FooMBRO>();

			mock.Expect(x => x.ValueField);
		}

		[Test]
		public void ShouldExpectMethodCallWithVariable()
		{
			int value = 5;
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Duplicate(value)).Returns(() => value * 2);

			Assert.AreEqual(value * 2, mock.Object.Duplicate(value));
		}

		[Test]
		public void ShouldExpectMethodCallWithMethodCall()
		{
			int value = 5;
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Duplicate(GetValue(value))).Returns(() => value * 2);

			Assert.AreEqual(value * 2, mock.Object.Duplicate(value * 2));
		}

		private int GetValue(int value)
		{
			return value * 2;
		}

		[Test]
		public void ShouldMatchAnyArgument()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Duplicate(It.IsAny<int>())).Returns(5);

			Assert.AreEqual(5, mock.Object.Duplicate(5));
			Assert.AreEqual(5, mock.Object.Duplicate(25));
		}

		[Test]
		public void ShouldMatchPredicateArgument()
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

			Assert.AreEqual(1, mock.Object.Duplicate(3));
			Assert.AreEqual(0, mock.Object.Duplicate(0));
			Assert.AreEqual(0, mock.Object.Duplicate(-5));
			Assert.AreEqual(2, mock.Object.Duplicate(5));
			Assert.AreEqual(2, mock.Object.Duplicate(6));
		}

		[Test]
		public void ShouldExpectCallWithoutReturnValue()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Execute());

			mock.Object.Execute();
		}

		[ExpectedException(typeof(FormatException))]
		[Test]
		public void ShouldThrowIfExpectingThrows()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoReturnInt()).Throws(new FormatException());

			mock.Object.DoReturnInt();
		}

		[Test]
		public void ShouldExecuteCallbackWhenVoidMethodIsCalled()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Expect(x => x.Execute()).Callback(() => called = true);

			mock.Object.Execute();
			Assert.IsTrue(called);
		}

		[Test]
		public void ShouldExecuteCallbackWhenNonVoidMethodIsCalled()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Expect(x => x.DoReturnInt()).Callback(() => called = true).Returns(1);

			Assert.AreEqual(1, mock.Object.DoReturnInt());
			Assert.IsTrue(called);
		}

		[Test]
		public void ShouldExpectRanges()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoIntArgReturnInt(It.IsInRange(1, 5, Range.Inclusive))).Returns(1);
			mock.Expect(x => x.DoIntArgReturnInt(It.IsInRange(6, 10, Range.Exclusive))).Returns(2);

			Assert.AreEqual(1, mock.Object.DoIntArgReturnInt(1));
			Assert.AreEqual(1, mock.Object.DoIntArgReturnInt(2));
			Assert.AreEqual(1, mock.Object.DoIntArgReturnInt(5));

			Assert.AreEqual(2, mock.Object.DoIntArgReturnInt(7));
			Assert.AreEqual(2, mock.Object.DoIntArgReturnInt(9));
		}

		[ExpectedException(typeof(MockException))]
		[Test]
		public void ShouldNotMatchOutOfRange()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoIntArgReturnInt(It.IsInRange(1, 5, Range.Exclusive))).Returns(1);

			Assert.AreEqual(1, mock.Object.DoIntArgReturnInt(2));

			int throwHere = mock.Object.DoIntArgReturnInt(1);
		}

		[Test]
		public void ShouldExpectRangeWithVariableAndMethodInvocation()
		{
			var mock = new Mock<IFoo>();
			var from = 1;

			mock.Expect(x => x.DoIntArgReturnInt(It.IsInRange(from, GetToRange(), Range.Inclusive))).Returns(1);

			Assert.AreEqual(1, mock.Object.DoIntArgReturnInt(1));
		}

		[Test]
		public void ShouldExpectRangeLazyEval()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);
			var from = "a";
			var to = "d";

			mock.Expect(x => x.DoStringArgReturnInt(It.IsInRange(from, to, Range.Inclusive))).Returns(1);

			Assert.AreEqual(1, mock.Object.DoStringArgReturnInt("b"));

			from = "c";

			Assert.AreEqual(default(int), mock.Object.DoStringArgReturnInt("b"));
		}

		[Test]
		public void ShouldExpectMatchRegexAndLazyEval()
		{
			var mock = new Mock<IFoo>(MockBehavior.Loose);
			var reg = "[a-d]+";

			mock.Expect(x => x.DoStringArgReturnInt(It.IsRegex(reg))).Returns(1);
			mock.Expect(x => x.DoStringArgReturnInt(It.IsRegex(reg, RegexOptions.IgnoreCase))).Returns(2);

			Assert.AreEqual(1, mock.Object.DoStringArgReturnInt("b"));
			Assert.AreEqual(1, mock.Object.DoStringArgReturnInt("abc"));
			Assert.AreEqual(2, mock.Object.DoStringArgReturnInt("B"));
			Assert.AreEqual(2, mock.Object.DoStringArgReturnInt("BC"));

			reg = "[c-d]+";

			// Will not match neither the 1 and 2 return values we had.
			Assert.AreEqual(default(int), mock.Object.DoStringArgReturnInt("b"));
		}

		[Test]
		public void ShouldReturnService()
		{
			var provider = new Mock<IServiceProvider>();

			provider.Expect(x => x.GetService(typeof(IFooService))).Returns(new FooService());

			Assert.That(provider.Object.GetService(typeof(IFooService)) is FooService);
		}

		[Test]
		public void ShouldCallFirstExpectThatMatches()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute("ping")).Returns("I'm alive!");
			mock.Expect(x => x.Execute(It.IsAny<string>())).Throws(new ArgumentException());

			Assert.AreEqual("I'm alive!", mock.Object.Execute("ping"));
			try
			{
				mock.Object.Execute("asdf");
				Assert.Fail("didn't throw");
			}
			catch (ArgumentException)
			{
			}
		}

		[Test]
		public void ShouldTestEvenNumbers()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoIntArgReturnInt(It.Is<int>(i => i % 2 == 0))).Returns(1);

			Assert.AreEqual(1, mock.Object.DoIntArgReturnInt(2));
		}

		[Test]
		public void ShouldGetTypeReturnATypeAssignableFromInterfaceType()
		{
			var mock = new Mock<IFoo>();
			Assert.IsTrue(typeof(IFoo).IsAssignableFrom(mock.Object.GetType()));
		}

		[Test]
		public void ShouldEqualsMethodWorkAsReferenceEquals()
		{
			var mock1 = new Mock<IFoo>();
			var mock2 = new Mock<IFoo>();

			Assert.IsTrue(mock1.Object.Equals(mock1.Object));
			Assert.IsFalse(mock1.Object.Equals(mock2.Object));
		}

		[Test]
		public void ShouldGetHashCodeReturnDifferentCodeForEachMock()
		{
			var mock1 = new Mock<IFoo>();
			var mock2 = new Mock<IFoo>();

			Assert.AreEqual(mock1.Object.GetHashCode(), mock1.Object.GetHashCode());
			Assert.AreEqual(mock2.Object.GetHashCode(), mock2.Object.GetHashCode());
			Assert.AreNotEqual(mock1.Object.GetHashCode(), mock2.Object.GetHashCode());
		}

		[Test]
		public void ShouldToString()
		{
			var mock = new Mock<IFoo>();
			Assert.IsFalse(string.IsNullOrEmpty(mock.Object.ToString()));
		}

		[Ignore("Castle.DynamicProxy2 doesn't seem to call interceptors for ToString, GetHashCode")]
		[Test]
		public void ShouldOverrideObjectMethods()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.GetHashCode()).Returns(1);
			mock.Expect(x => x.ToString()).Returns("foo");

			Assert.AreEqual("foo", mock.Object.ToString());
			Assert.AreEqual(1, mock.Object.GetHashCode());
		}

		[Test]
		public void ShouldMockAbstractClass()
		{
			var mock = new Mock<FooBase>();
			mock.Expect(x => x.Check("foo")).Returns(false);

			Assert.IsFalse(mock.Object.Check("foo"));
			Assert.IsTrue(mock.Object.Check("bar"));
		}

		[Test]
		public void ShouldOverrideNonVirtualForMBRO()
		{
			var mock = new Mock<FooMBRO>();
			mock.Expect(x => x.True()).Returns(false);

			Assert.IsFalse(mock.Object.True());
		}

		[Test]
		public void ShouldCallUnderlyingMBRO()
		{
			var mock = new Mock<FooMBRO>();

			Assert.IsTrue(mock.Object.True());
		}

		[Test]
		public void ShouldCallUnderlyingClassEquals()
		{
			var mock = new Mock<FooOverrideEquals>();
			var mock2 = new Mock<FooOverrideEquals>();

			mock.Object.Name = "Foo";
			mock2.Object.Name = "Foo";

			Assert.IsTrue(mock.Object.Equals(mock2.Object));
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfSealedClass()
		{
			var mock = new Mock<FooSealed>();
		}

		[Test]
		public void ShouldThrowIfVerifiableExpectationNotCalled()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoReturnInt()).Verifiable().Returns(5);

			try
			{
				mock.Verify();
				Assert.Fail("Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Test]
		public void ShouldThrowWithEvaluatedExpressionsIfVerifiableExpectationNotCalled()
		{
			var expectedArg = "lorem,ipsum";
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoStringArgReturnInt(expectedArg.Substring(0,5))).Verifiable().Returns(5);

			try
			{
				mock.Verify();
				Assert.Fail("Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.VerificationFailed, mex.Reason);
				Assert.IsTrue(mex.Message.Contains(@".DoStringArgReturnInt(""lorem"")"), "Contains evaluated expected argument.");
			}
		}

		[Test]
		public void ShouldThrowExpressionsIfVerifiableExpectationWithLambdaMatcherNotCalled()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoStringArgReturnInt(It.Is<string>(s => string.IsNullOrEmpty(s))))
				.Verifiable().Returns(5);

			try
			{
				mock.Verify();
				Assert.Fail("Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.VerificationFailed, mex.Reason);
				Assert.IsTrue(mex.Message.Contains(@".DoStringArgReturnInt(Is(s => IsNullOrEmpty(s)))"), "Contains evaluated expected argument.");
			}
		}

		[Test]
		public void ShouldNotThrowVerifyAndNoVerifiableExpectations()
		{
			var mock = new Mock<IFoo>();

			mock.Verify();
		}

		[Test]
		public void ShouldThrowIfVerifyAllAndNonVerifiable()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoReturnInt()).Returns(1);

			try
			{
				mock.VerifyAll();
				Assert.Fail("Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.AreEqual(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Test]
		public void ShouldRenderReadableMessageForVerifyFailures()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoReturnInt());
			mock.Expect(x => x.DoReturnString());
			mock.Expect(x => x.DoStringArgReturnInt("Hello World"));

			try
			{
				mock.VerifyAll();
				Assert.Fail("Should have thrown");
			}
			catch (Exception ex)
			{
				Assert.That(ex.Message.Contains("x => x.DoReturnInt()"));
				Assert.That(ex.Message.Contains("x => x.DoReturnString()"));
				Assert.That(ex.Message.Contains("x => x.DoStringArgReturnInt(\"Hello World\")"));
			} 
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfExpectOnNonVirtual()
		{
			var mock = new Mock<FooBase>();
			mock.Expect(x => x.True()).Returns(false);
		}

		[Test]
		public void ShouldNotThrowIfExpectOnNonVirtualButMBRO()
		{
			var mock = new Mock<FooMBRO>();
			mock.Expect(x => x.True()).Returns(false);
		}

		[Test]
		public void ShouldOverridePreviousExpectation()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoReturnInt()).Returns(5);

			Assert.AreEqual(5, mock.Object.DoReturnInt());

			mock.Expect(x => x.DoReturnInt()).Returns(10);

			Assert.AreEqual(10, mock.Object.DoReturnInt());
		}

		[Test]
		public void ShouldReceiveClassCtorArguments()
		{
			var mock = new Mock<FooWithConstructors>(MockBehavior.Default, "Hello", 26);

			Assert.AreEqual("Hello", mock.Object.StringValue);
			Assert.AreEqual(26, mock.Object.IntValue);

			// Should also construct without args.
			mock = new Mock<FooWithConstructors>(MockBehavior.Default);

			Assert.AreEqual(null, mock.Object.StringValue);
			Assert.AreEqual(0, mock.Object.IntValue);
		}

		[Test]
		public void ShouldReceiveClassCtorArgumentsMBRO()
		{
			var mock = new Mock<FooWithConstructorsMBRO>(MockBehavior.Default, "Hello", 26);

			Assert.AreEqual("Hello", mock.Object.StringValue);
			Assert.AreEqual(26, mock.Object.IntValue);

			// Should also construct without args.
			mock = new Mock<FooWithConstructorsMBRO>(MockBehavior.Default);

			Assert.AreEqual(null, mock.Object.StringValue);
			Assert.AreEqual(0, mock.Object.IntValue);
		}

		[Test]
		public void ShouldConstructClassWithNoDefaultConstructor()
		{
			var mock = new Mock<ClassWithNoDefaultConstructor>(MockBehavior.Default, "Hello", 26);

			Assert.AreEqual("Hello", mock.Object.StringValue);
			Assert.AreEqual(26, mock.Object.IntValue);
		}

		[Test]
		public void ShouldConstructClassWithNoDefaultConstructorAndNullValue()
		{
			var mock = new Mock<ClassWithNoDefaultConstructor>(MockBehavior.Default, null, 26);

			Assert.AreEqual(null, mock.Object.StringValue);
			Assert.AreEqual(26, mock.Object.IntValue);
		}

		[Test]
		public void ShouldConstructClassWithNoDefaultConstructorMBRO()
		{
			var mock = new Mock<ClassWithNoDefaultConstructorMBRO>(MockBehavior.Default, "Hello", 26);

			Assert.AreEqual("Hello", mock.Object.StringValue);
			Assert.AreEqual(26, mock.Object.IntValue);
		}

		[Test]
		public void ShouldConstructClassWithNoDefaultConstructorAndNullValueMBRO()
		{
			var mock = new Mock<ClassWithNoDefaultConstructorMBRO>(MockBehavior.Default, null, 26);

			Assert.AreEqual(null, mock.Object.StringValue);
			Assert.AreEqual(26, mock.Object.IntValue);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ShouldThrowIfNoMatchingConstructorFound()
		{
			var mock = new Mock<ClassWithNoDefaultConstructor>(25, true);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ShouldThrowIfNoMatchingConstructorFoundMBRO()
		{
			var mock = new Mock<ClassWithNoDefaultConstructorMBRO>(25, true);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ShouldThrowIfArgumentsPassedForInterface()
		{
			var mock = new Mock<IFoo>(25, true);
		}

		[Test]
		public void ShouldCallCallbackWithoutArgumentsForMethodCallWithArguments()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Expect(x => x.DoVoidArgs(It.IsAny<string>())).Callback(() => called = true);

			mock.Object.DoVoidArgs("blah");
			Assert.IsTrue(called);
		}

		[Test]
		public void ShouldCallCallbackWithOneArgument()
		{
			var mock = new Mock<IFoo>();
			string callbackArg = null;
			mock.Expect(x => x.DoVoidArgs(It.IsAny<string>())).Callback((string s) => callbackArg = s);

			mock.Object.DoVoidArgs("blah");
			Assert.AreEqual("blah", callbackArg);
		}

		[Test]
		public void ShouldCallCallbackWithTwoArguments()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			mock.Expect(x => x.DoVoidArgs(It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2) => { callbackArg1 = s1; callbackArg2 = s2; });

			mock.Object.DoVoidArgs("blah1", "blah2");
			Assert.AreEqual("blah1", callbackArg1);
			Assert.AreEqual("blah2", callbackArg2);
		}

		[Test]
		public void ShouldCallCallbackWithThreeArguments()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			mock.Expect(x => x.DoVoidArgs(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; });

			mock.Object.DoVoidArgs("blah1", "blah2", "blah3");
			Assert.AreEqual("blah1", callbackArg1);
			Assert.AreEqual("blah2", callbackArg2);
			Assert.AreEqual("blah3", callbackArg3);
		}

		[Test]
		public void ShouldCallCallbackWithFourArguments()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			string callbackArg4 = null;
			mock.Expect(x => x.DoVoidArgs(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3, string s4) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; callbackArg4 = s4; });

			mock.Object.DoVoidArgs("blah1", "blah2", "blah3", "blah4");
			Assert.AreEqual("blah1", callbackArg1);
			Assert.AreEqual("blah2", callbackArg2);
			Assert.AreEqual("blah3", callbackArg3);
			Assert.AreEqual("blah4", callbackArg4);
		}

		[Test]
		public void ShouldCallCallbackWithOneArgumentForNonVoidMethod()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			mock.Expect(x => x.Execute(It.IsAny<string>()))
				.Callback((string s1) => callbackArg1 = s1)
				.Returns("foo");

			mock.Object.Execute("blah1");
			Assert.AreEqual("blah1", callbackArg1);
		}

		[Test]
		public void ShouldCallCallbackWithTwoArgumentsForNonVoidMethod()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2) => { callbackArg1 = s1; callbackArg2 = s2; })
				.Returns("foo");

			mock.Object.Execute("blah1", "blah2");
			Assert.AreEqual("blah1", callbackArg1);
			Assert.AreEqual("blah2", callbackArg2);
		}

		[Test]
		public void ShouldCallCallbackWithThreeArgumentsForNonVoidMethod()
		{
			var mock = new Mock<IFoo>();
			string callbackArg1 = null;
			string callbackArg2 = null;
			string callbackArg3 = null;
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Callback((string s1, string s2, string s3) => { callbackArg1 = s1; callbackArg2 = s2; callbackArg3 = s3; })
				.Returns("foo");

			mock.Object.Execute("blah1", "blah2", "blah3");
			Assert.AreEqual("blah1", callbackArg1);
			Assert.AreEqual("blah2", callbackArg2);
			Assert.AreEqual("blah3", callbackArg3);
		}

		[Test]
		public void ShouldCallCallbackWithFourArgumentsForNonVoidMethod()
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
			Assert.AreEqual("blah1", callbackArg1);
			Assert.AreEqual("blah2", callbackArg2);
			Assert.AreEqual("blah3", callbackArg3);
			Assert.AreEqual("blah4", callbackArg4);
		}

		[Test]
		public void ShouldReturnUsingOneArgument()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(It.IsAny<string>()))
				.Returns((string s) => s);

			string result = mock.Object.Execute("blah1");
			Assert.AreEqual("blah1", result);
		}

		[Test]
		public void ShouldReturnUsingTwoArguments()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2) => s1 + s2);

			string result = mock.Object.Execute("blah1", "blah2");
			Assert.AreEqual("blah1blah2", result);
		}

		[Test]
		public void ShouldReturnUsingThreeArguments()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2, string s3) => s1 + s2 + s3);

			string result = mock.Object.Execute("blah1", "blah2", "blah3");
			Assert.AreEqual("blah1blah2blah3", result);
		}

		[Test]
		public void ShouldReturnUsingFourArguments()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns((string s1, string s2, string s3, string s4) => s1 + s2 + s3 + s4);

			string result = mock.Object.Execute("blah1", "blah2", "blah3", "blah4");
			Assert.AreEqual("blah1blah2blah3blah4", result);
		}

		[Test]
		public void ShouldMatchDifferentOverloads()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(foo => foo.DoTypeOverload(It.IsAny<Bar>()))
				.Returns(true);
			mock.Expect(foo => foo.DoTypeOverload(It.IsAny<Baz>()))
				.Returns(false);

			bool bar = mock.Object.DoTypeOverload(new Bar());
			bool baz = mock.Object.DoTypeOverload(new Baz());

			Assert.IsTrue(bar);
			Assert.IsFalse(baz);
		}

		// ShouldExpectPropertyWithIndexer
		// ShouldReceiveArgumentValuesOnCallback
		// ShouldReceiveArgumentValuesOnReturns
		// ShouldInterceptPropertySetter?
		// ShouldSupportByRefArguments?
		// ShouldSupportOutArguments?

		public sealed class FooSealed { }
		class FooService : IFooService { }
		interface IFooService { }

		private int GetToRange()
		{
			return 5;
		}

		public class ClassWithNoDefaultConstructorMBRO : MarshalByRefObject
		{
			public ClassWithNoDefaultConstructorMBRO(string stringValue, int intValue)
			{
				this.StringValue = stringValue;
				this.IntValue = intValue;
			}

			public string StringValue { get; set; }
			public int IntValue { get; set; }
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

		public class FooWithConstructorsMBRO : MarshalByRefObject
		{
			public FooWithConstructorsMBRO(string stringValue, int intValue)
			{
				this.StringValue = stringValue;
				this.IntValue = intValue;
			}

			public FooWithConstructorsMBRO()
			{
			}

			public override string ToString()
			{
				return base.ToString();
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

		public class FooMBRO : MarshalByRefObject
		{
			public int ValueField;

			public bool True()
			{
				return true;
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
		}

		public class Bar { }
		public class Baz { }

		public abstract class FooBase
		{
			public abstract void Do(int value);

			public virtual bool Check(string value)
			{
				return true;
			}

			public bool True()
			{
				return true;
			}
		}
	}
}
