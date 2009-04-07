using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class PropertiesFixture
	{
		public interface IIndexedFoo
		{
			string this[int key] { get; set; }
		}

		[Fact]
		public void ShouldThrowIfSetupSetIndexer()
		{
			var foo = new Mock<IIndexedFoo>();

			Assert.Throws<ArgumentException>(() => foo.SetupSet(f => f[0]));
		}

		[Fact]
		public void ShouldSetIndexer()
		{
			var foo = new Mock<IIndexedFoo>(MockBehavior.Strict);

			foo.SetupSet(f => f[0] = "foo");

			foo.Object[0] = "foo";
		}

		[Fact]
		public void ShouldSetIndexerWithValueMatcher()
		{
			var foo = new Mock<IIndexedFoo>(MockBehavior.Strict);

			foo.SetupSet(f => f[0] = It.IsAny<string>());

			foo.Object[0] = "foo";
		}

		[Fact(Skip = "Not supported for now")]
		public void ShouldSetIndexerWithIndexMatcher()
		{
			var foo = new Mock<IIndexedFoo>(MockBehavior.Strict);

			foo.SetupSet(f => f[It.IsAny<int>()] = "foo");

			foo.Object[18] = "foo";
		}

		[Fact(Skip = "Not supported for now")]
		public void ShouldSetIndexerWithBothMatcher()
		{
			var foo = new Mock<IIndexedFoo>(MockBehavior.Strict);

			foo.SetupSet(f => f[It.IsAny<int>()] = It.IsAny<string>());

			foo.Object[18] = "foo";
		}
	}
}
