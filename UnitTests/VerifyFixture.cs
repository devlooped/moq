using System;
using Moq;
using Xunit;

#if !SILVERLIGHT
using System.Threading.Tasks;
#endif

namespace Moq.Tests
{
	public class VerifyFixture
	{
		[Fact]
		public void ThrowsIfVerifiableExpectationNotCalled()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Submit()).Verifiable();

			var mex = Assert.Throws<MockVerificationException>(() => mock.Verify());
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifiableExpectationNotCalledWithMessage()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Submit()).Verifiable("Kaboom!");

			var mex = Assert.Throws<MockVerificationException>(() => mock.Verify());
			Assert.Contains("Kaboom!", mex.Message);
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsWithEvaluatedExpressionsIfVerifiableExpectationNotCalled()
		{
			var expectedArg = "lorem,ipsum";
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Execute(expectedArg.Substring(0, 5)))
				.Returns("ack")
				.Verifiable();

			var mex = Assert.Throws<MockVerificationException>(() => mock.Verify());
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.Contains(@".Execute(""lorem"")"), "Contains evaluated expected argument.");
		}

		[Fact]
		public void ThrowsWithExpressionIfVerifiableExpectationWithLambdaMatcherNotCalled()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Execute(It.Is<string>(s => string.IsNullOrEmpty(s))))
				.Returns("ack")
				.Verifiable();

			var mex = Assert.Throws<MockVerificationException>(() => mock.Verify());
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.Contains(@".Execute(It.Is<String>(s => String.IsNullOrEmpty(s)))", mex.Message);
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

			var mex = Assert.Throws<MockVerificationException>(() => mock.VerifyAll());
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyVoidMethodWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			var mex = Assert.Throws<MockException>(() => mock.Verify(f => f.Submit()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
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

			var mex = Assert.Throws<MockException>(() => mock.Verify(f => f.Execute("ping")));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
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

			var me = Assert.Throws<MockException>(
				() => mock.Verify(f => f.Execute("ping"), "Execute should have been invoked with 'ping'"));
			Assert.True(me.Message.Contains("Execute should have been invoked with 'ping'"));
			Assert.True(me.Message.Contains("f.Execute(\"ping\")"));
		}

		[Fact]
		public void VerifiesVoidMethodWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			var me = Assert.Throws<MockException>(
				() => mock.Verify(f => f.Submit(), "Submit should be invoked"));
			Assert.True(me.Message.Contains("Submit should be invoked"));
			Assert.True(me.Message.Contains("f.Submit()"));
		}

		[Fact]
		public void VerifiesPropertyGetWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			var me = Assert.Throws<MockException>(() => mock.VerifyGet(f => f.Value, "Nobody called .Value"));
			Assert.True(me.Message.Contains("Nobody called .Value"));
			Assert.True(me.Message.Contains("f.Value"));
		}

		[Fact]
		public void VerifiesPropertySetWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			var me = Assert.Throws<MockException>(() => mock.VerifySet(f => f.Value = It.IsAny<int?>(), "Nobody called .Value"));
			Assert.True(me.Message.Contains("Nobody called .Value"));
			Assert.True(me.Message.Contains("f.Value"));
		}

		[Fact]
		public void VerifiesPropertySetValueWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			var e = Assert.Throws<MockException>(() => mock.VerifySet(f => f.Value = 5, "Nobody called .Value"));
			Assert.Contains("Nobody called .Value", e.Message);
			Assert.Contains("f.Value", e.Message);
		}

		[Fact]
		public void AsInterfaceVerifiesReturningMethodWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			var e = Assert.Throws<MockException>(
				() => mock.Verify(f => f.Execute("ping"), "Execute should have been invoked with 'ping'"));

			Assert.True(e.Message.Contains("Execute should have been invoked with 'ping'"));
			Assert.True(e.Message.Contains("f.Execute(\"ping\")"));
		}

		[Fact]
		public void AsInferfaceVerifiesVoidMethodWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			var e = Assert.Throws<MockException>(() => mock.Verify(f => f.Submit(), "Submit should be invoked"));

			Assert.True(e.Message.Contains("Submit should be invoked"));
			Assert.True(e.Message.Contains("f.Submit()"));
		}

		[Fact]
		public void AsInterfaceVerifiesPropertyGetWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			var e = Assert.Throws<MockException>(() => mock.VerifyGet(f => f.Value, "Nobody called .Value"));
			Assert.True(e.Message.Contains("Nobody called .Value"));
			Assert.True(e.Message.Contains("f.Value"));
		}

		[Fact]
		public void AsInterfaceVerifiesPropertySetWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IBar>();

			var e = Assert.Throws<MockException>(
				() => mock.VerifySet(f => f.Value = It.IsAny<int?>(), "Nobody called .Value"));
			Assert.True(e.Message.Contains("Nobody called .Value"));
			Assert.True(e.Message.Contains("f.Value"));
		}

		[Fact]
		public void AsInterfaceVerifiesPropertySetValueWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IBar>();

			var e = Assert.Throws<MockException>(() => mock.VerifySet(f => f.Value = 5, "Nobody called .Value"));
			Assert.True(e.Message.Contains("Nobody called .Value"));
			Assert.True(e.Message.Contains("f.Value"));
		}

		[Fact]
		public void ThrowsIfVerifyPropertyGetWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			var e = Assert.Throws<MockException>(() => mock.VerifyGet(f => f.Value));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, e.Reason);
		}

		[Fact]
		public void VerifiesPropertySetWithExpression()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Value = 5;

			mock.VerifySet(f => f.Value = It.IsAny<int?>());
		}

		[Fact]
		public void ThrowsIfVerifyPropertySetWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			var e = Assert.Throws<MockException>(() => mock.VerifySet(f => f.Value = It.IsAny<int?>()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, e.Reason);
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

			var me = Assert.Throws<MockException>(() => mock.VerifySet(m => m.Value = 2, "foo"));
			Assert.Contains("foo", me.Message);

			mock.Object.Value = 2;

			mock.VerifySet(m => m.Value = 2, "foo");
		}

		[Fact]
		public void VerifiesSetterWithActionAndMatcher()
		{
			var mock = new Mock<IFoo>();

			Assert.Throws<MockException>(() => mock.VerifySet(m => m.Value = It.IsAny<int>()));
			mock.Object.Value = 2;

			mock.VerifySet(m => m.Value = It.IsAny<int>());
			mock.VerifySet(m => m.Value = It.IsInRange(1, 2, Range.Inclusive));
			mock.VerifySet(m => m.Value = It.Is<int>(i => i % 2 == 0));
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

			var mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Submit(), Times.AtMostOnce()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.StartsWith(
				"\r\nExpected invocation on the mock at most once, but was 2 times: foo => foo.Submit()"));
		}

		[Fact]
		public void ThrowsIfVerifyVoidAtMostAndMoreThanNCalls()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Submit();
			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.AtMost(2));

			mock.Object.Submit();

			var mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Submit(), Times.AtMost(2)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.StartsWith(
				"\r\nExpected invocation on the mock at most 2 times, but was 3 times: foo => foo.Submit()"));
		}

		[Fact]
		public void ThrowsIfVerifyVoidNeverAndOneCall()
		{
			var mock = new Mock<IFoo>();

			mock.Verify(foo => foo.Submit(), Times.Never());

			mock.Object.Submit();

			var mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Never()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.StartsWith(
				"\r\nExpected invocation on the mock should never have been performed, but was 1 times: foo => foo.Submit()"));
		}

		[Fact]
		public void ThrowsIfVerifyVoidAtLeastOnceAndNotCalls()
		{
			var mock = new Mock<IFoo>();

			var mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.AtLeastOnce()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.StartsWith(
				"\r\nExpected invocation on the mock at least once, but was never performed: foo => foo.Submit()"));

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.AtLeastOnce());
		}

		[Fact]
		public void ThrowsIfVerifyVoidAtLeastAndLessThanNCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Submit();
			mock.Object.Submit();

			var mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.AtLeast(3)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.StartsWith(
				"\r\nExpected invocation on the mock at least 3 times, but was 2 times: foo => foo.Submit()"));

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

			var mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Exactly(5)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.StartsWith(
				"\r\nExpected invocation on the mock exactly 5 times, but was 4 times: foo => foo.Submit()"));

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Exactly(5));

			mock.Object.Submit();

			mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Exactly(5)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.StartsWith(
				"\r\nExpected invocation on the mock exactly 5 times, but was 6 times: foo => foo.Submit()"));
		}

		[Fact]
		public void ThrowsIfVerifyVoidOnceAndLessOrMoreThanACall()
		{
			var mock = new Mock<IFoo>();

			var mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Once()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.StartsWith(
				"\r\nExpected invocation on the mock once, but was 0 times: foo => foo.Submit()"));

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Once());

			mock.Object.Submit();

			mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Once()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.StartsWith(
				"\r\nExpected invocation on the mock once, but was 2 times: foo => foo.Submit()"));
		}

		[Fact]
		public void ThrowsIfVerifyVoidBetweenExclusiveAndLessOrEqualsFromOrMoreOrEqualToCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Submit();

			var mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.StartsWith(
				"\r\nExpected invocation on the mock between 1 and 4 times (Exclusive), but was 1 times: foo => foo.Submit()"));

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive));

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive));

			mock.Object.Submit();

			mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.StartsWith(
				"\r\nExpected invocation on the mock between 1 and 4 times (Exclusive), but was 4 times: foo => foo.Submit()"));
		}

		[Fact]
		public void ThrowsIfVerifyVoidBetweenInclusiveAndLessFromOrMoreToCalls()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Submit();

			var mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.StartsWith(
				"\r\nExpected invocation on the mock between 2 and 4 times (Inclusive), but was 1 times: foo => foo.Submit()"));

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive));

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive));

			mock.Object.Submit();
			mock.Object.Submit();

			mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.True(mex.Message.StartsWith(
				"\r\nExpected invocation on the mock between 2 and 4 times (Inclusive), but was 5 times: foo => foo.Submit()"));
		}

		[Fact]
		public void ThrowsIfVerifyReturningAtMostOnceAndMoreThanOneCall()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.AtMostOnce());

			mock.Object.Execute("");

			MockException mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Execute(""), Times.AtMostOnce()));
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

			MockException mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Execute(""), Times.AtMost(2)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyReturningNeverAndOneCall()
		{
			var mock = new Mock<IFoo>();

			mock.Verify(foo => foo.Execute(""), Times.Never());

			mock.Object.Execute("");

			MockException mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Execute(""), Times.Never()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyReturningAtLeastOnceAndNotCalls()
		{
			var mock = new Mock<IFoo>();

			MockException mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Execute(""), Times.AtLeastOnce()));
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

			MockException mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Execute(""), Times.AtLeast(3)));
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

			MockException mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Execute(""), Times.Exactly(5)));
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

			MockException mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Execute(""), Times.Between(1, 4, Range.Exclusive)));
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

			mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Execute(""), Times.Between(2, 4, Range.Inclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyGetGetAtMostOnceAndMoreThanOneCall()
		{
			var mock = new Mock<IFoo>();
			var value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.AtMostOnce());

			value = mock.Object.Value;

			MockException mex = Assert.Throws<MockException>(
				() => mock.VerifyGet(foo => foo.Value, Times.AtMostOnce()));
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

			MockException mex = Assert.Throws<MockException>(
				() => mock.VerifyGet(foo => foo.Value, Times.AtMost(2)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyGetGetNeverAndOneCall()
		{
			var mock = new Mock<IFoo>();

			mock.VerifyGet(foo => foo.Value, Times.Never());

			var value = mock.Object.Value;

			MockException mex = Assert.Throws<MockException>(
				() => mock.VerifyGet(foo => foo.Value, Times.Never()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void ThrowsIfVerifyGetGetAtLeastOnceAndNotCalls()
		{
			var mock = new Mock<IFoo>();

			MockException mex = Assert.Throws<MockException>(
				() => mock.VerifyGet(foo => foo.Value, Times.AtLeastOnce()));
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

			MockException mex = Assert.Throws<MockException>(
				() => mock.VerifyGet(foo => foo.Value, Times.AtLeast(3)));
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

			MockException mex = Assert.Throws<MockException>(
				() => mock.VerifyGet(foo => foo.Value, Times.Exactly(5)));
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

			MockException mex = Assert.Throws<MockException>(
				() => mock.VerifyGet(foo => foo.Value, Times.Between(1, 4, Range.Exclusive)));
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

			MockException mex = Assert.Throws<MockException>(
				() => mock.VerifyGet(foo => foo.Value, Times.Between(2, 4, Range.Inclusive)));
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

		[Fact]
		public void ThrowsIfVerifySetAtMostOnceAndMoreThanOneCall()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Value = 3;

			mock.VerifySet(f => f.Value = 3, Times.AtMostOnce());

			mock.Object.Value = 3;

			MockException mex = Assert.Throws<MockException>(
				() => mock.VerifySet(f => f.Value = 3, Times.AtMostOnce()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
		}

		[Fact]
		public void IncludesActualCallsInFailureMessage()
		{
			var mock = new Moq.Mock<IFoo>();

			mock.Object.Execute("ping");
			mock.Object.Echo(42);
			mock.Object.Submit();

			var mex = Assert.Throws<MockException>(() => mock.Verify(f => f.Execute("pong")));

			Assert.Contains(
				Environment.NewLine +
				"Performed invocations:" + Environment.NewLine +
				"IFoo.Execute(\"ping\")" + Environment.NewLine +
				"IFoo.Echo(42)" + Environment.NewLine +
				"IFoo.Submit()",
				mex.Message);
		}

		[Fact]
		public void IncludesMessageAboutNoActualCallsInFailureMessage()
		{
			var mock = new Moq.Mock<IFoo>();

			MockException mex = Assert.Throws<MockException>(() => mock.Verify(f => f.Execute("pong")));

			Assert.Contains(Environment.NewLine + "No invocations performed.", mex.Message);
		}

        [Fact]
        public void MatchesDerivedTypesForGenericTypes()
        {
            var mock = new Mock<IBaz>();
            mock.Object.Call(new BazParam());
            mock.Object.Call(new BazParam2());

            mock.Verify(foo => foo.Call(It.IsAny<IBazParam>()), Times.Exactly(2));
        }

#if !SILVERLIGHT
        [Fact]
        public void DoesNotThrowCollectionModifiedWhenMoreInvocationsInterceptedDuringVerfication()
        {
            var mock = new Mock<IFoo>();
            Parallel.For(0, 100, (i) =>
            {
                mock.Object.Submit();
                mock.Verify(foo => foo.Submit());
            });
        }
#endif

		public interface IBar
		{
			int? Value { get; set; }
		}

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

        public interface IBazParam
        {
        }

        public interface IBaz
        {
            void Call<T>(T param) where T:IBazParam;
        }

        public class BazParam:IBazParam
        {
        }

        public class BazParam2:BazParam
        {
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
