using System;
using System.Linq;
using Xunit;

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
		public void ShouldSupportItIsAny()
		{
			var target = (from f in Mocks.Of<IFoo>()
						  where f.Bar.Baz(It.IsAny<string>()).Value == 5
						  select f)
						  .First();

			Assert.Equal(target.Bar.Baz("foo").Value, 5);
			Assert.Equal(target.Bar.Baz("bar").Value, 5);
		}

		[Fact]
		public void TranslateToFluentMocks()
		{
			var target = (from f in Mocks.Of<IFoo>()
							where f.Bar.Baz("hey").Value == 5
							select f)
							.First();

			// f.Bar.Baz("hey").Value 
			// f						 => Mock.Get(f)
			// [f].Bar					 => .FluentMock(mock => mock.Bar)
			// [f.Bar].Baz("hey")		 => .FluentMock(mock => mock.Baz("hey")

			// [f.Bar.Baz("hey")].Value  => .Setup(mock => mock.Value).Returns(..) != null

			Assert.Equal(target.Bar.Baz("hey").Value, 5);

			// This is the actual translation and what gets executed.
			var instance2 = (from f in Mocks.CreateQueryable<IFoo>()
							 where
								 Mock.Get(f)
									 .FluentMock(f1 => f1.Bar)
									 .FluentMock(b1 => b1.Baz("hey"))
									 .Setup(b2 => b2.Value).Returns(5) != null
							 select f)
						   .First();

			Assert.Equal(instance2.Bar.Baz("hey").Value, 5);
		}

		[Fact]
		public void ShouldSupportBoolean()
		{
			var target = Mocks.Of<IBaz>().First(x => x.IsValid);

			Assert.True(target.IsValid);
		}

		[Fact]
		public void ShouldSupportBooleanNegation()
		{
			var target = Mocks.Of<IBaz>().First(x => !x.IsValid);

			Assert.False(target.IsValid);
		}

		[Fact]
		public void ShouldSupportBooleanEqualsTrue()
		{
			var target = Mocks.Of<IBaz>().First(f => f.IsValid == true);

			Assert.True(target.IsValid);
		}

		[Fact]
		public void ShouldSupportTrueEqualsBoolean()
		{
			var target = Mocks.Of<IBaz>().First(f => true == f.IsValid);

			Assert.True(target.IsValid);
		}

		[Fact]
		public void ShouldSupportBooleanEqualsFalse()
		{
			var target = Mocks.Of<IBaz>().Where(f => f.IsValid == false).First();

			Assert.False(target.IsValid);
		}

		[Fact]
		public void ShouldSupportBooleanInCondition()
		{
			var target = Mocks.Of<IBaz>().First(x => x.IsValid && x.Value == 1);

			Assert.True(target.IsValid);
			Assert.Equal(1, target.Value);
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

		public interface IFoo
		{
			IBar Bar { get; set; }
			string Name { get; set; }
			IBar Find(string id);
			AttributeTargets Targets { get; set; }
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