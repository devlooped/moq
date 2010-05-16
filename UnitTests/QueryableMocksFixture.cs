using System;
using System.Linq;
using Xunit;

namespace Moq.Tests
{
	public class QueryableMocksFixture
	{
		[Fact(Timeout = 10000)]
		public void ShouldSupportReturningMultipleMocks()
		{
			var mocks = (from foo in Mocks.CreateQuery<IFoo>()
						 from bar in Mocks.CreateQuery<IBar>()
						 where
								 foo.Name == "Foo" &&
								 foo.Find("1").Baz(It.IsAny<string>()).Value == 1 &&
								 bar.Id == "A"
						 select new { Foo = foo, Bar = bar })
						 .First();

			Assert.Equal(mocks.Foo.Name, "Foo");
			Assert.Equal(mocks.Foo.Find("1").Baz("hello").Value, 1);
			Assert.Equal(mocks.Bar.Id, "A");
		}

		[Fact(Timeout = 10000)]
		public void ShouldSupportMultipleSetups()
		{
			var instance = (from f in Mocks.CreateQuery<IFoo>()
							where
								f.Name == "Foo" &&
								f.Find("1").Baz(It.Is<string>(s => s.Length > 0)).Value == 99 &&
								f.Bar.Id == "25" &&
								f.Bar.Ping(It.IsAny<string>()) == "ack" &&
								f.Bar.Ping("error") == "error" &&
								f.Bar.Baz(It.IsAny<string>()).Value == 5
							select f)
							.First();

			Assert.Equal(instance.Name, "Foo");
			Assert.Equal(instance.Find("1").Baz("asdf").Value, 99);
			Assert.Equal(instance.Bar.Id, "25");
			Assert.Equal(instance.Bar.Ping("blah"), "ack");
			Assert.Equal(instance.Bar.Ping("error"), "error");
			Assert.Equal(instance.Bar.Baz("foo").Value, 5);
		}

		[Fact(Timeout = 10000)]
		public void ShouldSupportItIsAny()
		{
			var instance = (from f in Mocks.CreateQuery<IFoo>()
							where f.Bar.Baz(It.IsAny<string>()).Value == 5
							select f)
							.First();

			Assert.Equal(instance.Bar.Baz("foo").Value, 5);
			Assert.Equal(instance.Bar.Baz("bar").Value, 5);
		}

		[Fact(Timeout = 10000)]
		public void TranslateToFluentMocks()
		{
			var instance = (from f in Mocks.CreateQuery<IFoo>()
							where f.Bar.Baz("hey").Value == 5
							select f)
							.First();

			// f.Bar.Baz("hey").Value 
			// f						 => Mock.Get(f)
			// [f].Bar					 => .FluentMock(mock => mock.Bar)
			// [f.Bar].Baz("hey")		 => .FluentMock(mock => mock.Baz("hey")

			// [f.Bar.Baz("hey")].Value  => .Setup(mock => mock.Value).Returns(..) != null

			Assert.Equal(instance.Bar.Baz("hey").Value, 5);

			// This is the actual translation and what gets executed.
			var instance2 = (from f in Mocks.CreateReal<IFoo>()
							 where
								 Mock.Get(f)
									 .FluentMock(f1 => f1.Bar)
									 .FluentMock(b1 => b1.Baz("hey"))
									 .Setup(b2 => b2.Value).Returns(5) != null
							 select f)
						   .First();

			Assert.Equal(instance2.Bar.Baz("hey").Value, 5);
		}

		[Fact(Timeout = 10000)]
		public void ShouldSupportBoolean()
		{
			var instance = Mocks.CreateQuery<IBaz>().First(x => x.IsValid);

			Assert.True(instance.IsValid);
		}

		[Fact(Timeout = 10000)]
		public void ShouldSupportBooleanNegation()
		{
			var instance = Mocks.CreateQuery<IBaz>().First(x => !x.IsValid);

			Assert.False(instance.IsValid);
		}

		[Fact(Timeout = 10000)]
		public void ShouldSupportBooleanEqualsTrue()
		{
			var instance = Mocks.CreateQuery<IBaz>().First(f => f.IsValid == true);

			Assert.True(instance.IsValid);
		}

		[Fact(Timeout = 10000)]
		public void ShouldSupportTrueEqualsBoolean()
		{
			var instance = Mocks.CreateQuery<IBaz>().First(f => true == f.IsValid);

			Assert.True(instance.IsValid);
		}

		[Fact(Timeout = 10000)]
		public void ShouldSupportBooleanEqualsFalse()
		{
			var instance = Mocks.CreateQuery<IBaz>().Where(f => f.IsValid == false).First();

			Assert.False(instance.IsValid);
		}

		[Fact(Timeout = 10000)]
		public void ShouldSupportBooleanInCondition()
		{
			var instance = Mocks.CreateQuery<IBaz>().First(x => x.IsValid && x.Value == 1);

			Assert.True(instance.IsValid);
			Assert.Equal(1, instance.Value);
		}

		[Fact(Timeout = 10000)]
		public void ShouldSupportEnumExpression()
		{
			var instance = Mocks.CreateQuery<IFoo>().First(f => f.Targets == AttributeTargets.Class);

			Assert.Equal(AttributeTargets.Class, instance.Targets);
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
		}
	}
}