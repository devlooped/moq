using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class ExtensionsFixture
	{
		[Fact]
		public void IsMockeableReturnsFalseForValueType()
		{
			Assert.False(typeof(int).IsMockeable());
		}
	}
}
