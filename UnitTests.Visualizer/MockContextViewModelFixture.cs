using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Xunit;

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
		public void AddExpandedMockViewModelToMocks()
		{
			var target = new MockContextViewModel(new Mock<IComponent>());

			Assert.Equal(1, target.Mocks.Count());
			Assert.NotNull(target.Mocks.ElementAt(0));
			Assert.True(target.Mocks.ElementAt(0).IsExpanded);
		}

		[Fact]
		public void AddExpandedSetupViewModelToContainers()
		{
			var mock = new Mock<IConvertible>();
			//mock.Setup(c => c.ToString(It.IsAny<IFormatProvider>()));
			//mock.Setup(c => c.ToSingle(It.IsAny<IFormatProvider>()));

			var target = new MockContextViewModel(mock);

			var setup = (ContainerViewModel<SetupViewModel>)target.Mocks.ElementAt(0)
				.Containers.Single(c => c.Name == "Setups");
			Assert.NotNull(setup);
			//Assert.Equal(2, setup.Children.Count());
			Assert.True(setup.IsExpanded);
		}

		[Fact]
		public void AddExpandedOtherCallsViewModelToMockContainers()
		{
			var mock = new Mock<IConvertible>();
			mock.Setup(c => c.ToString(It.IsAny<IFormatProvider>()));
			//mock.Object.ToString(CultureInfo.CurrentCulture);
			//mock.Object.ToChar(CultureInfo.CurrentCulture);
			//mock.Object.ToDateTime(CultureInfo.CurrentCulture);

			var target = new MockContextViewModel(mock);

			var call = (ContainerViewModel<CallViewModel>)target.Mocks.ElementAt(0)
				.Containers.Single(c => c.Name == "Invocations without setup");
			Assert.NotNull(call);
			//Assert.Equal(2, call.Children.Count());
			Assert.True(call.IsExpanded);
		}

		[Fact]
		public void AddExpandedMockViewModelToInnerMockContainers()
		{
			var mock = new Mock<IList<IFormattable>>();
			//mock.Setup(c => c[0].ToString(It.IsAny<string>(), It.IsAny<IFormatProvider>()));

			var target = new MockContextViewModel(mock);

			var call = (ContainerViewModel<MockViewModel>)target.Mocks.ElementAt(0)
				.Containers.Single(c => c.Name == "Inner Mocks");
			Assert.NotNull(call);
			//Assert.Equal(1, call.Children.Count());
			Assert.True(call.IsExpanded);
		}
	}
}