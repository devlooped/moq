using System;
using Xunit;

namespace Moq.Tests
{
	public class MockedFixture
	{
		[Fact]
		public void InterfaceMockedShouldImplementMocked()
		{
			Mock<IFoo> mock = new Mock<IFoo>();
			IFoo mocked = mock.Object;
			Assert.True(mocked is IMocked<IFoo>);
		}

		[Fact]
		public void MockOfMockedInterfaceShouldReturnSame()
		{
			Mock<IFoo> mock = new Mock<IFoo>();
			IMocked<IFoo> mocked = mock.Object as IMocked<IFoo>;
			Assert.Same(mock, mocked.Mock);
		}

		[Fact]
		public void ClassMockedShouldImplementMocked()
		{
			Mock<Foo> mock = new Mock<Foo>();
			Foo mocked = mock.Object;
			Assert.True(mocked is IMocked<Foo>);
		}

		[Fact]
		public void MockOfMockedClassShouldReturnSame()
		{
			Mock<Foo> mock = new Mock<Foo>();
			IMocked<Foo> mocked = mock.Object as IMocked<Foo>;
			Assert.Same(mock, mocked.Mock);
		}

		public class FooWithCtor
		{
			public FooWithCtor(int a) { }
		}

		[Fact]
		public void ClassWithCtorMockedShouldImplementMocked()
		{
			Mock<FooWithCtor> mock = new Mock<FooWithCtor>(5);
			FooWithCtor mocked = mock.Object;
			Assert.True(mocked is IMocked<FooWithCtor>);
		}

		[Fact]
		public void MockOfMockedClassWithCtorShouldReturnSame()
		{
			Mock<FooWithCtor> mock = new Mock<FooWithCtor>(5);
			IMocked<FooWithCtor> mocked = mock.Object as IMocked<FooWithCtor>;
			Assert.Same(mock, mocked.Mock);
		}

		[Fact]
		public void GetReturnsMockForAMocked()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;
			Assert.Same(mock, Mock.Get(mocked));
		}

		[Fact]
		public void GetReturnsMockForAMockedAbstract()
		{
			var mock = new Mock<FooBase>();
			var mocked = mock.Object;
			Assert.Same(mock, Mock.Get(mocked));
		}

		[Fact]
		public void GetThrowsIfObjectIsNotMocked()
		{
			Assert.Throws<ArgumentException>(
				"Object instance was not created by Moq.\r\nParameter name: mocked",
				() => Mock.Get("foo"));
		}

		public class FooBase
		{
		}
		
		public class Foo : FooBase
		{
		}
		
		public interface IFoo
		{
		}
	}
}