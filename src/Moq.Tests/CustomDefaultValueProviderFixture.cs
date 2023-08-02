// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class CustomDefaultValueProviderFixture
	{
		[Fact]
		public void Custom_DefaultValueProvider_implementations_have_Kind_Custom()
		{
			var customDefaultValueProvider = new ConstantDefaultValueProvider(null);

			Assert.Equal(DefaultValue.Custom, customDefaultValueProvider.Kind);
		}

		[Fact]
		public void Custom_DefaultValueProvider_gets_invoked()
		{
			const int expectedReturnValue = 42;
			var constantDefaultValueProvider = new ConstantDefaultValueProvider(expectedReturnValue);
			var mock = new Mock<IFoo>() { DefaultValueProvider = constantDefaultValueProvider };

			var actualReturnValue = mock.Object.GetValue();

			Assert.Equal(expectedReturnValue, actualReturnValue);
		}

		[Fact]
		public void Default_values_from_custom_providers_are_not_cached()
		{
			// NOTE: This specification is not set in stone; it simply documents Moq's behavior at the
			// time when custom default value providers became part of the public API. It might very well
			// make sense to cache default return values. This could be achieved by turning the purpose-
			// bound `Mock.InnerMocks` dictionary into a more generic `Mock.CachedDefaultValues`.

			var mock = new Mock<IFoo>();

			var values1 = mock.Object.GetValues();
			var values2 = mock.Object.GetValues();

			Assert.NotSame(values1, values2);
		}

		[Fact]
		public void Mocks_inherit_custom_default_value_provider_from_MockRepository()
		{
			var customDefaultValueProvider = new ConstantDefaultValueProvider(null);
			var mockRepository = new MockRepository(MockBehavior.Default) { DefaultValueProvider = customDefaultValueProvider };

			var mock = mockRepository.Create<IFoo>();

			Assert.Equal(DefaultValue.Custom, mock.DefaultValue);
			Assert.Same(customDefaultValueProvider, mock.DefaultValueProvider);
		}

		[Fact]
		public void Custom_default_value_provider_does_not_interfere_with_recursive_mocking()
		{
			var nullDefaultValueProvider = new ConstantDefaultValueProvider(null);
			var outerMock = new Mock<IFoo>() { DefaultValueProvider = nullDefaultValueProvider };

			outerMock.Setup(om => om.Inner.GetValue());
			//                      ^^^^^^
			// If the custom default value provider were used to determine this value,
			// then it would be `null` instead of a mocked `IFoo` instance. Multi-dot expressions
			// must always use `MockDefaultValueProvider`, regardless of the outermost mock's
			// configured default value provider.
			var inner = outerMock.Object.Inner;

			Assert.NotNull(inner);
			Assert.IsAssignableFrom<IFoo>(inner);
		}

		[Fact]
		public void Inner_mocks_inherit_custom_default_value_provider_from_outer_mock()
		{
			const int expectedReturnValue = 42;
			var customDefaultValueProvider = new ConstantDefaultValueProvider(expectedReturnValue);
			var outerMock = new Mock<IFoo>() { DefaultValueProvider = customDefaultValueProvider };

			outerMock.Setup(om => om.Inner.GetValues()); // we don't care about GetValues, all we want here is a multi-dot expression
			var inner = outerMock.Object.Inner;
			var innerMock = Mock.Get(inner);

			Assert.Same(outerMock.DefaultValueProvider, innerMock.DefaultValueProvider);

			var actualReturnValue = inner.GetValue();

			Assert.Equal(expectedReturnValue, actualReturnValue);
		}

		public interface IFoo
		{
			int Value { get; set; }
			int GetValue();
			int[] GetValues();
			IFoo Inner { get; }
		}

		private sealed class ConstantDefaultValueProvider : DefaultValueProvider
		{
			private object value;

			public ConstantDefaultValueProvider(object value)
			{
				this.value = value;
			}

			protected internal override object GetDefaultValue(Type type, Mock mock)
			{
				return this.value;
			}
		}
	}
}
