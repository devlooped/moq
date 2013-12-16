using System;
using System.Collections.Generic;
using System.Linq;
#if !NET3x && !SILVERLIGHT
using System.Threading.Tasks;
#endif
using Xunit;

namespace Moq.Tests
{
	public class EmptyDefaultValueProviderFixture
	{
		[Fact]
		public void ProvidesNullString()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("StringValue").GetGetMethod());

			Assert.Null(value);
		}

		[Fact]
		public void ProvidesDefaultInt()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("IntValue").GetGetMethod());

			Assert.Equal(default(int), value);
		}

		[Fact]
		public void ProvidesNullInt()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("NullableIntValue").GetGetMethod());

			Assert.Null(value);
		}

		[Fact]
		public void ProvidesDefaultBool()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("BoolValue").GetGetMethod());

			Assert.Equal(default(bool), value);
		}

		[Fact]
		public void ProvidesDefaultEnum()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Platform").GetGetMethod());

			Assert.Equal(default(PlatformID), value);
		}

		[Fact]
		public void ProvidesEmptyEnumerable()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Indexes").GetGetMethod());
			Assert.True(value is IEnumerable<int> && ((IEnumerable<int>)value).Count() == 0);
		}

		[Fact]
		public void ProvidesEmptyArray()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Bars").GetGetMethod());
			Assert.True(value is IBar[] && ((IBar[])value).Length == 0);
		}

		[Fact]
		public void ProvidesNullReferenceTypes()
		{
			var provider = new EmptyDefaultValueProvider();

			var value1 = provider.ProvideDefault(typeof(IFoo).GetProperty("Bar").GetGetMethod());
			var value2 = provider.ProvideDefault(typeof(IFoo).GetProperty("Object").GetGetMethod());

			Assert.Null(value1);
			Assert.Null(value2);
		}

		[Fact]
		public void ProvideEmptyQueryable()
		{
			var provider = new EmptyDefaultValueProvider();
			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("Queryable").GetGetMethod());

			Assert.IsAssignableFrom<IQueryable<int>>(value);
			Assert.Equal(0, ((IQueryable<int>)value).Count());
		}

		[Fact]
		public void ProvideEmptyQueryableObjects()
		{
			var provider = new EmptyDefaultValueProvider();
			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("QueryableObjects").GetGetMethod());

			Assert.IsAssignableFrom<IQueryable>(value);
			Assert.Equal(0, ((IQueryable)value).Cast<object>().Count());
		}

#if !NET3x && !SILVERLIGHT
		[Fact]
		public void ProvidesDefaultTask()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("TaskValue").GetGetMethod());

			Assert.NotNull(value);
			Assert.True(((Task)value).IsCompleted);
		}

		[Fact]
		public void ProvidesDefaultGenericTaskOfValueType()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("GenericTaskOfValueType").GetGetMethod());

			Assert.NotNull(value);
			Assert.True(((Task)value).IsCompleted);
			Assert.Equal(default(int), ((Task<int>)value).Result);
		}

		[Fact]
		public void ProvidesDefaultGenericTaskOfReferenceType()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("GenericTaskOfReferenceType").GetGetMethod());

			Assert.NotNull(value);
			Assert.True(((Task)value).IsCompleted);
			Assert.Equal(default(string), ((Task<string>)value).Result);
		}

		[Fact]
		public void ProvidesDefaultTaskOfGenericTask()
		{
			var provider = new EmptyDefaultValueProvider();

			var value = provider.ProvideDefault(typeof(IFoo).GetProperty("TaskOfGenericTaskOfValueType").GetGetMethod());

			Assert.NotNull(value);
			Assert.True(((Task)value).IsCompleted);
			Assert.Equal(default(int), ((Task<Task<int>>) value).Result.Result);
		}
#endif

		public interface IFoo
		{
			object Object { get; set; }
			IBar Bar { get; set; }
			string StringValue { get; set; }
			int IntValue { get; set; }
			bool BoolValue { get; set; }
			int? NullableIntValue { get; set; }
			PlatformID Platform { get; set; }
			IEnumerable<int> Indexes { get; set; }
			IBar[] Bars { get; set; }
			IQueryable<int> Queryable { get; }
			IQueryable QueryableObjects { get; }
#if !NET3x && !SILVERLIGHT
			Task TaskValue { get; set; }
			Task<int> GenericTaskOfValueType { get; set; }
			Task<string> GenericTaskOfReferenceType { get; set; }
			Task<Task<int>> TaskOfGenericTaskOfValueType { get; set; }
#endif
		}

		public interface IBar { }
	}
}
