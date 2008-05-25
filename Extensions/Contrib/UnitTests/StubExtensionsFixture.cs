using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Linq.Expressions;

namespace Moq.Tests
{
	public class StubExtensionsFixture
	{
		[Fact]
		public void ShouldExpectPropertyGetterAndSetterWithSameExpression()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectGet(x => x.ValueProperty).Returns(25);
			mock.ExpectSet(x => x.ValueProperty);

			mock.Object.ValueProperty = 7;
			Assert.Equal(25, mock.Object.ValueProperty);
		}

		[Fact]
		public void ShouldExpectPropertyGetterAndSetterWithoutSameExpression()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectGet(x => x.ValueProperty).Returns(25);
			mock.ExpectSet(y => y.ValueProperty);

			mock.Object.ValueProperty = 7;
			Assert.Equal(25, mock.Object.ValueProperty);
		}
	}

	public interface IFoo
	{
		int ValueProperty { get; set; }
	}
}
