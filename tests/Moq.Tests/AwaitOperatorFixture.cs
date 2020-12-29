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
		public async Task Callback__on_awaited_non_generic_Task()
		{
			var invoked = false;

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.DoSomethingTaskAsync())).Callback(() => invoked = true);

			Assert.False(invoked);

			await mock.Object.DoSomethingTaskAsync();

			Assert.True(invoked);
		}

		[Fact]
		public async Task Callback__on_awaited_non_generic_ValueTask()
		{
			var invoked = false;

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.DoSomethingValueTaskAsync())).Callback(() => invoked = true);

			Assert.False(invoked);

			await mock.Object.DoSomethingValueTaskAsync();

			Assert.True(invoked);
		}

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
		public async Task Callback__on_awaited_ValueTask()
		{
			var invoked = false;

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetNameValueTaskAsync())).Callback(() => invoked = true);

			Assert.False(invoked);

			await mock.Object.GetNameValueTaskAsync();

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
		public async Task Returns__on_awaited_ValueTask()
		{
			var expectedName = "Alice";

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetNameValueTaskAsync())).Returns(expectedName);

			var actualName = await mock.Object.GetNameValueTaskAsync();

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
		public async Task Throws__on_awaited_ValueTask()
		{
			var expectedException = new Exception();

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetNameValueTaskAsync())).Throws(expectedException);

			var task = mock.Object.GetNameValueTaskAsync();
			var actualException = await Assert.ThrowsAsync<Exception>(async () => await task);

			Assert.Same(expectedException, actualException);
		}

		[Fact]
		public async Task Callback__on_awaited_Task__of_property()
		{
			var invoked = false;

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.Friend.DoSomethingTaskAsync())).Callback(() => invoked = true);

			var friend = mock.Object.Friend;

			Assert.False(invoked);

			await friend.DoSomethingTaskAsync();

			Assert.True(invoked);
		}

		[Fact]
		public async Task Callback__on_awaited_ValueTask__of_property()
		{
			var invoked = false;

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.Friend.DoSomethingValueTaskAsync())).Callback(() => invoked = true);

			var friend = mock.Object.Friend;

			Assert.False(invoked);

			await friend.DoSomethingValueTaskAsync();

			Assert.True(invoked);
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
		public async Task Callback__on_property__of_awaited_ValueTask()
		{
			var invoked = false;

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetFriendValueTaskAsync()).Name).Callback(() => invoked = true);

			var friend = await mock.Object.GetFriendValueTaskAsync();

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
		public async Task Returns__on_property__of_awaited_ValueTask()
		{
			var expectedName = "Alice";

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetFriendValueTaskAsync()).Name).Returns(expectedName);

			var friend = await mock.Object.GetFriendValueTaskAsync();
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

		[Fact]
		public async Task Throws__on_property__of_awaited_ValueTask()
		{
			var expectedException = new Exception();

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetFriendValueTaskAsync()).Name).Throws(expectedException);

			var friend = await mock.Object.GetFriendValueTaskAsync();
			var actualException = Assert.Throws<Exception>(() => friend.Name);

			Assert.Same(expectedException, actualException);
		}

		public interface IPerson
		{
			IPerson Friend { get; }
			string Name { get; }
			Task<string> GetNameTaskAsync();
			ValueTask<string> GetNameValueTaskAsync();
			Task<IPerson> GetFriendTaskAsync();
			ValueTask<IPerson> GetFriendValueTaskAsync();
			Task DoSomethingTaskAsync();
			ValueTask DoSomethingValueTaskAsync();
		}
	}
}
