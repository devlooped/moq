using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Visualizer.Tests
{
	public class SetupViewModelFixture
	{
		[Fact]
		public void CtorSetsSetupExpression()
		{
			var target = new SetupViewModel("foo", false, false);

			Assert.Equal("foo", target.SetupExpression);
		}

		[Fact]
		public void CtorSetsIsVerifiable()
		{
			var target = new SetupViewModel("foo", true, false);

			Assert.Equal(true, target.IsVerifiable);
		}

		[Fact]
		public void CtorSetsIsNever()
		{
			var target = new SetupViewModel("foo", false, true);

			Assert.Equal(true, target.IsNever);
		}

		[Fact]
		public void CtorSetContainers()
		{
			var expectedContainer1 = new ContainerViewModel<ICloneable>("bar", new ICloneable[0]);
			var expectedContainer2 = new ContainerViewModel<IFormattable>("baz", new IFormattable[0]);

			var target = new SetupViewModel("foo", false, false, expectedContainer1, expectedContainer2);

			Assert.Equal(2, target.Containers.Count());
			Assert.Same(expectedContainer1, target.Containers.ElementAt(0));
			Assert.Same(expectedContainer2, target.Containers.ElementAt(1));
		}

		[Fact]
		public void CtorSetsIsExpandedAsFalse()
		{
			var target = new SetupViewModel("foo", false, false);

			Assert.False(target.IsExpanded);
		}
	}
}