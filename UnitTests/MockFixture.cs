using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections;
using System.Linq.Expressions;

namespace Moq.Tests
{
	[TestFixture]
	public class MockFixture
	{
		[Test]
		public void ShouldCreateMockAndExposeInterface()
		{
			var mock = new Mock<ICloneable>();

			ICloneable cloneable = mock.Value;

			Assert.IsNotNull(cloneable);
		}

		[Test]
		public void ShouldExpectCall()
		{
			var mock = new Mock<ICloneable>();
			var clone = new object();

			mock.Expect(x => x.Clone()).Returns(clone);

			Assert.AreEqual(clone, mock.Value.Clone());
		}

		[Test]
		public void ShouldExpectDifferentMethodCalls()
		{
			var mock = new Mock<IFoo>();
		
			mock.Expect(x => x.Do1()).Returns(1);
			mock.Expect(x => x.Do2()).Returns("foo");

			Assert.AreEqual(1, mock.Value.Do1());
			Assert.AreEqual("foo", mock.Value.Do2());
		}

		[Test]
		public void ShouldExpectCallWithArgument()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoInt(1)).Returns(11);
			mock.Expect(x => x.DoInt(2)).Returns(22);

			Assert.AreEqual(11, mock.Value.DoInt(1));
			Assert.AreEqual(22, mock.Value.DoInt(2));
		}

		[Test]
		public void ShouldExpectCallWithNullArgument()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoArgument(null)).Returns(1);
			mock.Expect(x => x.DoArgument("foo")).Returns(2);

			Assert.AreEqual(1, mock.Value.DoArgument(null));
			Assert.AreEqual(2, mock.Value.DoArgument("foo"));
		}

		[Test]
		public void ShouldExpectCallWithVariable()
		{
			int value = 25;
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoArgument(null)).Returns(value);

			Assert.AreEqual(value, mock.Value.DoArgument(null));
		}

		[Test]
		public void ShouldExpectCallWithLambdaLazyEvaluate()
		{
			int a = 25;
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoArgument(a.ToString())).Returns(() => a);
			a = 10;

			Assert.AreEqual(10, mock.Value.DoArgument("10"));
		}

		[Test]
		public void ShouldExpectReturnPropertyValue()
		{
			var mock = new Mock<IFoo>();
			
			mock.Expect(x => x.ValueProperty).Returns(25);

			Assert.AreEqual(25, mock.Value.ValueProperty);
		}

		[ExpectedException(typeof(NotSupportedException))]
		[Test]
		public void ShouldThrowIfExpectFieldValue()
		{
			var mock = new Mock<Foo>();

			mock.Expect(x => x.ValueField);
		}

		[Test]
		public void ShouldExpectMethodCallWithVariable()
		{
			int value = 5;
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Duplicate(value)).Returns(() => value * 2);

			Assert.AreEqual(value * 2, mock.Value.Duplicate(value));
		}

		[Test]
		public void ShouldExpectMethodCallWithMethodCall()
		{
			int value = 5;
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Duplicate(GetValue(value))).Returns(() => value * 2);

			Assert.AreEqual(value * 2, mock.Value.Duplicate(value * 2));
		}

		private int GetValue(int value)
		{
			return value * 2;
		}

		[Test]
		public void ShouldMatchAnyArgument()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Duplicate(It.IsAny<int>())).Returns(() => 5);

			Assert.AreEqual(5, mock.Value.Duplicate(5));
			Assert.AreEqual(5, mock.Value.Duplicate(25));
		}

		[Test]
		public void ShouldMatchPredicateArgument()
		{
			var mock = new Mock<IFoo>();

			mock.
				Expect(x => x.Duplicate(It.Is<int>(value => value < 5 && value > 0))).
				Returns(() => 1);
			mock.Expect(x => x.Duplicate(It.Is<int>(value => value <= 0))).Returns(() => 0);
			mock.Expect(x => x.Duplicate(It.Is<int>(value => value >= 5))).Returns(() => 2);

			Assert.AreEqual(1, mock.Value.Duplicate(3));
			Assert.AreEqual(0, mock.Value.Duplicate(0));
			Assert.AreEqual(0, mock.Value.Duplicate(-5));
			Assert.AreEqual(2, mock.Value.Duplicate(5));
			Assert.AreEqual(2, mock.Value.Duplicate(6));
		}

		[Test]
		public void ShouldExpectCallWithoutReturnValue()
		{
			int i = default(int);
			string str = default(string);

			Console.Write(i);
			Console.Write(str);

			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Execute());

			mock.Value.Execute();
		}

		[Test]
		public void ShouldNotThowIfUnexpectedCallWithoutReturnValue()
		{
			var mock = new Mock<IFoo>();

			mock.Value.Execute();
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[Test]
		public void ShouldThowIfUnexpectedCallWithReturnValue()
		{
			var mock = new Mock<IFoo>();

			int value = mock.Value.DoArgument("foo");
		}

		[ExpectedException(typeof(FormatException))]
		[Test]
		public void ShouldThrowIfExpectingThrows()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Do1()).Throws(new FormatException());

			mock.Value.Do1();
		}

		[Test]
		public void ShouldExecuteCallbackWhenVoidMethodIsCalled()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Expect(x => x.Execute()).Callback(() => called = true);

			mock.Value.Execute();
			Assert.IsTrue(called);
		}

		[Test]
		public void ShouldExecuteCallbackWhenNonVoidMethodIsCalled()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Expect(x => x.Do1()).Callback(() => called = true).Returns(1);

			Assert.AreEqual(1, mock.Value.Do1());
			Assert.IsTrue(called);
		}

		[Test]
		public void ShouldExpectRanges()
		{
			var mock = new Mock<IFoo>();
			
			mock.Expect(x => x.DoInt(It.IsInRange(1, 5, true))).Returns(1);
			mock.Expect(x => x.DoInt(It.IsInRange(6, 10, false))).Returns(2);

			Assert.AreEqual(1, mock.Value.DoInt(1));
			Assert.AreEqual(1, mock.Value.DoInt(2));
			Assert.AreEqual(1, mock.Value.DoInt(5));

			Assert.AreEqual(2, mock.Value.DoInt(7));
			Assert.AreEqual(2, mock.Value.DoInt(9));
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[Test]
		public void ShouldNotExpectOutOfRange()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoInt(It.IsInRange(1, 5, false))).Returns(1);

			Assert.AreEqual(1, mock.Value.DoInt(1));
		}

		// ShouldAllowDynamicResultThroughFunc
		// ShouldThrowIfStaticMethodArgumentDoesNotHaveComparerAttribute

		public class Foo : MarshalByRefObject
		{
			public int ValueField;
		}

		public interface IFoo
		{
			int DoInt(int arg);
			int Do1();
			string Do2();

			int DoArgument(string arg);

			int Duplicate(int value);

			void Execute();

			int ValueProperty { get; set; }
			int WriteOnlyValue { set; }
		}
	}
}
