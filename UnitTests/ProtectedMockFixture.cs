using System;
using Moq.Protected;
using Xunit;
using System.Linq.Expressions;

namespace Moq.Tests
{
	public partial class ProtectedMockFixture
	{
		[Fact]
		public void ThrowsIfNullMock()
		{
			Assert.Throws<ArgumentNullException>(() => ProtectedExtension.Protected((Mock<string>)null));
		}

		[Fact]
		public void ThrowsIfSetupNullVoidMethodName()
		{
			Assert.Throws<ArgumentNullException>(() => new Mock<FooBase>().Protected().Setup(null));
		}

		[Fact]
		public void ThrowsIfSetupEmptyVoidMethodName()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Setup(string.Empty));
		}

		[Fact]
		public void ThrowsIfSetupResultNullMethodName()
		{
			Assert.Throws<ArgumentNullException>(() => new Mock<FooBase>().Protected().Setup<int>(null));
		}

		[Fact]
		public void ThrowsIfSetupResultEmptyMethodName()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Setup<int>(string.Empty));
		}

		[Fact]
		public void ThrowsIfSetupVoidMethodNotFound()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Setup("Foo"));
		}

		[Fact]
		public void ThrowsIfSetupResultMethodNotFound()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Setup<int>("Foo"));
		}

		[Fact]
		public void ThrowsIfSetupPublicVoidMethod()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Setup("Public"));
		}

		[Fact]
		public void ThrowsIfSetupPublicResultMethod()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Setup<int>("PublicInt"));
		}

		[Fact]
		public void ThrowsIfSetupNonVirtualVoidMethod()
		{
			Assert.Throws<NotSupportedException>(() => new Mock<FooBase>().Protected().Setup("NonVirtual"));
		}

		[Fact]
		public void ThrowsIfSetupNonVirtualResultMethod()
		{
			Assert.Throws<NotSupportedException>(() => new Mock<FooBase>().Protected().Setup<int>("NonVirtualInt"));
		}

		[Fact]
		public void SetupAllowsProtectedInternalVoidMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Protected().Setup("ProtectedInternal");
			mock.Object.ProtectedInternal();

			mock.VerifyAll();
		}

		[Fact]
		public void SetupAllowsProtectedInternalResultMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Protected()
				.Setup<int>("ProtectedInternalInt")
				.Returns(5);

			Assert.Equal(5, mock.Object.ProtectedInternalInt());
		}

		[Fact]
		public void SetupAllowsProtectedVoidMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Protected().Setup("Protected");
			mock.Object.DoProtected();

			mock.VerifyAll();
		}

		[Fact]
		public void SetupAllowsProtectedResultMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Protected()
				.Setup<int>("ProtectedInt")
				.Returns(5);

			Assert.Equal(5, mock.Object.DoProtectedInt());
		}

		[Fact]
		public void ThrowsIfSetupVoidMethodIsProperty()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Setup("ProtectedValue"));
		}

		[Fact]
		public void SetupResultAllowsProperty()
		{
			var mock = new Mock<FooBase>();
			mock.Protected()
				.Setup<string>("ProtectedValue")
				.Returns("foo");

			Assert.Equal("foo", mock.Object.GetProtectedValue());
		}

		[Fact]
		public void ThrowsIfSetupGetNullPropertyName()
		{
			Assert.Throws<ArgumentNullException>(() => new Mock<FooBase>().Protected().SetupGet<string>(null));
		}

		[Fact]
		public void ThrowsIfSetupGetEmptyPropertyName()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().SetupGet<string>(string.Empty));
		}

		[Fact]
		public void ThrowsIfSetupGetPropertyNotFound()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().SetupGet<int>("Foo"));
		}

		[Fact]
		public void ThrowsIfSetupGetPropertyWithoutPropertyGet()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().SetupGet<int>("OnlySet"));
		}

		[Fact]
		public void ThrowsIfSetupGetPublicPropertyGet()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().SetupGet<int>("PublicValue"));
		}

		[Fact]
		public void ThrowsIfSetupGetNonVirtualProperty()
		{
			Assert.Throws<NotSupportedException>(() => new Mock<FooBase>().Protected().SetupGet<string>("NonVirtualValue"));
		}

		[Fact]
		public void SetupGetAllowsProtectedInternalPropertyGet()
		{
			var mock = new Mock<FooBase>();
			mock.Protected()
				.SetupGet<string>("ProtectedInternalValue")
				.Returns("foo");

			Assert.Equal("foo", mock.Object.ProtectedInternalValue);
		}

		[Fact]
		public void SetupGetAllowsProtectedPropertyGet()
		{
			var mock = new Mock<FooBase>();
			mock.Protected()
				.SetupGet<string>("ProtectedValue")
				.Returns("foo");

			Assert.Equal("foo", mock.Object.GetProtectedValue());
		}

		[Fact]
		public void ThrowsIfSetupSetNullPropertyName()
		{
			Assert.Throws<ArgumentNullException>(
				() => new Mock<FooBase>().Protected().SetupSet<string>(null, ItExpr.IsAny<string>()));
		}

		[Fact]
		public void ThrowsIfSetupSetEmptyPropertyName()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<FooBase>().Protected().SetupSet<string>(string.Empty, ItExpr.IsAny<string>()));
		}

		[Fact]
		public void ThrowsIfSetupSetPropertyNotFound()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<FooBase>().Protected().SetupSet<int>("Foo", ItExpr.IsAny<int>()));
		}

		[Fact]
		public void ThrowsIfSetupSetPropertyWithoutPropertySet()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<FooBase>().Protected().SetupSet<int>("OnlyGet", ItExpr.IsAny<int>()));
		}

		[Fact]
		public void ThrowsIfSetupSetPublicPropertySet()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<FooBase>().Protected().SetupSet<int>("PublicValue", ItExpr.IsAny<int>()));
		}

		[Fact]
		public void ThrowsIfSetupSetNonVirtualProperty()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<FooBase>().Protected().SetupSet<string>("NonVirtualValue", ItExpr.IsAny<string>()));
		}

		[Fact]
		public void SetupSetAllowsProtectedInternalPropertySet()
		{
			var mock = new Mock<FooBase>();
			var value = string.Empty;
			mock.Protected()
				.SetupSet<string>("ProtectedInternalValue", ItExpr.IsAny<string>())
				.Callback(v => value = v);

			mock.Object.ProtectedInternalValue = "foo";

			Assert.Equal("foo", value);
			mock.VerifyAll();
		}

		[Fact]
		public void SetupSetAllowsProtectedPropertySet()
		{
			var mock = new Mock<FooBase>();
			var value = string.Empty;
			mock.Protected()
				.SetupSet<string>("ProtectedValue", ItExpr.IsAny<string>())
				.Callback(v => value = v);

			mock.Object.SetProtectedValue("foo");

			Assert.Equal("foo", value);
			mock.VerifyAll();
		}

		[Fact]
		public void ThrowsIfNullArgs()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected()
				.Setup<string>("StringArg", null)
				.Returns("null"));
		}

		[Fact]
		public void AllowMatchersForArgs()
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
				.Setup<string>("StringArg", ItExpr.Is<string>(s => s.Length >= 2))
				.Returns("long");
			mock.Protected()
				.Setup<string>("StringArg", ItExpr.Is<string>(s => s.Length < 2))
				.Returns("short");

			Assert.Equal("short", mock.Object.DoStringArg("f"));
			Assert.Equal("long", mock.Object.DoStringArg("foo"));

			mock = new Mock<FooBase>();
			mock.CallBase = true;

			mock.Protected()
				.Setup<string>("TwoArgs", ItExpr.IsAny<string>(), 5)
				.Returns("done");

			Assert.Equal("done", mock.Object.DoTwoArgs("foobar", 5));
			Assert.Equal("echo", mock.Object.DoTwoArgs("echo", 15));

			mock = new Mock<FooBase>();
			mock.CallBase = true;

			mock.Protected()
				.Setup<string>("TwoArgs", ItExpr.IsAny<string>(), ItExpr.IsInRange(1, 3, Range.Inclusive))
				.Returns("inrange");

			Assert.Equal("inrange", mock.Object.DoTwoArgs("foobar", 2));
			Assert.Equal("echo", mock.Object.DoTwoArgs("echo", 4));
		}

		[Fact]
		public void ResolveOverloads()
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
		public void ThrowsIfSetReturnsForVoidMethod()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<MethodOverloads>().Protected().Setup<string>("Do", "1", "2").Returns("3"));
		}

		[Fact]
		public void SetupResultAllowsProtectedMethodInBaseClass()
		{
			var mock = new Mock<FooDerived>();
			mock.Protected()
				.Setup<int>("ProtectedInt")
				.Returns(5);

			Assert.Equal(5, mock.Object.DoProtectedInt());
		}

		[Fact]
		public void ThrowsIfVerifyNullVoidMethodName()
		{
			Assert.Throws<ArgumentNullException>(() => new Mock<FooBase>().Protected().Verify(null, Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyEmptyVoidMethodName()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Verify(string.Empty, Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyNullResultMethodName()
		{
			Assert.Throws<ArgumentNullException>(() => new Mock<FooBase>().Protected().Verify<int>(null, Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyEmptyResultMethodName()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Verify<int>(string.Empty, Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyVoidMethodNotFound()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Verify("Foo", Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyResultMethodNotFound()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Verify<int>("Foo", Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyPublicVoidMethod()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().Verify("Public", Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyPublicResultMethod()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<FooBase>().Protected().Verify<int>("PublicInt", Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyNonVirtualVoidMethod()
		{
			Assert.Throws<NotSupportedException>(() => new Mock<FooBase>().Protected().Verify("NonVirtual", Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyNonVirtualResultMethod()
		{
			Assert.Throws<NotSupportedException>(
				() => new Mock<FooBase>().Protected().Verify<int>("NonVirtualInt", Times.Once()));
		}

		[Fact]
		public void VerifyAllowsProtectedInternalVoidMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Object.ProtectedInternal();

			mock.Protected().Verify("ProtectedInternal", Times.Once());
		}

		[Fact]
		public void VerifyAllowsProtectedInternalResultMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Object.ProtectedInternalInt();

			mock.Protected().Verify<int>("ProtectedInternalInt", Times.Once());
		}

		[Fact]
		public void VerifyAllowsProtectedVoidMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Object.DoProtected();

			mock.Protected().Verify("Protected", Times.Once());
		}

		[Fact]
		public void VerifyAllowsProtectedResultMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Object.DoProtectedInt();

			mock.Protected().Verify<int>("ProtectedInt", Times.Once());
		}

		[Fact]
		public void ThrowsIfVerifyVoidMethodIsProperty()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<FooBase>().Protected().Verify("ProtectedValue", Times.Once()));
		}

		[Fact]
		public void VerifyResultAllowsProperty()
		{
			var mock = new Mock<FooBase>();
			mock.Object.GetProtectedValue();

			mock.Protected().Verify<string>("ProtectedValue", Times.Once());
		}

		[Fact]
		public void ThrowsIfVerifyVoidMethodTimesNotReached()
		{
			var mock = new Mock<FooBase>();
			mock.Object.DoProtected();

			Assert.Throws<MockException>(() => mock.Protected().Verify("Protected", Times.Exactly(2)));
		}

		[Fact]
		public void ThrowsIfVerifyResultMethodTimesNotReached()
		{
			var mock = new Mock<FooBase>();
			mock.Object.DoProtectedInt();

			Assert.Throws<MockException>(() => mock.Protected().Verify("ProtectedInt", Times.Exactly(2)));
		}

		[Fact]
		public void DoesNotThrowIfVerifyVoidMethodTimesReached()
		{
			var mock = new Mock<FooBase>();
			mock.Object.DoProtected();
			mock.Object.DoProtected();

			mock.Protected().Verify("Protected", Times.Exactly(2));
		}

		[Fact]
		public void DoesNotThrowIfVerifyReturnMethodTimesReached()
		{
			var mock = new Mock<FooBase>();
			mock.Object.DoProtectedInt();
			mock.Object.DoProtectedInt();

			mock.Protected().Verify<int>("ProtectedInt", Times.Exactly(2));
		}

		[Fact]
		public void ThrowsIfVerifyPropertyTimesNotReached()
		{
			var mock = new Mock<FooBase>();
			mock.Object.GetProtectedValue();

			Assert.Throws<MockException>(() => mock.Protected().Verify<string>("ProtectedValue", Times.Exactly(2)));
		}

		[Fact]
		public void DoesNotThrowIfVerifyPropertyTimesReached()
		{
			var mock = new Mock<FooBase>();
			mock.Object.GetProtectedValue();
			mock.Object.GetProtectedValue();

			mock.Protected().Verify<string>("ProtectedValue", Times.Exactly(2));
		}

		[Fact]
		public void ThrowsIfVerifyGetNullPropertyName()
		{
			Assert.Throws<ArgumentNullException>(() => new Mock<FooBase>().Protected().VerifyGet<int>(null, Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyGetEmptyPropertyName()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().VerifyGet<int>(string.Empty, Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyGetPropertyNotFound()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().VerifyGet<int>("Foo", Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyGetPropertyWithoutPropertyGet()
		{
			Assert.Throws<ArgumentException>(() => new Mock<FooBase>().Protected().VerifyGet<int>("OnlySet", Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyGetIsPublicPropertyGet()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<FooBase>().Protected().VerifyGet<string>("PublicValue", Times.Once()));
		}

		[Fact]
		public void VerifyGetAllowsProtectedInternalPropertyGet()
		{
			var mock = new Mock<FooBase>();
			var value = mock.Object.ProtectedInternalValue;

			mock.Protected().VerifyGet<string>("ProtectedInternalValue", Times.Once());
		}

		[Fact]
		public void VerifyGetAllowsProtectedPropertyGet()
		{
			var mock = new Mock<FooBase>();
			mock.Object.GetProtectedValue();

			mock.Protected().VerifyGet<string>("ProtectedValue", Times.Once());
		}

		[Fact]
		public void ThrowsIfVerifyGetNonVirtualPropertyGet()
		{
			Assert.Throws<NotSupportedException>(
				() => new Mock<FooBase>().Protected().VerifyGet<string>("NonVirtualValue", Times.Once()));
		}

		[Fact]
		public void ThrowsIfVerifyGetTimesNotReached()
		{
			var mock = new Mock<FooBase>();
			mock.Object.GetProtectedValue();

			Assert.Throws<MockException>(() => mock.Protected().VerifyGet<string>("ProtectedValue", Times.Exactly(2)));
		}

		[Fact]
		public void DoesNotThrowIfVerifyGetPropertyTimesReached()
		{
			var mock = new Mock<FooBase>();
			mock.Object.GetProtectedValue();
			mock.Object.GetProtectedValue();

			mock.Protected().VerifyGet<string>("ProtectedValue", Times.Exactly(2));
		}
	}

	public partial class ProtectedMockFixture
	{

		[Fact]
		public void ThrowsIfVerifySetNullPropertyName()
		{
			Assert.Throws<ArgumentNullException>(
				() => new Mock<FooBase>().Protected().VerifySet<string>(null, Times.Once(), ItExpr.IsAny<string>()));
		}

		[Fact]
		public void ThrowsIfVerifySetEmptyPropertyName()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<FooBase>().Protected().VerifySet<string>(string.Empty, Times.Once(), ItExpr.IsAny<int>()));
		}

		[Fact]
		public void ThrowsIfVerifySetPropertyNotFound()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<FooBase>().Protected().VerifySet<int>("Foo", Times.Once(), ItExpr.IsAny<int>()));
		}

		[Fact]
		public void ThrowsIfVerifySetPropertyWithoutPropertySet()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<FooBase>().Protected().VerifySet<int>("OnlyGet", Times.Once(), ItExpr.IsAny<int>()));
		}

		[Fact]
		public void ThrowsIfVerifySetPublicPropertySet()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<FooBase>().Protected().VerifySet<int>("PublicValue", Times.Once(), ItExpr.IsAny<int>()));
		}

		[Fact]
		public void ThrowsIfVerifySetNonVirtualPropertySet()
		{
			Assert.Throws<ArgumentException>(
				() => new Mock<FooBase>().Protected().VerifySet<string>("NonVirtualValue", Times.Once(), ItExpr.IsAny<string>()));
		}

		[Fact]
		public void VerifySetAllowsProtectedInternalPropertySet()
		{
			var mock = new Mock<FooBase>();
			mock.Object.ProtectedInternalValue = "foo";

			mock.Protected().VerifySet<string>("ProtectedInternalValue", Times.Once(), "bar");
		}

		[Fact]
		public void VerifySetAllowsProtectedPropertySet()
		{
			var mock = new Mock<FooBase>();
			mock.Object.SetProtectedValue("foo");

			mock.Protected().VerifySet<string>("ProtectedValue", Times.Once(), ItExpr.IsAny<string>());
		}

		[Fact]
		public void ThrowsIfVerifySetTimesNotReached()
		{
			var mock = new Mock<FooBase>();
			mock.Object.SetProtectedValue("Foo");

			Assert.Throws<MockException>(
				() => mock.Protected().VerifySet<string>("ProtectedValue", Times.Exactly(2), ItExpr.IsAny<string>()));
		}

		[Fact]
		public void DoesNotThrowIfVerifySetPropertyTimesReached()
		{
			var mock = new Mock<FooBase>();
			mock.Object.SetProtectedValue("foo");
			mock.Object.SetProtectedValue("foo");

			mock.Protected().VerifySet<string>("ProtectedValue", Times.Exactly(2), ItExpr.IsAny<int>());
		}

		public class MethodOverloads
		{
			public void ExecuteDo(int a, int b)
			{
				this.Do(a, b);
			}

			public void ExecuteDo(string a, string b)
			{
				this.Do(a, b);
			}

			public int ExecuteDoReturn(int a, int b)
			{
				return this.DoReturn(a, b);
			}

			protected virtual void Do(int a, int b)
			{
			}

			protected virtual void Do(string a, string b)
			{
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

		public class FooBase
		{
			public virtual string PublicValue { get; set; }

			protected internal virtual string ProtectedInternalValue { get; set; }

			protected string NonVirtualValue { get; set; }

			protected virtual int OnlyGet
			{
				get { return 0; }
			}

			protected virtual int OnlySet
			{
				set { }
			}

			protected virtual string ProtectedValue { get; set; }

			protected virtual int this[int index]
			{
				get { return 0; }
				set { }
			}

			public void DoProtected()
			{
				this.Protected();
			}

			public int DoProtectedInt()
			{
				return this.ProtectedInt();
			}

			public string DoStringArg(string arg)
			{
				return this.StringArg(arg);
			}

			public string DoTwoArgs(string arg, int arg1)
			{
				return this.TwoArgs(arg, arg1);
			}

			public string GetProtectedValue()
			{
				return this.ProtectedValue;
			}

			public virtual void Public()
			{
			}

			public virtual int PublicInt()
			{
				return 10;
			}

			public void SetProtectedValue(string value)
			{
				this.ProtectedValue = value;
			}

			internal protected virtual void ProtectedInternal()
			{
			}

			internal protected virtual int ProtectedInternalInt()
			{
				return 0;
			}

			protected virtual void Protected()
			{
			}

			protected virtual int ProtectedInt()
			{
				return 2;
			}

			protected void NonVirtual()
			{
			}

			protected int NonVirtualInt()
			{
				return 2;
			}

			protected virtual string StringArg(string arg)
			{
				return arg;
			}

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