// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;

using Xunit;

namespace Moq.Tests
{
	public class NestedTypeMatchersFixture
	{
		#region SubstituteTypeMatchers helper extension method

		[Theory]
		[InlineData(typeof(It.IsSubtype<Enum>), typeof(AttributeTargets), typeof(AttributeTargets))]
		[InlineData(typeof(It.IsSubtype<Enum>[]), typeof(AttributeTargets[]), typeof(AttributeTargets[]))]
		[InlineData(typeof(IEnumerable<It.IsSubtype<Enum>>), typeof(IEnumerable<AttributeTargets>), typeof(IEnumerable<AttributeTargets>))]
		[InlineData(typeof(IEnumerable<It.IsSubtype<Enum>[]>), typeof(IEnumerable<AttributeTargets[]>), typeof(IEnumerable<AttributeTargets[]>))]
		[InlineData(typeof(IEnumerable<It.IsSubtype<Enum>>[]), typeof(IEnumerable<AttributeTargets>[]), typeof(IEnumerable<AttributeTargets>[]))]
		public void SubstituteTypeMatchers_substitutes_matches(Type type, Type other, Type expected)
		{
			Assert.Equal(expected, actual: type.SubstituteTypeMatchers(other));
		}

		[Theory]
		[InlineData(typeof(It.IsSubtype<Enum>), typeof(object), typeof(It.IsSubtype<Enum>))]
		[InlineData(typeof(It.IsSubtype<Enum>[]), typeof(object[]), typeof(It.IsSubtype<Enum>[]))]
		[InlineData(typeof(IEnumerable<It.IsSubtype<Enum>>), typeof(IEnumerable<object>), typeof(IEnumerable<It.IsSubtype<Enum>>))]
		[InlineData(typeof(IEnumerable<It.IsSubtype<Enum>[]>), typeof(IEnumerable<object[]>), typeof(IEnumerable<It.IsSubtype<Enum>[]>))]
		public void SubstituteTypeMatchers_does_not_substitute_mismatches(Type type, Type other, Type expected)
		{
			Assert.Equal(expected, actual: type.SubstituteTypeMatchers(other));
		}

		[Theory]
		[InlineData(typeof(It.IsAnyType[]), typeof(IEnumerable<object>), typeof(It.IsAnyType[]))]
		[InlineData(typeof(IEnumerable<It.IsAnyType>), typeof(object[]), typeof(IEnumerable<It.IsAnyType>))]
		public void SubstituteTypeMatchers_does_not_substitute_mismatched_composite_kind(Type type, Type other, Type expected)
		{
			Assert.Equal(expected, actual: type.SubstituteTypeMatchers(other));
		}

		[Theory]
		[InlineData(typeof(It.IsAnyType), typeof(IEnumerable<object>), typeof(IEnumerable<object>))]
		[InlineData(typeof(It.IsAnyType), typeof(object[]), typeof(object[]))]
		public void SubstituteTypeMatchers_gives_precedence_to_type_matchers_over_composite_kind(Type type, Type other, Type expected)
		{
			Assert.Equal(expected, actual: type.SubstituteTypeMatchers(other));
		}

		[Theory]
		[InlineData(typeof(Tuple<It.IsSubtype<Enum>, object, It.IsAnyType, It.IsSubtype<Enum>>), typeof(Tuple<AttributeTargets, object, object, object>), typeof(Tuple<AttributeTargets, object, object, It.IsSubtype<Enum>>))]
		public void SubstituteTypeMatchers_may_substitute_only_some_parts(Type type, Type other, Type expected)
		{
			Assert.Equal(expected, actual: type.SubstituteTypeMatchers(other));
		}

		#endregion

		#region Test cases

		[Fact]
		public void It_IsAnyType_used_as_generic_type_argument_of_method()
		{
			var mock = new Mock<IX>();
			mock.Setup(m => m.Method<It.IsAnyType>(It.IsAny<IEnumerable<It.IsAnyType>>())).Verifiable();
			mock.Object.Method(new string[] { "a", "b" });
			mock.Verify();
		}

		public interface IX
		{
			void Method<T>(IEnumerable<T> args);
		}

		#endregion
	}
}
