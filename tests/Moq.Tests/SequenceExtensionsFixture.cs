// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Threading.Tasks;

using Xunit;

namespace Moq.Tests
{
	public class SequenceExtensionsFixture
	{
		[Fact]
		public void PerformSequence()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.Do())
				.Returns(2)
				.Returns(3)
				.Returns(() => 4)
				.Throws<InvalidOperationException>();

			Assert.Equal(2, mock.Object.Do());
			Assert.Equal(3, mock.Object.Do());
			Assert.Equal(4, mock.Object.Do());
			Assert.Throws<InvalidOperationException>(() => mock.Object.Do());
		}

		[Fact]
		public async Task PerformSequenceAsync()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.DoAsync())
				.ReturnsAsync(2)
				.ReturnsAsync(3)
				.ThrowsAsync(new InvalidOperationException());

			Assert.Equal(2, mock.Object.DoAsync().Result);
			Assert.Equal(3, mock.Object.DoAsync().Result);
			await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.Object.DoAsync());
		}

		[Fact]
		public async Task PerformSequenceValueTaskAsync()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.DoValueAsync())
				.ReturnsAsync(2)
				.ReturnsAsync(() => 3)
				.ThrowsAsync(new InvalidOperationException());

			Assert.Equal(2, mock.Object.DoValueAsync().Result);
			Assert.Equal(3, mock.Object.DoValueAsync().Result);
			await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.Object.DoValueAsync());
		}

		[Fact]
		public async Task PerformSequenceVoidAsync()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.DoVoidAsync())
				.PassAsync()
				.PassAsync()
				.ThrowsAsync(new InvalidOperationException());

			await mock.Object.DoVoidAsync();
			await mock.Object.DoVoidAsync();
			await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.Object.DoVoidAsync());
		}

		[Fact]
		public async Task PerformSequenceValueTaskVoidAsync()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.DoValueVoidAsync())
				.PassAsync()
				.PassAsync()
				.ThrowsAsync(new InvalidOperationException());

			await mock.Object.DoValueVoidAsync();
			await mock.Object.DoValueVoidAsync();
			await Assert.ThrowsAsync<InvalidOperationException>(async () => await mock.Object.DoValueVoidAsync());
		}

		[Fact]
		public async Task PerformSequenceAsync_ReturnsAsync_for_Task_with_value_function()
		{
			var mock = new Mock<IFoo>();
			mock.SetupSequence(m => m.DoAsync())
				.ReturnsAsync(() => 1)
				.ReturnsAsync(() => 2);

			Assert.Equal(1, await mock.Object.DoAsync());
			Assert.Equal(2, await mock.Object.DoAsync());
		}

		[Fact]
		public async Task PerformSequenceAsync_ReturnsAsync_for_ValueTask_with_value_function()
		{
			var mock = new Mock<IFoo>();
			mock.SetupSequence(m => m.DoValueAsync())
				.ReturnsAsync(() => 1)
				.ReturnsAsync(() => 2);

			Assert.Equal(1, await mock.Object.DoValueAsync());
			Assert.Equal(2, await mock.Object.DoValueAsync());
		}

		[Fact]
		public void PerformSequenceOnProperty()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.Value)
				.Returns("foo")
				.Returns("bar")
				.Throws<InvalidOperationException>();

			string temp;
			Assert.Equal("foo", mock.Object.Value);
			Assert.Equal("bar", mock.Object.Value);
			Assert.Throws<InvalidOperationException>(() => temp = mock.Object.Value);
		}

		[Fact]
		public void PerformSequenceWithThrowsFirst()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.Do())
				.Throws<Exception>()
				.Returns(1);

			Assert.Throws<Exception>(() => mock.Object.Do());
			Assert.Equal(1, mock.Object.Do());
		}

		[Fact]
		public void PerformSequenceWithCallBase()
		{
			var mock = new Mock<Foo>();

			mock.SetupSequence(x => x.Do())
				.Returns("Good")
				.CallBase()
				.Throws<InvalidOperationException>();

			Assert.Equal("Good", mock.Object.Do());
			Assert.Equal("Ok", mock.Object.Do());
			Assert.Throws<InvalidOperationException>(() => mock.Object.Do());
		}

		[Fact]
		public void Setting_up_a_sequence_overrides_any_preexisting_setup()
		{
			const string valueFromPreviousSetup = "value from previous setup";

			// Arrange: set up a sequence as the second setup and consume it
			var mock = new Mock<IFoo>();
			mock.Setup(m => m.Value).Returns(valueFromPreviousSetup);
			mock.SetupSequence(m => m.Value).Returns("1");
			var _ = mock.Object.Value;

			// Act: ask sequence for value when it is exhausted
			var actual = mock.Object.Value;

			// Assert: should have got the default value, not the one configured by the overridden setup
			Assert.Equal(default(string), actual);
			Assert.NotEqual(valueFromPreviousSetup, actual);
		}

		[Fact]
		public void When_sequence_exhausted_and_there_was_no_previous_setup_return_value_is_default()
		{
			// Arrange: set up a sequence as the only setup and consume it
			var mock = new Mock<IFoo>();
			mock.SetupSequence(m => m.Value)
				.Returns("1");
			var _ = mock.Object.Value;

			// Act: ask sequence for value when it is exhausted
			string actual = mock.Object.Value;

			// Assert: should have got the default value
			Assert.Equal(default(string), actual);
		}

		[Fact]
		public void When_sequence_overexhausted_and_new_responses_are_configured_those_are_used_on_next_invocation()
		{
			// Arrange: set up a sequence and overexhaust it, then set up more responses
			var mock = new Mock<IFoo>();
			var sequenceSetup = mock.SetupSequence(m => m.Value).Returns("1"); // configure 1st response
			var _ = mock.Object.Value; // 1st invocation
			_ = mock.Object.Value; // 2nd invocation
			sequenceSetup.Returns("2"); // configure 2nd response
			sequenceSetup.Returns("3"); // configure 3nd response

			// Act: 3rd invocation. will we get back the 2nd configured response, or the 3rd (since we're on the 3rd invocation)?
			string actual = mock.Object.Value;

			// Assert: no configured response should be skipped, therefore we should get the 2nd one
			Assert.Equal("2", actual);
		}

		[Fact]
		public void Verify_can_verify_invocation_count_for_sequences()
		{
			var mock = new Mock<IFoo>();
			mock.SetupSequence(m => m.Do());

			mock.Object.Do();
			mock.Object.Do();

			mock.Verify(m => m.Do(), Times.Exactly(2));
		}

		[Fact]
		public void Func_are_invoked_deferred()
		{
			var mock = new Mock<IFoo>();
			var i = 0;
			mock.SetupSequence(m => m.Do())
				.Returns(() => i);

			i++;

			Assert.Equal(i, mock.Object.Do());
		}

		[Fact]
		public async Task Func_are_invoked_deferred_for_Task()
		{
			var mock = new Mock<IFoo>();
			var i = 0;
			mock.SetupSequence(m => m.DoAsync())
				.ReturnsAsync(() => i);

			i++;

			Assert.Equal(i, await mock.Object.DoAsync());
		}

		[Fact]
		public async Task Func_are_invoked_deferred_for_ValueTask()
		{
			var mock = new Mock<IFoo>();
			var i = 0;
			mock.SetupSequence(m => m.DoValueAsync())
				.ReturnsAsync(() => i);

			i++;

			Assert.Equal(i, await mock.Object.DoValueAsync());
		}

		[Fact]
		public void Func_can_be_treated_as_return_value()
		{
			var mock = new Mock<IFoo>();
			Func<int> func = () => 1;
			mock.SetupSequence(m => m.GetFunc())
				.Returns(func);

			Assert.Equal(func, mock.Object.GetFunc());
		}

		[Fact]
		public void Keep_Func_as_return_value_when_setup_method_returns_implicitly_casted_type()
		{
			var mock = new Mock<IFoo>();
			Func<object> funcObj = () => 1;
			Func<Delegate> funcDel = () => new Action(() => { });
			Func<MulticastDelegate> funcMulticastDel = () => new Action(() => { });
			mock.SetupSequence(m => m.GetObj()).Returns(funcObj);
			mock.SetupSequence(m => m.GetDel()).Returns(funcDel);
			mock.SetupSequence(m => m.GetMulticastDel()).Returns(funcMulticastDel);

			Assert.Equal(funcObj, mock.Object.GetObj());
			Assert.Equal(funcDel, mock.Object.GetDel());
			Assert.Equal(funcMulticastDel, mock.Object.GetMulticastDel());
		}

		public interface IFoo
		{
			string Value { get; set; }

			int Do();

			Task<int> DoAsync();

			ValueTask<int> DoValueAsync();

			Task DoVoidAsync();

			ValueTask DoValueVoidAsync();

			Func<int> GetFunc();

			object GetObj();

			Delegate GetDel();

			MulticastDelegate GetMulticastDel();
		}

		public class Foo
		{
			public virtual string Do()
			{
				return "Ok";
			}

			public virtual Task<string> DoAsync()
			{
				var tcs = new TaskCompletionSource<string>();
				tcs.SetResult("Ok");
				return tcs.Task;
			}
		}
	}
}
