using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Moq.Tests
{
	public class MockDefaultValueProviderFixture
	{
		[Fact]
		public void ConstructorWithNullMockThrows()
		{
			Assert.Throws<ArgumentNullException>(() =>
				new MockDefaultValueProvider(null));
		}

		[Fact]
		public void MockIsCorrect()
		{
			var owner = new Mock<IFoo>();
			var sut = new MockDefaultValueProvider(owner);

			var acutal = sut.Owner;

			Assert.Equal(owner, acutal);
		}

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

			Assert.Throws<MockVerificationException>(() => mock.Verify());
		}

		[Fact]
		public void ProvideInnerValueProviderReturnsNewEmptyDefaultValueProvider()
		{
			var owner = new Mock<IFoo>();
			var sut = new MockDefaultValueProvider(owner);

			var actual = sut.ProvideInnerValueProvider(owner);

			var provider = Assert.IsType<MockDefaultValueProvider>(actual);
			Assert.Equal(owner, provider.Owner);
			Assert.NotEqual(sut, actual);
		}

		[Fact]
		public void ProvideDefaultProvidesMockWithCorrectDefaultValueProvider()
		{
			var owner = new Mock<IFoo>();
			var sut = new MockDefaultValueProvider(owner);

			var actual = sut.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod());

			var mock = Assert.IsType<Mock<IBar>>(Mock.Get((IBar)actual));
			Assert.IsType<EmptyDefaultValueProvider>(mock.DefaultValueProvider);
			Assert.NotEqual(owner.DefaultValueProvider, mock.DefaultValueProvider);
		}

		[Fact]
		public void ProvideDefaultProvidesCorrectInnerMock()
		{
			var owner = new Mock<IFoo>();
			var sut = new MockDefaultValueProvider(owner);
			owner.DefaultValueProvider = sut;

			var actual = sut.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod());

			var mock = Assert.IsType<Mock<IBar>>(Mock.Get((IBar)actual));
			var provider = Assert.IsType<MockDefaultValueProvider>(mock.DefaultValueProvider);
			Assert.Equal(mock, provider.Owner);
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
