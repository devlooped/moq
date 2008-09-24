using System;
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
		public void NewMocksHaveSameBehaviorAndDefaultValueAsOwner()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectGet(m => m.Bar.Value).Returns(5);

			var barMock = Mock.Get(mock.Object.Bar);

			Assert.Equal(mock.Behavior, barMock.Behavior);
			Assert.Equal(mock.DefaultValue, barMock.DefaultValue);
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
		public void CreatesMockForAccessedPropertyWithVoidMethod()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(m => m.Bar.Baz.Do());

			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			Assert.NotNull(mock.Object.Bar);
			Assert.NotNull(mock.Object.Bar.Baz);

			mock.Object.Bar.Baz.Do();

			mock.VerifyAll();
		}

		[Fact]
		public void CreatesMockForAccessedPropertyWithSetterWithValue()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectSet(m => m.Bar.Value, 5);

			Assert.NotNull(mock.Object.Bar);
			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Bar.Value = 5;

			mock.VerifyAll();
		}

		[Fact]
		public void CreatesMockForAccessedPropertyWithSetter()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectSet(m => m.Bar.Value);

			Assert.NotNull(mock.Object.Bar);
			Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			mock.Object.Bar.Value = 5;

			mock.VerifyAll();
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
		public void VerifiesReturnWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(m => m.Bar.Do("ping")).Returns("ack").Verifiable();

			Assert.Throws<MockException>(() => mock.Verify(m => m.Bar.Do("ping")));

			var result = mock.Object.Bar.Do("ping");

			Assert.Equal("ack", result);
			mock.Verify(m => m.Bar.Do("ping"));
		}

		[Fact]
		public void VerifiesVoidWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(m => m.Bar.Baz.Do());

			Assert.Throws<MockException>(() => mock.Verify(m => m.Bar.Baz.Do()));

			mock.Object.Bar.Baz.Do();

			mock.Verify(m => m.Bar.Baz.Do());
		}

		[Fact]
		public void VerifiesGetWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectGet(m => m.Bar.Value).Returns(5);

			Assert.Throws<MockException>(() => mock.VerifyGet(m => m.Bar.Value));

			var result = mock.Object.Bar.Value;

			Assert.Equal(5, result);

			mock.VerifyGet(m => m.Bar.Value);
		}

		[Fact]
		public void VerifiesGetWithExpression2()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(m => m.Bar.Value).Returns(5);

			Assert.Throws<MockException>(() => mock.Verify(m => m.Bar.Value));

			var result = mock.Object.Bar.Value;

			Assert.Equal(5, result);

			mock.Verify(m => m.Bar.Value);
		}

		[Fact]
		public void VerifiesSetWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectSet(m => m.Bar.Value);

			Assert.Throws<MockException>(() => mock.VerifySet(m => m.Bar.Value));

			mock.Object.Bar.Value = 5;

			mock.VerifySet(m => m.Bar.Value);
		}

		[Fact]
		public void VerifiesSetWithExpressionAndValue()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectSet(m => m.Bar.Value, 5);

			Assert.Throws<MockException>(() => mock.VerifySet(m => m.Bar.Value, 5));

			mock.Object.Bar.Value = 5;

			mock.VerifySet(m => m.Bar.Value, 5);
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
			void Do();
		}
	}
}
