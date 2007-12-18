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

			ICloneable cloneable = mock.Instance;

			Assert.IsNotNull(cloneable);
		}

		[Test]
		public void ShouldExpectCallReturn()
		{
			var mock = new Mock<ICloneable>();
			var clone = new object();

			mock.Expect(x => x.Clone()).Returns(clone);

			Assert.AreEqual(clone, mock.Instance.Clone());
		}

		[Test]
		public void ShouldExpectDifferentMethodCalls()
		{
			var mock = new Mock<IFoo>();
		
			mock.Expect(x => x.Do1()).Returns(1);
			mock.Expect(x => x.Do2()).Returns("foo");

			Assert.AreEqual(1, mock.Instance.Do1());
			Assert.AreEqual("foo", mock.Instance.Do2());
		}

		[Test]
		public void ShouldExpectCallWithArgument()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoInt(1)).Returns(11);
			mock.Expect(x => x.DoInt(2)).Returns(22);

			Assert.AreEqual(11, mock.Instance.DoInt(1));
			Assert.AreEqual(22, mock.Instance.DoInt(2));
		}

		[Test]
		public void ShouldExpectCallWithNullArgument()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoArgument(null)).Returns(1);
			mock.Expect(x => x.DoArgument("foo")).Returns(2);

			Assert.AreEqual(1, mock.Instance.DoArgument(null));
			Assert.AreEqual(2, mock.Instance.DoArgument("foo"));
		}

		[Test]
		public void ShouldExpectCallWithVariable()
		{
			int value = 25;
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoArgument(null)).Returns(value);

			Assert.AreEqual(value, mock.Instance.DoArgument(null));
		}

		[Test]
		public void ShouldExpectCallWithReferenceLazyEvaluate()
		{
			int a = 25;
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoArgument(a.ToString())).Returns(() => a);
			a = 10;

			Assert.AreEqual(10, mock.Instance.DoArgument("10"));

			a = 20;

			Assert.AreEqual(20, mock.Instance.DoArgument("20"));
		}

		[Test]
		public void ShouldExpectReturnPropertyValue()
		{
			var mock = new Mock<IFoo>();
			
			mock.Expect(x => x.ValueProperty).Returns(25);

			Assert.AreEqual(25, mock.Instance.ValueProperty);
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

			Assert.AreEqual(value * 2, mock.Instance.Duplicate(value));
		}

		[Test]
		public void ShouldExpectMethodCallWithMethodCall()
		{
			int value = 5;
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Duplicate(GetValue(value))).Returns(() => value * 2);

			Assert.AreEqual(value * 2, mock.Instance.Duplicate(value * 2));
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

			Assert.AreEqual(5, mock.Instance.Duplicate(5));
			Assert.AreEqual(5, mock.Instance.Duplicate(25));
		}

		[Test]
		public void ShouldMatchPredicateArgument()
		{
			var mock = new Mock<IFoo>();

			mock.
				Expect(x => x.Duplicate(It.Is<int>(value => value < 5 && value > 0))).
				Returns(() => 1);
			mock.
				Expect(x => x.Duplicate(It.Is<int>(value => value <= 0))).
				Returns(() => 0);
			mock.
				Expect(x => x.Duplicate(It.Is<int>(value => value >= 5))).
				Returns(() => 2);

			Assert.AreEqual(1, mock.Instance.Duplicate(3));
			Assert.AreEqual(0, mock.Instance.Duplicate(0));
			Assert.AreEqual(0, mock.Instance.Duplicate(-5));
			Assert.AreEqual(2, mock.Instance.Duplicate(5));
			Assert.AreEqual(2, mock.Instance.Duplicate(6));
		}

		[Test]
		public void ShouldExpectCallWithoutReturnValue()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Execute());

			mock.Instance.Execute();
		}

		[Test]
		public void ShouldNotThowIfUnexpectedCallWithoutReturnValue()
		{
			var mock = new Mock<IFoo>();

			mock.Instance.Execute();
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[Test]
		public void ShouldThowIfUnexpectedCallWithReturnValue()
		{
			var mock = new Mock<IFoo>();

			int value = mock.Instance.DoArgument("foo");
		}

		[ExpectedException(typeof(FormatException))]
		[Test]
		public void ShouldThrowIfExpectingThrows()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Do1()).Throws(new FormatException());

			mock.Instance.Do1();
		}

		[Test]
		public void ShouldExecuteCallbackWhenVoidMethodIsCalled()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Expect(x => x.Execute()).Callback(() => called = true);

			mock.Instance.Execute();
			Assert.IsTrue(called);
		}

		[Test]
		public void ShouldExecuteCallbackWhenNonVoidMethodIsCalled()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Expect(x => x.Do1()).Callback(() => called = true).Returns(1);

			Assert.AreEqual(1, mock.Instance.Do1());
			Assert.IsTrue(called);
		}

		[Test]
		public void ShouldExpectRanges()
		{
			var mock = new Mock<IFoo>();
			
			mock.Expect(x => x.DoInt(It.IsInRange(1, 5, Range.Inclusive))).Returns(1);
			mock.Expect(x => x.DoInt(It.IsInRange(6, 10, Range.Exclusive))).Returns(2);

			Assert.AreEqual(1, mock.Instance.DoInt(1));
			Assert.AreEqual(1, mock.Instance.DoInt(2));
			Assert.AreEqual(1, mock.Instance.DoInt(5));

			Assert.AreEqual(2, mock.Instance.DoInt(7));
			Assert.AreEqual(2, mock.Instance.DoInt(9));
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[Test]
		public void ShouldNotExpectOutOfRange()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoInt(It.IsInRange(1, 5, Range.Exclusive))).Returns(1);

			Assert.AreEqual(1, mock.Instance.DoInt(1));
		}

		[Test]
		public void ShouldExpectRangeWithVariableAndMethodInvocation()
		{
			var mock = new Mock<IFoo>();
			var from = 1;

			mock.Expect(x => x.DoInt(It.IsInRange(from, GetToRange(), Range.Inclusive))).Returns(1);

			Assert.AreEqual(1, mock.Instance.DoInt(1));
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[Test]
		public void ShouldExpectRangeLazyEval()
		{
			var mock = new Mock<IFoo>();
			var from = "a";
			var to = "d";

			mock.Expect(x => x.DoArgument(It.IsInRange(from, to, Range.Inclusive))).Returns(1);

			Assert.AreEqual(1, mock.Instance.DoArgument("b"));

			from = "c";

			Assert.AreEqual(1, mock.Instance.DoArgument("b"));
		}

		private int GetToRange()
		{
			return 5;
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
			void Execute(string arg1, int arg2);

			int ValueProperty { get; set; }
			int WriteOnlyValue { set; }
		}
	}
}
