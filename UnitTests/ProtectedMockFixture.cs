using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq.Protected;
using System.Linq.Expressions;

namespace Moq.Tests
{
	[TestFixture]
	public class ProtectedMockFixture
	{
		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfNullMock()
		{
			ProtectedExtension.Protected((Mock<string>)null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfNullMemberName()
		{
			new Mock<FooBase>().Protected().Expect((string)null);
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfNullMemberName2()
		{
			new Mock<FooBase>().Protected().Expect<int>((string)null);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfEmptyMemberName()
		{
			new Mock<FooBase>().Protected().Expect(string.Empty);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfEmptyMemberName2()
		{
			new Mock<FooBase>().Protected().Expect<int>(string.Empty);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfVoidMethodNotFound()
		{
			var mock = new Mock<FooBase>();

			mock.Protected().Expect("Foo");
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfVoidMethodIsPublic()
		{
			var mock = new Mock<FooBase>();

			mock.Protected().Expect("DoVoid");
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfVoidMethodIsProtectedInternal()
		{
			var mock = new Mock<FooBase>();

			mock.Protected().Expect("ProtectedInternal");
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfVoidMethodIsInternal()
		{
			var mock = new Mock<FooBase>();

			mock.Protected().Expect("Internal");
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfVoidMethodIsPublicProperty()
		{
			var mock = new Mock<FooBase>();

			mock.Protected().Expect("PublicValue");
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfReturnMethodNotFound()
		{
			var mock = new Mock<FooBase>();

			mock.Protected().Expect<int>("Foo");
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfReturnMethodIsPublic()
		{
			var mock = new Mock<FooBase>();

			mock.Protected().Expect<int>("DoInt");
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfReturnMethodIsProtectedInternal()
		{
			var mock = new Mock<FooBase>();

			mock.Protected().Expect<int>("ProtectedInternalInt");
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfReturnMethodIsInternal()
		{
			var mock = new Mock<FooBase>();

			mock.Protected().Expect<int>("InternalInt");
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfReturnMethodIsPublicProperty()
		{
			var mock = new Mock<FooBase>();

			mock.Protected().Expect<string>("PublicValue");
		}

		[Test]
		public void ShouldExpectProtectedMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Protected()
				 .Expect<int>("Int")
				 .Returns(5);

			Assert.AreEqual(5, mock.Object.DoInt());
		}

		[Test]
		public void ShouldExpectProtectedMethodVoid()
		{
			var mock = new Mock<FooBase>();

			mock
				.Protected()
				.Expect("Void");

			mock.Object.DoVoid();

			mock.VerifyAll();
		}

		[Test]
		public void ShouldExpectProperty()
		{
			var mock = new Mock<FooBase>();

			mock
				.Protected()
				.Expect<string>("ProtectedValue")
				.Returns("foo");

			Assert.AreEqual("foo", mock.Object.GetProtectedValue());
		}

		[Test]
		public void ShouldExpectGetProperty()
		{
			var mock = new Mock<FooBase>();

			mock
				.Protected()
				.ExpectGet<string>("ProtectedValue")
				.Returns("foo");

			Assert.AreEqual("foo", mock.Object.GetProtectedValue());
		}

		[Test]
		public void ShouldExpectPropertySet()
		{
			var mock = new Mock<FooBase>();
			var value = "";

			mock
				.Protected()
				.ExpectSet<string>("ProtectedValue")
				.Callback(v => value = v);

			mock.Object.SetProtectedValue("foo");

			Assert.AreEqual("foo", value);
			mock.VerifyAll();
		}

		[ExpectedException(typeof(ArgumentNullException))]
		[Test]
		public void ShouldThrowIfNullPropertySet()
		{
			new Mock<FooBase>().Protected().ExpectSet<string>(null);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfEmptyPropertySet()
		{
			new Mock<FooBase>().Protected().ExpectSet<string>(string.Empty);
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfPropertySetNotExists()
		{
			new Mock<FooBase>().Protected().ExpectSet<string>("Foo");
		}

		[ExpectedException(typeof(ArgumentException))]
		[Test]
		public void ShouldThrowIfPropertySetIsPublic()
		{
			var mock = new Mock<FooBase>();

			mock.Protected().ExpectSet<string>("PublicValue");
		}

		[Test]
		public void ShouldAllowMatchersForArg()
		{
			var mock = new Mock<FooBase>();

			mock.Protected()
				.Expect<string>("StringArg", ItExpr.Is<string>(s => s == "bar"))
				.Returns("baz");

			Assert.AreEqual("baz", mock.Object.DoStringArg("bar"));


			mock.Protected()
				.Expect<string>("StringArg",
					ItExpr.Is<string>(s => s.Length >= 2))
				.Returns("long");
			mock.Protected()
				.Expect<string>("StringArg", 
					ItExpr.Is<string>(s => s.Length < 2))
				.Returns("short");

			Assert.AreEqual("short", mock.Object.DoStringArg("f"));
			Assert.AreEqual("long", mock.Object.DoStringArg("foo"));


			mock.Protected()
				.Expect<string>("TwoArgs",
					ItExpr.IsAny<string>(),
					5)
				.Returns("done");
	
			Assert.AreEqual("done", mock.Object.DoTwoArgs("foobar", 5));
			Assert.AreEqual("echo", mock.Object.DoTwoArgs("echo", 15));

			mock.Protected()
				.Expect<string>("TwoArgs",
					ItExpr.IsAny<string>(),
					ItExpr.IsInRange(1, 3, Range.Inclusive))
				.Returns("inrange");

			Assert.AreEqual("inrange", mock.Object.DoTwoArgs("foobar", 2));
			Assert.AreEqual("echo", mock.Object.DoTwoArgs("echo", 4));
		}

		// ShouldExpectIndexedProperty

		public class FooBase
		{
			internal protected virtual void ProtectedInternal() { }
			internal virtual void Internal() { }
			internal protected virtual void ProtectedInternalInt() { }
			internal virtual void InternalInt() { }

			public void DoVoid() { Void(); }
			protected virtual void Void() { }

			public void DoVoidArg(string arg) { VoidArg(arg); }
			protected virtual void VoidArg(string arg) { }

			public int DoInt() { return Int(); }
			protected virtual int Int() { return 0; }

			public string DoStringArg(string arg) { return StringArg(arg); }
			protected virtual string StringArg(string arg) { return arg; }

			public string PublicValue { get; set; }
			public string GetProtectedValue() { return ProtectedValue; }
			public void SetProtectedValue(string value) { ProtectedValue = value; }
			protected virtual string ProtectedValue { get; set; }

			protected virtual int this[int index] { get { return 0; } set { } }


			public string DoTwoArgs(string arg, int arg1) { return TwoArgs(arg, arg1); } 
			protected virtual string TwoArgs(string arg, int arg1)
			{
				return arg;
			}
		}
	}
}
