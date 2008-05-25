using Xunit;

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

		[Fact]
		public void ShouldStubPropertyWithoutInitialValue()
		{
			var mock = new Mock<IFoo>();

			mock.Stub(f => f.ValueProperty);

			Assert.Equal(0, mock.Object.ValueProperty);

			mock.Object.ValueProperty = 5;

			Assert.Equal(5, mock.Object.ValueProperty);
		}

		[Fact]
		public void ShouldStubPropertyWithInitialValue()
		{
			var mock = new Mock<IFoo>();

			mock.Stub(f => f.ValueProperty, 5);

			Assert.Equal(5, mock.Object.ValueProperty);

			mock.Object.ValueProperty = 15;

			Assert.Equal(15, mock.Object.ValueProperty);
		}
	}

	public interface IFoo
	{
		int ValueProperty { get; set; }
	}
}
