// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

		[Fact]
		public void Provides_completed_Task_containing_default_value_for_Task_of_value_type()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			var task = (Task<int>)GetDefaultValueForProperty(nameof(IFoo.TaskOfValueType), mock);

			Assert.NotNull(task);
			Assert.True(task.IsCompleted);
			Assert.Equal(default(int), task.Result);
		}

		[Fact]
		public void Provides_completed_Task_containing_empty_value_for_Task_of_emptyable_type()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			var task = (Task<int[]>)GetDefaultValueForProperty(nameof(IFoo.TaskOfEmptyableType), mock);

			Assert.NotNull(task);
			Assert.True(task.IsCompleted);
			Assert.NotNull(task.Result);
			Assert.Empty(task.Result);
		}

		[Fact]
		public void Provides_completed_Task_containing_null_for_Task_of_unmockable_reference_type()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			var task = (Task<IndexOutOfRangeException>)GetDefaultValueForProperty(nameof(IFoo.TaskOfUnmockableReferenceType), mock);

			Assert.NotNull(task);
			Assert.True(task.IsCompleted);
			Assert.Null(task.Result);
		}

		[Fact]
		public void Provides_completed_Task_containing_mocked_value_for_Task_of_mockable_type()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			var task = (Task<IBar>)GetDefaultValueForProperty(nameof(IFoo.TaskOfMockableType), mock);

			Assert.NotNull(task);
			Assert.True(task.IsCompleted);
			Assert.NotNull(task.Result);
			Assert.True(task.Result is IMocked);
		}

		[Fact]
		public void Provides_completed_Task_containing_completed_Task_containing_whatever_for_Task_of_Task_of_whatever()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			var task = (Task<Task<int>>)GetDefaultValueForProperty(nameof(IFoo.TaskOfTaskOfWhatever), mock);

			Assert.NotNull(task);
			Assert.True(task.IsCompleted);
			Assert.NotNull(task.Result);
			Assert.True(task.Result.IsCompleted);
		}

		[Fact]
		public void Provides_completed_ValueTask_containing_default_value_for_ValueTask_of_value_type()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			var task = (ValueTask<int>)GetDefaultValueForProperty(nameof(IFoo.ValueTaskOfValueType), mock);

			Assert.True(task.IsCompleted);
			Assert.Equal(default(int), task.Result);
		}

		[Fact]
		public void Provides_completed_ValueTask_containing_empty_value_for_ValueTask_of_emptyable_type()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			var task = (ValueTask<int[]>)GetDefaultValueForProperty(nameof(IFoo.ValueTaskOfEmptyableType), mock);

			Assert.True(task.IsCompleted);
			Assert.NotNull(task.Result);
			Assert.Empty(task.Result);
		}

		[Fact]
		public void Provides_completed_ValueTask_containing_null_for_Task_of_unmockable_reference_type()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			var task = (ValueTask<IndexOutOfRangeException>)GetDefaultValueForProperty(nameof(IFoo.ValueTaskOfUnmockableReferenceType), mock);

			Assert.True(task.IsCompleted);
			Assert.Null(task.Result);
		}

		[Fact]
		public void Provides_completed_ValueTask_containing_mocked_value_for_ValueTask_of_mockable_type()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			var task = (ValueTask<IBar>)GetDefaultValueForProperty(nameof(IFoo.ValueTaskOfMockableType), mock);

			Assert.True(task.IsCompleted);
			Assert.NotNull(task.Result);
			Assert.True(task.Result is IMocked);
		}

		[Fact]
		public void Provides_completed_ValueTask_of_completed_Task_of_mocked_value_for_ValueTask_of_Task_of_mockable_type()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			var task = (ValueTask<Task<IBar>>)GetDefaultValueForProperty(nameof(IFoo.ValueTaskOfTaskOfMockableType), mock);

			Assert.True(task.IsCompleted);
			Assert.NotNull(task.Result);
			Assert.True(task.Result.IsCompleted);
			Assert.NotNull(task.Result.Result);
			Assert.True(task.Result.Result is IMocked);
		}

		[Fact]
		public void Provides_ValueTuple_of_empty_array_and_completed_task_of_mocked_type()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			var value = GetDefaultValueForProperty(nameof(IFoo.ValueTupleOfReferenceTypeArrayAndTaskOfReferenceType), mock);

			var (bars, barTask) = ((IBar[], Task<IBar>))value;
			Assert.NotNull(bars);
			Assert.Empty(bars);
			Assert.NotNull(barTask);
			Assert.True(barTask.IsCompleted);
			Assert.IsAssignableFrom<IBar>(barTask.Result);
			Assert.True(barTask.Result is IMocked);
		}

		[Fact]
		public async Task Mock_wrapped_in_completed_Task_gets_included_in_verification_of_outer_mock()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };

			var bar = await mock.Object.TaskOfMockableType;
			var barMock = Mock.Get(bar);
			barMock.Setup(b => b.Do()).Verifiable();

			var ex = Assert.Throws<MockException>(() => mock.Verify());
			Assert.True(ex.IsVerificationError);
		}

		[Fact]
		public async Task Mock_wrapped_in_completed_ValueTask_gets_included_in_verification_of_outer_mock()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };

			var bar = await mock.Object.ValueTaskOfMockableType;
			var barMock = Mock.Get(bar);
			barMock.Setup(b => b.Do()).Verifiable();

			var ex = Assert.Throws<MockException>(() => mock.Verify());
			Assert.True(ex.IsVerificationError);
		}

		private static object GetDefaultValueForProperty(string propertyName, Mock<IFoo> mock)
		{
			var propertyGetter = typeof(IFoo).GetProperty(propertyName).GetGetMethod();
			return DefaultValueProvider.Mock.GetDefaultReturnValue(propertyGetter, mock);
		}

		public interface IFoo
		{
			IBar Bar { get; set; }
			string Value { get; set; }
			IEnumerable<int> Indexes { get; set; }
			IBar[] Bars { get; set; }
			Task<int> TaskOfValueType { get; set; }
			Task<int[]> TaskOfEmptyableType { get; set; }
			Task<IndexOutOfRangeException> TaskOfUnmockableReferenceType { get; set; }
			Task<IBar> TaskOfMockableType { get; set; }
			Task<Task<int>> TaskOfTaskOfWhatever { get; set; }
			ValueTask<int> ValueTaskOfValueType { get; set; }
			ValueTask<int[]> ValueTaskOfEmptyableType { get; set; }
			ValueTask<IndexOutOfRangeException> ValueTaskOfUnmockableReferenceType { get; set; }
			ValueTask<IBar> ValueTaskOfMockableType { get; set; }
			ValueTask<Task<IBar>> ValueTaskOfTaskOfMockableType { get; set; }
			(IBar[], Task<IBar>) ValueTupleOfReferenceTypeArrayAndTaskOfReferenceType { get; }
		}

		public interface IBar { void Do(); }
	}
}
