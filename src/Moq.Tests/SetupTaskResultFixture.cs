// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Threading.Tasks;

using Xunit;

namespace Moq.Tests
{
	public class SetupTaskResultFixture
	{
		private readonly Exception Exception = new Exception("bad");
		private readonly Exception SecondException = new Exception("very bad");

		private readonly IPerson Friend = Mock.Of<IPerson>(p => p.Name == "Alice");
		private readonly string NameOfFriend = "Alice";
		private readonly string SecondNameOfFriend = "Alicia";

		private readonly IPerson SecondFriend = Mock.Of<IPerson>(p => p.Name == "Betty");

		public interface IPerson
		{
			string Name { get; set; }
			Task<string> GetNameTaskAsync();
			ValueTask<string> GetNameValueTaskAsync();

			IPerson Friend { get; set; }
			Task<IPerson> GetFriendTaskAsync();
			Task<IPerson> GetFriendTaskAsync(string friendName);
			ValueTask<IPerson> GetFriendValueTaskAsync();
			ValueTask<IPerson> GetFriendValueTaskAsync(string friendName);
		}

		[Fact]
		public async Task Setup__task_Result__creates_a_single_setup()
		{
			var person = new Mock<IPerson>() { DefaultValue = DefaultValue.Mock };
			person.Setup(p => p.GetFriendTaskAsync().Result);
			var friend = Mock.Get(await person.Object.GetFriendTaskAsync());
			Assert.Single(person.Setups);
			Assert.Empty(friend.Setups);
		}

		[Fact]
		public async Task Mock_Of__completed_Task()
		{
			var person = Mock.Of<IPerson>(p => p.GetFriendTaskAsync().Result == Friend);
			var friend = await person.GetFriendTaskAsync();
			Assert.Same(Friend, friend);
		}

		[Fact]
		public async Task Mock_Of__completed_ValueTask()
		{
			var person = Mock.Of<IPerson>(p => p.GetFriendValueTaskAsync().Result == Friend);
			var friend = await person.GetFriendValueTaskAsync();
			Assert.Same(Friend, friend);
		}

		[Fact]
		public async Task Mock_Of__property_of__completed_Task()
		{
			var person = Mock.Of<IPerson>(p => p.GetFriendTaskAsync().Result.Name == NameOfFriend);
			var friend = await person.GetFriendTaskAsync();
			Assert.Equal(NameOfFriend, friend.Name);
		}

		[Fact]
		public async Task Mock_Of__property_of__completed_ValueTask()
		{
			var person = Mock.Of<IPerson>(p => p.GetFriendValueTaskAsync().Result.Name == NameOfFriend);
			var friend = await person.GetFriendValueTaskAsync();
			Assert.Equal(NameOfFriend, friend.Name);
		}

		[Fact]
		public async Task Mock_Of__properties_of__completed_Task()
		{
			var person = Mock.Of<IPerson>(p => p.GetFriendTaskAsync().Result.Name == NameOfFriend
			                                && p.GetFriendTaskAsync().Result.Friend == SecondFriend);
			var friend = await person.GetFriendTaskAsync();
			Assert.Equal(NameOfFriend, friend.Name);
			Assert.Same(SecondFriend, friend.Friend);
		}

		[Fact]
		public async Task Mock_Of__properties_of__completed_ValueTask()
		{
			var person = Mock.Of<IPerson>(p => p.GetFriendValueTaskAsync().Result.Name == NameOfFriend
			                                && p.GetFriendValueTaskAsync().Result.Friend == SecondFriend);
			var friend = await person.GetFriendValueTaskAsync();
			Assert.Equal(NameOfFriend, friend.Name);
			Assert.Same(SecondFriend, friend.Friend);
		}

		[Fact]
		public async Task Setup__completed_Task__Returns()
		{
			var person = new Mock<IPerson>();
			person.Setup(p => p.GetFriendTaskAsync().Result).Returns(Friend);
			var friend = await person.Object.GetFriendTaskAsync();
			Assert.Same(Friend, friend);
		}

		[Fact]
		public async Task Setup__completed_Task__Returns_callback()
		{
			var person = new Mock<IPerson>();
			person.Setup(p => p.GetFriendTaskAsync().Result).Returns(() => Friend);
			var friend = await person.Object.GetFriendTaskAsync();
			Assert.Same(Friend, friend);
		}

		[Fact]
		public async Task Setup__completed_Task__Returns_IInvocation_callback()
		{
			var person = new Mock<IPerson>();
			person.Setup(p => p.GetFriendTaskAsync().Result).Returns((IInvocation _) => Friend);
			var friend = await person.Object.GetFriendTaskAsync();
			Assert.Same(Friend, friend);
		}

		[Fact]
		public async Task Setup__completed_Task__Throws()
		{
			var person = new Mock<IPerson>();
			person.Setup(p => p.GetFriendTaskAsync().Result).Throws(Exception);
			var exception = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendTaskAsync());
			Assert.Same(Exception, exception);
		}

		[Fact]
		public async Task Setup__completed_Task_from_Func__Throws()
		{
			var person = new Mock<IPerson>();
			person.Setup(p => p.GetFriendTaskAsync().Result).Throws(() => Exception);
			var exception = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendTaskAsync());
			Assert.Same(Exception, exception);
		}

		[Fact]
		public async Task Setup__completed_Task_from_Func_with_params__Throws()
		{
			var person = new Mock<IPerson>();
			person.Setup(p => p.GetFriendTaskAsync(NameOfFriend).Result).Throws((string s) => new Exception(NameOfFriend));
			var exception = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendTaskAsync(NameOfFriend));
			Assert.Equal(NameOfFriend, exception.Message);
		}

		[Fact]
		public async Task Setup__completed_ValueTask__Returns()
		{
			var person = new Mock<IPerson>();
			person.Setup(p => p.GetFriendValueTaskAsync().Result).Returns(Friend);
			var friend = await person.Object.GetFriendValueTaskAsync();
			Assert.Same(Friend, friend);
		}

		[Fact]
		public async Task Setup__completed_ValueTask__Returns_callback()
		{
			var person = new Mock<IPerson>();
			person.Setup(p => p.GetFriendValueTaskAsync().Result).Returns(()=> Friend);
			var friend = await person.Object.GetFriendValueTaskAsync();
			Assert.Same(Friend, friend);
		}

		[Fact]
		public async Task Setup__completed_ValueTask__Throws()
		{
			var person = new Mock<IPerson>();
			person.Setup(p => p.GetFriendValueTaskAsync().Result).Throws(Exception);
			var exception = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendValueTaskAsync());
			Assert.Same(Exception, exception);
		}

		[Fact]
		public async Task Setup__completed_ValueTask_from_Func__Throws()
		{
			var person = new Mock<IPerson>();
			person.Setup(p => p.GetFriendValueTaskAsync().Result).Throws(() => Exception);
			var exception = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendValueTaskAsync());
			Assert.Same(Exception, exception);
		}

		[Fact]
		public async Task Setup__completed_ValueTask_from_Func_with_params__Throws()
		{
			var person = new Mock<IPerson>();
			person.Setup(p => p.GetFriendValueTaskAsync(NameOfFriend).Result).Throws((string s) => new Exception(NameOfFriend));
			var exception = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendValueTaskAsync(NameOfFriend));
			Assert.Equal(NameOfFriend, exception.Message);
		}

		[Fact]
		public async Task SetupGet__property_of__completed_Task__Returns()
		{
			var person = new Mock<IPerson>();
			person.SetupGet(p => p.GetFriendTaskAsync().Result.Name).Returns(NameOfFriend);
			var friend = await person.Object.GetFriendTaskAsync();
			Assert.Equal(NameOfFriend, friend.Name);
		}

		[Fact]
		public async Task SetupGet__property_of__completed_Task__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupGet(p => p.GetFriendTaskAsync().Result.Name).Throws(Exception);
			var friend = await person.Object.GetFriendTaskAsync();
			var exception = Assert.Throws<Exception>(() => friend.Name);
			Assert.Same(Exception, exception);
		}

		[Fact]
		public async Task SetupGet__property_of__completed_Task_from_func__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupGet(p => p.GetFriendTaskAsync().Result.Name).Throws(() => Exception);
			var friend = await person.Object.GetFriendTaskAsync();
			var exception = Assert.Throws<Exception>(() => friend.Name);
			Assert.Same(Exception, exception);
		}

		[Fact]
		public async Task SetupGet__property_of__completed_ValueTask__Returns()
		{
			var person = new Mock<IPerson>();
			person.Setup(m => m.GetFriendValueTaskAsync().Result.Name).Returns(NameOfFriend);
			var friend = await person.Object.GetFriendValueTaskAsync();
			Assert.Equal(NameOfFriend, friend.Name);
		}

		[Fact]
		public async Task SetupGet__property_of__completed_ValueTask__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupGet(p => p.GetFriendValueTaskAsync().Result.Name).Throws(Exception);
			var friend = await person.Object.GetFriendValueTaskAsync();
			var exception = Assert.Throws<Exception>(() => friend.Name);
			Assert.Same(Exception, exception);
		}

		[Fact]
		public async Task SetupGet__property_of__completed_ValueTask_from_func__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupGet(p => p.GetFriendValueTaskAsync().Result.Name).Throws(() => Exception);
			var friend = await person.Object.GetFriendValueTaskAsync();
			var exception = Assert.Throws<Exception>(() => friend.Name);
			Assert.Same(Exception, exception);
		}

		[Fact]
		public async Task SetupSequence__completed_Task__Returns()
		{
			var person = new Mock<IPerson>();
			person.SetupSequence(p => p.GetFriendTaskAsync().Result).Returns(Friend).Returns(SecondFriend);
			var friend = await person.Object.GetFriendTaskAsync();
			var secondFriend = await person.Object.GetFriendTaskAsync();
			Assert.Same(Friend, friend);
			Assert.Same(SecondFriend, secondFriend);
		}

		[Fact]
		public async Task SetupSequence__completed_Task__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupSequence(p => p.GetFriendTaskAsync().Result).Throws(Exception).Throws(SecondException);
			var exception = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendTaskAsync());
			var secondException = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendTaskAsync());
			Assert.Same(Exception, exception);
			Assert.Same(SecondException, secondException);
		}

		[Fact]
		public async Task SetupSequence__completed_Task_from_func__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupSequence(p => p.GetFriendTaskAsync().Result).Throws(() => Exception).Throws(() => SecondException);
			var exception = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendTaskAsync());
			var secondException = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendTaskAsync());
			Assert.Same(Exception, exception);
			Assert.Same(SecondException, secondException);
		}

		[Fact]
		public async Task SetupSequence__completed_ValueTask__Returns()
		{
			var person = new Mock<IPerson>();
			person.SetupSequence(p => p.GetFriendValueTaskAsync().Result).Returns(Friend).Returns(SecondFriend);
			var friend = await person.Object.GetFriendValueTaskAsync();
			var secondFriend = await person.Object.GetFriendValueTaskAsync();
			Assert.Same(Friend, friend);
			Assert.Same(SecondFriend, secondFriend);
		}

		[Fact]
		public async Task SetupSequence__completed_ValueTask__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupSequence(p => p.GetFriendValueTaskAsync().Result).Throws(Exception).Throws(SecondException);
			var exception = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendValueTaskAsync());
			var secondException = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendValueTaskAsync());
			Assert.Same(Exception, exception);
			Assert.Same(SecondException, secondException);
		}

		[Fact]
		public async Task SetupSequence__completed_ValueTask_from_func__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupSequence(p => p.GetFriendValueTaskAsync().Result).Throws(() => Exception).Throws(() => SecondException);
			var exception = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendValueTaskAsync());
			var secondException = await Assert.ThrowsAsync<Exception>(async () => await person.Object.GetFriendValueTaskAsync());
			Assert.Same(Exception, exception);
			Assert.Same(SecondException, secondException);
		}

		[Fact]
		public async Task SetupSequence__property_of__completed_Task__Returns()
		{
			var person = new Mock<IPerson>();
			person.SetupSequence(p => p.GetFriendTaskAsync().Result.Name).Returns(NameOfFriend).Returns(SecondNameOfFriend);
			var friend = await person.Object.GetFriendTaskAsync();
			Assert.Equal(NameOfFriend, friend.Name);
			Assert.Equal(SecondNameOfFriend, friend.Name);
		}

		[Fact]
		public async Task SetupSequence__property_of__completed_Task__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupSequence(p => p.GetFriendTaskAsync().Result.Name).Throws(Exception).Throws(SecondException);
			var friend = await person.Object.GetFriendTaskAsync();
			var exception = Assert.Throws<Exception>(() => friend.Name);
			var secondException = Assert.Throws<Exception>(() => friend.Name);
			Assert.Same(Exception, exception);
			Assert.Same(SecondException, secondException);
		}

		[Fact]
		public async Task SetupSequence__property_of__completed_Task_from_func__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupSequence(p => p.GetFriendTaskAsync().Result.Name).Throws(() => Exception).Throws(() => SecondException);
			var friend = await person.Object.GetFriendTaskAsync();
			var exception = Assert.Throws<Exception>(() => friend.Name);
			var secondException = Assert.Throws<Exception>(() => friend.Name);
			Assert.Same(Exception, exception);
			Assert.Same(SecondException, secondException);
		}

		[Fact]
		public async Task SetupSequence__property_of__completed_ValueTask__Returns()
		{
			var person = new Mock<IPerson>();
			person.SetupSequence(p => p.GetFriendValueTaskAsync().Result.Name).Returns(NameOfFriend).Returns(SecondNameOfFriend);
			var friend = await person.Object.GetFriendValueTaskAsync();
			Assert.Equal(NameOfFriend, friend.Name);
			Assert.Equal(SecondNameOfFriend, friend.Name);
		}

		[Fact]
		public async Task SetupSequence__property_of__completed_ValueTask__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupSequence(p => p.GetFriendValueTaskAsync().Result.Name).Throws(Exception).Throws(SecondException);
			var friend = await person.Object.GetFriendValueTaskAsync();
			var exception = Assert.Throws<Exception>(() => friend.Name);
			var secondException = Assert.Throws<Exception>(() => friend.Name);
			Assert.Same(Exception, exception);
			Assert.Same(SecondException, secondException);
		}

		[Fact]
		public async Task SetupSequence__property_of__completed_ValueTask_from_func__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupSequence(p => p.GetFriendValueTaskAsync().Result.Name).Throws(() => Exception).Throws(() => SecondException);
			var friend = await person.Object.GetFriendValueTaskAsync();
			var exception = Assert.Throws<Exception>(() => friend.Name);
			var secondException = Assert.Throws<Exception>(() => friend.Name);
			Assert.Same(Exception, exception);
			Assert.Same(SecondException, secondException);
		}

		[Fact]
		public async Task SetupSet__property_of__completed_Task__Callback()
		{
			string setToValue = null;
			var person = new Mock<IPerson>();
			person.SetupSet(p => p.GetFriendTaskAsync().Result.Name = It.IsAny<string>()).Callback((string value) => setToValue = value);
			var friend = await person.Object.GetFriendTaskAsync();
			Assert.Null(setToValue);
			friend.Name = NameOfFriend;
			Assert.Equal(NameOfFriend, setToValue);
		}

		[Fact]
		public async Task SetupSet__property_of__completed_ValueTask__Callback()
		{
			string setToValue = null;
			var person = new Mock<IPerson>();
			person.SetupSet(p => p.GetFriendValueTaskAsync().Result.Name = It.IsAny<string>()).Callback((string value) => setToValue = value);
			var friend = await person.Object.GetFriendValueTaskAsync();
			Assert.Null(setToValue);
			friend.Name = NameOfFriend;
			Assert.Equal(NameOfFriend, setToValue);
		}

		[Fact]
		public async Task SetupSet__property_of__completed_Task__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupSet(p => p.GetFriendTaskAsync().Result.Name = It.IsAny<string>()).Throws(Exception);
			var friend = await person.Object.GetFriendTaskAsync();
			var exception = Assert.Throws<Exception>(() => friend.Name = NameOfFriend);
			Assert.Same(Exception, exception);
		}

		[Fact]
		public async Task SetupSet__property_of__completed_Task_from_func__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupSet(p => p.GetFriendTaskAsync().Result.Name = It.IsAny<string>()).Throws(() => Exception);
			var friend = await person.Object.GetFriendTaskAsync();
			var exception = Assert.Throws<Exception>(() => friend.Name = NameOfFriend);
			Assert.Same(Exception, exception);
		}

		[Fact]
		public async Task SetupSet__property_of__completed_ValueTask__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupSet(p => p.GetFriendValueTaskAsync().Result.Name = It.IsAny<string>()).Throws(Exception);
			var friend = await person.Object.GetFriendValueTaskAsync();
			var exception = Assert.Throws<Exception>(() => friend.Name = NameOfFriend);
			Assert.Same(Exception, exception);
		}

		[Fact]
		public async Task SetupSet__property_of__completed_ValueTask_from_func__Throws()
		{
			var person = new Mock<IPerson>();
			person.SetupSet(p => p.GetFriendValueTaskAsync().Result.Name = It.IsAny<string>()).Throws(() => Exception);
			var friend = await person.Object.GetFriendValueTaskAsync();
			var exception = Assert.Throws<Exception>(() => friend.Name = NameOfFriend);
			Assert.Same(Exception, exception);
		}
	}
}
