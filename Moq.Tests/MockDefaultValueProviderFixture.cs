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

			var value = GetDefaultValueForProperty(nameof(IFoo.Bar), mock);

			Assert.NotNull(value);
			Assert.True(value is IMocked);
		}

		[Fact]
		public void CachesProvidedValue()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };

			var value1 = mock.Object.Bar;
			var value2 = mock.Object.Bar;

			Assert.Same(value1, value2);
		}

		[Fact]
		public void ProvidesEmptyValueIfNotMockeable()
		{
			var mock = new Mock<IFoo>();

			var value = GetDefaultValueForProperty(nameof(IFoo.Value), mock);
			Assert.Equal(default(string), value);

			value = GetDefaultValueForProperty(nameof(IFoo.Indexes), mock);
			Assert.True(value is IEnumerable<int> && ((IEnumerable<int>)value).Count() == 0);

			value = GetDefaultValueForProperty(nameof(IFoo.Bars), mock);
			Assert.True(value is IBar[] && ((IBar[])value).Length == 0);
		}

		[Fact]
		public void NewMocksHaveSameBehaviorAndDefaultValueAsOwner()
		{
			var mock = new Mock<IFoo>();

			var value = GetDefaultValueForProperty(nameof(IFoo.Bar), mock);

			var barMock = Mock.Get((IBar)value);

			Assert.Equal(mock.Behavior, barMock.Behavior);
			Assert.Equal(mock.DefaultValue, barMock.DefaultValue);
		}

		[Fact]
		public void NewMocksHaveSameCallBaseAsOwner()
		{
			var mock = new Mock<IFoo> { CallBase = true };

			var value = GetDefaultValueForProperty(nameof(IFoo.Bar), mock);

			var barMock = Mock.Get((IBar)value);

			Assert.Equal(mock.CallBase, barMock.CallBase);
		}

		[Fact]
		public void CreatedMockIsVerifiedWithOwner()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };

			var bar = mock.Object.Bar;
			var barMock = Mock.Get(bar);
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

		private static object GetDefaultValueForProperty(string propertyName, Mock<IFoo> mock)
		{
			var propertyGetter = typeof(IFoo).GetProperty(propertyName).GetGetMethod();
			return MockDefaultValueProvider.Instance.GetDefaultReturnValue(propertyGetter, mock);
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
