// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Xunit;

namespace Moq.Tests
{
	public class ExtensionsFixture
	{
		#region Public Methods

		[Fact]
		public void OnceDoesNotThrowOnSecondCallIfCountWasResetBefore()
		{
			var mock = new Mock<IFooReset>();
			mock.Setup(foo => foo.Execute("ping")).Returns("ack");

			mock.Object.Execute("ping");
			mock.Invocations.Clear();
			mock.Object.Execute("ping");
			mock.Verify(o => o.Execute("ping"), Times.Once());
		}

		[Fact]
		public void SetupDoesNotApplyAfterMockWasReset()
		{
			var mock = new Mock<IFooReset>();
			mock.Setup(foo => foo.Execute("ping")).Returns("ack");
			mock.Reset();

			var result = mock.Object.Execute("ping");
			Assert.Null(result);
		}

		[Fact]
		public void Loose()
		{
			var myMock = new Mock<IEnumerable<int>>(MockBehavior.Loose);
			myMock
				.Setup(a => a.ToString())
				.Returns("Hello");
			myMock.Reset();
			Assert.NotEqual("Hello", myMock.Object.ToString());
			myMock.VerifyAll();
		}

		[Fact]
		public void Strict()
		{
			var myMock = new Mock<IEnumerable<int>>(MockBehavior.Strict);
			myMock
				.Setup(a => a.ToString())
				.Returns("Hello");
			myMock.Reset();
			Assert.NotEqual("Hello", myMock.Object.ToString());
			myMock.VerifyAll();
		}

		[Fact]
		public void LooseNoCall()
		{
			var myMock = new Mock<IEnumerable<int>>(MockBehavior.Loose);
			myMock
				.Setup(a => a.ToString())
				.Returns("Hello");
			myMock.Reset();
			myMock.VerifyAll();
		}

		[Fact]
		public void StrictNoCall()
		{
			var myMock = new Mock<IEnumerable<int>>(MockBehavior.Strict);
			myMock
				.Setup(a => a.ToString())
				.Returns("Hello");
			myMock.Reset();
			myMock.VerifyAll();
		}

		[Fact]
		public void GetInvocationsOf_returns_only_invocations_of_right_method()
		{
			var myMock = new Mock<IMethods>(MockBehavior.Loose);

			myMock.Object.BoolAndParamsString(true);
			myMock.Object.OutInt(out int anInt);
			myMock.Object.BoolAndParamsString(true);

			var actualInvocationOnBoolAndParamsString = myMock.GetInvocationsOf(x => x.BoolAndParamsString(default));
			int ignoredInt;
			var actualInvocationOnOutInt = myMock.GetInvocationsOf(x => x.OutInt(out ignoredInt));

			Assert.Equal(new[] { myMock.Invocations[0], myMock.Invocations[2] }, actualInvocationOnBoolAndParamsString);
			Assert.Equal(new[] { myMock.Invocations[1] }, actualInvocationOnOutInt);
		}

		[Fact]
		public void GetInvocationsOf_returns_only_invocations_with_right_overload()
		{
			var myMock = new Mock<IMethods>(MockBehavior.Loose);

			myMock.Object.OverloadedMethod("hi");
			myMock.Object.OverloadedMethod("bye");

			var actualInvocationOnRightOverload = myMock.GetInvocationsOf(x => x.OverloadedMethod(default(string)));
			var actualInvocationOnWrongOverload = myMock.GetInvocationsOf(x => x.OverloadedMethod(default, default));

			Assert.Equal(myMock.Invocations, actualInvocationOnRightOverload);
			Assert.Empty(actualInvocationOnWrongOverload);
		}

		[Fact]
		public void GetInvocationsOf_returns_only_invocations_with_right_generic_type()
		{
			var myMock = new Mock<IMethods>(MockBehavior.Loose);

			myMock.Object.GenericMethod(5);

			var actualInvocationOnInt = myMock.GetInvocationsOf(x => x.GenericMethod<int>(default));
			var actualInvocationOnObject = myMock.GetInvocationsOf(x => x.GenericMethod<object>(default));
			var actualInvocationOnDouble = myMock.GetInvocationsOf(x => x.GenericMethod<double>(default));

			Assert.Equal(myMock.Invocations, actualInvocationOnInt);
			Assert.Empty(actualInvocationOnObject);
			Assert.Empty(actualInvocationOnDouble);
		}

		#endregion

		[Fact]
		public void IsExtensionMethod_recognizes_extension_method_as_such()
		{
			var isExtensionMethodMethod = typeof(Moq.Extensions).GetMethod(nameof(Moq.Extensions.IsExtensionMethod));

			Assert.True(isExtensionMethodMethod.IsExtensionMethod());
		}

		[Fact]
		public void IsExtensionMethod_does_not_recognize_method_as_extension_method()
		{
			var thisMethod = (MethodInfo)MethodBase.GetCurrentMethod();

			Assert.False(thisMethod.IsExtensionMethod());
		}

		[Theory]
		[InlineData(nameof(IMethods.Empty), "")]
		[InlineData(nameof(IMethods.Int), "int")]
		[InlineData(nameof(IMethods.IntAndString), "int, string")]
		[InlineData(nameof(IMethods.InInt), "in int")]
		[InlineData(nameof(IMethods.RefInt), "ref int")]
		[InlineData(nameof(IMethods.OutInt), "out int")]
		[InlineData(nameof(IMethods.BoolAndParamsString), "bool, params string[]")]
		public void GetParameterTypeList_formats_parameter_lists_correctly(string methodName, string expected)
		{
			var actual = GetParameterTypeList(methodName);
			Assert.Equal(expected, actual);
		}

		private string GetParameterTypeList(string methodName)
		{
			var method = typeof(IMethods).GetMethod(methodName);
			return method.GetParameterTypeList();
		}

		[Fact]
		public void CanRead_returns_false_for_true_write_only_property()
		{
			var property = typeof(WithWriteOnlyProperty).GetProperty("Property");
			Assert.False(property.CanRead(out var getter));
			Assert.Null(getter);
		}

		[Fact]
		public void CanRead_identifies_getter_in_true_read_only_property()
		{
			var property = typeof(WithReadOnlyProperty).GetProperty("Property");
			Assert.True(property.CanRead(out var getter));
			Assert.Equal(typeof(WithReadOnlyProperty), getter.DeclaringType);
		}

		[Fact]
		public void CanRead_identifies_getter_when_declared_in_base_class()
		{
			var property = typeof(OverridesOnlySetter).GetProperty("Property");
			Assert.False(property.CanRead);
			Assert.True(property.CanRead(out var getter));
			Assert.Equal(typeof(WithAutoProperty), getter.DeclaringType);
		}

		[Fact]
		public void CanWrite_returns_false_for_true_read_only_property()
		{
			var property = typeof(WithReadOnlyProperty).GetProperty("Property");
			Assert.False(property.CanWrite(out var setter));
			Assert.Null(setter);
		}

		[Fact]
		public void CanWrite_identifies_setter_in_true_write_only_property()
		{
			var property = typeof(WithWriteOnlyProperty).GetProperty("Property");
			Assert.True(property.CanWrite(out var setter));
			Assert.Equal(typeof(WithWriteOnlyProperty), setter.DeclaringType);
		}

		[Fact]
		public void CanWrite_identifies_setter_when_declared_in_base_class()
		{
			var property = typeof(OverridesOnlyGetter).GetProperty("Property");
			Assert.False(property.CanWrite);
			Assert.True(property.CanWrite(out var setter));
			Assert.Equal(typeof(WithAutoProperty), setter.DeclaringType);
		}

		public interface IMethods
		{
			void Empty();
			void Int(int arg1);
			void IntAndString(int arg1, string arg2);
			void InInt(in int arg1);
			void RefInt(ref int arg1);
			void OutInt(out int arg1);
			void BoolAndParamsString(bool arg1, params string[] arg2);
			void OverloadedMethod(string arg);
			void OverloadedMethod(int arg);
			void OverloadedMethod(string arg1, int arg2);
			void GenericMethod<T>(T arg);
		}

		public class WithReadOnlyProperty
		{
			public virtual object Property => null;
		}

		public class WithWriteOnlyProperty
		{
			public virtual object Property { set { } }
		}

		public class WithAutoProperty
		{
			public virtual object Property { get; set; }
		}

		public class OverridesOnlyGetter : WithAutoProperty
		{
			public override object Property { get => base.Property; }
		}

		public class OverridesOnlySetter : WithAutoProperty
		{
			public override object Property { set => base.Property = value; }
		}
	}

	public interface IFooReset
	{
		#region Public Methods

		object Execute(string ping);

		#endregion
	}
}
