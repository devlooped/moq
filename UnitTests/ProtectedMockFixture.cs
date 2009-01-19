using System;
using Moq.Protected;
using Xunit;

namespace Moq.Tests
{
	public class ProtectedMockFixture
	{
		[Fact]
		public void ShouldThrowIfNullMock()
		{
			Assert.Throws<ArgumentNullException>(() => ProtectedExtension.Protected((Mock<string>)null));
		}

		[Fact]
		public void ShouldThrowIfNullMemberName()
		{
			Assert.Throws<ArgumentNullException>(() => new Mock<FooBase>().Protected().Setup((string)null));
		}

		[Fact]
		public void ShouldThrowIfNullMemberName2()
		{
			Assert.Throws<ArgumentNullException>(() => new Mock<FooBase>().Protected().Setup<int>((string)null));
		}

		[Fact]
		public void ShouldThrowIfEmptyMemberName()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Setup(string.Empty));
		}

		[Fact]
		public void ShouldThrowIfEmptyMemberName2()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Setup<int>(string.Empty));
		}

		[Fact]
		public void ShouldThrowIfVoidMethodNotFound()
		{
			var mock = new Mock<FooBase>();

			Assert.Throws<ArgumentException>(() => mock.Protected().Setup("Foo"));
		}

		[Fact]
		public void ShouldThrowIfVoidMethodIsPublic()
		{
			var mock = new Mock<FooBase>();

			Assert.Throws<ArgumentException>(() => mock.Protected().Setup("DoVoid"));
		}

		[Fact]
		public void ShouldNotThrowIfVoidMethodIsProtectedInternal()
		{
			var mock = new Mock<FooBase>();

			mock.Protected().Setup("ProtectedInternal");
		}

		[Fact]
		public void ShouldThrowIfVoidMethodIsInternal()
		{
			var mock = new Mock<FooBase>();

			Assert.Throws<ArgumentException>(() => mock.Protected().Setup("Internal"));
		}

		[Fact]
		public void ShouldThrowIfVoidMethodIsPublicProperty()
		{
			var mock = new Mock<FooBase>();

			Assert.Throws<ArgumentException>(() => mock.Protected().Setup("PublicValue"));
		}

		[Fact]
		public void ShouldThrowIfReturnMethodNotFound()
		{
			var mock = new Mock<FooBase>();

			Assert.Throws<ArgumentException>(() => mock.Protected().Setup<int>("Foo"));
		}

		[Fact]
		public void ShouldThrowIfReturnMethodIsPublic()
		{
			var mock = new Mock<FooBase>();

			Assert.Throws<ArgumentException>(() => mock.Protected().Setup<int>("DoInt"));
		}

		[Fact]
		public void ShouldNotThrowIfReturnMethodIsProtectedInternal()
		{
			var mock = new Mock<FooBase>();

			mock.Protected().Setup<int>("ProtectedInternalInt");
		}

		[Fact]
		public void ShouldThrowIfReturnMethodIsInternal()
		{
			var mock = new Mock<FooBase>();

			Assert.Throws<ArgumentException>(() => mock.Protected().Setup<int>("InternalInt"));
		}

		[Fact]
		public void ShouldThrowIfReturnMethodIsPublicProperty()
		{
			var mock = new Mock<FooBase>();

			Assert.Throws<ArgumentException>(() => mock.Protected().Setup<string>("PublicValue"));
		}

		[Fact]
		public void ShouldExpectProtectedMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Protected()
				 .Setup<int>("Int")
				 .Returns(5);

			Assert.Equal(5, mock.Object.DoInt());
		}

		[Fact]
		public void ShouldExpectProtectedInternalVirtualMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Protected()
				 .Setup<int>("ProtectedInternalInt")
				 .Returns(5);

			Assert.Equal(5, mock.Object.DoProtectedInternalInt());
		}

		[Fact]
		public void ShouldExpectProtectedMethodVoid()
		{
			var mock = new Mock<FooBase>();

			mock
				.Protected()
				.Setup("Void");

			mock.Object.DoVoid();

			mock.VerifyAll();
		}

		[Fact]
		public void ShouldExpectProperty()
		{
			var mock = new Mock<FooBase>();

			mock
				.Protected()
				.Setup<string>("ProtectedValue")
				.Returns("foo");

			Assert.Equal("foo", mock.Object.GetProtectedValue());
		}

		[Fact]
		public void ShouldExpectGetProperty()
		{
			var mock = new Mock<FooBase>();

			mock
				.Protected()
				.SetupGet<string>("ProtectedValue")
				.Returns("foo");

			Assert.Equal("foo", mock.Object.GetProtectedValue());
		}

		[Fact]
		public void ShouldExpectPropertySet()
		{
			var mock = new Mock<FooBase>();
			var value = "";

			mock
				.Protected()
				.SetupSet<string>("ProtectedValue")
				.Callback(v => value = v);

			mock.Object.SetProtectedValue("foo");

			Assert.Equal("foo", value);
			mock.VerifyAll();
		}

		[Fact]
		public void ShouldThrowIfNullPropertySet()
		{
			Assert.Throws<ArgumentNullException>(() => new Mock<FooBase>().Protected().SetupSet<string>(null));
		}

		[Fact]
		public void ShouldThrowIfEmptyPropertySet()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().SetupSet<string>(string.Empty));
		}

		[Fact]
		public void ShouldThrowIfPropertySetNotExists()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().SetupSet<string>("Foo"));
		}

		[Fact]
		public void ShouldThrowIfPropertySetIsPublic()
		{
			var mock = new Mock<FooBase>();

			Assert.Throws<ArgumentException>(() => mock.Protected().SetupSet<string>("PublicValue"));
		}

		[Fact]
		public void NullIsInvalidForArgument()
		{
			var mock = new Mock<FooBase>();

			Assert.Throws<ArgumentException>(() => mock.Protected()
				.Setup<string>("StringArg", null)
				.Returns("null"));
		}

		[Fact]
		public void ShouldAllowMatchersForArg()
		{
			var mock = new Mock<FooBase>();

			mock.Protected()
				.Setup<string>("StringArg", ItExpr.IsNull<string>())
				.Returns("null");

			Assert.Equal("null", mock.Object.DoStringArg(null));


			mock.Protected()
				.Setup<string>("StringArg", ItExpr.Is<string>(s => s == "bar"))
				.Returns("baz");

			Assert.Equal("baz", mock.Object.DoStringArg("bar"));


			mock = new Mock<FooBase>();

			mock.Protected()
				.Setup<string>("StringArg",
					ItExpr.Is<string>(s => s.Length >= 2))
				.Returns("long");
			mock.Protected()
				.Setup<string>("StringArg", 
					ItExpr.Is<string>(s => s.Length < 2))
				.Returns("short");

			Assert.Equal("short", mock.Object.DoStringArg("f"));
			Assert.Equal("long", mock.Object.DoStringArg("foo"));


			mock = new Mock<FooBase>();
			mock.CallBase = true;

			mock.Protected()
				.Setup<string>("TwoArgs",
					ItExpr.IsAny<string>(),
					5)
				.Returns("done");
	
			Assert.Equal("done", mock.Object.DoTwoArgs("foobar", 5));
			Assert.Equal("echo", mock.Object.DoTwoArgs("echo", 15));

			mock = new Mock<FooBase>();
			mock.CallBase = true;

			mock.Protected()
				.Setup<string>("TwoArgs",
					ItExpr.IsAny<string>(),
					ItExpr.IsInRange(1, 3, Range.Inclusive))
				.Returns("inrange");

			Assert.Equal("inrange", mock.Object.DoTwoArgs("foobar", 2));
			Assert.Equal("echo", mock.Object.DoTwoArgs("echo", 4));
		}

		[Fact]
		public void ShouldResolveOverloads()
		{
			// NOTE: There are two overloads named "Do" and "DoReturn"

			var mock = new Mock<MethodOverloads>();
			mock.Protected().Setup("Do", 1, 2).Verifiable();
			mock.Protected().Setup<string>("DoReturn", "1", "2").Returns("3").Verifiable();

			mock.Object.ExecuteDo(1, 2);
			Assert.Equal("3", mock.Object.ExecuteDoReturn("1", "2"));

			mock.Verify();
		}

		[Fact]
		public void ShouldThrowIfSetReturnForVoid()
		{
			var mock = new Mock<MethodOverloads>();

			Assert.Throws<ArgumentException>(() => mock.Protected().Setup<string>("Do", "1", "2").Returns("3"));
		}

		[Fact]
		public void ShouldExpectProtectedMethodInBaseClass()
		{
			var mock = new Mock<FooDerived>();
			mock.Protected()
				 .Setup<int>("Int")
				 .Returns(5);

			Assert.Equal(5, mock.Object.DoInt());
		}

		public class MethodOverloads
		{
			public void ExecuteDo(int a, int b)
			{
				Do(a, b);
			}

			protected virtual void Do(int a, int b)
			{
			}

			public void ExecuteDo(string a, string b)
			{
				Do(a, b);
			}

			protected virtual void Do(string a, string b)
			{
			}

			public int ExecuteDoReturn(int a, int b)
			{
				return DoReturn(a, b);
			}

			protected virtual int DoReturn(int a, int b)
			{
				return a + b;
			}

			public string ExecuteDoReturn(string a, string b)
			{
				return DoReturn(a, b);
			}

			protected virtual string DoReturn(string a, string b)
			{
				return a + b;
			}
		}

		// ShouldExpectIndexedProperty

		public class FooBase
		{
			protected event EventHandler Doing;

			internal protected virtual int ProtectedInternal() { return 0; }
			internal virtual void Internal() { }

			public int DoProtectedInternalInt() { return ProtectedInternalInt(); }
			internal protected virtual int ProtectedInternalInt() { return 0; }
			internal virtual int InternalInt() { return 0; }

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

		public class FooDerived : FooBase
		{
		}
	}
}
