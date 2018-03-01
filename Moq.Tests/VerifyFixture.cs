using System;
using Moq;
using Xunit;

using System.Threading.Tasks;

namespace Moq.Tests
{
	public class VerifyFixture
	{
		[Fact]
		public void ThrowsIfVerifiableExpectationNotCalled()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Submit()).Verifiable();

			var mex = Assert.Throws<MockException>(() => mock.Verify());
			Assert.True(mex.IsVerificationError);
		}

		[Fact]
		public void ThrowsIfVerifiableExpectationNotCalledWithMessage()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Submit()).Verifiable("Kaboom!");

			var mex = Assert.Throws<MockException>(() => mock.Verify());
			Assert.True(mex.IsVerificationError);
			Assert.Contains("Kaboom!", mex.Message);
		}

		[Fact]
		public void ThrowsWithEvaluatedExpressionsIfVerifiableExpectationNotCalled()
		{
			var expectedArg = "lorem,ipsum";
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Execute(expectedArg.Substring(0, 5)))
				.Returns("ack")
				.Verifiable();

			var mex = Assert.Throws<MockException>(() => mock.Verify());
			Assert.True(mex.IsVerificationError);
			Assert.True(mex.Message.Contains(@".Execute(""lorem"")"), "Contains evaluated expected argument.");
		}

		[Fact]
		public void ThrowsWithExpressionIfVerifiableExpectationWithLambdaMatcherNotCalled()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(x => x.Execute(It.Is<string>(s => string.IsNullOrEmpty(s))))
				.Returns("ack")
				.Verifiable();

			var mex = Assert.Throws<MockException>(() => mock.Verify());
			Assert.True(mex.IsVerificationError);
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

			var mex = Assert.Throws<MockException>(() => mock.VerifyAll());
			Assert.True(mex.IsVerificationError);
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
			Assert.Contains("Execute should have been invoked with 'ping'", me.Message);
			Assert.Contains("f.Execute(\"ping\")", me.Message);
		}

		[Fact]
		public void VerifiesVoidMethodWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			var me = Assert.Throws<MockException>(
				() => mock.Verify(f => f.Submit(), "Submit should be invoked"));
			Assert.Contains("Submit should be invoked", me.Message);
			Assert.Contains("f.Submit()", me.Message);
		}

		[Fact]
		public void VerifiesPropertyGetWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			var me = Assert.Throws<MockException>(() => mock.VerifyGet(f => f.Value, "Nobody called .Value"));
			Assert.Contains("Nobody called .Value", me.Message);
			Assert.Contains("f.Value", me.Message);
		}

		[Fact]
		public void VerifiesPropertySetWithExpressionAndMessage()
		{
			var mock = new Mock<IFoo>();

			var me = Assert.Throws<MockException>(() => mock.VerifySet(f => f.Value = It.IsAny<int?>(), "Nobody called .Value"));
			Assert.Contains("Nobody called .Value", me.Message);
			Assert.Contains("f.Value", me.Message);
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

			Assert.Contains("Execute should have been invoked with 'ping'", e.Message);
			Assert.Contains("f.Execute(\"ping\")", e.Message);
		}

		[Fact]
		public void AsInferfaceVerifiesVoidMethodWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			var e = Assert.Throws<MockException>(() => mock.Verify(f => f.Submit(), "Submit should be invoked"));

			Assert.Contains("Submit should be invoked", e.Message);
			Assert.Contains("f.Submit()", e.Message);
		}

		[Fact]
		public void AsInterfaceVerifiesPropertyGetWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IFoo>();

			var e = Assert.Throws<MockException>(() => mock.VerifyGet(f => f.Value, "Nobody called .Value"));
			Assert.Contains("Nobody called .Value", e.Message);
			Assert.Contains("f.Value", e.Message);
		}

		[Fact]
		public void AsInterfaceVerifiesPropertySetWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IBar>();

			var e = Assert.Throws<MockException>(
				() => mock.VerifySet(f => f.Value = It.IsAny<int?>(), "Nobody called .Value"));
			Assert.Contains("Nobody called .Value", e.Message);
			Assert.Contains("f.Value", e.Message);
		}

		[Fact]
		public void AsInterfaceVerifiesPropertySetValueWithExpressionAndMessage()
		{
			var disposable = new Mock<IDisposable>();
			var mock = disposable.As<IBar>();

			var e = Assert.Throws<MockException>(() => mock.VerifySet(f => f.Value = 5, "Nobody called .Value"));
			Assert.Contains("Nobody called .Value", e.Message);
			Assert.Contains("f.Value", e.Message);
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
			Assert.Contains("Expected invocation on the mock at most once, but was 2 times: foo => foo.Submit()", mex.Message);
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
			Assert.Contains("Expected invocation on the mock at most 2 times, but was 3 times: foo => foo.Submit()", mex.Message);
		}

		[Fact]
		public void ThrowsIfVerifyVoidNeverAndOneCall()
		{
			var mock = new Mock<IFoo>();

			mock.Verify(foo => foo.Submit(), Times.Never());

			mock.Object.Submit();

			var mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Never()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.Contains("Expected invocation on the mock should never have been performed, but was 1 times: foo => foo.Submit()", mex.Message);
		}

		[Fact]
		public void ThrowsIfVerifyVoidAtLeastOnceAndNotCalls()
		{
			var mock = new Mock<IFoo>();

			var mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.AtLeastOnce()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.Contains("Expected invocation on the mock at least once, but was never performed: foo => foo.Submit()", mex.Message);

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
			Assert.Contains("Expected invocation on the mock at least 3 times, but was 2 times: foo => foo.Submit()", mex.Message);

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
			Assert.Contains("Expected invocation on the mock exactly 5 times, but was 4 times: foo => foo.Submit()", mex.Message);

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Exactly(5));

			mock.Object.Submit();

			mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Exactly(5)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.Contains("Expected invocation on the mock exactly 5 times, but was 6 times: foo => foo.Submit()", mex.Message);
		}

		[Fact]
		public void ThrowsIfVerifyVoidOnceAndLessOrMoreThanACall()
		{
			var mock = new Mock<IFoo>();

			var mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Once()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.Contains("Expected invocation on the mock once, but was 0 times: foo => foo.Submit()", mex.Message);

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Once());

			mock.Object.Submit();

			mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Once()));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.Contains("Expected invocation on the mock once, but was 2 times: foo => foo.Submit()", mex.Message);
		}

		[Fact]
		public void ThrowsIfVerifyVoidBetweenExclusiveAndLessOrEqualsFromOrMoreOrEqualToCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Submit();

			var mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.Contains("Expected invocation on the mock between 1 and 4 times (Exclusive), but was 1 times: foo => foo.Submit()", mex.Message);

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive));

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive));

			mock.Object.Submit();

			mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.Contains("Expected invocation on the mock between 1 and 4 times (Exclusive), but was 4 times: foo => foo.Submit()", mex.Message);
		}

		[Fact]
		public void ThrowsIfVerifyVoidBetweenInclusiveAndLessFromOrMoreToCalls()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Submit();

			var mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.Contains("Expected invocation on the mock between 2 and 4 times (Inclusive), but was 1 times: foo => foo.Submit()", mex.Message);

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive));

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive));

			mock.Object.Submit();
			mock.Object.Submit();

			mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive)));
			Assert.Equal(MockException.ExceptionReason.VerificationFailed, mex.Reason);
			Assert.Contains("Expected invocation on the mock between 2 and 4 times (Inclusive), but was 5 times: foo => foo.Submit()", mex.Message);
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
			mock.Object.Save(new object[] {1, 2, "hello"});

			var mex = Assert.Throws<MockException>(() => mock.Verify(f => f.Execute("pong")));

			Assert.Contains(
				Environment.NewLine +
				"Performed invocations: " + Environment.NewLine +
				"IFoo.Execute(\"ping\")" + Environment.NewLine +
				"IFoo.Echo(42)" + Environment.NewLine +
				"IFoo.Submit()" + Environment.NewLine +
				"IFoo.Save([1, 2, \"hello\"])",
				mex.Message);
		}

		[Fact]
		public void IncludesActualValuesFromVerifyNotVariableNames()
		{
			var expectedArg = "lorem,ipsum";
			var mock = new Moq.Mock<IFoo>();

			var mex = Assert.Throws<MockException>(() => mock.Verify(f => f.Execute(expectedArg.Substring(0, 5))));
			Assert.Contains("f.Execute(\"lorem\")", mex.Message);
		}

		[Fact]
		public void IncludesActualValuesFromSetups()
		{
			var expectedArg = "lorem,ipsum";
			var mock = new Moq.Mock<IFoo>();
			mock.Setup(f => f.Save(expectedArg.Substring(0, 5)));

			var mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Save("never")));
			Assert.Contains("f.Save(\"lorem\")", mex.Message);
		}

		[Fact]
		public void IncludesMessageAboutNoActualCallsInFailureMessage()
		{
			var mock = new Moq.Mock<IFoo>();

			MockException mex = Assert.Throws<MockException>(() => mock.Verify(f => f.Execute("pong")));

			Assert.Contains(Environment.NewLine + "No invocations performed.", mex.Message);
		}

		[Fact]
		public void IncludesMessageAboutNoSetupCallsInFailureMessage()
		{
			var mock = new Moq.Mock<IFoo>();

			MockException mex = Assert.Throws<MockException>(() => mock.Verify(f => f.Execute("pong")));

			Assert.Contains(Environment.NewLine + "No setups configured.", mex.Message);
		}

		[Fact]
		public void MatchesDerivedTypesForGenericTypes()
		{
			var mock = new Mock<IBaz>();
			mock.Object.Call(new BazParam());
			mock.Object.Call(new BazParam2());

			mock.Verify(foo => foo.Call(It.IsAny<IBazParam>()), Times.Exactly(2));
		}

		[Fact]
		public void NullArrayValuesForActualInvocationArePrintedAsNullInMockExeptionMessage()
		{
			var strings = new string[] { "1", null, "3" };
			var mock = new Mock<IArrays>();
			mock.Object.Method(strings);
			var mex = Assert.Throws<MockException>(() => mock.Verify(_ => _.Method(null)));
			Assert.Contains(
				@"Performed invocations: " + Environment.NewLine +
				@"IArrays.Method([""1"", null, ""3""])",
				mex.Message);
		}

		[Fact]
		public void LargeEnumerablesInActualInvocationAreNotCutOffFor10Elements()
		{
			var strings = new string[] { "1", null, "3", "4", "5", "6", "7", "8", "9", "10" };
			var mock = new Mock<IArrays>();
			mock.Object.Method(strings);
			var mex = Assert.Throws<MockException>(() => mock.Verify(_ => _.Method(null)));
			Assert.Contains(
				@"Performed invocations: " + Environment.NewLine +
				@"IArrays.Method([""1"", null, ""3"", ""4"", ""5"", ""6"", ""7"", ""8"", ""9"", ""10""])",
				mex.Message);
		}

		[Fact]
		public void LargeEnumerablesInActualInvocationAreCutOffAfter10Elements()
		{
			var strings = new string[] { "1", null, "3", "4", "5", "6", "7", "8", "9", "10", "11" };
			var mock = new Mock<IArrays>();
			mock.Object.Method(strings);
			var mex = Assert.Throws<MockException>(() => mock.Verify(_ => _.Method(null)));
			Assert.Contains(
				@"Performed invocations: " + Environment.NewLine +
				@"IArrays.Method([""1"", null, ""3"", ""4"", ""5"", ""6"", ""7"", ""8"", ""9"", ""10"", ...])",
				mex.Message);
		}

		[Fact]
		public void NullArrayValuesForExpectedInvocationArePrintedAsNullInMockExeptionMessage()
		{
			var strings = new string[] { "1", null, "3" };
			var mock = new Mock<IArrays>();
			mock.Object.Method(null);
			var mex = Assert.Throws<MockException>(() => mock.Verify(_ => _.Method(strings)));
			Assert.Contains(@"Expected invocation on the mock at least once, but was never performed: _ => _.Method([""1"", null, ""3""])", mex.Message);
		}

		[Fact]
		public void LargeEnumerablesInExpectedInvocationAreNotCutOffFor10Elements()
		{
			var strings = new string[] { "1", null, "3", "4", "5", "6", "7", "8", "9", "10" };
			var mock = new Mock<IArrays>();
			mock.Object.Method(null);
			var mex = Assert.Throws<MockException>(() => mock.Verify(_ => _.Method(strings)));
			Assert.Contains(@"Expected invocation on the mock at least once, but was never performed: _ => _.Method([""1"", null, ""3"", ""4"", ""5"", ""6"", ""7"", ""8"", ""9"", ""10""])", mex.Message);
		}

		[Fact]
		public void LargeEnumerablesInExpectedInvocationAreCutOffAfter10Elements()
		{
			var strings = new string[] { "1", null, "3", "4", "5", "6", "7", "8", "9", "10", "11" };
			var mock = new Mock<IArrays>();
			mock.Object.Method(null);
			var mex = Assert.Throws<MockException>(() => mock.Verify(_ => _.Method(strings)));
			Assert.Contains(@"Expected invocation on the mock at least once, but was never performed: _ => _.Method([""1"", null, ""3"", ""4"", ""5"", ""6"", ""7"", ""8"", ""9"", ""10"", ...])", mex.Message);
		}

		/// <summary>
		/// Warning, this is a flaky test and doesn't fail when run as standalone. Running all tests at once will increase the chances of that test to fail.
		/// </summary>
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

#if !NETCORE
		[Fact]
		public void Enabling_diagnostic_file_info_leads_to_that_information_in_verification_error_messages()
		{
			var repository = new MockRepository(MockBehavior.Default);
			repository.Switches |= Switches.CollectDiagnosticFileInfoForSetups;

			var mock = repository.Create<IFoo>();
			mock.Setup(m => m.Submit());

			var ex = Assert.Throws<MockException>(() => repository.VerifyAll());
			Assert.Contains("in ", ex.Message);
			Assert.Contains("VerifyFixture.cs: line ", ex.Message);
		}
#endif

		[Fact]
		public void Disabling_diagnostic_file_info_leads_to_that_information_missing_in_verification_error_messages()
		{
			var repository = new MockRepository(MockBehavior.Default);
			repository.Switches &= ~Switches.CollectDiagnosticFileInfoForSetups;

			var mock = repository.Create<IFoo>();
			mock.Setup(m => m.Submit());

			var ex = Assert.Throws<MockException>(() => repository.VerifyAll());
			Assert.DoesNotContain("in ", ex.Message);
			Assert.DoesNotContain("VerifyFixture.cs: line ", ex.Message);
		}

		[Fact]
		public void CanVerifyMethodThatIsNamedLikeEventAddAccessor()
		{
			var mock = new Mock<IHaveMethodsNamedLikeEventAccessors>();
			mock.Object.add_Something();
			mock.Verify(m => m.add_Something(), Times.Once);
		}

		[Fact]
		public void CanVerifyMethodThatIsNamedLikeEventRemoveAccessor()
		{
			var mock = new Mock<IHaveMethodsNamedLikeEventAccessors>();
			mock.Object.remove_Something();
			mock.Verify(m => m.remove_Something(), Times.Once);
		}

		[Fact]
		public void Verify_ignores_conditional_setups()
		{
			var mock = new Mock<IFoo>();
			mock.When(() => true).Setup(m => m.Submit()).Verifiable();

			var exception = Record.Exception(() =>
			{
				mock.Verify();
			});

			Assert.Null(exception);
		}

		[Fact]
		public void VerifyAll_ignores_conditional_setups()
		{
			var mock = new Mock<IFoo>();
			mock.When(() => true).Setup(m => m.Submit());

			var exception = Record.Exception(() =>
			{
				mock.VerifyAll();
			});

			Assert.Null(exception);
		}

		[Fact, Obsolete("As long as SetupSet(Expression) still exists, this test is required.")]
		public void SetupGet_property_does_not_override_SetupSet_for_same_property_and_with_same_setup_expression()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet(m => m.Value).Verifiable("setup for setter");
			mock.SetupGet(m => m.Value).Verifiable("setup for getter");

			var _ = mock.Object.Value;
			var exception = Record.Exception(() =>
			{
				mock.VerifyAll();
			});

			Assert.IsAssignableFrom<MockException>(exception);
			Assert.Contains("setup for setter:", exception.Message);
		}

		[Fact, Obsolete("As long as SetupSet(Expression) still exists, this test is required.")]
		public void SetupSet_property_does_not_override_SetupGet_for_same_property_and_with_same_setup_expression()
		{
			var mock = new Mock<IFoo>();

			mock.SetupGet(m => m.Value).Verifiable("setup for getter");
			mock.SetupSet(m => m.Value).Verifiable("setup for setter");

			mock.Object.Value = 42;
			var exception = Record.Exception(() =>
			{
				mock.VerifyAll();
			});

			Assert.IsAssignableFrom<MockException>(exception);
			Assert.Contains("setup for getter:", exception.Message);
		}

		[Fact]
		public void Verify_if_successful_marks_matched_invocation_as_verified()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Submit();

			var invocation = mock.Invocations.ToArray()[0];
			Assert.False(invocation.Verified);

			mock.Verify(m => m.Submit());
			Assert.True(invocation.Verified);
		}

		[Fact]
		public void Verify_if_successful_marks_only_matched_invocations_as_verified()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Echo(1);
			mock.Object.Echo(2);
			mock.Object.Echo(3);

			var invocations = mock.Invocations.ToArray();
			Assert.False(invocations[0].Verified);
			Assert.False(invocations[1].Verified);
			Assert.False(invocations[2].Verified);

			mock.Verify(m => m.Echo(It.Is<int>(i => i != 2)));
			Assert.True(invocations[0].Verified);
			Assert.False(invocations[1].Verified);
			Assert.True(invocations[2].Verified);
		}

		[Fact]
		public void Verify_if_unsuccessful_marks_no_matched_invocations_as_verified()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Echo(1);
			mock.Object.Echo(2);
			mock.Object.Echo(3);

			var invocations = mock.Invocations.ToArray();
			Assert.False(invocations[0].Verified);
			Assert.False(invocations[1].Verified);
			Assert.False(invocations[2].Verified);

			Assert.Throws<MockException>(() => mock.Verify(m => m.Echo(It.Is<int>(i => i != 2)), Times.Exactly(1)));
			Assert.False(invocations[0].Verified);
			Assert.False(invocations[1].Verified);
			Assert.False(invocations[2].Verified);
		}

		[Fact]
		public void VerifyNoOtherCalls_succeeds_if_no_calls_were_made()
		{
			var mock = new Mock<IFoo>();
			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyNoOtherCalls_succeeds_if_no_calls_were_made_on_mock_created_by_Mock_Of()
		{
			var mocked = Mock.Of<IFoo>();
			var mock = Mock.Get(mocked);

			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyNoOtherCalls_succeeds_if_no_calls_were_made_on_mock_created_by_Mock_Of_with_single_dot_predicate()
		{
			var mocked = Mock.Of<IFoo>(m => m.Value == 1);
			var mock = Mock.Get(mocked);

			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyNoOtherCalls_succeeds_if_no_calls_were_made_on_mock_created_by_Mock_Of_with_multi_dot_predicate()
		{
			var mocked = Mock.Of<IFoo>(m => m.Bar.Value == 1);
			var mock = Mock.Get(mocked);

			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyNoOtherCalls_fails_if_an_unverified_call_was_made()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Echo(1);
			Assert.Throws<MockException>(() => mock.VerifyNoOtherCalls());
		}

		[Fact]
		public void VerifyNoOtherCalls_includes_unverified_calls_in_exception_message()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Echo(1);
			var ex = Assert.Throws<MockException>(() => mock.VerifyNoOtherCalls());
			Assert.Contains(".Echo(1)", ex.Message);
		}

		[Fact]
		public void VerifyNoOtherCalls_succeeds_if_a_verified_call_was_made()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Echo(1);
			mock.Verify(m => m.Echo(1));
			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyNoOtherCalls_succeeds_if_several_verified_call_were_made()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Echo(1);
			mock.Object.Echo(2);
			mock.Object.Echo(3);
			mock.Object.Submit();
			mock.Verify(m => m.Echo(It.IsAny<int>()));
			mock.Verify(m => m.Submit());
			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyNoOtherCalls_fails_if_several_verified_calls_and_several_unverified_call_were_made()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Echo(1);
			mock.Object.Echo(2);
			mock.Object.Echo(3);
			mock.Object.Submit();
			mock.Verify(m => m.Echo(It.Is<int>(i => i > 1)));
			var ex = Assert.Throws<MockException>(() => mock.VerifyNoOtherCalls());
			Assert.Contains(".Echo(1)", ex.Message);
			Assert.Contains(".Submit()", ex.Message);
		}

		[Fact]
		public void VerifyNoOtherCalls_succeeds_with_DefaultValue_Mock_and_multi_dot_Verify_expression()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			mock.Object.Bar.Poke();

			mock.Verify(m => m.Bar.Poke());
			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyNoOtherCalls_succeeds_with_DefaultValue_Mock_and_multi_dot_VerifyGet_expression()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			var value = mock.Object.Bar.Value;

			mock.VerifyGet(m => m.Bar.Value);
			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyNoOtherCalls_succeeds_with_DefaultValue_Mock_and_multi_dot_VerifySet_expression()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			mock.Object.Bar.Value = 42;

			mock.VerifySet(m => m.Bar.Value = 42);
			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyNoOtherCalls_performs_recursive_verification()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			mock.Object.Bar.Poke();

			mock.VerifyGet(m => m.Bar);
			Assert.Throws<MockException>(() => mock.VerifyNoOtherCalls()); // should fail due to the unverified call to `Poke`
		}

		[Fact]
		public void VerifyNoOtherCalls_requires_explicit_verification_of_automocked_properties_that_are_not_used_transitively()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };

			var _ = mock.Object.Bar;

			// Even though `Bar` is mockable and will be automatically mocked, it isn't used "transitively",
			// i.e. in a way to get at one of its members. Therefore, it ought to be verified explicitly.
			// Because we don't verify it, a verification exception should be thrown:
			Assert.Throws<MockException>(() => mock.VerifyNoOtherCalls());
		}

		[Fact(Skip = "Not yet implemented.")]
		public void VerifyNoOtherCalls_can_tell_apart_transitive_and_nontransitive_usages_of_automocked_properties()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };

			object _;
			_ = mock.Object.Bar;
			_ = mock.Object.Bar.Value;

			mock.Verify(m => m.Bar.Value);

			// `Bar` was used both in a "transitive" and non-transitive way. We would expect that the former
			// doesn't have to be explicitly verified (as it's implied by the verification of `Bar.Value`).
			// However, the non-transitive call ought to be explicitly verified. Because we don't, a verific-
			// ation exception is expected: (THIS DOES NOT WORK YET.)
			Assert.Throws<MockException>(() => mock.VerifyNoOtherCalls());

			// HINT TO IMPLEMENTERS: One relatively easy way to implement this, given the way Moq is currently
			// build, would be to record all invocations with a globally unique, steadily increasing sequence
			// number. This would make it possible to say, for any two invocations (regardless of the mock on
			// which they occurred), which one happened earlier. Let's look at two calls of method X. The
			// earlier invocation happens at "time" t0, the later invocation happens at "time" t1 (t0 < t1).
			// If X returns a mock object, and that object has no invocations happening between t0 and t1,
			// then the first invocation of X was non-transitive. Likewise, the very last invocation of method
			// X is non-transitive if there are no invocations on the sub-object that occur later.
		}

		public interface IBar
		{
			int? Value { get; set; }
			void Poke();
		}

		public interface IFoo
		{
			IBar Bar { get; }
			int WriteOnly { set; }
			int? Value { get; set; }
			void EchoRef<T>(ref T value);
			void EchoOut<T>(out T value);
			int Echo(int value);
			void Submit();
			string Execute(string command);
			void Save(object o);
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

		public interface IArrays
		{
			void Method(string[] strings);
		}

		public interface IHaveMethodsNamedLikeEventAccessors
		{
			void add_Something();
			void remove_Something();
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

			var ex = Assert.Throws<MockException>(() => mock.VerifyAll());
			Assert.True(ex.IsVerificationError);
			Assert.Contains("x => x.Submit()", ex.Message);
			Assert.Contains("x => x.Echo(1)", ex.Message);
			Assert.Contains("x => x.Execute(\"ping\")", ex.Message);
		}
	}
}
