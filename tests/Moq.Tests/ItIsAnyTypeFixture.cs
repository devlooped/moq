// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using Xunit;

namespace Moq.Tests
{
	public class ItIsAnyTypeFixture
	{
		[Fact]
		public void Setup_without_It_IsAnyType()
		{
			var mock = new Mock<IX>();
			mock.Setup(x => x.Method<bool>());
			mock.Setup(x => x.Method<int>());
			mock.Setup(x => x.Method<object>());
			mock.Setup(x => x.Method<string>());

			mock.Object.Method<bool>();
			mock.Object.Method<int>();
			mock.Object.Method<object>();
			mock.Object.Method<string>();

			mock.VerifyAll();
		}

		[Fact]
		public void Setup_with_It_IsAnyType()
		{
			var invocationCount = 0;
			var mock = new Mock<IX>();
			mock.Setup(x => x.Method<It.IsAnyType>()).Callback(() => invocationCount++);

			mock.Object.Method<bool>();
			mock.Object.Method<int>();
			mock.Object.Method<object>();
			mock.Object.Method<string>();

			Assert.Equal(4, invocationCount);
		}

		[Fact]
		public void Verify_with_It_IsAnyType()
		{
			var mock = new Mock<IX>();

			mock.Object.Method<bool>();
			mock.Object.Method<int>();
			mock.Object.Method<object>();
			mock.Object.Method<string>();

			mock.Verify(x => x.Method<It.IsAnyType>(), Times.Exactly(4));
		}

		public interface IX
		{
			void Method<T>();
		}
	}
}
