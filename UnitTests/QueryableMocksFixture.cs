using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class QueryableMocksFixture
	{
		[Fact]
		public void ShouldSupportReturningMultipleMocks()
		{
			var mocks = (from foo in Mocks.Query<IFoo>()
						 from bar in Mocks.Query<IBar>()
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

		[Fact]
		public void ShouldSupportMultipleSetups()
		{
			var instance = (from f in Mocks.Query<IFoo>()
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

		[Fact]
		public void ShouldSupportItIsAny()
		{
			var instance = (from f in Mocks.Query<IFoo>()
							where f.Bar.Baz(It.IsAny<string>()).Value == 5
							select f)
							.First();

			Assert.Equal(instance.Bar.Baz("foo").Value, 5);
			Assert.Equal(instance.Bar.Baz("bar").Value, 5);
		}

		[Fact]
		public void TranslateToFluentMocks()
		{
			var instance = (from f in Mocks.Query<IFoo>()
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

		public interface IFoo
		{
			IBar Bar { get; set; }
			string Name { get; set; }
			IBar Find(string id);
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
		}
	}
}
