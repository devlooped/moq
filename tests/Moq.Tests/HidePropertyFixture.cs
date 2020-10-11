// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using Xunit;

namespace Moq.Tests
{
	public class HidePropertyFixture
	{
		public class A
		{
			public string Prop { get; }
		}

		public class B : A
		{
			public new virtual int Prop { get; }
		}

		public class C : B
		{
		}

		[Fact]
		public void SetupsDerivedProperty()
		{
			var mock = new Mock<C>();
			var value = 5;

			mock.Setup(m => m.Prop).Returns(value);

			Assert.Equal(value, mock.Object.Prop);
		}
	}
}
