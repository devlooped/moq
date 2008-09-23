using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class VerifyFixture
	{
		[Fact]
		public void ThrowsIfVerifiableExpectationNotCalled()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Submit()).Verifiable();

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

			mock.Expect(x => x.Execute(expectedArg.Substring(0, 5)))
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

			mock.Expect(x => x.Execute(It.Is<string>(s => string.IsNullOrEmpty(s))))
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

			mock.Expect(x => x.Submit());

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

			mock.Expect(x => x.Submit());
			mock.Expect(x => x.Echo(1));
			mock.Expect(x => x.Execute("ping"));

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
		public void ThrowsIfVerifyPropertyGetWithExpressionFails()
		{
			var mock = new Mock<IFoo>();

			try
			{
				mock.VerifyGet(f => f.Value);
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
		public void VerifiesSetter()
		{
			var mock = new Mock<IFoo>();
			mock.ExpectSet(m => m.Value, 5);

			mock.Object.Value = 1;
			mock.Object.Value = 5;

			mock.VerifySet(m => m.Value);
			Assert.Throws<MockException>(() => mock.VerifySet(m => m.Value, 2));
			mock.VerifySet(m => m.Value, 5);
		}

		public interface IFoo
		{
			int? Value { get; set; }
			int Echo(int value);
			void Submit();
			string Execute(string command);
		}
	}
}
