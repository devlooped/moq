// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Tests.Async
{
	using System.Threading.Tasks;

	using Moq.Async;

	using Xunit;

	public class AwaitableFixture
	{
		[Fact]
		public void TryGetResultRecursive_is_recursive()
		{
			const int expectedResult = 42;
			var obj = Task.FromResult(Task.FromResult(expectedResult));
			var result = Awaitable.TryGetResultRecursive(obj);
			Assert.Equal(expectedResult, result);
		}
	}
}
