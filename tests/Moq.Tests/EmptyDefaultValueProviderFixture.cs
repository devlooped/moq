// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

namespace Moq.Tests
{
	public class EmptyDefaultValueProviderFixture
	{
		[Fact]
		public void ProvidesNullString()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.StringValue));

			Assert.Null(value);
		}

		[Fact]
		public void ProvidesDefaultInt()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.IntValue));

			Assert.Equal(default(int), value);
		}

		[Fact]
		public void ProvidesNullInt()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.NullableIntValue));

			Assert.Null(value);
		}

		[Fact]
		public void ProvidesDefaultBool()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.BoolValue));

			Assert.Equal(default(bool), value);
		}

		[Fact]
		public void ProvidesDefaultEnum()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.UriKind));

			Assert.Equal(default(UriKind), value);
		}

		[Fact]
		public void ProvidesEmptyEnumerable()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.Indexes));
			Assert.True(value is IEnumerable<int> && ((IEnumerable<int>)value).Count() == 0);
		}

		[Fact]
		public void ProvidesEmptyArray()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.Bars));
			Assert.True(value is IBar[] && ((IBar[])value).Length == 0);
		}

		[Fact]
		public void ProvidesNullReferenceTypes()
		{
			var value1 = GetDefaultValueForProperty(nameof(IFoo.Bar));
			var value2 = GetDefaultValueForProperty(nameof(IFoo.Object));

			Assert.Null(value1);
			Assert.Null(value2);
		}

		[Fact]
		public void ProvideEmptyQueryable()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.Queryable));

			Assert.IsAssignableFrom<IQueryable<int>>(value);
			Assert.Equal(0, ((IQueryable<int>)value).Count());
		}

		[Fact]
		public void ProvideEmptyQueryableObjects()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.QueryableObjects));

			Assert.IsAssignableFrom<IQueryable>(value);
			Assert.Equal(0, ((IQueryable)value).Cast<object>().Count());
		}

		[Fact]
		public void ProvidesDefaultTask()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.TaskValue));

			Assert.NotNull(value);
			Assert.True(((Task)value).IsCompleted);
		}

		[Fact]
		public void ProvidesDefaultGenericTaskOfValueType()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.GenericTaskOfValueType));

			Assert.NotNull(value);
			Assert.True(((Task)value).IsCompleted);
			Assert.Equal(default(int), ((Task<int>)value).Result);
		}

		[Fact]
		public void ProvidesDefaultGenericTaskOfReferenceType()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.GenericTaskOfReferenceType));

			Assert.NotNull(value);
			Assert.True(((Task)value).IsCompleted);
			Assert.Equal(default(string), ((Task<string>)value).Result);
		}

		[Fact]
		public void ProvidesDefaultTaskOfGenericTask()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.TaskOfGenericTaskOfValueType));

			Assert.NotNull(value);
			Assert.True(((Task)value).IsCompleted);
			Assert.Equal(default(int), ((Task<Task<int>>) value).Result.Result);
		}

		[Fact]
		public void ProvidesDefaultValueTaskOfValueType()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.ValueTaskOfValueType));

			var result = (ValueTask<int>)value;
			Assert.True(result.IsCompleted);
			Assert.Equal(default(int), result.Result);
		}

		[Fact]
		public void ProvidesDefaultValueTaskOfValueTypeArray()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.ValueTaskOfValueTypeArray));

			var result = (ValueTask<int[]>)value;
			Assert.True(result.IsCompleted);
			Assert.NotNull(result.Result);
			Assert.Empty(result.Result);
		}

		[Fact]
		public void ProvidesDefaultValueTaskOfReferenceType()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.ValueTaskOfReferenceType));

			var result = (ValueTask<string>)value;
			Assert.True(result.IsCompleted);
			Assert.Equal(default(string), result.Result);
		}

		[Fact]
		public void ProvidesDefaultValueTaskOfTaskOfValueType()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.ValueTaskOfTaskOfValueType));

			var result = (ValueTask<Task<int>>)value;
			Assert.True(result.IsCompleted);
			Assert.NotNull(result.Result);
			Assert.True(result.Result.IsCompleted);
			Assert.Equal(default(int), result.Result.Result);
		}

		[Fact]
		public void ProvidesDefaultValueTupleOfReferenceTypeArrayAndTaskOfReferenceType()
		{
			var value = GetDefaultValueForProperty(nameof(IFoo.ValueTupleOfReferenceTypeArrayAndTaskOfReferenceType));

			var (bars, barTask) = ((IBar[], Task<IBar>))value;
			Assert.NotNull(bars);
			Assert.Empty(bars);
			Assert.NotNull(barTask);
			Assert.True(barTask.IsCompleted);
			Assert.Equal(default(IBar), barTask.Result);
		}

		private static object GetDefaultValueForProperty(string propertyName)
		{
			var propertyGetter = typeof(IFoo).GetProperty(propertyName).GetGetMethod();
			return DefaultValueProvider.Empty.GetDefaultReturnValue(propertyGetter, new Mock<IFoo>());
		}

		public interface IFoo
		{
			object Object { get; set; }
			IBar Bar { get; set; }
			string StringValue { get; set; }
			int IntValue { get; set; }
			bool BoolValue { get; set; }
			int? NullableIntValue { get; set; }
			UriKind UriKind { get; set; }
			IEnumerable<int> Indexes { get; set; }
			IBar[] Bars { get; set; }
			IQueryable<int> Queryable { get; }
			IQueryable QueryableObjects { get; }
			Task TaskValue { get; set; }
			Task<int> GenericTaskOfValueType { get; set; }
			Task<string> GenericTaskOfReferenceType { get; set; }
			Task<Task<int>> TaskOfGenericTaskOfValueType { get; set; }
			ValueTask<int> ValueTaskOfValueType { get; set; }
			ValueTask<int[]> ValueTaskOfValueTypeArray { get; set; }
			ValueTask<string> ValueTaskOfReferenceType { get; set; }
			ValueTask<Task<int>> ValueTaskOfTaskOfValueType { get; set; }
			(IBar[], Task<IBar>) ValueTupleOfReferenceTypeArrayAndTaskOfReferenceType { get; }
		}

		public interface IBar { }
	}
}
