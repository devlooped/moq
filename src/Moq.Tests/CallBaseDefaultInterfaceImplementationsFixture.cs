// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

#if FEATURE_DEFAULT_INTERFACE_IMPLEMENTATIONS

using System;

using Xunit;

namespace Moq.Tests
{
	public class CallBaseDefaultInterfaceImplementationsFixture
	{
		[Fact]
		public void CallBase__configured_on_mock__succeeds()
		{
			var mock = new Mock<IX>() { CallBase = true };
			mock.Object.Inert();
		}

		[Fact]
		public void CallBase__configured_on_setup__succeeds()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Inert()).CallBase();
			mock.Object.Inert();
		}

		[Fact]
		public void Reference_typed_return_value__is_returned()
		{
			var mock = new Mock<IX>() { CallBase = true };
			var returnValue = mock.Object.ReturnsHello();
			Assert.Equal("Hello", returnValue);
		}

		[Fact]
		public void Value_typed_return_value__is_returned()
		{
			var mock = new Mock<IX>() { CallBase = true };
			var returnValue = mock.Object.ReturnsFortyTwo();
			Assert.Equal(42, returnValue);
		}

		[Fact]
		public void Can_deal_with__value_typed_argument()
		{
			var mock = new Mock<IX>() { CallBase = true };
			var returnValue = mock.Object.ReturnsIntArg(42);
			Assert.Equal(42, returnValue);
		}

		[Fact]
		public void Can_deal_with__reference_typed_argument()
		{
			var mock = new Mock<IX>() { CallBase = true };
			var returnValue = mock.Object.ReturnsStringArg("Echo");
			Assert.Equal("Echo", returnValue);
		}

		[Fact]
		public void Can_deal_with__generic_argument()
		{
			var mock = new Mock<IX>() { CallBase = true };
			var returnValue = mock.Object.ReturnsArg<bool>(true);
			Assert.True(returnValue);
		}

		[Fact]
		public void Can_deal_with__value_typed_by_ref_argument()
		{
			var mock = new Mock<IX>() { CallBase = true };
			var arg = 42;
			var returnValue = mock.Object.Increment(ref arg);
			Assert.Equal(42, returnValue);
			Assert.Equal(43, arg);
		}

		[Fact]
		public void Can_deal_with__reference_typed_by_ref_argument()
		{
			var mock = new Mock<IX>() { CallBase = true };
			var arg = "C+";
			var returnValue = mock.Object.AppendPlus(ref arg);
			Assert.Equal("C+", returnValue);
			Assert.Equal("C++", arg);
		}

		[Fact]
		public void Does_not_lose_modified_arguments__when_exception_thrown()
		{
			var mock = new Mock<IX>() { CallBase = true };
			var arg = "C+";
			Assert.Throws<ArithmeticException>(() => mock.Object.AppendPlusThenThrow(ref arg));
			Assert.Equal("C++", arg);
		}

		[Fact]
		public void Should_call__most_specific__most_derived__implementation()
		{
			var mock = new Mock<IZ>() { CallBase = true };
			Assert.Equal(nameof(IZ), mock.Object.WhoAmI());
		}

		public interface IX
		{
			void Inert()
			{
			}

			int ReturnsFortyTwo() => 42;
			string ReturnsHello() => "Hello";

			int ReturnsIntArg(int arg) => arg;
			string ReturnsStringArg(string arg) => arg;
			T ReturnsArg<T>(T arg) => arg;

			int Increment(ref int arg) => arg++;
			string AppendPlus(ref string arg)
			{
				var argBefore = arg;
				arg += "+";
				return argBefore;
			}

			string AppendPlusThenThrow(ref string arg)
			{
				arg += "+";
				throw new ArithmeticException();
			}
		}

		public interface IY
		{
			string WhoAmI() => nameof(IY);
		}

		public interface IZ : IY
		{
			string IY.WhoAmI() => nameof(IZ);
		}
	}
}

#endif
