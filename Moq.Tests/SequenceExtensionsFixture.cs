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
				.Throws<InvalidOperationException>();

			Assert.Equal(2, mock.Object.Do());
			Assert.Equal(3, mock.Object.Do());
			Assert.Throws<InvalidOperationException>(() => mock.Object.Do());
		}

		[Fact]
		public void PerformSequenceAsync()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.DoAsync())
				.ReturnsAsync(2)
				.ReturnsAsync(3)
				.ThrowsAsync(new InvalidOperationException());

			Assert.Equal(2, mock.Object.DoAsync().Result);
			Assert.Equal(3, mock.Object.DoAsync().Result);

			try
			{
				var x = mock.Object.DoAsync().Result;
			}
			catch (AggregateException ex)
			{
				Assert.IsType<InvalidOperationException>(ex.GetBaseException());
			}
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
		public void When_sequence_exhausted_and_there_was_a_previous_setup_return_value_is_determined_by_that_one()
		{
			const string valueFromPreviousSetup = "value from previous setup";

			// Arrange: set up a sequence as the second setup and consume it
			var mock = new Mock<IFoo>();
			mock.Setup(m => m.Value).Returns(valueFromPreviousSetup);
			mock.SetupSequence(m => m.Value).Returns("1");
			var _ = mock.Object.Value;

			// Act: ask sequence for value when it is exhausted
			var actual = mock.Object.Value;

			// Assert: should have got the value configured by the previous setup
			Assert.Equal(valueFromPreviousSetup, actual);
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

		public interface IFoo
		{
			string Value { get; set; }

			int Do();

			Task<int> DoAsync();
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
