// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace Moq.Tests
{
	public class CustomTypeMatchersFixture
	{
		[Fact]
		public void Setup_with_custom_type_matcher()
		{
			var invocationCount = 0;
			var mock = new Mock<IX>();
			mock.Setup(x => x.Method<IntOrString>()).Callback(() => invocationCount++);

			mock.Object.Method<bool>();
			mock.Object.Method<int>();
			mock.Object.Method<int>();
			mock.Object.Method<object>();
			mock.Object.Method<string>();

			Assert.Equal(3, invocationCount);
		}

		[Fact]
		public void Verify_with_custom_type_matcher()
		{
			var mock = new Mock<IX>();

			mock.Object.Method<bool>();
			mock.Object.Method<int>();
			mock.Object.Method<int>();
			mock.Object.Method<object>();
			mock.Object.Method<string>();

			mock.Verify(x => x.Method<IntOrString>(), Times.Exactly(3));
		}

		[Fact]
		public void Cannot_use_type_matcher_with_parameterized_constructor_directly_in_Setup()
		{
			var mock = new Mock<IX>();

			Action setup = () => mock.Setup(x => x.Method<Picky>());

			var ex = Assert.Throws<ArgumentException>(setup);
			Assert.Contains("Picky does not have a default (public parameterless) constructor", ex.Message);
		}

		[Fact]
		public void Cannot_use_nested_type_matcher_with_parameterized_constructor_directly_in_Setup()
		{
			var mock = new Mock<IX>();

			Action setup = () => mock.Setup(x => x.Method<Picky[]>());

			var ex = Assert.Throws<ArgumentException>(setup);
			Assert.Contains("Picky does not have a default (public parameterless) constructor", ex.Message);
		}

		[Fact]
		public void Cannot_use_type_matcher_with_parameterized_constructor_directly_in_Verify()
		{
			var mock = new Mock<IX>();

			Action verify = () => mock.Verify(x => x.Method<Picky>(), Times.Never);

			var ex = Assert.Throws<ArgumentException>(verify);
			Assert.Contains("Picky does not have a default (public parameterless) constructor", ex.Message);
		}

		[Fact]
		public void Cannot_use_nested_type_matcher_with_parameterized_constructor_directly_in_Verify()
		{
			var mock = new Mock<IX>();

			Action verify = () => mock.Verify(x => x.Method<Picky[]>(), Times.Never);

			var ex = Assert.Throws<ArgumentException>(verify);
			Assert.Contains("Picky does not have a default (public parameterless) constructor", ex.Message);
		}

		[Fact]
		public void Can_use_type_matcher_derived_from_one_having_a_parameterized_constructor()
		{
			var mock = new Mock<IX>();

			mock.Object.Method<bool>();
			mock.Object.Method<int>();
			mock.Object.Method<object>();
			mock.Object.Method<string>();
			mock.Object.Method<string>();

			mock.Verify(x => x.Method<PickyIntOrString>(), Times.Exactly(3));
		}

		[Fact]
		public void Can_use_struct_type_matchers()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Method<AnyStruct>());
		}

		[Fact]
		public void Must_use_custom_type_matcher_when_type_constraints_present()
		{
			var mock = new Mock<IY>();
			//mock.Setup(y => y.Method<It.IsAnyType>());  // wouldn't work because of the type constraint
			mock.Setup(y => y.Method<AnyException>());

			mock.Object.Method<ArgumentException>();

			mock.VerifyAll();
		}

		[Fact]
		public void Must_use_TypeMatcherAttribute_when_type_constraints_present_that_prevents_direct_implementation_of_ITypeMatcher()
		{
			var mock = new Mock<IZ>();
			mock.Setup(z => z.DelegateMethod<AnyDelegate>());
			mock.Setup(z => z.EnumMethod<AnyEnum>());

			mock.Object.DelegateMethod<Action>();
			mock.Object.EnumMethod<AttributeTargets>();

			mock.VerifyAll();
		}

		[Fact]
		public void It_IsAny_works_with_custom_matcher()
		{
			var invocationCount = 0;
			var mock = new Mock<IX>();
			mock.Setup(m => m.Method(It.IsAny<IntOrString>())).Callback((object arg) => invocationCount++);

			mock.Object.Method(true);
			mock.Object.Method(42);
			mock.Object.Method("42");
			mock.Object.Method(new Exception("42"));
			mock.Object.Method((string)null);

			Assert.Equal(3, invocationCount);
		}

		[Fact]
		public void It_IsNotNull_works_with_custom_matcher()
		{
			var invocationCount = 0;
			var mock = new Mock<IX>();
			mock.Setup(m => m.Method(It.IsNotNull<IntOrString>())).Callback((object arg) => invocationCount++);

			mock.Object.Method(true);
			mock.Object.Method(42);
			mock.Object.Method("42");
			mock.Object.Method(new Exception("42"));
			mock.Object.Method((string)null);

			Assert.Equal(2, invocationCount);
		}

		[Fact]
		public void It_Is_works_with_custom_matcher()
		{
			var acceptableArgs = new object[] { 42, "42" };

			var invocationCount = 0;
			var mock = new Mock<IX>();
			mock.Setup(m => m.Method(It.Is<IntOrString>((arg, _) => acceptableArgs.Contains(arg))))
			    .Callback((object arg) => invocationCount++);

			mock.Object.Method(42);
			mock.Object.Method(7);
			Assert.Equal(1, invocationCount);

			mock.Object.Method("42");
			mock.Object.Method("7");
			mock.Object.Method((string)null);
			Assert.Equal(2, invocationCount);
		}

		[Fact]
		public void It_Is_works_with_custom_comparer()
		{
			var acceptableArg = "FOO";

			var invocationCount = 0;
			var mock = new Mock<IX>();
			mock.Setup(m => m.Method(It.Is<string>(acceptableArg, StringComparer.OrdinalIgnoreCase)))
				.Callback((object arg) => invocationCount++);

			mock.Object.Method("foo");
			mock.Object.Method("bar");
			Assert.Equal(1, invocationCount);

			mock.Object.Method("FOO");
			mock.Object.Method("foo");
			mock.Object.Method((string)null);
			Assert.Equal(3, invocationCount);
		}

		[Fact]
		public void It_Is_object_works_with_custom_comparer()
		{
			var acceptableArg = "FOO";

			var invocationCount = 0;
			var mock = new Mock<IX>();
			mock.Setup(m => m.Method(It.Is<object>(acceptableArg, new ObjectStringOrdinalIgnoreCaseComparer())))
				.Callback((object arg) => invocationCount++);

			mock.Object.Method("foo");
			mock.Object.Method("bar");
			Assert.Equal(1, invocationCount);

			mock.Object.Method("FOO");
			mock.Object.Method("foo");
			mock.Object.Method((string)null);
			Assert.Equal(3, invocationCount);
		}

		public interface IX
		{
			void Method<T>();
			void Method<T>(T arg);
		}

		public interface IY
		{
			void Method<TException>() where TException : Exception;
		}

		public interface IZ
		{
			void DelegateMethod<TDelegate>() where TDelegate : Delegate;
			void EnumMethod<TEnum>() where TEnum : Enum;
		}

		[TypeMatcher]
		public sealed class IntOrString : ITypeMatcher
		{
			public bool Matches(Type typeArgument)
			{
				return typeArgument == typeof(int) || typeArgument == typeof(string);
			}
		}

		public sealed class PickyIntOrString : Picky
		{
			public PickyIntOrString() : base(typeof(int), typeof(string))
			{
			}
		}

		[TypeMatcher]
		public class Picky : ITypeMatcher
		{
			private readonly Type[] types;

			public Picky(params Type[] types)
			{
				this.types = types;
			}

			public bool Matches(Type typeArgument)
			{
				return Array.IndexOf(this.types, typeArgument) >= 0;
			}
		}

		[TypeMatcher]
		public class AnyException : Exception, ITypeMatcher
		{
			public bool Matches(Type typeArgument)
			{
				return true;
			}
		}

		[TypeMatcher(typeof(It.IsAnyType))]
		public enum AnyEnum { }

		[TypeMatcher(typeof(It.IsAnyType))]
		public delegate void AnyDelegate();

		[TypeMatcher]
		public struct AnyStruct : ITypeMatcher
		{
			public bool Matches(Type typeArgument) => typeArgument.IsValueType;
		}

		public class ObjectStringOrdinalIgnoreCaseComparer : IEqualityComparer<object>
		{
			private static IEqualityComparer<string> InternalComparer => StringComparer.OrdinalIgnoreCase;

			public new bool Equals(object x, object y)
			{
				return InternalComparer.Equals((string)x, (string)y);
			}

			public int GetHashCode(object obj)
			{
				return InternalComparer.GetHashCode((string)obj);
			}
		}
	}
}
