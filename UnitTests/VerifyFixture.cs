using System;
using Xunit;
using Moq;
using System.Diagnostics;

namespace Moq.Tests
{
	public class VerifyFixture
	{
		[Fact]
		public void ThrowsIfVerifiableExpectationNotCalled()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Submit()).Verifiable();

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
		public void ThrowsIfVerifiableExpectationNotCalledWithMessage()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Submit()).Verifiable("Kaboom!");

			try
			{
				mock.Verify();
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Contains("Kaboom!", mex.Message);
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void ThrowsWithEvaluatedExpressionsIfVerifiableExpectationNotCalled()
		{
			var expectedArg = "lorem,ipsum";
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Execute(expectedArg.Substring(0, 5)))
				.Returns("ack")
				.Verifiable();

			try
			{
				mock.Verify();
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
				Assert.True(mex.Message.Contains(@".Execute(""lorem"")"), "Contains evaluated expected argument.");
			}
		}

		[Fact]
		public void ThrowsWithExpressionIfVerifiableExpectationWithLambdaMatcherNotCalled()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Execute(It.Is<string>(s => string.IsNullOrEmpty(s))))
				.Returns("ack")
				.Verifiable();

			try
			{
				mock.Verify();
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
				Assert.True(mex.Message.Contains(@".Execute(Is<String>(s => IsNullOrEmpty(s)))"), "Contains evaluated expected argument.");
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

			mock.Setup(x => x.Submit());

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
		public void ThrowsIfVerifyVoidMethodWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.Verify(f => f.Submit());
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

			mock.Object.Submit();

			mock.Verify(f => f.Submit());
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

			var v = mock.Object.Value;

			mock.VerifyGet(f => f.Value);
		}

		[Fact]
		public void VerifiesReturningMethodWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.Verify(f => f.Execute("ping"), "Execute should have been invoked with 'ping'");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Execute should have been invoked with 'ping'"));
				Assert.True(me.Message.Contains("f.Execute(\"ping\")"));
			}
		}

		[Fact]
		public void VerifiesVoidMethodWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.Verify(f => f.Submit(), "Submit should be invoked");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Submit should be invoked"));
				Assert.True(me.Message.Contains("f.Submit()"));
			}
		}

		[Fact]
		public void VerifiesPropertyGetWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.VerifyGet(f => f.Value, "Nobody called .Value");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Nobody called .Value"));
				Assert.True(me.Message.Contains("f.Value"));
			}
		}

		[Fact]
		public void VerifiesPropertySetWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			MockException me = Assert.Throws<MockException>(() =>
				mock.VerifySet(f => f.Value, "Nobody called .Value"));

			Assert.True(me.Message.Contains("Nobody called .Value"));
			Assert.True(me.Message.Contains("f.Value"));
		}

		[Fact]
		public void VerifiesPropertySetValueWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.VerifySet(f => f.Value, 5, "Nobody called .Value");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Nobody called .Value"));
				Assert.True(me.Message.Contains("f.Value"));
			}
		}

		[Fact]
		public void AsInterfaceVerifiesReturningMethodWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			try
			{
				mock.Verify(f => f.Execute("ping"), "Execute should have been invoked with 'ping'");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Execute should have been invoked with 'ping'"));
				Assert.True(me.Message.Contains("f.Execute(\"ping\")"));
			}
		}

		[Fact]
		public void AsInferfaceVerifiesVoidMethodWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			try
			{
				mock.Verify(f => f.Submit(), "Submit should be invoked");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Submit should be invoked"));
				Assert.True(me.Message.Contains("f.Submit()"));
			}
		}

		[Fact]
		public void AsInterfaceVerifiesPropertyGetWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			MockException me = Assert.Throws<MockException>(() =>
				mock.VerifyGet(f => f.Value, "Nobody called .Value"));
			Assert.True(me.Message.Contains("Nobody called .Value"));
			Assert.True(me.Message.Contains("f.Value"));
		}

		[Fact]
		public void AsInterfaceVerifiesPropertySetWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			try
			{
				mock.VerifySet(f => f.Value, "Nobody called .Value");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Nobody called .Value"));
				Assert.True(me.Message.Contains("f.Value"));
			}
		}

		[Fact]
		public void AsInterfaceVerifiesPropertySetValueWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			try
			{
				mock.VerifySet(f => f.Value, 5, "Nobody called .Value");
			}
			catch (MockException me)
			{
				Assert.True(me.Message.Contains("Nobody called .Value"));
				Assert.True(me.Message.Contains("f.Value"));
			}
		}

		[Fact]
		public void ThrowsIfVerifyPropertyGetWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			MockException mex = Assert.Throws<MockException>(() => mock.VerifyGet(f => f.Value));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void VerifiesPropertySetWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Value = 5;

			mock.VerifySet(f => f.Value);
		}

		[Fact]
		public void ThrowsIfVerifyPropertySetWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.VerifySet(f => f.Value);
				Assert.True(false, "Should have thrown");
			}
			catch (MockException mex)
			{
				Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			}
		}

		[Fact]
		public void VerifiesSetterWithExpression()
		{
			var mock = new Mock<IFoo>();
			mock.SetupSet(m => m.Value, 5);

			mock.Object.Value = 1;
			mock.Object.Value = 5;

			mock.VerifySet(m => m.Value);
			Assert.Throws<MockException>(() => mock.VerifySet(m => m.Value, 2));
			mock.VerifySet(m => m.Value, 5);
		}

		[Fact]
		public void VerifiesSetterWithAction()
		{
			var mock = new Mock<IFoo>();

			Assert.Throws<MockException>(() => mock.VerifySet(m => m.Value = 2));

			mock.Object.Value = 2;

			mock.VerifySet(m => m.Value = 2);
		}

		[Fact]
		public void VerifiesSetterWithActionAndMessage()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.VerifySet(m => m.Value = 2, "foo");
			}
			catch (MockException me)
			{
				Assert.Contains("foo", me.Message);
			}

			mock.Object.Value = 2;

			mock.VerifySet(m => m.Value = 2, "foo");
		}

		[Fact]
		public void VerifiesRefWithExpression()
		{
			var mock = new Mock<IFoo>();
			var expected = "ping";

			Assert.Throws<MockException>(() => mock.Verify(m => m.EchoRef(ref expected)));

			mock.Object.EchoRef(ref expected);

			mock.Verify(m => m.EchoRef(ref expected));
		}

		[Fact]
		public void VerifiesOutWithExpression()
		{
			var mock = new Mock<IFoo>();
			var expected = "ping";

			Assert.Throws<MockException>(() => mock.Verify(m => m.EchoOut(out expected)));

			mock.Object.EchoOut(out expected);

			mock.Verify(m => m.EchoOut(out expected));
		}

		[Fact]
		public void ThrowsIfVerifyVoidAtMostOnceAndMoreThanOneCall()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.AtMostOnce());

			mock.Object.Submit();

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Submit(), Times.AtMostOnce()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyVoidAtMostAndMoreThanNCalls()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Submit();
			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.AtMost(2));

			mock.Object.Submit();

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Submit(), Times.AtMost(2)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyVoidNeverAndOneCall()
		{
			var mock = new Mock<IFoo>();

			mock.Verify(foo => foo.Submit(), Times.Never());

			mock.Object.Submit();

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Submit(), Times.Never()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyVoidAtLeastOnceAndNotCalls()
		{
			var mock = new Mock<IFoo>();

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Submit(), Times.AtLeastOnce()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.AtLeastOnce());
		}

		[Fact]
		public void ThrowsIfVerifyVoidAtLeastAndLessThanNCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Submit();
			mock.Object.Submit();

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Submit(), Times.AtLeast(3)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.AtLeast(3));
		}

		[Fact]
		public void ThrowsIfVerifyVoidExactlyAndLessOrMoreThanNCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Submit();
			mock.Object.Submit();
			mock.Object.Submit();
			mock.Object.Submit();

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Submit(), Times.Exactly(5)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Exactly(5));

			mock.Object.Submit();

			mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Exactly(5)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyVoidBetweenExclusiveAndLessOrEqualsFromOrMoreOrEqualToCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Submit();

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive));

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive));

			mock.Object.Submit();

			mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyVoidBetweenInclusiveAndLessFromOrMoreToCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Submit();

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive));

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive));

			mock.Object.Submit();
			mock.Object.Submit();

			mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyReturningAtMostOnceAndMoreThanOneCall()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.AtMostOnce());

			mock.Object.Execute("");

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Execute(""), Times.AtMostOnce()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyReturningAtMostAndMoreThanNCalls()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Execute("");
			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.AtMost(2));

			mock.Object.Execute("");

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Execute(""), Times.AtMost(2)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyReturningNeverAndOneCall()
		{
			var mock = new Mock<IFoo>();

			mock.Verify(foo => foo.Execute(""), Times.Never());

			mock.Object.Execute("");

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Execute(""), Times.Never()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyReturningAtLeastOnceAndNotCalls()
		{
			var mock = new Mock<IFoo>();

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Execute(""), Times.AtLeastOnce()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.AtLeastOnce());
		}

		[Fact]
		public void ThrowsIfVerifyReturningAtLeastAndLessThanNCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Execute("");
			mock.Object.Execute("");

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Execute(""), Times.AtLeast(3)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.AtLeast(3));
		}

		[Fact]
		public void ThrowsIfVerifyReturningExactlyAndLessOrMoreThanNCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Execute("");
			mock.Object.Execute("");
			mock.Object.Execute("");
			mock.Object.Execute("");

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Execute(""), Times.Exactly(5)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.Exactly(5));

			mock.Object.Execute("");

			mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Execute(""), Times.Exactly(5)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyReturningBetweenExclusiveAndLessOrEqualsFromOrMoreOrEqualToCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Execute("");

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Execute(""), Times.Between(1, 4, Range.Exclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.Between(1, 4, Range.Exclusive));

			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.Between(1, 4, Range.Exclusive));

			mock.Object.Execute("");

			mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Execute(""), Times.Between(1, 4, Range.Exclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyReturningBetweenInclusiveAndLessFromOrMoreToCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Execute("");

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Execute(""), Times.Between(2, 4, Range.Inclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.Between(2, 4, Range.Inclusive));

			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.Between(2, 4, Range.Inclusive));

			mock.Object.Execute("");
			mock.Object.Execute("");

			mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Execute(""), Times.Between(2, 4, Range.Inclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyGetGetAtMostOnceAndMoreThanOneCall()
		{
			var mock = new Mock<IFoo>();
			var value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.AtMostOnce());

			value = mock.Object.Value;

			MockException mex = Assert.Throws<MockException>(() =>
				mock.VerifyGet(foo => foo.Value, Times.AtMostOnce()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyGetGetAtMostAndMoreThanNCalls()
		{
			var mock = new Mock<IFoo>();
			var value = mock.Object.Value;
			value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.AtMost(2));

			value = mock.Object.Value;

			MockException mex = Assert.Throws<MockException>(() =>
				mock.VerifyGet(foo => foo.Value, Times.AtMost(2)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyGetGetNeverAndOneCall()
		{
			var mock = new Mock<IFoo>();

			mock.VerifyGet(foo => foo.Value, Times.Never());

			var value = mock.Object.Value;

			MockException mex = Assert.Throws<MockException>(() =>
				mock.VerifyGet(foo => foo.Value, Times.Never()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyGetGetAtLeastOnceAndNotCalls()
		{
			var mock = new Mock<IFoo>();

			MockException mex = Assert.Throws<MockException>(() =>
				mock.VerifyGet(foo => foo.Value, Times.AtLeastOnce()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			var value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.AtLeastOnce());
		}

		[Fact]
		public void ThrowsIfVerifyGetGetAtLeastAndLessThanNCalls()
		{
			var mock = new Mock<IFoo>();

			var value = mock.Object.Value;
			value = mock.Object.Value;

			MockException mex = Assert.Throws<MockException>(() =>
				mock.VerifyGet(foo => foo.Value, Times.AtLeast(3)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.AtLeast(3));
		}

		[Fact]
		public void ThrowsIfVerifyGetGetExactlyAndLessOrMoreThanNCalls()
		{
			var mock = new Mock<IFoo>();

			var value = mock.Object.Value;
			value = mock.Object.Value;
			value = mock.Object.Value;
			value = mock.Object.Value;

			MockException mex = Assert.Throws<MockException>(() =>
				mock.VerifyGet(foo => foo.Value, Times.Exactly(5)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.Exactly(5));

			value = mock.Object.Value;

			mex = Assert.Throws<MockException>(() => mock.VerifyGet(foo => foo.Value, Times.Exactly(5)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyGetGetBetweenExclusiveAndLessOrEqualsFromOrMoreOrEqualToCalls()
		{
			var mock = new Mock<IFoo>();

			var value = mock.Object.Value;

			MockException mex = Assert.Throws<MockException>(() =>
				mock.VerifyGet(foo => foo.Value, Times.Between(1, 4, Range.Exclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.Between(1, 4, Range.Exclusive));

			value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.Between(1, 4, Range.Exclusive));

			value = mock.Object.Value;

			mex = Assert.Throws<MockException>(() =>
				mock.VerifyGet(foo => foo.Value, Times.Between(1, 4, Range.Exclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyGetGetBetweenInclusiveAndLessFromOrMoreToCalls()
		{
			var mock = new Mock<IFoo>();

			var value = mock.Object.Value;

			MockException mex = Assert.Throws<MockException>(() =>
				mock.VerifyGet(foo => foo.Value, Times.Between(2, 4, Range.Inclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);

			value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.Between(2, 4, Range.Inclusive));

			value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.Between(2, 4, Range.Inclusive));

			value = mock.Object.Value;
			value = mock.Object.Value;

			mex = Assert.Throws<MockException>(() =>
				mock.VerifyGet(foo => foo.Value, Times.Between(2, 4, Range.Inclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

#if !SILVERLIGHT
		[Fact]
		public void ThrowsIfVerifySetAtMostOnceAndMoreThanOneCall()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Value = 3;

			mock.VerifySet(f => f.Value = 3, Times.AtMostOnce());

			mock.Object.Value = 3;

			MockException mex = Assert.Throws<MockException>(() =>
				mock.VerifySet(f => f.Value = 3, Times.AtMostOnce()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}
#endif

		public interface IFoo
		{
			int WriteOnly { set; }
			int? Value { get; set; }
			void EchoRef<T>(ref T value);
			void EchoOut<T>(out T value);
			int Echo(int value);
			void Submit();
			string Execute(string command);
		}
	}
}

namespace SomeNamespace
{
	public class VerifyExceptionsFixture
	{
		[Fact]
		public void RendersReadableMessageForVerifyFailures()
		{
			var mock = new Mock<Moq.Tests.VerifyFixture.IFoo>();

			mock.Setup(x => x.Submit());
			mock.Setup(x => x.Echo(1));
			mock.Setup(x => x.Execute("ping"));

			try
			{
				mock.VerifyAll();
				Assert.True(false, "Should have thrown");
			}
			catch (Exception ex)
			{
				Assert.True(ex.Message.Contains("x => x.Submit()"));
				Assert.True(ex.Message.Contains("x => x.Echo(1)"));
				Assert.True(ex.Message.Contains("x => x.Execute(\"ping\")"));
			}
		}
	}
}
