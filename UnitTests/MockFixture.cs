using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Collections;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Moq.Tests
{
	[TestFixture]
	public class MockFixture
	{
		[Test]
		public void ShouldCreateMockAndExposeInterface()
		{
			var mock = new Mock<ICloneable>();
			
			ICloneable cloneable = mock.Object;

			Assert.IsNotNull(cloneable);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfNullExpectAction()
		{
			var mock = new Mock<ICloneable>();

			mock.Expect((Expression<Action<ICloneable>>)null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfNullExpectFunction()
		{
			var mock = new Mock<ICloneable>();

			mock.Expect((Expression<Func<ICloneable, string>>)null);
		}

		[Test]
		public void ShouldExpectCallReturn()
		{
			var mock = new Mock<ICloneable>();
			var clone = new object();

			mock.Expect(x => x.Clone()).Returns(clone);

			Assert.AreEqual(clone, mock.Object.Clone());
		}

		[Test]
		public void ShouldExpectDifferentMethodCalls()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Do1()).Returns(1);
			mock.Expect(x => x.Do2()).Returns("foo");

			Assert.AreEqual(1, mock.Object.Do1());
			Assert.AreEqual("foo", mock.Object.Do2());
		}

		[Test]
		public void ShouldExpectCallWithArgument()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoInt(1)).Returns(11);
			mock.Expect(x => x.DoInt(2)).Returns(22);

			Assert.AreEqual(11, mock.Object.DoInt(1));
			Assert.AreEqual(22, mock.Object.DoInt(2));
		}

		[Test]
		public void ShouldExpectCallWithNullArgument()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoArgument(null)).Returns(1);
			mock.Expect(x => x.DoArgument("foo")).Returns(2);

			Assert.AreEqual(1, mock.Object.DoArgument(null));
			Assert.AreEqual(2, mock.Object.DoArgument("foo"));
		}

		[Test]
		public void ShouldExpectCallWithVariable()
		{
			int value = 25;
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoArgument(null)).Returns(value);

			Assert.AreEqual(value, mock.Object.DoArgument(null));
		}

		[Test]
		public void ShouldExpectCallWithReferenceLazyEvaluate()
		{
			int a = 25;
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.DoArgument(a.ToString())).Returns(() => a);
			a = 10;

			Assert.AreEqual(10, mock.Object.DoArgument("10"));

			a = 20;

			Assert.AreEqual(20, mock.Object.DoArgument("20"));
		}

		[Test]
		public void ShouldExpectReturnPropertyValue()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.ValueProperty).Returns(25);

			Assert.AreEqual(25, mock.Object.ValueProperty);
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

			Assert.AreEqual(value * 2, mock.Object.Duplicate(value));
		}

		[Test]
		public void ShouldExpectMethodCallWithMethodCall()
		{
			int value = 5;
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Duplicate(GetValue(value))).Returns(() => value * 2);

			Assert.AreEqual(value * 2, mock.Object.Duplicate(value * 2));
		}

		private int GetValue(int value)
		{
			return value * 2;
		}

		[Test]
		public void ShouldMatchAnyArgument()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Duplicate(It.IsAny<int>())).Returns(5);

			Assert.AreEqual(5, mock.Object.Duplicate(5));
			Assert.AreEqual(5, mock.Object.Duplicate(25));
		}

		[Test]
		public void ShouldMatchPredicateArgument()
		{
			var mock = new Mock<IFoo>();

			mock.
				Expect(x => x.Duplicate(It.Is<int>(value => value < 5 && value > 0))).
				Returns(1);
			mock.
				Expect(x => x.Duplicate(It.Is<int>(value => value <= 0))).
				Returns(0);
			mock.
				Expect(x => x.Duplicate(It.Is<int>(value => value >= 5))).
				Returns(2);

			Assert.AreEqual(1, mock.Object.Duplicate(3));
			Assert.AreEqual(0, mock.Object.Duplicate(0));
			Assert.AreEqual(0, mock.Object.Duplicate(-5));
			Assert.AreEqual(2, mock.Object.Duplicate(5));
			Assert.AreEqual(2, mock.Object.Duplicate(6));
		}

		[Test]
		public void ShouldExpectCallWithoutReturnValue()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Execute());

			mock.Object.Execute();
		}

		[Test]
		public void ShouldNotThowIfUnexpectedCallWithoutReturnValue()
		{
			var mock = new Mock<IFoo>();

			mock.Object.Execute();
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[Test]
		public void ShouldThowIfUnexpectedCallWithReturnValue()
		{
			var mock = new Mock<IFoo>();

			int value = mock.Object.DoArgument("foo");
		}

		[ExpectedException(typeof(FormatException))]
		[Test]
		public void ShouldThrowIfExpectingThrows()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.Do1()).Throws(new FormatException());

			mock.Object.Do1();
		}

		[Test]
		public void ShouldExecuteCallbackWhenVoidMethodIsCalled()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Expect(x => x.Execute()).Callback(() => called = true);

			mock.Object.Execute();
			Assert.IsTrue(called);
		}

		[Test]
		public void ShouldExecuteCallbackWhenNonVoidMethodIsCalled()
		{
			var mock = new Mock<IFoo>();
			bool called = false;
			mock.Expect(x => x.Do1()).Callback(() => called = true).Returns(1);

			Assert.AreEqual(1, mock.Object.Do1());
			Assert.IsTrue(called);
		}

		[Test]
		public void ShouldExpectRanges()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoInt(It.IsInRange(1, 5, Range.Inclusive))).Returns(1);
			mock.Expect(x => x.DoInt(It.IsInRange(6, 10, Range.Exclusive))).Returns(2);

			Assert.AreEqual(1, mock.Object.DoInt(1));
			Assert.AreEqual(1, mock.Object.DoInt(2));
			Assert.AreEqual(1, mock.Object.DoInt(5));

			Assert.AreEqual(2, mock.Object.DoInt(7));
			Assert.AreEqual(2, mock.Object.DoInt(9));
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[Test]
		public void ShouldNotExpectOutOfRange()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoInt(It.IsInRange(1, 5, Range.Exclusive))).Returns(1);

			Assert.AreEqual(1, mock.Object.DoInt(1));
		}

		[Test]
		public void ShouldExpectRangeWithVariableAndMethodInvocation()
		{
			var mock = new Mock<IFoo>();
			var from = 1;

			mock.Expect(x => x.DoInt(It.IsInRange(from, GetToRange(), Range.Inclusive))).Returns(1);

			Assert.AreEqual(1, mock.Object.DoInt(1));
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[Test]
		public void ShouldExpectRangeLazyEval()
		{
			var mock = new Mock<IFoo>();
			var from = "a";
			var to = "d";

			mock.Expect(x => x.DoArgument(It.IsInRange(from, to, Range.Inclusive))).Returns(1);

			Assert.AreEqual(1, mock.Object.DoArgument("b"));

			from = "c";

			Assert.AreEqual(1, mock.Object.DoArgument("b"));
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[Test]
		public void ShouldExpectMatchRegexAndLazyEval()
		{
			var mock = new Mock<IFoo>();
			var reg = "[a-d]+";

			mock.Expect(x => x.DoArgument(It.IsRegex(reg))).Returns(1);
			mock.Expect(x => x.DoArgument(It.IsRegex(reg, RegexOptions.IgnoreCase))).Returns(2);

			Assert.AreEqual(1, mock.Object.DoArgument("b"));
			Assert.AreEqual(1, mock.Object.DoArgument("abc"));
			Assert.AreEqual(2, mock.Object.DoArgument("B"));
			Assert.AreEqual(2, mock.Object.DoArgument("BC"));

			reg = "[c-d]+";

			Assert.AreEqual(1, mock.Object.DoArgument("b"));
		}

		[Test]
		public void ShouldReturnService()
		{
			var provider = new Mock<IServiceProvider>();

			provider.Expect(x => x.GetService(typeof(IFooService))).Returns(new FooService());

			Assert.That(provider.Object.GetService(typeof(IFooService)) is FooService);
		}

		[Test]
		public void ShouldCallFirstExpectThatMatches()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.Execute("ping")).Returns("I'm alive!"); 
			mock.Expect(x => x.Execute(It.IsAny<string>())).Throws(new ArgumentException());

			Assert.AreEqual("I'm alive!", mock.Object.Execute("ping"));
			try
			{
				mock.Object.Execute("asdf");
				Assert.Fail("didn't throw");
			}
			catch (ArgumentException)
			{
			}
		}

		[Test]
		public void ShouldTestEvenNumbers()
		{
			var mock = new Mock<IFoo>();

			mock.Expect(x => x.DoInt(It.Is<int>(i => i % 2 == 0))).Returns(1);

			Assert.AreEqual(1, mock.Object.DoInt(2));
		}

		[Test]
		public void ShouldGetTypeReturnATypeAssignableFromInterfaceType()
		{
			var mock = new Mock<IFoo>();
			Assert.IsTrue(typeof(IFoo).IsAssignableFrom(mock.Object.GetType()));
		}

		[Test]
		public void ShouldEqualsMethodWorkAsReferenceEquals()
		{
			var mock1 = new Mock<IFoo>();
			var mock2 = new Mock<IFoo>();

			Assert.IsTrue(mock1.Object.Equals(mock1.Object));
			Assert.IsFalse(mock1.Object.Equals(mock2.Object));
		}

		[Test]
		public void ShouldGetHashCodeReturnDifferentCodeForEachMock()
		{
			var mock1 = new Mock<IFoo>();
			var mock2 = new Mock<IFoo>();

			Assert.AreEqual(mock1.Object.GetHashCode(), mock1.Object.GetHashCode());
			Assert.AreEqual(mock2.Object.GetHashCode(), mock2.Object.GetHashCode());
			Assert.AreNotEqual(mock1.Object.GetHashCode(), mock2.Object.GetHashCode());
		}

		[Test]
		public void ShouldToString()
		{
			var mock = new Mock<IFoo>();
			Assert.IsFalse(string.IsNullOrEmpty(mock.Object.ToString()));
		}

		[Test]
		public void ShouldOverrideObjectMethods()
		{
			var mock = new Mock<IFoo>();
			mock.Expect(x => x.ToString()).Returns("foo");

			Assert.AreEqual("foo", mock.Object.ToString());
		}

		class FooService : IFooService { }
		interface IFooService { }

		// ShouldAllowDynamicResultThroughFunc

		private int GetToRange()
		{
			return 5;
		}

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
			string Execute(string command);
			void Execute(string arg1, int arg2);

			int ValueProperty { get; set; }
			int WriteOnlyValue { set; }

		}
	}
}
