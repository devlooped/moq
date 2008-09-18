using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class AutoMockHierarchiesFixture
	{
		[Fact(Skip = "Feature pending")]
		public void CreatesMockForAccessedProperty()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(m => m.Bar).Returns(new Mock<IBar>().Object);

			Assert.Equal(5, mock.Object.Bar.Value);
		}

		public interface IFoo
		{
			IBar Bar { get; set; }
		}

		public interface IBar 
		{
			int Value { get; set; }
		}
	}
}
