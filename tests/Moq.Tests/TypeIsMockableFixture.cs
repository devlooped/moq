// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class TypeIsMockableFixture
	{
		[Theory]
		[InlineData(typeof(Struct))]
		[InlineData(typeof(Struct<bool>))]
		[InlineData(typeof(DateTime))]
		[InlineData(typeof(decimal))]
		[InlineData(typeof(int))]
		public void Type_IsMockable_returns_false_for_value_types(Type type)
		{
			Assert.True(type.IsValueType);
			Assert.False(type.IsMockable());
		}

		[Theory]
		[InlineData(typeof(Enumeration))]
		public void Type_IsMockable_returns_false_for_enum_types(Type type)
		{
			Assert.True(type.IsEnum);
			Assert.False(type.IsMockable());
		}

		[Theory]
		[InlineData(typeof(IInterface))]
		[InlineData(typeof(IInterface<bool>))]
		public void Type_IsMockable_returns_true_for_interfaces(Type type)
		{
			Assert.True(type.IsInterface);
			Assert.True(type.IsMockable());
		}

		[Theory]
		[InlineData(typeof(VoidDelegate))]
		[InlineData(typeof(NonVoidDelegate<bool>))]
		[InlineData(typeof(Action))]
		[InlineData(typeof(Action<bool>))]
		[InlineData(typeof(Func<bool>))]
		public void Type_IsMockable_returns_true_for_delegates(Type type)
		{
			Assert.True(type.IsSubclassOf(typeof(Delegate)));
			Assert.True(type.IsMockable());
		}

		[Theory]
		[InlineData(typeof(NonSealedClass))]
		[InlineData(typeof(NonSealedClass<bool>))]
		[InlineData(typeof(object))]
		public void Type_IsMockable_returns_true_for_non_sealed_classes(Type type)
		{
			Assert.True(!type.IsSealed && type.IsClass);
			Assert.True(type.IsMockable());
		}

		[Theory]
		[InlineData(typeof(AbstractClass))]
		[InlineData(typeof(AbstractClass<bool>))]
		public void Type_IsMockable_returns_true_for_abstract_classes(Type type)
		{
			Assert.True(type.IsAbstract && type.IsClass);
			Assert.True(type.IsMockable());
		}

		[Theory]
		[InlineData(typeof(StaticClass))]
		[InlineData(typeof(StaticClass<bool>))]
		public void Type_IsMockable_returns_false_for_static_classes(Type type)
		{
			Assert.True(type.IsAbstract && type.IsSealed && type.IsClass);
			Assert.False(type.IsMockable());
		}

		[Theory]
		[InlineData(typeof(SealedClass))]
		[InlineData(typeof(SealedClass<bool>))]
		[InlineData(typeof(string))]
		public void Type_IsMockable_returns_false_for_sealed_classes(Type type)
		{
			Assert.True(type.IsSealed && type.IsClass);
			Assert.False(type.IsMockable());
		}

		public struct Struct { }
		public struct Struct<T> { }

		public enum Enumeration { }

		public delegate void VoidDelegate();
		public delegate T NonVoidDelegate<T>();

		public interface IInterface { }
		public interface IInterface<T> { }

		public class NonSealedClass { }
		public class NonSealedClass<T> { }

		public abstract class AbstractClass { }
		public abstract class AbstractClass<T> { }

		public static class StaticClass { }
		public static class StaticClass<T> { }

		public sealed class SealedClass { }
		public sealed class SealedClass<T> { }
	}
}
