using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class AsMockExtensionsFixture
	{
		[Fact]
		public void ShouldGetMockInstanceFromPropertyReturns()
		{
			var mock = new Mock<IFoo>();
			var foo = mock.Setup(f => f.Value).Returns(5).AsMock();

			Assert.Same(mock, foo);
		}

		[Fact]
		public void ShouldGetMockInstanceFromMethodReturns()
		{
			var mock = new Mock<IFoo>();
			var foo = mock.Setup(f => f.Do()).Returns(true).AsMock();

			Assert.Same(mock, foo);
		}


		[Fact]
		public void ShouldGetMockedInstanceFromPropertyReturns()
		{
			var mock = new Mock<IFoo>();
			var foo = mock.Setup(f => f.Value).Returns(5).AsMocked();

			Assert.Same(mock.Object, foo);
		}

		[Fact]
		public void ShouldGetMockedInstanceFromMethodReturns()
		{
			var mock = new Mock<IFoo>();
			var foo = mock.Setup(f => f.Do()).Returns(true).AsMocked();

			Assert.Same(mock.Object, foo);
		}

		[Fact]
		public void ShouldGetMockedInstanceFromVoidMethodSetup()
		{
			var mock = new Mock<IFoo>();
			var foo = mock.Setup(f => f.Submit()).AsMocked();

			Assert.Same(mock.Object, foo);
		}

		[Fact]
		public void ShouldGetMockInstanceFromVoidMethodSetup()
		{
			var mock = new Mock<IFoo>();
			var foo = mock.Setup(f => f.Submit()).AsMock();

			Assert.Same(mock, foo);
		}

		[Fact]
		public void ShouldGetMockedInstanceFromVoidMethodCallback()
		{
			var mock = new Mock<IFoo>();
			var foo = mock.Setup(f => f.Submit()).Callback(() => {}).AsMocked<IFoo>();

			Assert.Same(mock.Object, foo);
		}

		[Fact]
		public void ShouldGetMockInstanceFromVoidMethodCallback()
		{
			var mock = new Mock<IFoo>();
			var foo = mock.Setup(f => f.Submit()).Callback(() => { }).AsMock<IFoo>();

			Assert.Same(mock, foo);
		}

		[Fact]
		public void ShouldGetMockedInstanceFromMethodReturnsCallback()
		{
			var mock = new Mock<IFoo>();
			var foo = mock.Setup(f => f.Do()).Returns(true).Callback(() => { }).AsMocked<IFoo>();

			Assert.Same(mock.Object, foo);
		}

		[Fact]
		public void ShouldGetMockInstanceFromMethodReturnsCallback()
		{
			var mock = new Mock<IFoo>();
			var foo = mock.Setup(f => f.Do()).Returns(true).Callback(() => { }).AsMock<IFoo>();

			Assert.Same(mock, foo);
		}

		public interface IFoo
		{
			int Value { get; set; }
			bool Do();
			void Submit();
		}
	}
}
