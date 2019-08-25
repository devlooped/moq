// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

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
		public void Cannot_use_type_matcher_with_parameterized_constructor_directly_in_Verify()
		{
			var mock = new Mock<IX>();

			Action verify = () => mock.Verify(x => x.Method<Picky>(), Times.Never);

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

		public interface IX
		{
			void Method<T>();
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
	}
}
