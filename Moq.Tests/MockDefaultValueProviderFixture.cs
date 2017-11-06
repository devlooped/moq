using System.Collections.Generic;
using System.Linq;
#if NETCORE
using System.Reflection;
#endif
using Xunit;

namespace Moq.Tests
{
	public class MockDefaultValueProviderFixture
	{
		[Fact]
		public void ProvidesMockValue()
		{
			var mock = new Mock<IFoo>();
			var provider = new MockDefaultValueProvider(mock);

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod());

			Assert.NotNull(value);
			Assert.True(value is IMocked);
		}

		[Fact]
		public void CachesProvidedValue()
		{
			var mock = new Mock<IFoo>();
			var provider = new MockDefaultValueProvider(mock);

			var value1 = provider.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod());
			var value2 = provider.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod());

			Assert.Same(value1, value2);
		}

		[Fact]
		public void ProvidesEmptyValueIfNotMockeable()
		{
			var mock = new Mock<IFoo>();
			var provider = new MockDefaultValueProvider(mock);

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Value").GetGetMethod());
			Assert.Equal(default(string), value);

			value = provider.ProvideDefault(typeof(IFoo).GetProperty("Value").GetGetMethod());
			Assert.Equal(default(string), value);

			value = provider.ProvideDefault(typeof(IFoo).GetProperty("Indexes").GetGetMethod());
			Assert.True(value is IEnumerable<int> && ((IEnumerable<int>)value).Count() == 0);

			value = provider.ProvideDefault(typeof(IFoo).GetProperty("Bars").GetGetMethod());
			Assert.True(value is IBar[] && ((IBar[])value).Length == 0);
		}

		[Fact]
		public void NewMocksHaveSameBehaviorAndDefaultValueAsOwner()
		{
			var mock = new Mock<IFoo>();
			var provider = new MockDefaultValueProvider(mock);

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod());

			var barMock = Mock.Get((IBar)value);

			Assert.Equal(mock.Behavior, barMock.Behavior);
			Assert.Equal(mock.DefaultValue, barMock.DefaultValue);
		}

		[Fact]
		public void NewMocksHaveSameCallBaseAsOwner()
		{
			var mock = new Mock<IFoo> { CallBase = true };
			var provider = new MockDefaultValueProvider(mock);

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod());

			var barMock = Mock.Get((IBar)value);

			Assert.Equal(mock.CallBase, barMock.CallBase);
		}

		[Fact]
		public void CreatedMockIsVerifiedWithOwner()
		{
			var mock = new Mock<IFoo>();
			var provider = new MockDefaultValueProvider(mock);

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod());

			var barMock = Mock.Get((IBar)value);
			barMock.Setup(b => b.Do()).Verifiable();

			var ex = Assert.Throws<MockException>(() => mock.Verify());
			Assert.True(ex.IsVerificationError);
		}

		[Fact]
		public void DefaultValueIsNotChangedWhenPerformingInternalInvocation()
		{
			var mockBar = new Mock<IBar> { DefaultValue = DefaultValue.Empty };
			var mockFoo = new Mock<IFoo>();
			mockFoo.SetupSet(m => m.Bar = mockBar.Object);
			Assert.Equal(DefaultValue.Empty, mockBar.DefaultValue);
		}

		[Fact]
		public void Inner_mocks_inherit_switches_of_parent_mock()
		{
			const Switches expectedSwitches = Switches.CollectDiagnosticFileInfoForSetups;

			var parentMock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock, Switches = expectedSwitches };
			var innerMock = Mock.Get(parentMock.Object.Bar);

			Assert.Equal(expectedSwitches, actual: innerMock.Switches);
		}

		public interface IFoo
		{
			IBar Bar { get; set; }
			string Value { get; set; }
			IEnumerable<int> Indexes { get; set; }
			IBar[] Bars { get; set; }
		}

		public interface IBar { void Do(); }
	}
}
