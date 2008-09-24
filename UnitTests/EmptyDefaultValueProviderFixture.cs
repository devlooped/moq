using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class EmptyDefaultValueProviderFixture
	{
		[Fact]
		public void ProvidesNullString()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("StringValue").GetGetMethod(), new object[0]);

			Assert.Null(value);
		}

		[Fact]
		public void ProvidesDefaultInt()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("IntValue").GetGetMethod(), new object[0]);

			Assert.Equal(default(int), value);
		}

		[Fact]
		public void ProvidesDefaultBool()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("BoolValue").GetGetMethod(), new object[0]);

			Assert.Equal(default(bool), value);
		}

		[Fact]
		public void ProvidesDefaultEnum()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Platform").GetGetMethod(), new object[0]);

			Assert.Equal(default(PlatformID), value);
		}

		[Fact]
		public void ProvidesEmptyEnumerable()
		{
			var provider = new MockDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Indexes").GetGetMethod(), new object[0]);
			Assert.True(value is IEnumerable<int> && ((IEnumerable<int>)value).Count() == 0);
		}

		[Fact]
		public void ProvidesEmptyArray()
		{
			var provider = new MockDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Bars").GetGetMethod(), new object[0]);
			Assert.True(value is IBar[] && ((IBar[])value).Length == 0);
		}

		[Fact]
		public void ProvidesNullReferenceTypes()
		{
			var provider = new EmptyDefaultValueProvider();

			var value1 = provider.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod(), new object[0]);
			var value2 = provider.ProvideDefault(typeof(IFoo).GetProperty("Object").GetGetMethod(), new object[0]);

			Assert.Null(value1);
			Assert.Null(value2);
		}

		public interface IFoo
		{
			object Object { get; set; }
			IBar Bar { get; set; }
			string StringValue { get; set; }
			int IntValue { get; set; }
			bool BoolValue { get; set; }
			PlatformID Platform { get; set; }
			IEnumerable<int> Indexes { get; set; }
			IBar[] Bars { get; set; }
		}

		public interface IBar { }
	}
}
