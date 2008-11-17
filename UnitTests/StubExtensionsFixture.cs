using Xunit;
using Moq.Stub;
using System;
using System.Reflection;

namespace Moq.Tests
{
	public class StubExtensionsFixture
	{
		[Fact]
		public void ShouldExpectPropertyGetterAndSetterWithSameExpression()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectGet(x => x.ValueProperty).Returns(25);
			mock.ExpectSet(x => x.ValueProperty);

			mock.Object.ValueProperty = 7;
			Assert.Equal(25, mock.Object.ValueProperty);
		}

		[Fact]
		public void ShouldExpectPropertyGetterAndSetterWithoutSameExpression()
		{
			var mock = new Mock<IFoo>();

			mock.ExpectGet(x => x.ValueProperty).Returns(25);
			mock.ExpectSet(y => y.ValueProperty);

			mock.Object.ValueProperty = 7;
			Assert.Equal(25, mock.Object.ValueProperty);
		}

		[Fact]
		public void ShouldStubPropertyWithoutInitialValue()
		{
			var mock = new Mock<IFoo>();

			mock.Stub(f => f.ValueProperty);

			Assert.Equal(0, mock.Object.ValueProperty);

			mock.Object.ValueProperty = 5;

			Assert.Equal(5, mock.Object.ValueProperty);
		}

		[Fact]
		public void ShouldStubPropertyWithInitialValue()
		{
			var mock = new Mock<IFoo>();

			mock.Stub(f => f.ValueProperty, 5);

			Assert.Equal(5, mock.Object.ValueProperty);

			mock.Object.ValueProperty = 15;

			Assert.Equal(15, mock.Object.ValueProperty);
		}

		[Fact]
		public void StubsAllProperties()
		{
			var mock = new Mock<IFoo>();

			mock.StubAll();

			mock.Object.ValueProperty = 5;
			Assert.Equal(5, mock.Object.ValueProperty);

			var obj = new object();
			mock.Object.Object = obj;
			Assert.Same(obj, mock.Object.Object);

			var bar = new Mock<IBar>();
			mock.Object.Bar = bar.Object;
			Assert.Same(bar.Object, mock.Object.Bar);
		}

		[Fact]
		public void StubsAllHierarchy()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };

			mock.StubAll();

			mock.Object.Bar.Value = 5;
			Assert.Equal(5, mock.Object.Bar.Value);
		}

		[Fact]
		public void StubsInheritedInterfaceProperties()
		{
			var mock = new Mock<IBaz>();

			mock.StubAll();

			mock.Object.Value = 5;
			Assert.Equal(5, mock.Object.Value);

			mock.Object.Name = "foo";
			Assert.Equal("foo", mock.Object.Name);
		}

		[Fact]
		public void StubsInheritedClassProperties()
		{
			var mock = new Mock<Base>();

			mock.StubAll();

			mock.Object.BaseValue = 5;
			Assert.Equal(5, mock.Object.BaseValue);

			mock.Object.Value = 10;
			Assert.Equal(10, mock.Object.Value);
		}

		private object GetValue() { return new object(); }

		public interface IFoo
		{
			int ValueProperty { get; set; }
			object Object { get; set; }
			IBar Bar { get; set; }
		}

		public class Derived : Base
		{
			public string Name { get; set; }
		}

		public abstract class Base : IBar
		{
			public int BaseValue { get; set; }
			public int Value { get; set; }
		}

		public interface IBar
		{
			int Value { get; set; }
		}

		public interface IBaz : IBar
		{
			string Name { get; set; }
		}
	}
}
