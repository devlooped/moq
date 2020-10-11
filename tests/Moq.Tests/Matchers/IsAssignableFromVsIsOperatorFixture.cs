// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;

using Xunit;

#pragma warning disable CS0183, CS0184

namespace Moq.Tests.Matchers
{
	// The purpose behind these tests is to find cases where `value is T` and
	// `typeof(T).IsAssignableFrom(value.GetType())` disagree (i.e. yield different
	// results). This is to check the validity of simplifying the implementation
	// of `It.IsAny<T>`, which used to run both checks (except when `value` == null;
	// see commit history). If both expressions generally yield the same results
	// (and it looks like that's the case) we can simplify and just run `value is T`.
	public class IsAssignableFromVsIsOperatorFixture
	{
		[Fact]
		public void Nullable_value_type_is_value_type()
		{
			int? value = 42;
			Assert.True(value is int);
			Assert.True(typeof(int).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Value_type_is_nullable_value_type()
		{
			int value = 42;
			Assert.True(value is int?);
			Assert.True(typeof(int?).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Value_type_is_object()
		{
			int value = 42;
			Assert.True(value is object);
			Assert.True(typeof(object).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Value_type_is_not_wider_value_type()
		{
			int value = 42;
			Assert.False(value is long);
			Assert.False(typeof(long).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Value_type_is_not_narrower_value_type()
		{
			long value = 42L;
			Assert.False(value is int);
			Assert.False(typeof(int).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Value_type_is_not_value_type_of_opposite_signedness()
		{
			int value = 42;
			Assert.False(value is uint);
			Assert.False(typeof(uint).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Value_type_as_object_is_not_wider_value_type()
		{
			int value = 42;
			Assert.False((object)value is long);
			Assert.False(typeof(long).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Value_type_array_is_not_nullable_value_type_array()
		{
			int[] value = new[] { 42 };
			Assert.False(value is int?[]);
			Assert.False(typeof(int?[]).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Value_type_array_as_object_is_not_nullable_value_type_array()
		{
			int[] value = new[] { 42 };
			Assert.False((object)value is int?[]);
			Assert.False(typeof(int?[]).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Value_type_array_is_not_object_array()
		{
			int[] value = new[] { 42 };
			Assert.False(value is object[]);
			Assert.False(typeof(object[]).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Value_type_array_is_wider_value_type_array_but_is_operator_says_no()
		{
			int[] value = new[] { 42 };
			Assert.False(value is uint[]);
			Assert.True(typeof(uint[]).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Value_type_array_is_wider_value_type_array_and_is_operator_agrees_when_value_as_object()
		{
			int[] value = new[] { 42 };
			Assert.True((object)value is uint[]);
			Assert.True(typeof(uint[]).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Value_type_is_interface()
		{
			int value = 42;
			Assert.True(value is IComparable);
			Assert.True(typeof(IComparable).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Reference_type_is_object()
		{
			string value = "42";
			Assert.True(value is object);
			Assert.True(typeof(object).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Reference_type_is_interface()
		{
			string value = "42";
			Assert.True(value is IComparable);
			Assert.True(typeof(IComparable).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Reference_type_is_super_class()
		{
			ArgumentNullException value = new ArgumentNullException();
			Assert.True(value is Exception);
			Assert.True(typeof(Exception).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Generic_type_is_not_more_general_generic_type()
		{
			IEnumerable<int> value = new List<int>();
			Assert.False(value is IEnumerable<object>);
			Assert.False(typeof(IEnumerable<object>).IsAssignableFrom(value.GetType()));
		}

		[Fact]
		public void Generic_type_is_not_generic_type_definition()
		{
			List<int> value = new List<int>();
			//Assert.True(value is List<>);
			Assert.False(typeof(List<>).IsAssignableFrom(value.GetType()));
		}
	}
}

#pragma warning restore CS0183, CS0184
