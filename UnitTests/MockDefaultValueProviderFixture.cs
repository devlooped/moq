using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod(), new object[0]);

			Assert.NotNull(value);
			Assert.True(value is IMocked);
		}

		[Fact]
		public void CachesProvidedValue()
		{
			var mock = new Mock<IFoo>();
			var provider = new MockDefaultValueProvider(mock);

			var value1 = provider.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod(), new object[0]);
			var value2 = provider.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod(), new object[0]);

			Assert.Same(value1, value2);
		}

		[Fact]
		public void ProvidesEmptyValueIfNotMockeable()
		{
			var mock = new Mock<IFoo>();
			var provider = new MockDefaultValueProvider(mock);

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Value").GetGetMethod(), new object[0]);
			Assert.Equal(default(string), value);

			value = provider.ProvideDefault(typeof(IFoo).GetProperty("Value").GetGetMethod(), new object[0]);
			Assert.Equal(default(string), value);

			value = provider.ProvideDefault(typeof(IFoo).GetProperty("Indexes").GetGetMethod(), new object[0]);
			Assert.True(value is IEnumerable<int> && ((IEnumerable<int>)value).Count() == 0);

			value = provider.ProvideDefault(typeof(IFoo).GetProperty("Bars").GetGetMethod(), new object[0]);
			Assert.True(value is IBar[] && ((IBar[])value).Length == 0);
		}

		[Fact]
		public void NewMocksHaveSameBehaviorAndDefaultValueAsOwner()
		{
			var mock = new Mock<IFoo>();
			var provider = new MockDefaultValueProvider(mock);

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod(), new object[0]);

			var barMock = Mock.Get((IBar)value);

			Assert.Equal(mock.Behavior, barMock.Behavior);
			Assert.Equal(mock.DefaultValue, barMock.DefaultValue);
		}

		public interface IFoo
		{
			IBar Bar { get; set; }
			string Value { get; set; }
			IEnumerable<int> Indexes { get; set; }
			IBar[] Bars { get; set; }
		}

		public interface IBar { }
	}
}
