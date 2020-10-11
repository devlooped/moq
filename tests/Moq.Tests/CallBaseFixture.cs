// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using Xunit;

namespace Moq.Tests
{
	public class CallBaseFixture
	{
		[Fact]
		public void Void_method_in_base_class_not_called_when_CallBase_not_specified()
		{
			var mock = new Mock<Base>();

			mock.Object.Method();

			Assert.False(mock.Object.MethodInvoked);
		}

		[Fact]
		public void Void_method_in_base_class_called_when_CallBase_specified_on_mock()
		{
			var mock = new Mock<Base>() { CallBase = true };

			mock.Object.Method();

			Assert.True(mock.Object.MethodInvoked);
		}

		[Fact]
		public void Void_method_in_base_class_called_when_CallBase_specified_on_setup()
		{
			var mock = new Mock<Base>();
			mock.Setup(m => m.Method()).CallBase();

			mock.Object.Method();

			Assert.True(mock.Object.MethodInvoked);
		}

		public class Base
		{
			public bool MethodInvoked { get; private set; }

			public virtual void Method()
			{
				this.MethodInvoked = true;
			}
		}
	}
}
