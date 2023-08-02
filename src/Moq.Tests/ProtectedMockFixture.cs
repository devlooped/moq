// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

using Moq.Protected;

using Xunit;

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
			Assert.Throws<ArgumentNullException>(() => new Mock<FooBase>().Protected().Setup<int>(null));
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
			var actual = Record.Exception(() =>
			{
				new Mock<FooBase>().Protected().Setup("Foo");
			});

			Assert.IsType<ArgumentException>(actual);
			Assert.Equal("No protected method FooBase.Foo found whose signature is compatible with the provided arguments ().", actual.Message);
		}

		[Fact]
		public void ThrowsIfSetupResultMethodNotFound()
		{
			var actual = Record.Exception(() =>
			{
				new Mock<FooBase>().Protected().Setup<int>("Foo");
			});

			Assert.IsType<ArgumentException>(actual);
			Assert.Equal("No protected method FooBase.Foo found whose signature is compatible with the provided arguments ().", actual.Message);
		}

		[Fact]
		public void ThrowsIfSetupIncompatibleArgumentSupplied()
		{
			var actual = Record.Exception(() =>
			{
				new Mock<FooBase>().Protected().Setup<string>("StringArg", ItExpr.IsAny<int>());
			});

			Assert.IsType<ArgumentException>(actual);
			Assert.Equal("No protected method FooBase.StringArg found whose signature is compatible with the provided arguments (int).", actual.Message);
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

			mock.Protected().Setup("ProtectedInternalGeneric", new[] { typeof(int) }, false);
			mock.Object.ProtectedInternalGeneric<int>();

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

			mock.Protected()
				.Setup<int>("ProtectedInternalReturnGeneric", new[] { typeof(int) }, false)
				.Returns(5);

			Assert.Equal(5, mock.Object.ProtectedInternalReturnGeneric<int>());
		}

		[Fact]
		public void SetupAllowsProtectedVoidMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Protected().Setup("Protected");
			mock.Object.DoProtected();
			mock.Protected().Setup("ProtectedGeneric", new[] { typeof(int) }, false);
			mock.Object.DoProtectedGeneric<int>();

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

			mock.Protected()
				.Setup<int>("ProtectedReturnGeneric", new[] { typeof(int) }, false)
				.Returns(5);

			Assert.Equal(5, mock.Object.DoProtectedReturnGeneric<int>());
		}

		[Fact]
		public void SetupAllowsProtectedMethodWithNullableParameters()
		{
			int? input = 5;
			int expectedOutput = 6;

			var mock = new Mock<FooBase>();
			mock.Protected()
				.Setup<int>("ProtectedWithNullableIntParam", input)
				.Returns(expectedOutput);

			Assert.Equal(expectedOutput, mock.Object.DoProtectedWithNullableIntParam(input));

			mock.Protected()
				.Setup<int?>("ProtectedWithGenericParam", new[] { typeof(int?) }, false, input)
				.Returns(expectedOutput);

			Assert.Equal(expectedOutput, mock.Object.DoProtectedWithGenericParam<int?>(input));
		}

		[Fact]
		public void SetupAllowsGenericProtectedMethodWithDiffrentGenericArguments()
		{
			var mock = new Mock<FooBase>();
			mock.Protected().Setup("ProtectedGeneric", new[] { typeof(int) }, false);
			mock.Protected().Setup("ProtectedGeneric", new[] { typeof(string) }, false);
			mock.Object.DoProtectedGeneric<int>();
			mock.Object.DoProtectedGeneric<string>();

			mock.VerifyAll();

			mock.Protected()
				.Setup<int>("ProtectedInternalReturnGeneric", new[] { typeof(int) }, false)
				.Returns(5);
			mock.Protected()
				.Setup<string>("ProtectedInternalReturnGeneric", new[] { typeof(string) }, false)
				.Returns("s");

			Assert.Equal(5, mock.Object.ProtectedInternalReturnGeneric<int>());
			Assert.Equal("s", mock.Object.ProtectedInternalReturnGeneric<string>());
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
			Assert.Throws<NotSupportedException>(
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
		public void SetupSetUsesTheValueParameter()
		{
			var mock = new Mock<FooBase>();
			var value = string.Empty;
			mock.Protected().SetupSet<string>("ProtectedValue", "foo").Callback(v => value = v);

			mock.Object.SetProtectedValue("not foo");
			Assert.Equal(string.Empty, value);

			mock.Object.SetProtectedValue("foo");
			Assert.Equal("foo", value);
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
			mock.Protected().Setup("Do", new[] { typeof(int) }, false, 1, 3).Verifiable();
			mock.Protected().Setup<string>("DoReturn", "1", "2").Returns("3").Verifiable();
			mock.Protected().Setup<string>("DoReturn", new[] { typeof(int), typeof(string) }, false, 1, "2")
				.Returns("4").Verifiable();

			mock.Object.ExecuteDo(1, 2);
			mock.Object.ExecuteDo<int>(1, 3);
			Assert.Equal("3", mock.Object.ExecuteDoReturn("1", "2"));
			Assert.Equal("4", mock.Object.ExecuteDoReturn<int, string>(1, "2"));

			mock.Verify();
		}

		[Fact]
		public void ResolveOverloadsSameFirstParameterType()
		{
			var mock = new Mock<MethodOverloads>();
			mock.Protected().Setup("SameFirstParameter", new object(), new object()).Verifiable();
			mock.Protected().Setup("SameFirstParameter", new object()).Verifiable();

			mock.Object.ExecuteSameFirstParameter(new object());
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

			mock.Protected()
				.Setup<int>("ProtectedReturnGeneric", new[] { typeof(int) }, false)
				.Returns(5);
			Assert.Equal(5, mock.Object.DoProtectedReturnGeneric<int>());
		}

		[Fact]
		public void SetupResultDefaulTwoOverloadsWithDerivedClassThrowsInvalidOperationException()
		{
			var mock = new Mock<MethodOverloads>();
			Assert.Throws<InvalidOperationException>(() => mock.Protected()
				.Setup<FooBase>("Overloaded", ItExpr.IsAny<MyDerived>())
				.Returns(new FooBase()));
		}

		[Fact]
		public void SetupResultExactParameterMatchTwoOverloadsWithDerivedClassShouldNotThrow()
		{
			var mock = new Mock<MethodOverloads>();
			var fooBase = new FooBase();
			mock.Protected()
				.Setup<FooBase>("Overloaded", true, ItExpr.IsAny<MyDerived>())
				.Returns(fooBase);
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
			var actual = Record.Exception(() =>
			{
				new Mock<FooBase>().Protected().Verify("Foo", Times.Once());
			});

			Assert.IsType<ArgumentException>(actual);
			Assert.Equal("No protected method FooBase.Foo found whose signature is compatible with the provided arguments ().", actual.Message);
		}

		[Fact]
		public void ThrowsIfVerifyResultMethodNotFound()
		{
			var actual = Record.Exception(() =>
			{
				new Mock<FooBase>().Protected().Verify<int>("Foo", Times.Once());
			});

			Assert.IsType<ArgumentException>(actual);
			Assert.Equal("No protected method FooBase.Foo found whose signature is compatible with the provided arguments ().", actual.Message);
		}

		[Fact]
		public void ThrowsIfVerifyIncompatibleArgumentSupplied()
		{
			var actual = Record.Exception(() =>
			{
				new Mock<FooBase>().Protected().Verify<string>("StringArg", Times.Once(), ItExpr.IsAny<int>());
			});

			Assert.IsType<ArgumentException>(actual);
			Assert.Equal("No protected method FooBase.StringArg found whose signature is compatible with the provided arguments (int).", actual.Message);
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

			mock.Object.ProtectedInternalGeneric<int>();
			mock.Protected().Verify("ProtectedInternalGeneric", new[] { typeof(int) }, Times.Once());
		}

		[Fact]
		public void VerifyAllowsProtectedInternalResultMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Object.ProtectedInternalInt();

			mock.Protected().Verify<int>("ProtectedInternalInt", Times.Once());

			mock.Object.ProtectedInternalReturnGeneric<int>();

			mock.Protected().Verify<int>("ProtectedInternalReturnGeneric", new[] { typeof(int) }, Times.Once());
		}

		[Fact]
		public void VerifyAllowsProtectedVoidMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Object.DoProtected();
			mock.Protected().Verify("Protected", Times.Once());

			mock.Object.DoProtectedGeneric<int>();
			mock.Protected().Verify("ProtectedGeneric", new[] { typeof(int) }, Times.Once());
		}

		[Fact]
		public void VerifyAllowsProtectedResultMethod()
		{
			var mock = new Mock<FooBase>();
			mock.Object.DoProtectedInt();
			mock.Protected().Verify<int>("ProtectedInt", Times.Once());

			mock.Object.DoProtectedReturnGeneric<int>();
			mock.Protected().Verify<int>("ProtectedReturnGeneric", new[] { typeof(int) }, Times.Once());
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
			Assert.Throws<NotSupportedException>(
				() => new Mock<FooBase>().Protected().VerifySet<string>("NonVirtualValue", Times.Once(), ItExpr.IsAny<string>()));
		}

		[Fact]
		public void VerifySetAllowsProtectedInternalPropertySet()
		{
			var mock = new Mock<FooBase>();
			mock.Object.ProtectedInternalValue = "foo";

			Assert.Throws<MockException>(() => mock.Protected().VerifySet<string>("ProtectedInternalValue", Times.Once(), "bar"));
			mock.Protected().VerifySet<string>("ProtectedInternalValue", Times.Once(), "foo");
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

			mock.Protected().VerifySet<string>("ProtectedValue", Times.Exactly(2), ItExpr.IsAny<string>());
		}

		[Fact]
		public void SetupShouldWorkWithExpressionTypes()
		{
			var mock = new Mock<FooBase>();
			var mocked = mock.Object;
			var protectedMock = mock.Protected();

			var expression = Expression.Constant(1);
			Expression setExpression1 = null;
			protectedMock.SetupSet<Expression>("ExpressionProperty", expression).Callback(expr => setExpression1 = expr);
			mocked.SetExpressionProperty(expression);
			Assert.Same(expression, setExpression1);

			var expression2 = Expression.Constant(2);
			Expression setExpression2 = null;
			protectedMock.SetupSet<Expression>("ExpressionProperty", ItExpr.Is<ConstantExpression>(e => (int)e.Value == 2)).Callback(expr => setExpression2 = expr);
			mocked.SetExpressionProperty(expression2);
			Assert.Same(expression2, setExpression2);

			var constantExpression = Expression.Constant(1);
			ConstantExpression setConstantExpression = null;
			protectedMock.SetupSet<ConstantExpression>("NotAMatcherExpressionProperty", constantExpression).Callback(expr => setConstantExpression = expr);
			mocked.SetNotAMatcherExpressionProperty(constantExpression);
			Assert.Same(constantExpression, setConstantExpression);

			var constantExpression2 = Expression.Constant(2);
			ConstantExpression setConstantExpression2 = null;
			protectedMock.SetupSet<ConstantExpression>("NotAMatcherExpressionProperty", ItExpr.Is<ConstantExpression>(e => (int)e.Value == 2)).Callback(expr => setConstantExpression2 = expr);
			mocked.SetNotAMatcherExpressionProperty(constantExpression2);
			Assert.Same(constantExpression2, setConstantExpression2);

			var method = typeof(FooBase).GetMethod(nameof(FooBase.MethodForReflection));
			var methodCallExpression = Expression.Call(method);
			MethodCallExpression setMethodCallExpression = null;
			protectedMock.SetupSet<MethodCallExpression>("MatcherExpressionProperty", methodCallExpression).Callback(expr => setMethodCallExpression = expr);
			mocked.SetMatcherExpressionProperty(methodCallExpression);
			Assert.Same(methodCallExpression, setMethodCallExpression);

			var methodCallExpression2 = Expression.Call(typeof(FooBase).GetMethod(nameof(FooBase.MethodForReflection2)));
			MethodCallExpression setMethodCallExpression2 = null;
			protectedMock.SetupSet<MethodCallExpression>("MatcherExpressionProperty", ItExpr.Is<MethodCallExpression>(e => e.Method != method)).Callback(expr => setMethodCallExpression2 = expr);
			mocked.SetMatcherExpressionProperty(methodCallExpression2);
			Assert.Same(methodCallExpression2, setMethodCallExpression2);

			Expression<Func<int, bool>> lambdaExpression = i => i < 5;
			LambdaExpression setLambdaExpression = null;
			protectedMock.SetupSet<LambdaExpression>("LambdaExpressionProperty", lambdaExpression).Callback(expr => setLambdaExpression = expr);
			mocked.SetLambdaExpressionProperty(lambdaExpression);
			Assert.Same(lambdaExpression, setLambdaExpression);

			Expression<Func<int, int>> lambdaExpression2 = i => i;
			LambdaExpression setLambdaExpression2 = null;
			protectedMock.SetupSet<LambdaExpression>("LambdaExpressionProperty", ItExpr.Is<LambdaExpression>(e => e == lambdaExpression2)).Callback(expr => setLambdaExpression2 = expr);
			mocked.SetLambdaExpressionProperty(lambdaExpression2);
			Assert.Same(lambdaExpression2, setLambdaExpression2);

		}

		public class ExpectedException : Exception
		{
			private static ExpectedException instance = new ExpectedException();
			public static ExpectedException Instance => instance;
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

			public void ExecuteDo<T>(T a, T b)
			{
				this.Do<T>(a, b);
			}

			public void ExecuteDo<T, T2>(T a, T2 b)
			{
				this.Do<T, T2>(a, b);
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

			protected virtual void Do<T>(T a, T b)
			{
			}

			protected virtual void Do<T, T2>(T a, T2 b)
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

			public T ExecuteDoReturn<T>(T a, T b)
			{
				return DoReturn<T>(a, b);
			}

			public T2 ExecuteDoReturn<T, T2>(T a, T2 b)
			{
				return DoReturn(a, b);
			}

			protected virtual string DoReturn(string a, string b)
			{
				return a + b;
			}

			protected virtual T DoReturn<T>(T a, T b)
			{
				return a;
			}

			protected virtual T2 DoReturn<T, T2>(T a, T2 b)
			{
				return b;
			}

			public void ExecuteSameFirstParameter(object a) { }

			protected virtual void SameFirstParameter(object a) { }

			protected virtual void SameFirstParameter(object a, object b) { }

			protected virtual FooBase Overloaded(MyBase myBase)
			{
				return null;
			}

			protected virtual FooBase Overloaded(MyDerived myBase)
			{
				return null;
			}
		}

		public class FooBase
		{
			public static void MethodForReflection() { }
			public static void MethodForReflection2() { }
			protected virtual Expression ExpressionProperty { get; set; }

			public void SetExpressionProperty(Expression expression)
			{
				ExpressionProperty = expression;
			}

			protected virtual ConstantExpression NotAMatcherExpressionProperty { get; set; }

			public void SetNotAMatcherExpressionProperty(ConstantExpression expression)
			{
				NotAMatcherExpressionProperty = expression;
			}

			protected virtual MethodCallExpression MatcherExpressionProperty { get; set; }

			public void SetMatcherExpressionProperty(MethodCallExpression expression)
			{
				MatcherExpressionProperty = expression;
			}

			protected virtual LambdaExpression LambdaExpressionProperty { get; set; }
			
			public void SetLambdaExpressionProperty(LambdaExpression expression)
			{
				LambdaExpressionProperty = expression;
			}

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

			public void DoProtectedGeneric<T>()
			{
				this.ProtectedGeneric<T>();
			}

			public int DoProtectedInt()
			{
				return this.ProtectedInt();
			}

			public T DoProtectedReturnGeneric<T>()
			{
				return this.ProtectedReturnGeneric<T>();
			}

			public int DoProtectedWithNullableIntParam(int? value)
			{
				return this.ProtectedWithNullableIntParam(value);
			}

			public T DoProtectedWithGenericParam<T>(T value)
			{
				return this.ProtectedWithGenericParam(value);
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

			internal protected virtual void ProtectedInternalGeneric<T>()
			{
			}

			internal protected virtual int ProtectedInternalInt()
			{
				return 0;
			}

			internal protected virtual T ProtectedInternalReturnGeneric<T>()
			{
				return default(T);
			}

			protected virtual void Protected()
			{
			}

			protected virtual void ProtectedGeneric<T>()
			{
			}

			protected virtual int ProtectedInt()
			{
				return 2;
			}

			protected virtual T ProtectedReturnGeneric<T>()
			{
				return default(T);
			}

			protected virtual int ProtectedWithNullableIntParam(int? value)
			{
				return value ?? 0;
			}

			protected virtual T ProtectedWithGenericParam<T>(T value)
			{
				return value;
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

		public class MyBase { }

		public class MyDerived : MyBase { }
	}
}
