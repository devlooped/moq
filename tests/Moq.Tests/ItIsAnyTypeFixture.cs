// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq;

using Xunit;

namespace Moq.Tests
{
	public class ItIsAnyTypeFixture
	{
		[Fact]
		public void Setup_without_It_IsAnyType()
		{
			var mock = new Mock<IX>();
			mock.Setup(x => x.Method<object>());
			mock.Setup(x => x.Method<bool>());
			mock.Setup(x => x.Method<int>());
			mock.Setup(x => x.Method<string>());

			mock.Object.Method<bool>();
			mock.Object.Method<int>();
			mock.Object.Method<object>();
			mock.Object.Method<string>();

			mock.VerifyAll();
		}

		[Fact]
		public void Setup_with_It_IsAnyType()
		{
			var invocationCount = 0;
			var mock = new Mock<IX>();
			mock.Setup(x => x.Method<It.IsAnyType>()).Callback(() => invocationCount++);

			mock.Object.Method<bool>();
			mock.Object.Method<int>();
			mock.Object.Method<object>();
			mock.Object.Method<string>();

			Assert.Equal(4, invocationCount);
		}

		[Fact]
		public void Verify_with_It_IsAnyType()
		{
			var mock = new Mock<IX>();

			mock.Object.Method<bool>();
			mock.Object.Method<int>();
			mock.Object.Method<object>();
			mock.Object.Method<string>();

			mock.Verify(x => x.Method<It.IsAnyType>(), Times.Exactly(4));
		}

		[Fact]
		public void Setup_with_It_IsAnyType_and_Callback()
		{
			object received = null;
			var mock = new Mock<IY>();
			mock.Setup(m => m.Method<It.IsAnyType>((It.IsAnyType)It.IsAny<object>()))
				.Callback((object arg) => received = arg);

			_ = mock.Object.Method<int>(42);

			Assert.Equal(42, received);
		}

		[Fact]
		public void Setup_with_It_IsAnyType_and_Returns()
		{
			var mock = new Mock<IY>();
			mock.Setup(m => m.Method<It.IsAnyType>((It.IsAnyType)It.IsAny<object>()))
			    .Returns(new Func<object, object>(arg => arg));

			Assert.Equal(42, mock.Object.Method<int>(42));
			Assert.Equal("42", mock.Object.Method<string>("42"));
		}

		[Fact]
		public void Setup_with_It_IsAnyType_default_return_value()
		{
			var mock = new Mock<IY>() { DefaultValue = DefaultValue.Empty };
			mock.Setup(m => m.Method<It.IsAnyType>((It.IsAnyType)It.IsAny<object>()));

			var result = mock.Object.Method<int[]>(null);

			// Let's make sure that default value providers don't suddenly start producing `It.IsAnyType` instances:
			Assert.IsNotType<It.IsAnyType>(result);

			// Rather, we expect the usual behavior:
			Assert.NotNull(result);
			Assert.IsType<int[]>(result);
			Assert.Empty((int[])result);
		}

		[Fact]
		public void Type_arguments_can_be_discovered_in_Callback_through_a_InvocationAction_callback()
		{
			Type typeArgument = null;
			var mock = new Mock<IZ>();
			mock.Setup(z => z.Method<It.IsAnyType>()).Callback(new InvocationAction(invocation =>
			{
				typeArgument = invocation.Method.GetGenericArguments()[0];
			}));

			_ = mock.Object.Method<string>();

			Assert.Equal(typeof(string), typeArgument);
		}

		[Fact]
		public void Type_arguments_can_be_discovered_in_Returns_through_a_InvocationFunc_callback()
		{
			var mock = new Mock<IZ>();
			mock.Setup(z => z.Method<It.IsAnyType>()).Returns(new InvocationFunc(invocation =>
			{
				var typeArgument = invocation.Method.GetGenericArguments()[0];
				return Activator.CreateInstance(typeArgument);
			}));

			var result = mock.Object.Method<DateTime>();

			Assert.IsType<DateTime>(result);
			Assert.Equal(default(DateTime), result);
		}

		[Fact]
		public void Setup_with_It_IsAny_It_IsAnyType()
		{
			object received = null;
			var mock = new Mock<IY>();
			mock.Setup(m => m.Method(It.IsAny<It.IsAnyType>()))
			    .Callback((object arg) => received = arg);

			_ = mock.Object.Method<int>(42);
			Assert.Equal(42, received);

			_ = mock.Object.Method<string>("42");
			Assert.Equal("42", received);
		}

		[Fact]
		public void Setup_with_It_IsNotNull_It_IsAnyType()
		{
			var invocationCount = 0;
			var mock = new Mock<IY>();
			mock.Setup(m => m.Method(It.IsNotNull<It.IsAnyType>()))
				.Callback((object arg) => invocationCount++);

			_ = mock.Object.Method<string>("42");
			Assert.Equal(1, invocationCount);

			_ = mock.Object.Method<string>(null);
			Assert.Equal(1, invocationCount);

			_ = mock.Object.Method<int>(42);
			Assert.Equal(2, invocationCount);

			_ = mock.Object.Method<int?>(42);
			Assert.Equal(3, invocationCount);

			_ = mock.Object.Method<int?>(null);
			Assert.Equal(3, invocationCount);
		}

		[Fact]
		public void Setup_with_It_Is_It_IsAnyType()
		{
			var acceptableArgs = new object[] { 42, "42" };

			var invocationCount = 0;
			var mock = new Mock<IY>();
			mock.Setup(m => m.Method(It.Is<It.IsAnyType>((x, _) => acceptableArgs.Contains(x))))
			    .Callback((object arg) => invocationCount++);

			_ = mock.Object.Method<string>("42");
			Assert.Equal(1, invocationCount);

			_ = mock.Object.Method<string>("7");
			Assert.Equal(1, invocationCount);

			_ = mock.Object.Method<int>(42);
			Assert.Equal(2, invocationCount);

			_ = mock.Object.Method<int>(7);
			Assert.Equal(2, invocationCount);
		}

		[Fact]
		public void It_Is_It_IsAnyType_will_throw_when_predicate_uninvokable()
		{
			Action useMatcher = () => _ = It.Is<It.IsAnyType>(arg => true);
			//                                                ^^^
			// When used like this, the predicate will have static type `It.IsAnyType`,
			// and no *actual* argument will ever be of that type; therefore, we expect
			// Moq to mark this as illegal use.

			Assert.Throws<ArgumentException>(useMatcher);
		}

		[Fact]
		public void Type_matcher_should_be_disallowed_in_Callback()
		{
			var mock = new Mock<IY>();
			var setup = mock.Setup(m => m.Method(It.IsAny<It.IsAnyType>()));

			Assert.Throws<ArgumentException>(() => setup.Callback<It.IsAnyType>(arg => { }));
			//                                                                  ^^^
			// Similar to the above test, no actual argument will ever have type `It.IsAnyType`;
			// Moq should mark this as illegal use, and one should be using `object` instead.

			_ = mock.Object.Method(true);
			//                     ^^^^
			// this would cause an `ArgumentException`: "... cannot be converted to type 'It.IsAnyType'"
		}

		[Fact]
		public void Type_matcher_should_be_disallowed_in_Returns()
		{
			var mock = new Mock<IY>();
			var setup = mock.Setup(m => m.Method(It.IsAny<It.IsAnyType>()));

			Assert.Throws<ArgumentException>(() => setup.Returns<It.IsAnyType>(arg => arg));
			//                                                                 ^^^
			// Similar to the above test, no actual argument will ever have type `It.IsAnyType`;
			// Moq should mark this as illegal use, and one should be using `object` instead.

			_ = mock.Object.Method(true);
			//                     ^^^^
			// this would cause an `ArgumentException`: "... cannot be converted to type 'It.IsAnyType'"
		}

		[Fact]
		public void Setup_with_It_Ref_It_IsAnyType_IsAny()
		{
			object received = null;
			var mock = new Mock<IY>();
			mock.Setup(m => m.ByRefMethod(ref It.Ref<It.IsAnyType>.IsAny))
			    .Callback(new ByRefMethodCallback<object>((ref object arg) => received = arg));

			var i = 42;
			_ = mock.Object.ByRefMethod<int>(ref i);
			Assert.Equal(42, received);

			var s = "42";
			_ = mock.Object.ByRefMethod<string>(ref s);
			Assert.Equal("42", received);
		}

		public interface IX
		{
			void Method<T>();
		}

		public interface IY
		{
			T Method<T>(T arg);
			T ByRefMethod<T>(ref T arg);
		}

		public delegate void ByRefMethodCallback<T>(ref T arg);

		public interface IZ
		{
			T Method<T>();
		}
	}
}
