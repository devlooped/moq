using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class InstanceMockFixture
	{
		[Fact]
		public void ShouldCallBase()
		{
			TestClass instance = new TestClass() { Id = 1 };
			Mock<TestClass> mock = Mock.Instance(instance);

			Assert.Equal(instance.Id, mock.Object.Id);
		}

		[Fact]
		public void ShouldUseSetupOverBase()
		{
			int expectedId = 2;
			TestClass instance = new TestClass() { Id = 1 };

			Mock<TestClass> mock = Mock.Instance(instance);
			mock.Setup(i => i.Id).Returns(expectedId);

			Assert.Equal(expectedId, mock.Object.Id);
		}

		[Fact]
		public void ShouldRecognizeVerify()
		{
			TestClass instance = new TestClass();

			Mock<TestClass> mock = Mock.Instance(instance);
			int id = mock.Object.Id;

			mock.Verify(i => i.Id, Times.Once);
		}

		[Fact]
		public void ShouldSuppportAs()
		{
			TestClass instance = new TestClass();

			Assert.DoesNotThrow(() =>
				{
					Mock<ITestClass> mock = Mock.Instance(instance).As<ITestClass>();
				});
		}

		public class TestClass : ITestClass
		{
			public virtual int Id { get; set; }
		}

		public interface ITestClass
		{
			int Id { get; set; }
		}
	}
}
