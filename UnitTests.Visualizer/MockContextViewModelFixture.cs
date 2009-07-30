using System.ComponentModel;
using Xunit;
using System.Linq;
using System;

namespace Moq.Visualizer.Tests
{
	public class MockContextViewModelFixture
	{
		[Fact]
		public void CtorSetsBehavior()
		{
			var target = new MockContextViewModel(new Mock<IComponent>(MockBehavior.Strict));

			Assert.Equal(MockBehavior.Strict, target.Behavior);
		}

		[Fact]
		public void CtorSetsCallBase()
		{
			var target = new MockContextViewModel(new Mock<IComponent> { CallBase = false });

			Assert.Equal(false, target.CallBase);
		}

		[Fact]
		public void CtorSetsDefaultValue()
		{
			var target = new MockContextViewModel(new Mock<IComponent> { DefaultValue = DefaultValue.Empty });

			Assert.Equal(DefaultValue.Empty, target.DefaultValue);
		}

		[Fact]
		public void AddMockViewModelToMocks()
		{
			var target = new MockContextViewModel(new Mock<IComponent>());

			Assert.Equal(1, target.Mocks.Count());
			Assert.NotNull(target.Mocks.ElementAt(0));
		}
	}
}