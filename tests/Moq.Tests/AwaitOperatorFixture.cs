// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Threading.Tasks;

using Xunit;

using static Moq.AwaitOperator;

namespace Moq.Tests
{
	public class AwaitOperatorFixture
	{
		[Fact]
		public async Task Callback__on_awaited_Task()
		{
			var invoked = false;

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetNameTaskAsync())).Callback(() => invoked = true);

			Assert.False(invoked);

			await mock.Object.GetNameTaskAsync();

			Assert.True(invoked);
		}

		[Fact]
		public async Task Returns__on_awaited_Task()
		{
			var expectedName = "Alice";

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetNameTaskAsync())).Returns(expectedName);

			var actualName = await mock.Object.GetNameTaskAsync();

			Assert.Equal(expectedName, actualName);
		}

		[Fact]
		public async Task Throws__on_awaited_Task()
		{
			var expectedException = new Exception();

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetNameTaskAsync())).Throws(expectedException);

			var task = mock.Object.GetNameTaskAsync();
			var actualException = await Assert.ThrowsAsync<Exception>(async () => await task);

			Assert.Same(expectedException, actualException);
		}

		[Fact]
		public async Task Callback__on_property__of_awaited_Task()
		{
			var invoked = false;

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetFriendTaskAsync()).Name).Callback(() => invoked = true);

			var friend = await mock.Object.GetFriendTaskAsync();

			Assert.False(invoked);

			_ = friend.Name;

			Assert.True(invoked);
		}

		[Fact]
		public async Task Returns__on_property__of_awaited_Task()
		{
			var expectedName = "Alice";

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetFriendTaskAsync()).Name).Returns(expectedName);

			var friend = await mock.Object.GetFriendTaskAsync();
			var actualName = friend.Name;

			Assert.Equal(expectedName, actualName);
		}

		[Fact]
		public async Task Throws__on_property__of_awaited_Task()
		{
			var expectedException = new Exception();

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetFriendTaskAsync()).Name).Throws(expectedException);

			var friend = await mock.Object.GetFriendTaskAsync();
			var actualException = Assert.Throws<Exception>(() => friend.Name);

			Assert.Same(expectedException, actualException);
		}

		public interface IPerson
		{
			string Name { get; }
			Task<string> GetNameTaskAsync();
			Task<IPerson> GetFriendTaskAsync();
		}
	}
}
