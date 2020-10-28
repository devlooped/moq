// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq;
using System.Collections.Generic;

using Xunit;

namespace Moq.Tests.Linq
{
	public class QueryableMocksFixture
	{
		[Fact]
		public void ShouldSupportReturningMultipleMocks()
		{
			var target = (from foo in Mocks.Of<IFoo>()
						  from bar in Mocks.Of<IBar>()
						  where
							foo.Name == "Foo" &&
							foo.Find("1").Baz(It.IsAny<string>()).Value == 1 &&
							bar.Id == "A"
						  select new { Foo = foo, Bar = bar })
						  .First();

			Assert.Equal("Foo", target.Foo.Name);
			Assert.Equal(1, target.Foo.Find("1").Baz("hello").Value);
			Assert.Equal("A", target.Bar.Id);
		}

		[Fact]
		public void ShouldSupportMultipleSetups()
		{
			var target = (from f in Mocks.Of<IFoo>()
						  where
							f.Name == "Foo" &&
							f.Find("1").Baz(It.Is<string>(s => s.Length > 0)).Value == 99 &&
							f.Bar.Id == "25" &&
							f.Bar.Ping(It.IsAny<string>()) == "ack" &&
							f.Bar.Ping("error") == "error" &&
							f.Bar.Baz(It.IsAny<string>()).Value == 5
						  select f)
						  .First();

			Assert.Equal("Foo", target.Name);
			Assert.Equal(99, target.Find("1").Baz("asdf").Value);
			Assert.Equal("25", target.Bar.Id);
			Assert.Equal("ack", target.Bar.Ping("blah"));
			Assert.Equal("error", target.Bar.Ping("error"));
			Assert.Equal(5, target.Bar.Baz("foo").Value);
		}

		[Fact]
		public void ShouldSupportEnum()
		{
			var target = Mocks.Of<IFoo>().First(f => f.Targets == AttributeTargets.Class);

			Assert.Equal(AttributeTargets.Class, target.Targets);
		}

		[Fact]
		public void ShoulSupportMethod()
		{
			var expected = new Mock<IBar>().Object;
			var target = Mocks.Of<IFoo>().First(x => x.Find(It.IsAny<string>()) == expected);

			Assert.Equal(expected, target.Find("3"));
		}

		[Fact]
		public void ShouldSupportIndexer()
		{
			var target = Mocks.Of<IBaz>().First(x => x["3", It.IsAny<bool>()] == 10);

			Assert.NotEqual(10, target["1", true]);
			Assert.Equal(10, target["3", true]);
			Assert.Equal(10, target["3", false]);
		}

		[Fact]
		public void ShouldSupportBooleanMethod()
		{
			var target = Mocks.Of<IBaz>().First(x => x.HasElements("3"));

			Assert.True(target.HasElements("3"));
		}

		[Fact]
		public void ShouldSupportBooleanMethodNegation()
		{
			var target = Mocks.Of<IBaz>().First(x => !x.HasElements("3"));

			Assert.False(target.HasElements("3"));
		}

		[Fact]
		public void ShouldSupportMultipleMethod()
		{
			var target = Mocks.Of<IBaz>().First(x => !x.HasElements("1") && x.HasElements("2"));

			Assert.False(target.HasElements("1"));
			Assert.True(target.HasElements("2"));
		}

		[Fact]
		public void ShouldSupportMocksFirst()
		{
			var target = Mocks.Of<IBaz>().First();

			Assert.NotNull(target);
		}

		[Fact]
		public void ShouldSupportMocksFirstOrDefault()
		{
			var target = Mocks.Of<IBaz>().FirstOrDefault();

			Assert.NotNull(target);
		}

		[Fact]
		public void ShouldSupportSettingDtoPropertyValue()
		{
			//var target = Mock.Of<IFoo>(x => x.Bar.Id == "foo");
			var target = Mock.Of<Dto>(x => x.Value == "foo");

			Assert.Equal("foo", target.Value);
		}

		[Fact]
		public void ShouldSupportSettingDtoProtectedPropertyValue()
		{
			var target = Mock.Of<Dto>(x => x.ProtectedValue == "foo");

			Assert.Equal("foo", target.ProtectedValue);
		}

		[Fact]
		public void ShouldSupportSettingDtoProtectedVirtualPropertyValue()
		{
			var target = Mock.Of<Dto>(x => x.ProtectedVirtualValue == "foo");

			Assert.Equal("foo", target.ProtectedVirtualValue);
		}

		[Fact]
		public void ShouldOneOfCreateNewMock()
		{
			var target = Mock.Of<IFoo>();

			Assert.NotNull(Mock.Get(target));
		}

		[Fact]
		public void ShouldOneOfWithPredicateCreateNewMock()
		{
			var target = Mock.Of<IFoo>(x => x.Name == "Foo");

			Assert.NotNull(Mock.Get(target));
			Assert.Equal("Foo", target.Name);
		}

		[Fact]
		public void ShouldAllowFluentOnReadOnlyGetterProperty()
		{
			var target = Mock.Of<IFoo>(x => x.Bars == new[] 
			{ 
				Mock.Of<IBar>(b => b.Id == "1"), 
				Mock.Of<IBar>(b => b.Id == "2"), 
			});

			Assert.NotNull(Mock.Get(target));
			Assert.Equal(2, target.Bars.Count());
		}

		[Fact]
		public void ShouldAllowFluentOnNonVirtualReadWriteProperty()
		{
			var target = Mock.Of<Dto>(x => x.Value == "foo");

			Assert.NotNull(Mock.Get(target));
			Assert.Equal("foo", target.Value);
		}

		[Fact]
		public void Strict_Mock_Of_will_throw_for_non_setup_property()
		{
			var foo = Mock.Of<IFoo>(MockBehavior.Strict);
			Assert.Throws<MockException>(() => _ = foo.Name);
		}

		[Fact]
		public void Strict_Mock_Of_with_specification_expression_will_throw_for_non_setup_property()
		{
			var foo = Mock.Of<IFoo>(f => f.Targets == default, MockBehavior.Strict);
			_ = foo.Targets;
			Assert.Throws<MockException>(() => _ = foo.Name);
		}

		[Fact]
		public void Strict_Mocks_Of_will_throw_for_non_setup_property()
		{
			var foo = Mocks.Of<IFoo>(MockBehavior.Strict).First();
			Assert.Throws<MockException>(() => _ = foo.Name);
		}

		[Fact]
		public void Strict_Mocks_Of_with_specification_expression_will_throw_for_non_setup_property()
		{
			var foo = Mocks.Of<IFoo>(f => f.Targets == default, MockBehavior.Strict).First();
			_ = foo.Targets;
			Assert.Throws<MockException>(() => _ = foo.Name);
		}

		[Fact]
		public void Multiple_mocks_with_query_comprehension_syntax__predicates_in_where_clause()
		{
			var x = (from x1 in Mocks.Of<IFoo>()
			         from __ in Mocks.Of<IFoo>()
			         where x1.Name == "1" && __.Name == "2"
			         select x1)
			        .First();
			Assert.Equal("1", x.Name);
		}

		[Fact]
		public void Multiple_mocks_with_query_comprehension_syntax__predicates_in_from_clause()
		{
			var x = (from x1 in Mocks.Of<IFoo>(_ => _.Name == "1")
			         from __ in Mocks.Of<IFoo>(_ => _.Name == "2")
			         select x1)
			        .First();
			Assert.Equal("1", x.Name);
		}

		public class Dto
		{
			public string Value { get; set; }
			public string ProtectedValue { get; protected set; }
			public virtual string ProtectedVirtualValue { get; protected set; }
		}

		public interface IFoo
		{
			IBar Bar { get; set; }
			string Name { get; set; }
			IBar Find(string id);
			AttributeTargets Targets { get; set; }
			IEnumerable<IBar> Bars { get; }
		}

		public interface IBar
		{
			IBaz Baz(string value);
			string Id { get; set; }
			string Ping(string command);
		}

		public interface IBaz
		{
			int Value { get; set; }
			int this[string key1, bool key2] { get; set; }
			bool IsValid { get; set; }
			bool HasElements(string key1);
		}
	}

	public class Foo
	{
		protected Foo()
		{
		}

		public virtual string Value { get; private set; }
	}

	public class FooFixture
	{
		[Fact]
		public void Test()
		{
			var remote = Mock.Of<Foo>(rt => rt.Value == "foo");
			Assert.Equal("foo", remote.Value);
		}
	}

	public interface IBar
	{
		Foo Foo { get; set; }
	}

	public class BarFixture
	{
		[Fact]
		public void Test()
		{
			var remote = Mock.Of<IBar>(rt => rt.Foo.Value == "foo");
			Assert.Equal("foo", remote.Foo.Value);
		}
	}
}
