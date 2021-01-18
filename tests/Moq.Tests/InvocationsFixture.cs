// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
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

			var expectedArguments = new[] { obj };

			Assert.Equal(expectedArguments, invocation.Arguments);
		}

		[Fact]
		public void MockInvocationsIncludeReturnValue_NoSetup()
		{
			var mock = new Mock<IComparable>();

			var obj = new object();

			mock.Object.CompareTo(obj);

			var invocation = mock.Invocations[0];

			Assert.Equal(0, invocation.ReturnValue);
			Assert.Null(invocation.Exception);
		}

		[Fact]
		public void MockInvocationsIncludeReturnValue_Setup()
		{
			var mock = new Mock<IComparable>();
			var obj = new object();
			mock.Setup(c => c.CompareTo(obj)).Returns(42);

			mock.Object.CompareTo(obj);

			var invocation = mock.Invocations[0];

			Assert.Equal(42, invocation.ReturnValue);
			Assert.Null(invocation.Exception);
		}

		[Fact]
		public void MockInvocationsIncludeReturnValue_BaseCall()
		{
			var mock = new Mock<Random>(1) // seed: 1
			{
				CallBase = true,
			};

			mock.Object.Next();

			var invocation = mock.Invocations[0];

			Assert.Equal(new Random(Seed: 1).Next(), invocation.ReturnValue);
			Assert.Null(invocation.Exception);
		}

		[Fact]
		public void MockInvocationsIncludeReturnValue_ReturnsException()
		{
			var mock = new Mock<ICloneable>();
			var returnValue = new Exception();
			mock.Setup(c => c.Clone()).Returns(returnValue);

			mock.Object.Clone();

			var invocation = mock.Invocations[0];

			Assert.Equal(returnValue, invocation.ReturnValue);
			Assert.Null(invocation.Exception);
		}

		[Fact]
		public void MockInvocationsIncludeException_Setup()
		{
			var mock = new Mock<IComparable>();
			var exception = new Exception("Message");
			mock.Setup(c => c.CompareTo(It.IsAny<object>())).Throws(exception);

			var thrown = Assert.Throws<Exception>(() => mock.Object.CompareTo(null));

			Assert.Equal(exception.Message, thrown.Message);

			var invocation = mock.Invocations[0];
			Assert.Same(thrown, invocation.Exception);
		}

		[Fact]
		public void MockInvocationsIncludeException_BaseCall_Virtual()
		{
			var mock = new Mock<Test>()
			{
				CallBase = true,
			};

			var thrown = Assert.Throws<InvalidOperationException>(() => mock.Object.ThrowingVirtualMethod());

			Assert.Equal("Message", thrown.Message);

			var invocation = mock.Invocations[0];
			Assert.Same(thrown, invocation.Exception);
		}

		[Fact]
		public void MockInvocationsIncludeException_MockException()
		{
			var mock = new Mock<ICloneable>(MockBehavior.Strict);

			var thrown = Assert.Throws<MockException>(() => mock.Object.Clone());

			Assert.Equal(MockExceptionReasons.NoSetup, thrown.Reasons);

			var invocation = mock.Invocations[0];
			Assert.Same(thrown, invocation.Exception);
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
		public void MatchingSetup_is_null_if_there_was_no_matching_setup()
		{
			var mock = new Mock<IX>();

			mock.Object.Do();
			var invocation = mock.Invocations.First();

			Assert.Null(invocation.MatchingSetup);
		}

		[Fact]
		public void MatchingSetup_is_set_if_there_was_a_matching_setup()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Do());
			var setup = mock.Setups.First();

			mock.Object.Do();
			var invocation = mock.Invocations.First();

			Assert.Same(setup, invocation.MatchingSetup);
		}

		[Fact]
		public void MatchingSetup_is_set_if_there_was_a_matching_setup_implicitly_created_by_SetupAllProperties()
		{
			var mock = new Mock<IX>();
			mock.SetupAllProperties();

			_ = mock.Object.Nested;
			var invocation = mock.Invocations.First();
			var setup = mock.Setups.First();

			Assert.Same(setup, invocation.MatchingSetup);
		}

		[Fact]
		public void MatchingSetup_is_set_if_there_was_a_matching_setup_implicitly_created_by_multi_dot_expression()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Nested.Do());
			var setup = mock.Setups.First();

			_ = mock.Object.Nested;
			var invocation = mock.Invocations.First();

			Assert.Same(setup, invocation.MatchingSetup);
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

		public class Test
		{
			public virtual int ThrowingVirtualMethod() => throw new InvalidOperationException("Message");
		}
	}
}
