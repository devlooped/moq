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
		public void Cannot_use_type_matcher_with_parameterized_constructor_directly()
		{
			var mock = new Mock<IX>();
			mock.Setup(x => x.Method<Picky>());

			// TODO: Ideally, failure should occur as early as possible, i.e. during setup.
			Action invocation = () => mock.Object.Method<object>();

			var ex = Assert.Throws<ArgumentException>(invocation);
			Assert.Contains("Picky does not have a default (public parameterless) constructor", ex.Message);
		}

		public interface IX
		{
			void Method<T>();
		}

		public sealed class IntOrString : ITypeMatcher
		{
			public bool Matches(Type typeArgument)
			{
				return typeArgument == typeof(int) || typeArgument == typeof(string);
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
	}
}
