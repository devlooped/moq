// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Moq;
using Moq.Protected;

using Xunit;

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
			Assert.Contains(@".Execute(It.Is<string>(s => string.IsNullOrEmpty(s)))", mex.Message);
		}

		[Fact]
		public void ThrowsWithExpressionIfVerifiableExpectationWithLambdaMatcherVariableNotCalled()
		{
			var mock = new Mock<IFoo>();

			Expression<Func<string, bool>> nullOrEmpty = s => string.IsNullOrEmpty(s);

			mock.Setup(x => x.Execute(It.Is(nullOrEmpty)))
				.Returns("ack")
				.Verifiable();

			var mex = Assert.Throws<MockException>(() => mock.Verify());
			Assert.True(mex.IsVerificationError);
			Assert.Contains(@".Execute(It.Is<string>(s => string.IsNullOrEmpty(s)))", mex.Message);
		}

		[Fact]
		public void ThrowsWithExpressionIfVerifiableExpectationWithLambdaMatcherVariableAndClosureAccessNotCalled()
		{
			var mock = new Mock<IFoo>();

			string password = "abc123";

			Expression<Func<string, bool>> matchingPassword = s => s == password;

			mock.Setup(x => x.Execute(It.Is(matchingPassword)))
				.Returns("ack")
				.Verifiable();

			var mex = Assert.Throws<MockException>(() => mock.Verify());
			Assert.True(mex.IsVerificationError);
			Assert.Contains(@".Execute(It.Is<string>(s => s == password)", mex.Message);
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
			Assert.True(mex.IsVerificationError);
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
			Assert.True(mex.IsVerificationError);
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

			var mex = Assert.Throws<MockException>(() => mock.VerifyGet(f => f.Value));
			Assert.True(mex.IsVerificationError);
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

			var mex = Assert.Throws<MockException>(() => mock.VerifySet(f => f.Value = It.IsAny<int?>()));
			Assert.True(mex.IsVerificationError);
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
			Assert.True(mex.IsVerificationError);
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
			Assert.True(mex.IsVerificationError);
			Assert.Contains("Expected invocation on the mock at most 2 times, but was 3 times: foo => foo.Submit()", mex.Message);
		}

		[Fact]
		public void ThrowsIfVerifyVoidNeverAndOneCall()
		{
			var mock = new Mock<IFoo>();

			mock.Verify(foo => foo.Submit(), Times.Never());

			mock.Object.Submit();

			var mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Never()));
			Assert.True(mex.IsVerificationError);
			Assert.Contains("Expected invocation on the mock should never have been performed, but was 1 times: foo => foo.Submit()", mex.Message);
		}

		[Fact]
		public void ThrowsIfVerifyVoidAtLeastOnceAndNotCalls()
		{
			var mock = new Mock<IFoo>();

			var mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.AtLeastOnce()));
			Assert.True(mex.IsVerificationError);
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
			Assert.True(mex.IsVerificationError);
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
			Assert.True(mex.IsVerificationError);
			Assert.Contains("Expected invocation on the mock exactly 5 times, but was 4 times: foo => foo.Submit()", mex.Message);

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Exactly(5));

			mock.Object.Submit();

			mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Exactly(5)));
			Assert.True(mex.IsVerificationError);
			Assert.Contains("Expected invocation on the mock exactly 5 times, but was 6 times: foo => foo.Submit()", mex.Message);
		}

		[Fact]
		public void ThrowsIfVerifyVoidOnceAndLessOrMoreThanACall()
		{
			var mock = new Mock<IFoo>();

			var mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Once()));
			Assert.True(mex.IsVerificationError);
			Assert.Contains("Expected invocation on the mock once, but was 0 times: foo => foo.Submit()", mex.Message);

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Once());

			mock.Object.Submit();

			mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Submit(), Times.Once()));
			Assert.True(mex.IsVerificationError);
			Assert.Contains("Expected invocation on the mock once, but was 2 times: foo => foo.Submit()", mex.Message);
		}

		[Fact]
		public void ThrowsIfVerifyVoidBetweenExclusiveAndLessOrEqualsFromOrMoreOrEqualToCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Submit();

			var mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive)));
			Assert.True(mex.IsVerificationError);
			Assert.Contains("Expected invocation on the mock between 1 and 4 times (Exclusive), but was 1 times: foo => foo.Submit()", mex.Message);

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive));

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive));

			mock.Object.Submit();

			mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Submit(), Times.Between(1, 4, Range.Exclusive)));
			Assert.True(mex.IsVerificationError);
			Assert.Contains("Expected invocation on the mock between 1 and 4 times (Exclusive), but was 4 times: foo => foo.Submit()", mex.Message);
		}

		[Fact]
		public void ThrowsIfVerifyVoidBetweenInclusiveAndLessFromOrMoreToCalls()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Submit();

			var mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive)));
			Assert.True(mex.IsVerificationError);
			Assert.Contains("Expected invocation on the mock between 2 and 4 times (Inclusive), but was 1 times: foo => foo.Submit()", mex.Message);

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive));

			mock.Object.Submit();

			mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive));

			mock.Object.Submit();
			mock.Object.Submit();

			mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Submit(), Times.Between(2, 4, Range.Inclusive)));
			Assert.True(mex.IsVerificationError);
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
			Assert.True(mex.IsVerificationError);
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
			Assert.True(mex.IsVerificationError);
		}

		[Fact]
		public void ThrowsIfVerifyReturningNeverAndOneCall()
		{
			var mock = new Mock<IFoo>();

			mock.Verify(foo => foo.Execute(""), Times.Never());

			mock.Object.Execute("");

			MockException mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Execute(""), Times.Never()));
			Assert.True(mex.IsVerificationError);
		}

		[Fact]
		public void ThrowsIfVerifyReturningAtLeastOnceAndNotCalls()
		{
			var mock = new Mock<IFoo>();

			MockException mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Execute(""), Times.AtLeastOnce()));
			Assert.True(mex.IsVerificationError);

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
			Assert.True(mex.IsVerificationError);

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
			Assert.True(mex.IsVerificationError);

			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.Exactly(5));

			mock.Object.Execute("");

			mex = Assert.Throws<MockException>(() => mock.Verify(foo => foo.Execute(""), Times.Exactly(5)));
			Assert.True(mex.IsVerificationError);
		}

		[Fact]
		public void ThrowsIfVerifyReturningBetweenExclusiveAndLessOrEqualsFromOrMoreOrEqualToCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Execute("");

			MockException mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Execute(""), Times.Between(1, 4, Range.Exclusive)));
			Assert.True(mex.IsVerificationError);

			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.Between(1, 4, Range.Exclusive));

			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.Between(1, 4, Range.Exclusive));

			mock.Object.Execute("");

			mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Execute(""), Times.Between(1, 4, Range.Exclusive)));
			Assert.True(mex.IsVerificationError);
		}

		[Fact]
		public void ThrowsIfVerifyReturningBetweenInclusiveAndLessFromOrMoreToCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Execute("");

			MockException mex = Assert.Throws<MockException>(() =>
				mock.Verify(foo => foo.Execute(""), Times.Between(2, 4, Range.Inclusive)));
			Assert.True(mex.IsVerificationError);

			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.Between(2, 4, Range.Inclusive));

			mock.Object.Execute("");

			mock.Verify(foo => foo.Execute(""), Times.Between(2, 4, Range.Inclusive));

			mock.Object.Execute("");
			mock.Object.Execute("");

			mex = Assert.Throws<MockException>(
				() => mock.Verify(foo => foo.Execute(""), Times.Between(2, 4, Range.Inclusive)));
			Assert.True(mex.IsVerificationError);
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
			Assert.True(mex.IsVerificationError);
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
			Assert.True(mex.IsVerificationError);
		}

		[Fact]
		public void ThrowsIfVerifyGetGetNeverAndOneCall()
		{
			var mock = new Mock<IFoo>();

			mock.VerifyGet(foo => foo.Value, Times.Never());

			var value = mock.Object.Value;

			MockException mex = Assert.Throws<MockException>(
				() => mock.VerifyGet(foo => foo.Value, Times.Never()));
			Assert.True(mex.IsVerificationError);
		}

		[Fact]
		public void ThrowsIfVerifyGetGetAtLeastOnceAndNotCalls()
		{
			var mock = new Mock<IFoo>();

			MockException mex = Assert.Throws<MockException>(
				() => mock.VerifyGet(foo => foo.Value, Times.AtLeastOnce()));
			Assert.True(mex.IsVerificationError);

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
			Assert.True(mex.IsVerificationError);

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
			Assert.True(mex.IsVerificationError);

			value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.Exactly(5));

			value = mock.Object.Value;

			mex = Assert.Throws<MockException>(() => mock.VerifyGet(foo => foo.Value, Times.Exactly(5)));
			Assert.True(mex.IsVerificationError);
		}

		[Fact]
		public void ThrowsIfVerifyGetGetBetweenExclusiveAndLessOrEqualsFromOrMoreOrEqualToCalls()
		{
			var mock = new Mock<IFoo>();

			var value = mock.Object.Value;

			MockException mex = Assert.Throws<MockException>(
				() => mock.VerifyGet(foo => foo.Value, Times.Between(1, 4, Range.Exclusive)));
			Assert.True(mex.IsVerificationError);

			value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.Between(1, 4, Range.Exclusive));

			value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.Between(1, 4, Range.Exclusive));

			value = mock.Object.Value;

			mex = Assert.Throws<MockException>(() =>
				mock.VerifyGet(foo => foo.Value, Times.Between(1, 4, Range.Exclusive)));
			Assert.True(mex.IsVerificationError);
		}

		[Fact]
		public void ThrowsIfVerifyGetGetBetweenInclusiveAndLessFromOrMoreToCalls()
		{
			var mock = new Mock<IFoo>();

			var value = mock.Object.Value;

			MockException mex = Assert.Throws<MockException>(
				() => mock.VerifyGet(foo => foo.Value, Times.Between(2, 4, Range.Inclusive)));
			Assert.True(mex.IsVerificationError);

			value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.Between(2, 4, Range.Inclusive));

			value = mock.Object.Value;

			mock.VerifyGet(foo => foo.Value, Times.Between(2, 4, Range.Inclusive));

			value = mock.Object.Value;
			value = mock.Object.Value;

			mex = Assert.Throws<MockException>(() =>
				mock.VerifyGet(foo => foo.Value, Times.Between(2, 4, Range.Inclusive)));
			Assert.True(mex.IsVerificationError);
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
			Assert.True(mex.IsVerificationError);
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

			Assert.True(mex.Message.ContainsConsecutiveLines(
				"      VerifyFixture.IFoo.Execute(\"ping\")",
				"      VerifyFixture.IFoo.Echo(42)",
				"      VerifyFixture.IFoo.Submit()",
				"      VerifyFixture.IFoo.Save([1, 2, \"hello\"])"));
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
		public void IncludesMessageAboutNoActualCallsInFailureMessage()
		{
			var mock = new Moq.Mock<IFoo>();

			MockException mex = Assert.Throws<MockException>(() => mock.Verify(f => f.Execute("pong")));

			Assert.Contains("   No invocations performed.", mex.Message);
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
		public void Should_verify_derived_as_generic_parameters()
		{
			//Arrange
			var mock = new Mock<IBaz>();

			//Act
			mock.Object.Subscribe<BazParam2>();
			mock.Object.Subscribe<BazParam>();
			mock.Object.Subscribe<IBazParam>();

			//Assert
			mock.Verify(foo => foo.Subscribe<IBazParam>(), Times.Exactly(3));
			mock.Verify(foo => foo.Subscribe<BazParam>(), Times.Exactly(2));
			mock.Verify(foo => foo.Subscribe<BazParam2>(), Times.Once);
		}

		[Fact]
		public void Should_not_verify_nongeneric_when_generic_invoked()
		{
			//Arrange
			var mock = new Mock<IBaz>();

			//Act
			mock.Object.Subscribe<IBazParam>();

			//Assert
			mock.Verify(foo => foo.Subscribe<IBazParam>(), Times.Once);
			mock.Verify(foo => foo.Subscribe(), Times.Never);
		}

		[Fact]
		public void Should_not_verify_generic_when_nongeneric_invoked()
		{
			//Arrange
			var mock = new Mock<IBaz>();

			//Act
			mock.Object.Subscribe();

			//Assert
			mock.Verify(foo => foo.Subscribe<IBazParam>(), Times.Never);
			mock.Verify(foo => foo.Subscribe(), Times.Once);
		}

		[Fact]
		public void NullArrayValuesForActualInvocationArePrintedAsNullInMockExeptionMessage()
		{
			var strings = new string[] { "1", null, "3" };
			var mock = new Mock<IArrays>();
			mock.Object.Method(strings);
			var mex = Assert.Throws<MockException>(() => mock.Verify(_ => _.Method(null)));
			Assert.True(mex.Message.ContainsConsecutiveLines(
				@"      VerifyFixture.IArrays.Method([""1"", null, ""3""])"));
		}

		[Fact]
		public void LargeEnumerablesInActualInvocationAreNotCutOffFor10Elements()
		{
			var strings = new string[] { "1", null, "3", "4", "5", "6", "7", "8", "9", "10" };
			var mock = new Mock<IArrays>();
			mock.Object.Method(strings);
			var mex = Assert.Throws<MockException>(() => mock.Verify(_ => _.Method(null)));
			Assert.True(mex.Message.ContainsConsecutiveLines(
				@"      VerifyFixture.IArrays.Method([""1"", null, ""3"", ""4"", ""5"", ""6"", ""7"", ""8"", ""9"", ""10""])"));
		}

		[Fact]
		public void LargeEnumerablesInActualInvocationAreCutOffAfter10Elements()
		{
			var strings = new string[] { "1", null, "3", "4", "5", "6", "7", "8", "9", "10", "11" };
			var mock = new Mock<IArrays>();
			mock.Object.Method(strings);
			var mex = Assert.Throws<MockException>(() => mock.Verify(_ => _.Method(null)));
			Assert.True(mex.Message.ContainsConsecutiveLines(
				@"      VerifyFixture.IArrays.Method([""1"", null, ""3"", ""4"", ""5"", ""6"", ""7"", ""8"", ""9"", ""10"", ...])"));
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

			var invocation = mock.MutableInvocations.ToArray()[0];
			Assert.False(invocation.IsVerified);

			mock.Verify(m => m.Submit());
			Assert.True(invocation.IsVerified);
		}

		[Fact]
		public void Verify_if_successful_marks_only_matched_invocations_as_verified()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Echo(1);
			mock.Object.Echo(2);
			mock.Object.Echo(3);

			var invocations = mock.MutableInvocations.ToArray();
			Assert.False(invocations[0].IsVerified);
			Assert.False(invocations[1].IsVerified);
			Assert.False(invocations[2].IsVerified);

			mock.Verify(m => m.Echo(It.Is<int>(i => i != 2)));
			Assert.True(invocations[0].IsVerified);
			Assert.False(invocations[1].IsVerified);
			Assert.True(invocations[2].IsVerified);
		}

		[Fact]
		public void Verify_if_unsuccessful_marks_no_matched_invocations_as_verified()
		{
			var mock = new Mock<IFoo>();
			mock.Object.Echo(1);
			mock.Object.Echo(2);
			mock.Object.Echo(3);

			var invocations = mock.MutableInvocations.ToArray();
			Assert.False(invocations[0].IsVerified);
			Assert.False(invocations[1].IsVerified);
			Assert.False(invocations[2].IsVerified);

			Assert.Throws<MockException>(() => mock.Verify(m => m.Echo(It.Is<int>(i => i != 2)), Times.Exactly(1)));
			Assert.False(invocations[0].IsVerified);
			Assert.False(invocations[1].IsVerified);
			Assert.False(invocations[2].IsVerified);
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

		// (This test is somewhat duplicate, but sets the stage for the test following right after it.)
		[Fact]
		public void VerifyNoOtherCalls_works_together_with_parameterized_Verify()
		{
			var cat = new Mock<ICat>();
			cat.Setup(x => x.Purr(15)).Returns("happy");

			var mood = cat.Object.Purr(15);

			cat.Verify(x => x.Purr(15));
			Assert.Equal("happy", mood);
			cat.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyNoOtherCalls_works_together_with_parameterless_Verify()
		{
			var cat = new Mock<ICat>();
			cat.Setup(x => x.Purr(15)).Returns("happy").Verifiable();

			var mood = cat.Object.Purr(15);

			cat.Verify();
			Assert.Equal("happy", mood);
			cat.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyNoOtherCalls_works_together_with_parameterless_VerifyAll()
		{
			var cat = new Mock<ICat>();
			cat.Setup(x => x.Purr(15)).Returns("happy");

			var mood = cat.Object.Purr(15);

			cat.VerifyAll();
			Assert.Equal("happy", mood);
			cat.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyNoOtherCalls_still_complains_about_surplus_call_after_VerifyAll()
		{
			var cat = new Mock<ICat>();
			cat.Setup(x => x.Purr(15)).Returns("happy");

			var mood = cat.Object.Purr(15);
			cat.Object.Hiss();

			cat.VerifyAll();
			Assert.Throws<MockException>(() => cat.VerifyNoOtherCalls());
		}

		[Fact]
		public void VerifyNoOtherCalls_works_with_a_combination_of_parameterised_Verify_and_VerifyAll()
		{
			var cat = new Mock<ICat>();
			cat.Setup(x => x.Purr(15)).Returns("happy");

			var mood = cat.Object.Purr(15);
			cat.Object.Hiss();

			cat.VerifyAll();
			cat.Verify(x => x.Hiss());
			cat.VerifyNoOtherCalls();
		}

		[Fact]
		public void VerifyNoOtherCalls_works_together_with_parameterless_VerifyAll_for_sequence_setups()
		{
			var mock = new Mock<ICat>();
			mock.SetupSequence(x => x.Hiss());

			mock.Object.Hiss();

			mock.VerifyAll();
			mock.VerifyNoOtherCalls();
		}

		[Fact]
		public void Verification_error_message_contains_complete_call_expression_for_delegate_mock()
		{
			var mock = new Mock<Action>();
			mock.Setup(m => m());

			var ex = Record.Exception(() => mock.Verify(m => m(), Times.Once()));

			Assert.Contains("but was 0 times: m => m()", ex.Message);
		}

		[Fact]
		public void Verification_error_message_contains_complete_call_expression_for_delegate_mock_with_parameters()
		{
			var mock = new Mock<Action<int, int>>();
			mock.Setup(m => m(1, It.IsAny<int>()));

			var ex = Record.Exception(() => mock.Verify(m => m(1, 2), Times.Once()));

			Assert.Contains("but was 0 times: m => m(1, 2)", ex.Message);
		}

		[Fact]
		public void VerifyAll_ignores_setups_from_SetupAllProperties()
		{
			var mock = new Mock<IFoo>();
			mock.SetupAllProperties();

			// This shouldn't fail. The intent behind the call to `SetupAllProperties` is to conveniently
			// auto-implement all properties such that they remember the values they're being set to.
			// But despite the `Setup` in the method name, they shouldn't create observable setups that
			// require verification. Otherwise, one would have to invoke each and every property accessor
			// to make `VerifyAll` happy. (This problem is exacerbated by the fact that `Mock.Of<T>`
			// performs a hidden call to `SetupAllProperties`, meaning that `VerifyAll` is quite useless
			// for mocks created that way if one has to call each and every property accessor.)
			mock.VerifyAll();
		}

		[Fact]
		public void VerifyAll_ignores_setups_from_SetupAllProperties_but_not_other_property_setup()
		{
			var mock = new Mock<IFoo>();
			mock.SetupAllProperties();
			mock.Setup(m => m.Bar);

			Assert.Throws<MockException>(() => mock.VerifyAll());
		}

		[Fact]
		public void VerifyAll_ignores_setups_from_SetupAllProperties_but_not_other_property_setup_unless_matched()
		{
			var mock = new Mock<IFoo>();
			mock.SetupAllProperties();
			mock.Setup(m => m.Bar);

			_ = mock.Object.Bar;

			mock.VerifyAll();
		}

		[Fact]
		public void VerifyProtectedMethodOnChildClass()
		{
			var mock = new Mock<Child>();
			mock.Protected().Setup("Populate", exactParameterMatch: true, ItExpr.Ref<ChildDto>.IsAny).CallBase().Verifiable();
			ChildDto dto = new ChildDto();
			_ = mock.Object.InvokePopulate(ref dto);

			mock.Protected().Verify("Populate", Times.Once(), exactParameterMatch: true, ItExpr.Ref<ChildDto>.IsAny);
		}

		[Fact]
		public void Verify_on_non_overridable_method_throws_NotSupportedException()
		{
			var mock = new Mock<Child>();
			Assert.Throws<NotSupportedException>(() =>
				mock.Verify(m => m.InvokePopulate(ref It.Ref<ChildDto>.IsAny), Times.Never));
		}

		[Fact]
		public void Verification_marks_invocations_of_inner_mocks_as_verified()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			mock.Setup(m => m.Value).Returns(1);
			mock.Setup(m => m.Bar.Value).Returns(2);

			// Invoke everything that has been set up, and verify everything:
			_ = mock.Object.Value;
			_ = mock.Object.Bar.Value;
			mock.VerifyAll();

			// The above call to `VerifyAll` should have marked all invocations as verified,
			// including those on the inner `Bar` mock:
			Mock.Get(mock.Object.Bar).VerifyNoOtherCalls();
		}

		[Fact]
		public void Verify__marks_invocations_as_verified__even_if_the_setups_they_were_matched_by_were_conditional()
		{
			var mock = new Mock<IFoo>();
			mock.When(() => true).Setup(m => m.Submit()).Verifiable();
			mock.Object.Submit();
			mock.Verify();
			mock.VerifyNoOtherCalls();
		}

		public class Exclusion_of_unreachable_inner_mocks
		{
			[Fact]
			public void Failing_setup_detached_at_root_is_excluded_from_verification()
			{
				var xMock = new Mock<IX>();

				// Set up a call that would fail verification:
				xMock.Setup(x => x.Y.M()).Verifiable("M never called");

				// Reset the root `.Y` of the above setup `.Y.M()` to something that'll pass verification:
				xMock.Setup(x => x.Y).Verifiable();
				_ = xMock.Object.Y;

				// The first setup should be shadowed by the second, therefore verification should pass:
				xMock.Verify();
			}

			[Fact]
			public void Failing_setup_detached_by_resetting_stubbed_property_is_excluded_from_verification()
			{
				var xMock = new Mock<IX> { DefaultValue = DefaultValue.Mock };

				// Setup an inner mock (as the initial value of a stubbed property) that would fail verification:
				xMock.SetupAllProperties();
				Mock.Get(xMock.Object.Y).Setup(y => y.M()).Verifiable("M never called");

				// Reset the stubbed property to a different value:
				xMock.Object.Y = null;

				// Inner mock no longer reachable through `xMock`, verification should succeed:
				xMock.Verify();
			}

			public interface IX
			{
				IY Y { get; set; }
			}

			public interface IY
			{
				void M();
			}
		}

		public class Verify_forbidden_side_effects
		{
			[Fact]
			public void Does_not_create_setups_seen_by_VerifyAll()
			{
				var mock = new Mock<IX>();
				mock.Verify(m => m.X.X, Times.Never);
				mock.VerifyAll();
			}

			[Fact]
			public void Does_not_counteract_MockBehavior_Strict()
			{
				var mock = new Mock<IX>(MockBehavior.Strict);
				mock.Verify(m => m.X.X, Times.Never);
				Assert.Throws<MockException>(() => mock.Object.X);
			}

			[Fact]
			public void Does_not_create_inner_mocks()
			{
				var mock = new Mock<IX>();
				mock.Verify(m => m.X.X, Times.Never);
				Assert.Throws<NullReferenceException>(() => _ = mock.Object.X.X);
			}

			[Fact]
			public void Does_not_override_existing_setups()
			{
				var mock = new Mock<IX>();
				var nested = new Mock<IX>();
				nested.Setup(m => m.Count).Returns(5);
				mock.Setup(m => m.X).Returns(nested.Object);

				mock.Verify(m => m.X.X, Times.Never);
				int c = mock.Object.X.Count;
				Assert.Equal(5, c);
			}

			public interface IX
			{
				IX X { get; }
				int Count { get; }
			}
		}

		public class Object_graph_loops
		{
			public interface IX
			{
				IX Self { get; }
			}

			[Fact]
			public void When_mock_returns_itself_via_setup_VerifyNoOtherCalls_wont_go_into_infinite_loop()
			{
				var mock = new Mock<IX>();
				mock.Setup(m => m.Self).Returns(mock.Object);
				mock.VerifyNoOtherCalls();
			}

			[Fact]
			public void When_mock_returns_itself_lazily_via_setup_VerifyNoOtherCalls_wont_go_into_infinite_loop()
			{
				var mock = new Mock<IX>();
				mock.Setup(m => m.Self).Returns(() => mock.Object);
				mock.VerifyNoOtherCalls();
			}

			[Fact]
			public void When_mock_returns_itself_via_setup_Verify_exception_message_wont_go_into_infinite_loop()
			{
				var mock = new Mock<IX>();
				mock.Setup(m => m.Self).Returns(mock.Object);
				_ = mock.Object.Self;

				var ex = Assert.Throws<MockException>(() => mock.Verify(m => m.Self, Times.Never));
				//                                                                   ^^^^^^^^^^^
				// We are intentionally provoking a verification exception so that Moq will have to
				// build an error message showing all invocations grouped by mock.

				Assert.Equal(2, SubstringCount(ex.Message, substring: mock.Name));
				// That message should mention our mock only twice: once in the heading above
				// the mock's invocations; and once for the invocation that returned it.

				int SubstringCount(string str, string substring)
				{
					int count = 0;
					int index = -1;
					while ((index = str.IndexOf(substring, index + 1)) >= 0) ++count;
					return count;
				}
			}
		}

		[Fact]
		public void Property_getter_setup_created_by_SetupAllProperties_should_not_fail_verification_even_when_not_matched()
		{
			var mock = new Mock<IFoo>();
			mock.SetupAllProperties();

			// Due to `SetupAllProperties` working in a lazy fashion,
			// this should create two setups (one for the getter, one for the setter).
			// Only invoke the setter:
			mock.Object.Value = default;

			// The getter hasn't been matched, but verification should still pass:
			mock.VerifyAll();
		}

		[Fact]
		public void Property_setter_setup_created_by_SetupAllProperties_should_not_fail_verification_even_when_not_matched()
		{
			var mock = new Mock<IFoo>();
			mock.SetupAllProperties();

			// Due to `SetupAllProperties` working in a lazy fashion,
			// this should create two setups (one for the getter, one for the setter).
			// Only invoke the getter:
			_ = mock.Object.Value;

			// The setter hasn't been matched, but verification should still pass:
			mock.VerifyAll();
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
			void Subscribe<T>() where T : IBazParam;
			void Subscribe();
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

		public interface ICat
		{
			string Purr(int amount);
			void Hiss();
		}

		public class ParentDto { }

		public class ChildDto : ParentDto { }

		public class Parent
		{
			protected virtual bool Populate(ref ParentDto dto)
			{
				return true;
			}
		}

		public class Child : Parent
		{
			protected virtual bool Populate(ref ChildDto dto)
			{
				return true;
			}

			public bool InvokePopulate(ref ChildDto dto)
			{
				return Populate(ref dto);
			}
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
