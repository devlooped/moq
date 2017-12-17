using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class ExtensionsFixture
	{
		#region Public Methods

		[Fact]
		public void IsMockeableReturnsFalseForValueType()
		{
			Assert.False(typeof(int).IsMockeable());
		}

		[Fact]
		public void OnceDoesNotThrowOnSecondCallIfCountWasResetBefore()
		{
			var mock = new Mock<IFooReset>();
			mock.Setup(foo => foo.Execute("ping")).Returns("ack");

			mock.Object.Execute("ping");
			mock.ResetCalls();
			mock.Object.Execute("ping");
			mock.Verify(o => o.Execute("ping"), Times.Once());
		}

		[Fact]
		public void SetupDoesNotApplyAfterMockWasReset()
		{
			var mock = new Mock<IFooReset>();
			mock.Setup(foo => foo.Execute("ping")).Returns("ack");
			mock.Reset();

			var result = mock.Object.Execute("ping");
			Assert.Null(result);
		}

		[Fact]
		public void Loose()
		{
			var myMock = new Mock<IEnumerable<int>>(MockBehavior.Loose);
			myMock
				.Setup(a => a.ToString())
				.Returns("Hello");
			myMock.Reset();
			Assert.NotEqual("Hello", myMock.Object.ToString());
			myMock.VerifyAll();
		}

		[Fact]
		public void Strict()
		{
			var myMock = new Mock<IEnumerable<int>>(MockBehavior.Strict);
			myMock
				.Setup(a => a.ToString())
				.Returns("Hello");
			myMock.Reset();
			Assert.NotEqual("Hello", myMock.Object.ToString());
			myMock.VerifyAll();
		}

		[Fact]
		public void LooseNoCall()
		{
			var myMock = new Mock<IEnumerable<int>>(MockBehavior.Loose);
			myMock
				.Setup(a => a.ToString())
				.Returns("Hello");
			myMock.Reset();
			myMock.VerifyAll();
		}

		[Fact]
		public void StrictNoCall()
		{
			var myMock = new Mock<IEnumerable<int>>(MockBehavior.Strict);
			myMock
				.Setup(a => a.ToString())
				.Returns("Hello");
			myMock.Reset();
			myMock.VerifyAll();
		}
		#endregion
	}

	public interface IFooReset
	{
		#region Public Methods

		object Execute(string ping);

		#endregion
	}
}
