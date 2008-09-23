using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Moq.Tests
{
	public class AutoMockHierarchiesFixture
	{
		[Fact]
		public void CreatesMockForAccessedProperty()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectGet(m => m.Bar.Value).Returns(5);

			Assert.Equal(5, mock.Object.Bar.Value);
		}

		[Fact]
		public void CreatesMockForAccessedPropertyWithMethod()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(m => m.Bar.Do("ping")).Returns("ack");
			mock.Expect(m => m.Bar.Baz.Do("ping")).Returns("ack");

			Assert.Equal("ack", mock.Object.Bar.Do("ping"));
			Assert.Equal("ack", mock.Object.Bar.Baz.Do("ping"));
			Assert.Equal(default(string), mock.Object.Bar.Do("foo"));
		}

		[Fact]
		public void VerifiesAllHierarchy()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(m => m.Bar.Do("ping")).Returns("ack");
			mock.Expect(m => m.Do("ping")).Returns("ack");

			mock.Object.Do("ping");
			var bar = mock.Object.Bar;

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());
		}

		[Fact]
		public void VerifiesHierarchy()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(m => m.Bar.Do("ping")).Returns("ack").Verifiable();
			mock.Expect(m => m.Do("ping")).Returns("ack");

			mock.Object.Do("ping");
			var bar = mock.Object.Bar;

			Assert.Throws<MockVerificationException>(() => mock.Verify());
		}

		[Fact]
		public void FieldAccessNotSupported()
		{
			var mock = new Mock<Foo>();

			Assert.Throws<NotSupportedException>(() => mock.Expect(m => m.BarField.Do("ping")));
		}

		[Fact]
		public void IntermediateMethodInvocationNotSupported()
		{
			var mock = new Mock<Foo>();

			Assert.Throws<NotSupportedException>(() => mock.Expect(m => m.GetBar().Do("ping")));
		}

		[Fact]
		public void NonMockeableTypeThrows()
		{
			var mock = new Mock<IFoo>();

			Assert.Throws<NotSupportedException>(() => mock.Expect(m => m.Bar.Value.ToString()));
		}

		[Fact]
		public void IntermediateIndexerAccessNotSupportedForNow()
		{
			var mock = new Mock<IFoo>();

			Assert.Throws<NotSupportedException>(() => mock.Expect(m => m[0].Do("ping")).Returns("ack"));
		}

		public class Foo : IFoo
		{

			public IBar BarField;
			public IBar Bar { get; set; }
			public IBar GetBar() { return null; }
			public IBar this[int index] { get { return null; } set { } }

			public string Do(string command)
			{
				throw new NotImplementedException();
			}
		}

		public interface IFoo
		{
			IBar Bar { get; set; }
			IBar this[int index] { get; set; }
			string Do(string command);
		}

		public interface IBar
		{
			int Value { get; set; }
			string Do(string command);
			IBaz Baz { get; set; }
		}

		public interface IBaz
		{
			string Do(string command);
		}
	}
}
