// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
			mock.Invocations.Clear();
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

		[Fact]
		public void IsExtensionMethod_recognizes_extension_method_as_such()
		{
			var isExtensionMethodMethod = typeof(Moq.Extensions).GetMethod(nameof(Moq.Extensions.IsExtensionMethod));

			Assert.True(isExtensionMethodMethod.IsExtensionMethod());
		}

		[Fact]
		public void IsExtensionMethod_does_not_recognize_method_as_extension_method()
		{
			var thisMethod = (MethodInfo)MethodBase.GetCurrentMethod();

			Assert.False(thisMethod.IsExtensionMethod());
		}
	}

	public interface IFooReset
	{
		#region Public Methods

		object Execute(string ping);

		#endregion
	}
}
