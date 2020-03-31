// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq;

using Xunit;

namespace Moq.Tests
{
	public class InvocationsFixture
	{
		[Fact]
		public void MockInvocationsAreRecorded()
		{
			var mock = new Mock<IComparable>();

			mock.Object.CompareTo(new object());

			Assert.Equal(1, mock.Invocations.Count);
		}

		[Fact]
		public void MockInvocationsIncludeInvokedMethod()
		{
			var mock = new Mock<IComparable>();

			mock.Object.CompareTo(new object());

			var invocation = mock.Invocations[0];

			var expectedMethod = typeof(IComparable).GetMethod(nameof(mock.Object.CompareTo));

			Assert.Equal(expectedMethod, invocation.Method);
		}

		[Fact]
		public void MockInvocationsIncludeArguments()
		{
			var mock = new Mock<IComparable>();

			var obj = new object();

			mock.Object.CompareTo(obj);

			var invocation = mock.Invocations[0];

			var expectedArguments = new[] {obj};

			Assert.Equal(expectedArguments, invocation.Arguments);
		}

		[Fact]
		public void MockInvocationsCanBeEnumerated()
		{
			var mock = new Mock<IComparable>();

			mock.Object.CompareTo(-1);
			mock.Object.CompareTo(0);
			mock.Object.CompareTo(1);

			var count = 0;

			using (var enumerator = mock.Invocations.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Assert.NotNull(enumerator.Current);

					count++;
				}
			}

			Assert.Equal(3, count);
		}

		[Fact]
		public void MockInvocationsCanBeCleared()
		{
			var mock = new Mock<IComparable>();

			mock.Object.CompareTo(new object());

			mock.Invocations.Clear();

			Assert.Equal(0, mock.Invocations.Count);
		}

		[Fact]
		public void MockInvocationsCanBeRetrievedByIndex()
		{
			var mock = new Mock<IComparable>();

			mock.Object.CompareTo(-1);
			mock.Object.CompareTo(0);
			mock.Object.CompareTo(1);

			var invocation = mock.Invocations[1];

			var arg = invocation.Arguments[0];

			Assert.Equal(0, arg);
		}

		[Fact]
		public void MockInvocationsIndexerThrowsIndexOutOfRangeWhenCollectionIsEmpty()
		{
			var mock = new Mock<IComparable>();

			Assert.Throws<IndexOutOfRangeException>(() => mock.Invocations[0]);
		}

		[Fact]
		public void Invocations_record_return_value()
		{
			var mock = new Mock<IComparable>();
			mock.Setup(m => m.CompareTo(It.IsAny<object>())).Returns(42);
			_ = mock.Object.CompareTo(default);
			var invocation = mock.MutableInvocations.ToArray()[0];
			Assert.Equal(42, invocation.ReturnValue);
		}

		[Fact]
		public void Invocations_for_object_methods_on_interface_proxy_record_return_value()
		{
			var mock = new Mock<IComparable>();
			mock.Setup(m => m.GetHashCode()).Returns(42);
			_ = mock.Object.GetHashCode();
			var invocation = mock.MutableInvocations.ToArray()[0];
			Assert.Equal(42, invocation.ReturnValue);
		}

		[Fact]
		public void Invocations_Clear_also_resets_setup_verification_state_of_regular_setups()
		{
			var mock = new Mock<IComparable>();
			mock.Setup(m => m.CompareTo(default));
			_ = mock.Object.CompareTo(default);
			mock.VerifyAll();  // ensure setup has been matched

			mock.Invocations.Clear();
			var ex = Assert.Throws<MockException>(() => mock.VerifyAll());
			Assert.Equal(MockExceptionReasons.UnmatchedSetup, ex.Reasons);
		}

		[Fact]
		public void Invocations_Clear_also_resets_setup_verification_state_of_sequence_setups()
		{
			var mock = new Mock<IComparable>();
			mock.SetupSequence(m => m.CompareTo(default));
			_ = mock.Object.CompareTo(default);
			mock.VerifyAll();  // ensure setup has been matched

			mock.Invocations.Clear();
			var ex = Assert.Throws<MockException>(() => mock.VerifyAll());
			Assert.Equal(MockExceptionReasons.UnmatchedSetup, ex.Reasons);
		}

		[Fact]
		public void Invocations_Clear_also_resets_setup_verification_state_of_inner_mock_setups()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Nested.Do());
			mock.Object.Nested.Do();
			mock.VerifyAll();  // ensure setup has been matched

			mock.Invocations.Clear();
			var ex = Assert.Throws<MockException>(() => mock.VerifyAll());
			Assert.Equal(MockExceptionReasons.UnmatchedSetup, ex.Reasons);
		}

		[Fact]
		[Obsolete()]
		public void Invocations_Clear_resets_count_kept_by_setup_AtMost()
		{
			var mock = new Mock<IComparable>();
			mock.Setup(m => m.CompareTo(default)).Returns(0).AtMostOnce();
			_ = mock.Object.CompareTo(default);

			mock.Invocations.Clear();
			_ = mock.Object.CompareTo(default);  // this second call should now count as the first
			var ex = Assert.Throws<MockException>(() => mock.Object.CompareTo(default));
			Assert.Equal(MockExceptionReasons.MoreThanOneCall, ex.Reasons);
		}

		[Fact]
		public void New_Mock_should_keep_record_of_invocations_caused_by_mocked_type_ctor()
		{
			var mock = new Mock<FlagInitiallySetToTrue>();
			mock.Setup(m => m.Flag).Returns(false);
			var mockObject = mock.Object;

			Assert.Single(mock.Invocations);
			Assert.False(mockObject.Flag);
		}

		[Fact]
		public void Mock_Of_should_keep_record_of_invocations_caused_by_mocked_type_ctor()
		{
			var mockObject = Mock.Of<FlagInitiallySetToTrue>(m => m.Flag == false);
			var mock = Mock.Get(mockObject);

			Assert.Single(mock.Invocations);
			Assert.False(mockObject.Flag);
		}

		[Fact]
		public void WasMatched_returns_false_if_there_was_no_matching_setup()
		{
			var mock = new Mock<IX>();

			mock.Object.Do();
			var invocation = mock.Invocations.First();

			Assert.False(invocation.WasMatched(out var matchingSetup));
			Assert.Null(matchingSetup);
		}

		[Fact]
		public void WasMatched_returns_true_if_there_was_a_matching_setup()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Do());
			var setup = mock.Setups.First();

			mock.Object.Do();
			var invocation = mock.Invocations.First();

			Assert.True(invocation.WasMatched(out var matchingSetup));
			Assert.Same(setup, matchingSetup);
		}

		public interface IX
		{
			IX Nested { get; }
			void Do();
		}

		public class FlagInitiallySetToTrue
		{
			public FlagInitiallySetToTrue()
			{
				this.Flag = true;
			}

			public virtual bool Flag { get; set; }
		}
	}
}
