// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class UnmatchableMatchersFixture
	{
		[Fact]
		public void Matchers_that_are_unmatchable_due_to_implicit_conversion_operator_cause_setup_failure()
		{
			var mock = new Mock<IX>();
			var ex = Assert.Throws<ArgumentException>(() => mock.Setup(x => x.UseB(It.IsAny<A>())));
			Assert.Contains("'It.IsAny<UnmatchableMatchersFixture.A>()' is unmatchable", ex.Message);
		}

		[Fact]
		public void Matchers_with_explicit_primitive_type_cast_are_not_considered_unmatchable()
		{
			var mock = new Mock<IX>();
			mock.Setup(x => x.UseLong((int)It.IsAny<long>()));
		}

		[Fact]
		public void Matchers_with_implicit_primitive_type_coercions_are_not_considered_unmatchable_1()
		{
			var mock = new Mock<IX>();
			mock.Setup(x => x.UseLong(It.IsAny<int>()));
		}

		[Fact]
		public void Matchers_with_implicit_primitive_nullable_type_coercions_are_not_considered_unmatchable_2()
		{
			var mock = new Mock<IX>();
			mock.Setup(x => x.UseNullableLong(It.IsAny<long>()));
		}

		public interface IX
		{
			void UseB(B arg);
			void UseInt(int arg);
			void UseLong(long arg);
			void UseNullableLong(long? arg);
		}

		public readonly struct A
		{
		}

		public readonly struct B
		{
			public static implicit operator B(A a)
			{
				return new B();
			}
		}
	}
}
