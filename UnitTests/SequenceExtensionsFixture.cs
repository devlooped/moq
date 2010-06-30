using System;
using Xunit;

namespace Moq.Tests
{
	public class SequenceExtensionsFixture
	{
		[Fact]
		public void PerformSequence()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.Do())
				.Returns(2)
				.Returns(3)
				.Throws<InvalidOperationException>();

			Assert.Equal(2, mock.Object.Do());
			Assert.Equal(3, mock.Object.Do());
			Assert.Throws<InvalidOperationException>(() => mock.Object.Do());
		}

		[Fact]
		public void PerformSequenceOnProperty()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.Value)
				.Returns("foo")
				.Returns("bar")
				.Throws<SystemException>();

			string temp;
			Assert.Equal("foo", mock.Object.Value);
			Assert.Equal("bar", mock.Object.Value);
			Assert.Throws<SystemException>(() => temp = mock.Object.Value);
		}

		public interface IFoo
		{
			string Value { get; set; }
			int Do();
		}
	}
}