// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Threading.Tasks;

using Moq.Async;

using Xunit;

namespace Moq.Tests.Async
{
	public class ValueTaskOfHandlerFixture
	{
		[Fact]
		public async Task CreateCompleted__returns_completed_task__with_correct_result()
		{
			var expectedResult = 42;

			var handler = new ValueTaskOfHandler(typeof(ValueTask<int>), typeof(int));
			var task = (ValueTask<int>)handler.CreateCompleted(expectedResult);

			var actualResult = await task;

			Assert.Equal(expectedResult, actualResult);
		}

		[Fact]
		public async Task CreateFaulted__returns_faulted_task__that_throws_correct_exception()
		{
			var expectedException = new Exception();

			var handler = new ValueTaskOfHandler(typeof(ValueTask<int>), typeof(int));
			var task = (ValueTask<int>)handler.CreateFaulted(expectedException);

			var actualException = await Assert.ThrowsAsync<Exception>(async () => await task);

			Assert.Same(expectedException, actualException);
		}

		[Fact]
		public void TryGetResult__can_extract_result__from_completed_Task()
		{
			var expectedResult = 42;

			var handler = new ValueTaskOfHandler(typeof(ValueTask<int>), typeof(int));
			var task = new ValueTask<int>(expectedResult);

			Assert.True(handler.TryGetResult(task, out var actualResult));
			Assert.Equal(expectedResult, actualResult);
		}

		[Fact]
		public void TryGetResult__cannot_extract_result__from_faulted_Task()
		{
			var handler = new ValueTaskOfHandler(typeof(ValueTask<int>), typeof(int));
			var task = new ValueTask<int>(Task.FromException<int>(new Exception()));

			Assert.False(handler.TryGetResult(task, out _));
		}

		[Fact]
		public void TryGetResult__cannot_extract_result__from_null()
		{
			var handler = new ValueTaskOfHandler(typeof(ValueTask<int>), typeof(int));

			Assert.False(handler.TryGetResult(null, out _));
		}
	}
}
