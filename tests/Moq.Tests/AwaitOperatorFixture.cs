// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Threading.Tasks;

using Xunit;

using static Moq.AwaitOperator;

namespace Moq.Tests
{
	public class AwaitOperatorFixture
	{
		[Fact]
		public async Task Returns__on_awaited_Task()
		{
			var expectedName = "Alice";

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetNameTaskAsync())).Returns(expectedName);

			var actualName = await mock.Object.GetNameTaskAsync();

			Assert.Equal(expectedName, actualName);
		}

		public interface IPerson
		{
			Task<string> GetNameTaskAsync();
		}
	}
}
