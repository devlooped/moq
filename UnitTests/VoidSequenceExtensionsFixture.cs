using System;
using Xunit;

namespace Moq.Tests
{
	public class VoidSequenceExtensionsFixture
	{
		[Fact]
		public void PerformSequence()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(m => m.Do())
				.Pass()
				.Throws<InvalidOperationException>()
				.Throws(new ArgumentException());

			mock.Object.Do();
			Assert.Throws<InvalidOperationException>(() => mock.Object.Do());
			Assert.Throws<ArgumentException>(() => mock.Object.Do());
		}

		[Fact]
		public void PerformSequenceWithThrowFirst()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(m => m.Do())
				.Throws<InvalidOperationException>()
				.Pass()
				.Throws(new ArgumentException());

			Assert.Throws<InvalidOperationException>(() => mock.Object.Do());
			mock.Object.Do();
			Assert.Throws<ArgumentException>(() => mock.Object.Do());
		}

		public interface IFoo
		{
			void Do();
		}
	}
}
