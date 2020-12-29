// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Threading.Tasks;

using Moq.Async;

using Xunit;

namespace Moq.Tests.Async
{
	public class AwaitableHandlerFixture
	{
		[Theory]
		[InlineData(typeof(AttributeTargets))]
		[InlineData(typeof(Func<bool>))]
		[InlineData(typeof(IAsyncResult))]
		[InlineData(typeof(int))]
		[InlineData(typeof(object))]
		public void TryGet__for_Task_of_X__returns_matching_TaskOfHandler(Type resultType)
		{
			var handler = AwaitableHandler.TryGet(typeof(Task<>).MakeGenericType(resultType));
			Assert.IsType<TaskOfHandler>(handler);
			Assert.Equal(resultType, handler.ResultType);
		}

		[Theory]
		[InlineData(typeof(AttributeTargets))]
		[InlineData(typeof(Func<bool>))]
		[InlineData(typeof(IAsyncResult))]
		[InlineData(typeof(int))]
		[InlineData(typeof(object))]
		public void TryGet__for_ValueTask_of_X__returns_matching_ValueTaskOfHandler(Type resultType)
		{
			var handler = AwaitableHandler.TryGet(typeof(ValueTask<>).MakeGenericType(resultType));
			Assert.IsType<ValueTaskOfHandler>(handler);
			Assert.Equal(resultType, handler.ResultType);
		}
	}
}
