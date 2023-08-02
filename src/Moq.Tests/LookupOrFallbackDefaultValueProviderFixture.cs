// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

namespace Moq.Tests
{
	public class LookupOrFallbackDefaultValueProviderFixture
	{
		[Theory]
		[InlineData(typeof(int), default(int))] // plain value type
		[InlineData(typeof(float), default(float))] // plain value type
		[InlineData(typeof(int?), null)] // nullable
		[InlineData(typeof(int[]), default(int[]))] // emptyable
		[InlineData(typeof(IEnumerable<>), null)] // emptyable
		[InlineData(typeof(string), default(string))] // primitive reference type
		[InlineData(typeof(Exception), default(Exception))] // reference type
		public void Produces_default_when_no_factory_registered(Type type, object expected)
		{
			var provider = new Provider();

			var actual = provider.GetDefaultValue(type);
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(typeof(int), 42)]
		public void Falls_back_to_default_generation_strategy_when_no_handler_available(Type type, object fallbackValue)
		{
			var provider = new Provider((t, _) => t == type ? fallbackValue : throw new NotSupportedException());
			provider.Deregister(type);

			var actual = provider.GetDefaultValue(type);

			Assert.Equal(fallbackValue, actual);
		}

		[Fact]
		public void Can_register_factory_for_specific_type()
		{
			var provider = new Provider();
			provider.Register(typeof(Exception), (_, __) => new InvalidOperationException());

			var actual = provider.GetDefaultValue(typeof(Exception));

			Assert.NotNull(actual);
			Assert.IsType<InvalidOperationException>(actual);
		}

		[Fact]
		public void Can_register_factory_for_generic_type()
		{
			var provider = new Provider();
			provider.Register(typeof(IEnumerable<>), (type, __) =>
			{
				var elementType = type.GetGenericArguments()[0];
				return Array.CreateInstance(elementType, 0);
			});

			var actual = provider.GetDefaultValue(typeof(IEnumerable<int>));

			Assert.NotNull(actual);
			Assert.IsType<int[]>(actual);
		}

		[Fact]
		public void Can_register_factory_for_array_type()
		{
			var provider = new Provider();
			provider.Register(typeof(Array), (type, __) =>
			{
				var elementType = type.GetElementType();
				return Array.CreateInstance(elementType, 0);
			});

			var actual = provider.GetDefaultValue(typeof(int[]));

			Assert.NotNull(actual);
			Assert.IsType<int[]>(actual);
		}

		[Fact]
		public void Produces_completed_Task()
		{
			var provider = new Provider();

			var actual = (Task)provider.GetDefaultValue(typeof(Task));

			Assert.True(actual.IsCompleted);
		}

		[Fact]
		public void Handling_of_Task_can_be_disabled()
		{
			var provider = new Provider();
			provider.Deregister(typeof(Task));

			var actual = provider.GetDefaultValue(typeof(Task));

			Assert.Null(actual);
		}

		[Fact]
		public void Produces_completed_generic_Task()
		{
			const int expected = 42;
			var provider = new Provider();
			provider.Register(typeof(int), (_, __) => expected);

			var actual = (Task<int>)provider.GetDefaultValue(typeof(Task<int>));

			Assert.True(actual.IsCompleted);
			Assert.Equal(42, actual.Result);
		}

		[Fact]
		public void Handling_of_generic_Task_can_be_disabled()
		{
			var provider = new Provider();
			provider.Deregister(typeof(Task<>));

			var actual = provider.GetDefaultValue(typeof(Task<int>));

			Assert.Null(actual);
		}

		[Fact]
		public void Produces_completed_ValueTask()
		{
			const int expectedResult = 42;
			var provider = new Provider();
			provider.Register(typeof(int), (_, __) => expectedResult);

			var actual = (ValueTask<int>)provider.GetDefaultValue(typeof(ValueTask<int>));

			Assert.True(actual.IsCompleted);
			Assert.Equal(42, actual.Result);
		}

		[Fact]
		public void Handling_of_ValueTask_can_be_disabled()
		{
			// If deregistration of ValueTask<> handling really works, the fallback strategy will produce
			// `default(ValueTask<int>)`, which equals a completed task containing 0 as result. So this test
			// needs to look different from the `Task<>` equivalent above.

			// We check for successful disabling of `ValueTask<>` handling indirectly, namely by
			// setting up a specific default value for `int` first, then we can verify that this value
			// does *not* get wrapped in a value task when we ask for `ValueTask<int>`

			const int unexpected = 42;
			var provider = new Provider();
			provider.Register(typeof(int), (_, __) => unexpected);
			provider.Deregister(typeof(ValueTask<>));

			var actual = (ValueTask<int>)provider.GetDefaultValue(typeof(ValueTask<int>));

			Assert.Equal(default(ValueTask<int>), actual);
			Assert.NotEqual(unexpected, actual.Result);
		}

		[Fact]
		public void Produces_2_tuple()
		{
			const int expectedIntResult = 42;
			const string expectedStringResult = "*";
			var provider = new Provider();
			provider.Register(typeof(int), (_, __) => expectedIntResult);
			provider.Register(typeof(string), (_, __) => expectedStringResult);

			var actual = ((int, string))provider.GetDefaultValue(typeof(ValueTuple<int, string>));

			Assert.Equal(expectedIntResult, actual.Item1);
			Assert.Equal(expectedStringResult, actual.Item2);
		}

		[Fact]
		public void Produces_3_tuple()
		{
			const int expectedIntResult = 42;
			const float expectedFloatResult = 3.1415f;
			const string expectedStringResult = "*";
			var provider = new Provider();
			provider.Register(typeof(int), (_, __) => expectedIntResult);
			provider.Register(typeof(float), (_, __) => expectedFloatResult);
			provider.Register(typeof(string), (_, __) => expectedStringResult);

			var actual = ((int, float, string))provider.GetDefaultValue(typeof(ValueTuple<int, float, string>));

			Assert.Equal(expectedIntResult, actual.Item1);
			Assert.Equal(expectedFloatResult, actual.Item2);
			Assert.Equal(expectedStringResult, actual.Item3);
		}

		[Fact]
		public void Handling_of_ValueTuple_can_be_disabled()
		{
			// This test follows the same logic as the one above for `ValueTask<>`.

			const string unexpectedString = "*";
			const int unexpectedInt = 42;
			var provider = new Provider();
			provider.Register(typeof(string), (_, __) => unexpectedString);
			provider.Register(typeof(int), (_, __) => unexpectedInt);
			provider.Deregister(typeof(ValueTuple<,>));

			var actual = ((string, int))provider.GetDefaultValue(typeof((string, int)));

			Assert.Equal((default(string), default(int)), actual);
			Assert.NotEqual((unexpectedString, unexpectedInt), actual);
		}

		/// <summary>
		/// Subclass of <see cref="LookupOrFallbackDefaultValueProvider"/> used as a test surrogate.
		/// </summary>
		private sealed class Provider : LookupOrFallbackDefaultValueProvider
		{
			private Mock<object> mock;
			private Func<Type, Mock, object> fallback;

			public Provider(Func<Type, Mock, object> fallback = null)
			{
				this.mock = new Mock<object>();
				this.fallback = fallback;
			}

			public object GetDefaultValue(Type type)
			{
				return base.GetDefaultValue(type, mock);
			}

			new public void Deregister(Type factoryKey)
			{
				base.Deregister(factoryKey);
			}

			new public void Register(Type factoryKey, Func<Type, Mock, object> factory)
			{
				base.Register(factoryKey, factory);
			}

			protected override object GetFallbackDefaultValue(Type type, Mock mock)
			{
				return this.fallback?.Invoke(type, mock)
				    ?? base.GetFallbackDefaultValue(type, mock);
			}
		}
	}
}
