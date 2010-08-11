using System;
using System.Linq;
using Xunit;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq.Tests
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

			Assert.Equal(target.Foo.Name, "Foo");
			Assert.Equal(target.Foo.Find("1").Baz("hello").Value, 1);
			Assert.Equal(target.Bar.Id, "A");
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

			Assert.Equal(target.Name, "Foo");
			Assert.Equal(target.Find("1").Baz("asdf").Value, 99);
			Assert.Equal(target.Bar.Id, "25");
			Assert.Equal(target.Bar.Ping("blah"), "ack");
			Assert.Equal(target.Bar.Ping("error"), "error");
			Assert.Equal(target.Bar.Baz("foo").Value, 5);
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

		public void Do()
		{
			Console.WriteLine("Done");
		}

		public class Dto
		{
			public string Value { get; set; }
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
}