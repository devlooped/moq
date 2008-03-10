using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Moq.Tests
{
    [TestFixture]
    public class MockedFixture
    {
		public class FooBase { }
		public class Foo : FooBase { }
		public interface IFoo { }

        [Test]
        public void InterfaceMockedShouldImplementMocked()
        {
            Mock<IFoo> mock = new Mock<IFoo>();
            IFoo mocked = mock.Object;
            Assert.IsTrue(mocked is IMocked<IFoo>);
        }

        [Test]
        public void MockOfMockedInterfaceShouldReturnSame()
        {
            Mock<IFoo> mock = new Mock<IFoo>();
            IMocked<IFoo> mocked = mock.Object as IMocked<IFoo>;
            Assert.AreSame(mock, mocked.Mock);
        }

        [Test]
        public void ClassMockedShouldImplementMocked()
        {
            Mock<Foo> mock = new Mock<Foo>();
            Foo mocked = mock.Object;
            Assert.IsTrue(mocked is IMocked<Foo>);
        }

        [Test]
        public void MockOfMockedClassShouldReturnSame()
        {
            Mock<Foo> mock = new Mock<Foo>();
            IMocked<Foo> mocked = mock.Object as IMocked<Foo>;
            Assert.AreSame(mock, mocked.Mock);
        }

		public class FooWithCtor 
		{
			public FooWithCtor(int a) { }
		}

		[Test]
		public void ClassWithCtorMockedShouldImplementMocked()
		{
			Mock<FooWithCtor> mock = new Mock<FooWithCtor>(5);
			FooWithCtor mocked = mock.Object;
			Assert.IsTrue(mocked is IMocked<FooWithCtor>);
		}

		[Test]
		public void MockOfMockedClassWithCtorShouldReturnSame()
		{
			Mock<FooWithCtor> mock = new Mock<FooWithCtor>(5);
			IMocked<FooWithCtor> mocked = mock.Object as IMocked<FooWithCtor>;
			Assert.AreSame(mock, mocked.Mock);
		}

		public class FooMBR : MarshalByRefObject { }

		[Test]
		public void ClassMBRMockedShouldImplementMocked()
		{
			Mock<FooMBR> mock = new Mock<FooMBR>();
			FooMBR mocked = mock.Object;
			Assert.IsTrue(mocked is IMocked<FooMBR>);
		}

		[Test]
		public void MockOfMockedClassMBRShouldReturnSame()
		{
			Mock<FooMBR> mock = new Mock<FooMBR>();
			IMocked<FooMBR> mocked = mock.Object as IMocked<FooMBR>;
			Assert.AreSame(mock, mocked.Mock);
		}

		[Test]
		public void GetReturnsMockForAMocked()
		{
			var mock = new Mock<IFoo>();
			var mocked = mock.Object;
			Assert.AreSame(mock, Mock.Get(mocked));
		}

		[Test]
		public void GetReturnsMockForAMockedAbstract()
		{
			var mock = new Mock<FooBase>();
			var mocked = mock.Object;
			Assert.AreSame(mock, Mock.Get(mocked));
		}

		[Test]
		[ExpectedException(ExceptionType = typeof(ArgumentException), ExpectedMessage = "Object instance was not created by Moq.\r\nParameter name: mocked")]
		public void GetThrowsIfObjectIsNotMocked()
		{
			Mock.Get("foo");
		}
    }
}
