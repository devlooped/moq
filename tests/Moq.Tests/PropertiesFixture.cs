// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class PropertiesFixture
	{
		public interface IFoo
		{
			IIndexedFoo Indexed { get; set; }
		}

		public interface IIndexedFoo
		{
			string this[int key] { get; set; }
			string this[int key1, string key2, bool key3, DateTime key4] { get; set; }

			IBar this[int key1, string key2, DateTime key4] { get; set; }
		}

		public interface IBar
		{
			string Value { get; set; }
		}

		[Fact]
		public void ShouldSupportMultipleIndexerGettersInFluentMock()
		{
			var foo = new Mock<IFoo>();

			foo.SetupGet(x => x.Indexed[It.IsAny<int>(), "foo", It.IsAny<DateTime>()].Value).Returns("bar");

			var result = foo.Object.Indexed[1, "foo", DateTime.Now].Value;

			Assert.Equal("bar", result);
		}

		[Fact]
		public void ShouldSupportMultipleIndexerGetters()
		{
			var foo = new Mock<IIndexedFoo>();

			foo.SetupGet(x => x[It.IsAny<int>(), "foo", true, It.IsAny<DateTime>()]).Returns("bar");

			var result = foo.Object[1, "foo", true, DateTime.Now];

			Assert.Equal("bar", result);
		}

		[Fact]
		public void ShouldSetIndexer()
		{
			var foo = new Mock<IIndexedFoo>(MockBehavior.Strict);

			foo.SetupSet(f => f[0] = "foo");

			foo.Object[0] = "foo";
		}

		[Fact]
		public void ShouldSetIndexerWithValueMatcher()
		{
			var foo = new Mock<IIndexedFoo>(MockBehavior.Strict);

			foo.SetupSet(f => f[0] = It.IsAny<string>());

			foo.Object[0] = "foo";
		}

		[Fact]
		public void ShouldSetIndexerWithIndexMatcher()
		{
			var foo = new Mock<IIndexedFoo>(MockBehavior.Strict);
			foo.SetupSet(f => f[It.IsAny<int>()] = "foo");
			foo.Object[18] = "foo";
		}

		[Fact]
		public void ShouldSetIndexerWithBothMatcher()
		{
			var foo = new Mock<IIndexedFoo>(MockBehavior.Strict);
			foo.SetupSet(f => f[It.IsAny<int>()] = It.IsAny<string>());
			foo.Object[18] = "foo";
		}

		[Fact]
		public void Can_Setup_virtual_property()
		{
			// verify our assumptions that A is indeed virtual (and non-sealed):
			var propertyGetter = typeof(Foo).GetProperty("A").GetGetMethod();
			Assert.True(propertyGetter.IsVirtual);
			Assert.False(propertyGetter.IsFinal);

			var mock = new Mock<Foo>();
			mock.Setup(m => m.A).Returns("mocked A");

			var a = mock.Object.A;

			Assert.Equal("mocked A", a);
		}

		[Fact]
		public void Can_Setup_virtual_property_that_implicitly_implements_a_property_from_inaccessible_interface()
		{
			// verify our assumptions that C is indeed virtual (and non-sealed):
			var propertyGetter = typeof(Foo).GetProperty("C").GetGetMethod();
			Assert.True(propertyGetter.IsVirtual);
			Assert.False(propertyGetter.IsFinal);

			var mock = new Mock<Foo>();
			mock.Setup(m => m.C).Returns("mocked C");

			var c = mock.Object.C;

			Assert.Equal("mocked C", c);
		}

		[Fact]
		public void Cannot_Setup_virtual_but_sealed_property()
		{
			// verify our assumptions that B is indeed virtual and sealed:
			var propertyGetter = typeof(Foo).GetProperty("B").GetGetMethod();
			Assert.True(propertyGetter.IsVirtual);
			Assert.True(propertyGetter.IsFinal);

			var mock = new Mock<Foo>();

			var exception = Record.Exception(() =>
			{
				mock.Setup(m => m.B).Returns("mocked B");
			});
			var b = mock.Object.B;

			Assert.NotEqual("mocked B", b); // it simply shouldn't be possible for Moq to intercept a sealed property;
			Assert.NotNull(exception);      // and Moq should tell us by throwing an exception.
		}

		[Fact]
		public void Cannot_Setup_virtual_but_sealed_property_that_implicitly_implements_a_property_from_inaccessible_interface()
		{
			// verify our assumptions that D is indeed virtual and sealed:
			var propertyGetter = typeof(Foo).GetProperty("D").GetGetMethod();
			Assert.True(propertyGetter.IsVirtual);
			Assert.True(propertyGetter.IsFinal);

			var mock = new Mock<Foo>();

			var exception = Record.Exception(() =>
			{
				mock.Setup(m => m.D).Returns("mocked D");
			});
			var d = mock.Object.D;

			Assert.NotEqual("mocked D", d); // it simply shouldn't be possible for Moq to intercept a sealed property;
			Assert.NotNull(exception);      // and Moq should tell us by throwing an exception.
		}

		[Fact]
		public void SetupAllProperties_does_not_throw_when_it_encounters_properties_that_cannot_be_setup()
		{
			var mock = new Mock<Foo>();

			var exception = Record.Exception(() =>
			{
				mock.SetupAllProperties();
			});

			Assert.Null(exception);
		}

		[Fact]
		public void SetupAllProperties_should_not_reset_indexer_setups()
		{
			var mock = new Mock<IIndexedFoo>();
			mock.SetupGet(m => m[1]).Returns("value from setup");
			Assert.Equal("value from setup", mock.Object[1]);
			mock.SetupAllProperties();
			Assert.Equal("value from setup", mock.Object[1]);
		}

		public abstract class FooBase
		{
			public abstract object A { get; }
			public abstract object B { get; }
		}

		internal interface IFooInternals
		{
			object C { get; }
			object D { get; }
		}

		public abstract class Foo : FooBase, IFooInternals
		{
			public override object A => "A";
			public sealed override object B => "B";
			public virtual object C => "C";
			public object D => "D";
		}
	}
}
