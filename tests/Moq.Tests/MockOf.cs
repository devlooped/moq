using Xunit;

namespace Moq.Tests
{
	public class MockOf
	{
		[Fact]
		public void NonGenericConstructorCreatesMock()
		{
			var mock = Mock.Of<FooNonGenericConstructor>(1);

			Mock.Get(mock).Setup(foo => foo.GetValue()).Returns(2);

			Assert.Equal(2, mock.GetValue());
		}

		[Fact]
		public void NonGenericConstructorWithMockBehaviourCreatesMock()
		{
			var mock = Mock.Of<FooNonGenericConstructor>(MockBehavior.Strict,1);

			Mock.Get(mock).Setup(foo => foo.GetValue()).Returns(2);

			Assert.Equal(2, mock.GetValue());
		}

		public class FooNonGenericConstructor
		{
			private readonly int value;

			public FooNonGenericConstructor(int value)
			{
				this.value = value;
			}

			public virtual int GetValue()
			{
				return value;
			}
		}
	}	
}