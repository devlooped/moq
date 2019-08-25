// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class ItIsAnyTypeFixture
	{
		[Fact]
		public void Setup_without_It_IsAnyType()
		{
			var mock = new Mock<IX>();
			mock.Setup(x => x.Method<bool>());
			mock.Setup(x => x.Method<int>());
			mock.Setup(x => x.Method<object>());
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

		public interface IX
		{
			void Method<T>();
		}

		public interface IY
		{
			T Method<T>(T arg);
		}

		public interface IZ
		{
			T Method<T>();
		}
	}
}
