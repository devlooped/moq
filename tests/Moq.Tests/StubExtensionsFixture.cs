// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using Xunit;

namespace Moq.Tests
{
	public class StubExtensionsFixture
	{
		[Fact]
		public void ShouldStubPropertyWithoutInitialValue()
		{
			var mock = new Mock<IFoo>();

			mock.SetupProperty(f => f.ValueProperty);

			Assert.Equal(0, mock.Object.ValueProperty);

			mock.Object.ValueProperty = 5;

			Assert.Equal(5, mock.Object.ValueProperty);
		}

		[Fact]
		public void ShouldStubPropertyWithInitialValue()
		{
			var mock = new Mock<IFoo>();

			mock.SetupProperty(f => f.ValueProperty, 5);

			Assert.Equal(5, mock.Object.ValueProperty);

			mock.Object.ValueProperty = 15;

			Assert.Equal(15, mock.Object.ValueProperty);
		}

		[Fact]
		public void StubsAllProperties()
		{
			var mock = new Mock<IFoo>();

			mock.SetupAllProperties();

			// Verify defaults
			Assert.Equal(default(int), mock.Object.ValueProperty);
			Assert.Null(mock.Object.Object);
			Assert.Null(mock.Object.Bar);
			Assert.Null(mock.Object.GetOnly);

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
		public void StubsAllCyrcularDependency()
		{
			var mock = new Mock<IHierarchy>();

			mock.SetupAllProperties();

			Assert.Null(mock.Object.Hierarchy);
		}

		[Fact]
		public void StubsAllHierarchy()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };

			mock.SetupAllProperties();

			mock.Object.Bar.Value = 5;
			Assert.Equal(5, mock.Object.Bar.Value);
		}

		[Fact]
		public void StubsInheritedInterfaceProperties()
		{
			var mock = new Mock<IBaz>();

			mock.SetupAllProperties();

			mock.Object.Value = 5;
			Assert.Equal(5, mock.Object.Value);

			mock.Object.Name = "foo";
			Assert.Equal("foo", mock.Object.Name);
		}

		[Fact]
		public void StubsInheritedClassProperties()
		{
			var mock = new Mock<Base>();

			mock.SetupAllProperties();

			mock.Object.BaseValue = 5;
			Assert.Equal(5, mock.Object.BaseValue);

			mock.Object.Value = 10;
			Assert.Equal(10, mock.Object.Value);
		}

		[Fact]
		public void Property_stubbed_by_SetupAllProperties_during_DefaultValue_Mock_has_initial_value_included_in_verification()
		{
			var mock = new Mock<IFoo> { DefaultValue = DefaultValue.Mock };
			mock.SetupAllProperties();

			var inner = Mock.Get(mock.Object.Bar);
			inner.SetupGet(m => m.Value).Verifiable();

			Assert.Throws<MockException>(() => mock.Verify());
		}

		[Fact]
		public void Property_stubbed_by_SetupAllProperties_during_DefaultValue_Mock_can_retain_new_values()
		{
			var mock = new Mock<IFoo> { DefaultValue = DefaultValue.Mock };
			mock.SetupAllProperties();

			mock.Object.Bar = new Bar { Value = 1 };
			Assert.Equal(1, mock.Object.Bar.Value);

			mock.Object.Bar = new Bar { Value = 2 };
			Assert.Equal(2, mock.Object.Bar.Value);
		}

		[Fact]
		public void Property_stubbed_by_SetupAllProperties_during_DefaultValue_Mock_can_retain_new_values_after_initial_value_queried()
		{
			var mock = new Mock<IFoo> { DefaultValue = DefaultValue.Mock };
			mock.SetupAllProperties();

			_ = mock.Object.Bar;  // this is the only difference between this test and the previous one

			mock.Object.Bar = new Bar { Value = 1 };
			Assert.Equal(1, mock.Object.Bar.Value);

			mock.Object.Bar = new Bar { Value = 2 };
			Assert.Equal(2, mock.Object.Bar.Value);
		}
		
		[Fact]
		public void Property_stubbed_by_SetupAllProperties_should_capture_DefaultValue_behaviour()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };
			mock.SetupAllProperties();
			mock.DefaultValue = DefaultValue.Empty;

			mock.Object.Bar.Value = 5;
			Assert.Equal(5, mock.Object.Bar.Value);
		}
		
		[Fact]
		public void Property_stubbed_by_SetupAllProperties_should_capture_DefaultValue_behaviour_for_inner_mocks()
		{
			var mock = new Mock<IHierarchy>() { DefaultValue = DefaultValue.Mock };
			mock.SetupAllProperties();
			mock.DefaultValue = DefaultValue.Empty;

			Assert.NotNull(mock.Object.Hierarchy.Hierarchy);
		}

		public abstract class WriteOnlyProperty
		{
			public abstract string Test { set; }
		}

		[Fact]
		public void Write_only_property_should_be_ignored_by_SetupAllProperties()
		{
			var mock = new Mock<WriteOnlyProperty>();
			mock.SetupAllProperties();
			
			mock.Object.Test = "test";
		}

		private object GetValue() { return new object(); }

		public interface IFoo
		{
			int ValueProperty { get; set; }
			object Object { get; set; }
			IBar Bar { get; set; }
			object GetOnly { get; }
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

		class Bar : IBar
		{
			public int Value { get; set; }
		}

		public interface IBaz : IBar
		{
			string Name { get; set; }
		}

		public interface IHierarchy
		{
			IHierarchy Hierarchy { get; set; }
		}
	}
}
