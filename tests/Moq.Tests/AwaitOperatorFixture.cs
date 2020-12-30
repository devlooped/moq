// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Xunit;

using static Moq.AwaitOperator;
using static Moq.Tests.AwaitOperatorFixture.AwaitSomeOperator;

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
		public async Task Callback__on_awaited_custom_awaitable()
		{
			var invoked = false;

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetNameSomeAsync())).Callback(() => invoked = true);

			Assert.False(invoked);

			await mock.Object.GetNameSomeAsync();

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
		public async Task Returns__on_awaited_custom_awaitable()
		{
			var expectedName = "Alice";

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetNameSomeAsync())).Returns(expectedName);

			var actualName = await mock.Object.GetNameSomeAsync();

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
		public async Task Throws__on_awaited_custom_awaitable()
		{
			var expectedException = new Exception();

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetNameSomeAsync())).Throws(expectedException);

			var task = mock.Object.GetNameSomeAsync();
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
		public async Task Callback__on_awaited_custom_awaitable__of_property()
		{
			var invoked = false;

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.Friend.GetNameSomeAsync())).Callback(() => invoked = true);

			var friend = mock.Object.Friend;

			Assert.False(invoked);

			await friend.GetNameSomeAsync();

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
		public async Task Callback__on_property__of_awaited_custom_awaitable()
		{
			var invoked = false;

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetFriendSomeAsync()).Name).Callback(() => invoked = true);

			var friend = await mock.Object.GetFriendSomeAsync();

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
		public async Task Returns__on_property__of_awaited_custom_awaitable()
		{
			var expectedName = "Alice";

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetFriendSomeAsync()).Name).Returns(expectedName);

			var friend = await mock.Object.GetFriendSomeAsync();
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

		[Fact]
		public async Task Throws__on_property__of_awaited_custom_awaitable()
		{
			var expectedException = new Exception();

			var mock = new Mock<IPerson>();
			mock.Setup(m => Await(m.GetFriendSomeAsync()).Name).Throws(expectedException);

			var friend = await mock.Object.GetFriendSomeAsync();
			var actualException = Assert.Throws<Exception>(() => friend.Name);

			Assert.Same(expectedException, actualException);
		}

		[Fact]
		public async Task SetupSequence_Pass__on_awaited_non_generic_Task()
		{
			var mock = new Mock<IPerson>();
			mock.SetupSequence(m => Await(m.DoSomethingTaskAsync())).Pass().Pass();

			var firstTask = mock.Object.DoSomethingTaskAsync();
			await firstTask;

			var secondTask = mock.Object.DoSomethingTaskAsync();
			await secondTask;

			Assert.NotSame(firstTask, secondTask);
		}

		[Fact]
		public async Task SetupSequence_Pass__on_awaited_non_generic_ValueTask()
		{
			var mock = new Mock<IPerson>();
			mock.SetupSequence(m => Await(m.DoSomethingValueTaskAsync())).Pass().Pass();

			var firstTask = mock.Object.DoSomethingValueTaskAsync();
			await firstTask;

			var secondTask = mock.Object.DoSomethingValueTaskAsync();
			await secondTask;

			Assert.NotSame(firstTask, secondTask);
		}

		[Fact]
		public async Task SetupSequence_Returns__on_awaited_Task()
		{
			var expectedFirstName = "Alice";
			var expectedSecondName = "Betty";

			var mock = new Mock<IPerson>();
			mock.SetupSequence(m => Await(m.GetNameTaskAsync())).Returns(expectedFirstName).Returns(expectedSecondName);

			var actualFirstName = await mock.Object.GetNameTaskAsync();
			var actualSecondName = await mock.Object.GetNameTaskAsync();

			Assert.Equal(expectedFirstName, actualFirstName);
			Assert.Equal(expectedSecondName, actualSecondName);
		}

		[Fact]
		public async Task SetupSequence_Returns__on_awaited_ValueTask()
		{
			var expectedFirstName = "Alice";
			var expectedSecondName = "Betty";

			var mock = new Mock<IPerson>();
			mock.SetupSequence(m => Await(m.GetNameValueTaskAsync())).Returns(expectedFirstName).Returns(expectedSecondName);

			var actualFirstName = await mock.Object.GetNameValueTaskAsync();
			var actualSecondName = await mock.Object.GetNameValueTaskAsync();

			Assert.Equal(expectedFirstName, actualFirstName);
			Assert.Equal(expectedSecondName, actualSecondName);
		}

		[Fact]
		public async Task SetupSequence_Throws__on_awaited_non_generic_Task()
		{
			var expectedFirstException = new Exception();
			var expectedSecondException = new Exception();

			var mock = new Mock<IPerson>();
			mock.SetupSequence(m => Await(m.DoSomethingTaskAsync())).Throws(expectedFirstException).Throws(expectedSecondException);

			var actualFirstException = await Assert.ThrowsAsync<Exception>(async () => await mock.Object.DoSomethingTaskAsync());
			var actualSecondException = await Assert.ThrowsAsync<Exception>(async () => await mock.Object.DoSomethingTaskAsync());

			Assert.Same(expectedFirstException, actualFirstException);
			Assert.Same(expectedSecondException, actualSecondException);
		}

		[Fact]
		public async Task SetupSequence_Throws__on_awaited_non_generic_ValueTask()
		{
			var expectedFirstException = new Exception();
			var expectedSecondException = new Exception();

			var mock = new Mock<IPerson>();
			mock.SetupSequence(m => Await(m.DoSomethingValueTaskAsync())).Throws(expectedFirstException).Throws(expectedSecondException);

			var actualFirstException = await Assert.ThrowsAsync<Exception>(async () => await mock.Object.DoSomethingValueTaskAsync());
			var actualSecondException = await Assert.ThrowsAsync<Exception>(async () => await mock.Object.DoSomethingValueTaskAsync());

			Assert.Same(expectedFirstException, actualFirstException);
			Assert.Same(expectedSecondException, actualSecondException);
		}

		[Fact]
		public async Task SetupSequence_Throws__on_awaited_Task()
		{
			var expectedFirstException = new Exception();
			var expectedSecondException = new Exception();

			var mock = new Mock<IPerson>();
			mock.SetupSequence(m => Await(m.GetNameTaskAsync())).Throws(expectedFirstException).Throws(expectedSecondException);

			var actualFirstException = await Assert.ThrowsAsync<Exception>(async () => await mock.Object.GetNameTaskAsync());
			var actualSecondException = await Assert.ThrowsAsync<Exception>(async () => await mock.Object.GetNameTaskAsync());

			Assert.Same(expectedFirstException, actualFirstException);
			Assert.Same(expectedSecondException, actualSecondException);
		}

		[Fact]
		public async Task SetupSequence_Throws__on_awaited_ValueTask()
		{
			var expectedFirstException = new Exception();
			var expectedSecondException = new Exception();

			var mock = new Mock<IPerson>();
			mock.SetupSequence(m => Await(m.GetNameValueTaskAsync())).Throws(expectedFirstException).Throws(expectedSecondException);

			var actualFirstException = await Assert.ThrowsAsync<Exception>(async () => await mock.Object.GetNameValueTaskAsync());
			var actualSecondException = await Assert.ThrowsAsync<Exception>(async () => await mock.Object.GetNameValueTaskAsync());

			Assert.Same(expectedFirstException, actualFirstException);
			Assert.Same(expectedSecondException, actualSecondException);
		}

		[Fact]
		public async Task Mock_Of()
		{
			var expectedName = "Alice";
			var expectedSameName = "also Alice";

			var mock = Mock.Of<IPerson>(m =>
				Await(m.GetFriendTaskAsync()).Name == expectedName &&
				Await(m.Friend.GetNameValueTaskAsync()) == expectedSameName);

			var friend = await mock.GetFriendTaskAsync();
			var actualName = friend.Name;

			Assert.Equal(expectedName, actualName);

			friend = mock.Friend;
			actualName = await friend.GetNameValueTaskAsync();

			Assert.Equal(expectedSameName, actualName);
		}

		public interface IPerson
		{
			IPerson Friend { get; }
			string Name { get; }
			Some<string> GetNameSomeAsync();
			Task<string> GetNameTaskAsync();
			ValueTask<string> GetNameValueTaskAsync();
			Some<IPerson> GetFriendSomeAsync();
			Task<IPerson> GetFriendTaskAsync();
			ValueTask<IPerson> GetFriendValueTaskAsync();
			Task DoSomethingTaskAsync();
			ValueTask DoSomethingValueTaskAsync();
		}

		public class Some<TResult>
		{
			private readonly Task<TResult> task;

			public Some(TResult result)
			{
				this.task = Task.FromResult(result);
			}

			public TaskAwaiter<TResult> GetAwaiter() => this.task.GetAwaiter();
		}

		public static class AwaitSomeOperator
		{
			public static TResult Await<TResult>(Some<TResult> some)
			{
				return default(TResult);
			}
		}
	}
}
