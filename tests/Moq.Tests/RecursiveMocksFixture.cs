// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Xunit;

namespace Moq.Tests
{
	public class RecursiveMocksFixture
	{
		[Fact]
		public void CreatesMockForAccessedProperty()
		{
			var mock = new Mock<IFoo>();

			mock.SetupGet(m => m.Bar.Value).Returns(5);

			Assert.Equal(5, mock.Object.Bar.Value);
		}

		[Fact]
		public void RetrievesSameMockForProperty()
		{
			var mock = new Mock<IFoo> { DefaultValue = DefaultValue.Mock };

			var b1 = mock.Object.Bar;
			var b2 = mock.Object.Bar;

			Assert.Same(b1, b2);
		}

		[Fact]
		public void NewMocksHaveSameBehaviorAndDefaultValueAsOwner()
		{
			var mock = new Mock<IFoo>();

			mock.SetupGet(m => m.Bar.Value).Returns(5);

			var barMock = Mock.Get(mock.Object.Bar);

			Assert.Equal(mock.Behavior, barMock.Behavior);
			Assert.Equal(mock.DefaultValue, barMock.DefaultValue);
		}

		[Fact]
		public void CreatesMockForAccessedPropertyWithMethod()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Bar.Do("ping")).Returns("ack");
			mock.Setup(m => m.Bar.Baz.Do("ping")).Returns("ack");

			Assert.Equal("ack", mock.Object.Bar.Do("ping"));
			Assert.Equal("ack", mock.Object.Bar.Baz.Do("ping"));
			Assert.Equal(default(string), mock.Object.Bar.Do("foo"));
		}

		[Fact]
		public void CreatesMockForAccessedPropertyWithVoidMethod()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Bar.Baz.Do());

			//Assert.Throws<MockVerificationException>(() => mock.VerifyAll());

			Assert.NotNull(mock.Object.Bar);
			Assert.NotNull(mock.Object.Bar.Baz);

			mock.Object.Bar.Baz.Do();

			mock.Verify(m => m.Bar.Baz.Do());
		}

		[Fact]
		public void CreatesMockForAccessedPropertyWithSetterWithValue()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet(m => m.Bar.Value = 5);

			Assert.NotNull(mock.Object.Bar);
			var ex = Assert.Throws<MockException>(() => mock.VerifyAll());
			Assert.True(ex.IsVerificationError);

			mock.Object.Bar.Value = 5;

			mock.VerifyAll();
		}

		[Fact]
		public void CreatesMockForAccessedPropertyWithSetter()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet(m => m.Bar.Value = It.IsAny<int>());

			Assert.NotNull(mock.Object.Bar);
			var ex = Assert.Throws<MockException>(() => mock.VerifyAll());
			Assert.True(ex.IsVerificationError);

			mock.Object.Bar.Value = 5;

			mock.VerifyAll();
		}

		[Fact]
		public void VerifiesAllHierarchy()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Bar.Do("ping")).Returns("ack");
			mock.Setup(m => m.Do("ping")).Returns("ack");

			mock.Object.Do("ping");
			var bar = mock.Object.Bar;

			var ex = Assert.Throws<MockException>(() => mock.VerifyAll());
			Assert.True(ex.IsVerificationError);
		}

		[Fact]
		public void VerifiesHierarchy()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Bar.Do("ping")).Returns("ack").Verifiable();
			mock.Setup(m => m.Do("ping")).Returns("ack");

			mock.Object.Do("ping");
			var bar = mock.Object.Bar;

			var ex = Assert.Throws<MockException>(() => mock.Verify());
			Assert.True(ex.IsVerificationError);
		}

		[Fact]
		public void VerifiesHierarchyMethodWithExpression()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };

			Assert.Throws<MockException>(() => mock.Verify(m => m.Bar.Do("ping")));

			mock.Object.Bar.Do("ping");
			mock.Verify(m => m.Bar.Do("ping"));
		}

		[Fact]
		public void VerifiesHierarchyPropertyGetWithExpression()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };

			Assert.Throws<MockException>(() => mock.VerifyGet(m => m.Bar.Value));

			var value = mock.Object.Bar.Value;
			mock.VerifyGet(m => m.Bar.Value);
		}

		[Fact]
		public void VerifiesHierarchyPropertySetWithExpression()
		{
			var mock = new Mock<IFoo>() { DefaultValue = DefaultValue.Mock };

			Assert.Throws<MockException>(() => mock.VerifySet(m => m.Bar.Value = It.IsAny<int>()));

			mock.Object.Bar.Value = 5;
			mock.VerifySet(m => m.Bar.Value = It.IsAny<int>());
		}

		[Fact]
		public void VerifiesReturnWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Bar.Do("ping")).Returns("ack").Verifiable();

			Assert.Throws<MockException>(() => mock.Verify(m => m.Bar.Do("ping")));

			var result = mock.Object.Bar.Do("ping");

			Assert.Equal("ack", result);
			mock.Verify(m => m.Bar.Do("ping"));
		}

		[Fact]
		public void VerifiesVoidWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Bar.Baz.Do());

			Assert.Throws<MockException>(() => mock.Verify(m => m.Bar.Baz.Do()));

			mock.Object.Bar.Baz.Do();

			mock.Verify(m => m.Bar.Baz.Do());
		}

		[Fact]
		public void VerifiesGetWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.SetupGet(m => m.Bar.Value).Returns(5);

			Assert.Throws<MockException>(() => mock.VerifyGet(m => m.Bar.Value));

			var result = mock.Object.Bar.Value;

			Assert.Equal(5, result);

			mock.VerifyGet(m => m.Bar.Value);
		}

		[Fact]
		public void VerifiesGetWithExpression2()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.Bar.Value).Returns(5);

			Assert.Throws<MockException>(() => mock.Verify(m => m.Bar.Value));

			var result = mock.Object.Bar.Value;

			Assert.Equal(5, result);

			mock.Verify(m => m.Bar.Value);
		}

		[Fact]
		public void VerifiesSetWithExpression()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet(m => m.Bar.Value = It.IsAny<int>());

			Assert.Throws<MockException>(() => mock.VerifySet(m => m.Bar.Value = It.IsAny<int>()));

			mock.Object.Bar.Value = 5;

			mock.VerifySet(m => m.Bar.Value = It.IsAny<int>());
		}

		[Fact]
		public void VerifiesSetWithExpressionAndValue()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSet(m => m.Bar.Value = 5);

			Assert.Throws<MockException>(() => mock.VerifySet(m => m.Bar.Value = 5));

			mock.Object.Bar.Value = 5;

			mock.VerifySet(m => m.Bar.Value = 5);
		}

		[Fact]
		public void FieldAccessNotSupported()
		{
			var mock = new Mock<Foo>();

			Assert.Throws<ArgumentException>(() => mock.Setup(m => m.BarField.Do("ping")));
		}

		[Fact]
		public void NonMockeableTypeThrows()
		{
			var mock = new Mock<IFoo>();

			Assert.Throws<ArgumentException>(() => mock.Setup(m => m.Bar.Value.ToString()));
		}

		[Fact]
		public void IntermediateIndexerAccessIsSupported()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m[0].Do("ping")).Returns("ack");

			var result = mock.Object[0].Do("ping");

			Assert.Equal("ack", result);
		}

		[Fact]
		public void IntermediateMethodInvocationAreSupported()
		{
			var mock = new Mock<IFoo>();

			mock.Setup(m => m.GetBar().Do("ping")).Returns("ack");

			var result = mock.Object.GetBar().Do("ping");

			Assert.Equal("ack", result);
		}

		[Fact]
		public void FullMethodInvocationsSupportedInsideFluent()
		{
			var fooMock = new Mock<IFoo>(MockBehavior.Strict);
			fooMock.Setup(f => f.Bar.GetBaz("hey").Value).Returns(5);

			Assert.Equal(5, fooMock.Object.Bar.GetBaz("hey").Value);
		}

		[Fact]
		public void FullMethodInvocationInsideFluentCanUseMatchers()
		{
			var fooMock = new Mock<IFoo>(MockBehavior.Strict);
			fooMock.Setup(f => f.Bar.GetBaz(It.IsAny<string>()).Value).Returns(5);

			Assert.Equal(5, fooMock.Object.Bar.GetBaz("foo").Value);
		}

		[Fact]
		public void Param_array_args_in_setup_expression_parts_are_compared_by_structural_equality_not_reference_equality()
		{
			var mock = new Mock<IFoo>();
			mock.Setup(m => m.GetBar(1).Value).Returns(1);
			mock.Setup(m => m.GetBar(1).OtherValue).Returns(2);
			Assert.Equal(1, mock.Object.GetBar(1).Value);
			Assert.Equal(2, mock.Object.GetBar(1).OtherValue);
		}

		public class Verify_can_tell_apart_different_arguments_in_intermediate_part_of_fluent_expressions
		{
			[Fact]
			public void When_set_up_by_DefaultValue_Mock_1()
			{
				var mock = new Mock<IBar> { DefaultValue = DefaultValue.Mock };
				mock.Object.GetBaz("actual").Do();
				mock.Verify(m => m.GetBaz("something else").Do(), Times.Never);
			}

			[Fact]
			public void When_manually_set_up_1()
			{
				var mock = new Mock<IBar>();
				mock.Setup(m => m.GetBaz(It.IsAny<string>()).Do());
				mock.Object.GetBaz("actual").Do();
				mock.Verify(m => m.GetBaz("something else").Do(), Times.Never);
			}

			[Fact]
			public void When_set_up_by_DefaultValue_Mock_2()
			{
				var mock = new Mock<IBar> { DefaultValue = DefaultValue.Mock };
				mock.Object.GetBaz("first").Do();
				mock.Object.GetBaz("second").Do();
				mock.Verify(m => m.GetBaz("first").Do(), Times.Once);
				mock.Verify(m => m.GetBaz("second").Do(), Times.Once);
			}

			[Fact]
			public void When_manually_set_up_2()
			{
				var mock = new Mock<IBar> { DefaultValue = DefaultValue.Mock };
				mock.Object.GetBaz("first").Do();
				mock.Object.GetBaz("second").Do();
				mock.Verify(m => m.GetBaz("first").Do(), Times.Once);
				mock.Verify(m => m.GetBaz("second").Do(), Times.Once);
			}

			[Fact]
			public void When_set_up_by_DefaultValue_Mock_3()
			{
				var mock = new Mock<IBar> { DefaultValue = DefaultValue.Mock };
				mock.Object.GetBaz("first").Do();
				mock.Object.GetBaz("second").Do();
				mock.Verify(m => m.GetBaz(It.IsAny<string>()).Do(), Times.Exactly(2));
			}

			[Fact]
			public void When_manually_set_up_3()
			{
				var mock = new Mock<IBar> { DefaultValue = DefaultValue.Mock };
				mock.Object.GetBaz("first").Do();
				mock.Object.GetBaz("second").Do();
				mock.Verify(m => m.GetBaz(It.IsAny<string>()).Do(), Times.Exactly(2));
			}
		}

		public class Inner_mock_reachability
		{
			[Fact]
			public void Reachable_if_set_up_using_eager_Returns()
			{
				var bar = new Mock<IBar>();
				bar.Setup(b => b.Value).Returns(42);

				var foo = new Mock<IFoo>();
				foo.Setup(f => f.Bar).Returns(bar.Object);
				foo.Setup(f => f.Bar.Baz);

				Assert.Equal(42, foo.Object.Bar.Value);
				bar.VerifyGet(b => b.Value, Times.Once);
			}

			[Fact]
			public void Not_reachable_if_set_up_using_lazy_Returns()
			{
				var bar = new Mock<IBar>();
				bar.Setup(b => b.Value).Returns(42);

				var foo = new Mock<IFoo>();
				foo.Setup(f => f.Bar).Returns(() => bar.Object);
				//                            ^^^^^^
				// Main difference to the above test. What we want to test for here is
				// that Moq won't execute user-provided callbacks to figure out a setup's
				// return value (as this could have side effects without Moq's control).

				foo.Setup(f => f.Bar.Baz);
				//              ^^^^^
				// ... and because Moq can't query the above setup to figure out there's
				// already an inner mock attached, it will create a fresh setup instead,
				// effectively "cutting off" the above `IBar` mock.

				Assert.NotEqual(42, foo.Object.Bar.Value);
				bar.VerifyGet(b => b.Value, Times.Never);
			}
		}

		public class Foo : IFoo
		{
			public IBar BarField;
			public IBar Bar { get; set; }
			public IBar GetBar() { return null; }
			public IBar GetBar(params int[] indices) { return null; }
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
			IBar GetBar();
			IBar GetBar(params int[] indices);
		}

		public interface IBar
		{
			int Value { get; set; }
			int OtherValue { get; set; }
			string Do(string command);
			IBaz Baz { get; set; }
			IBaz GetBaz(string value);
		}

		public interface IBaz
		{
			int Value { get; set; }
			string Do(string command);
			void Do();
		}
	}
}
