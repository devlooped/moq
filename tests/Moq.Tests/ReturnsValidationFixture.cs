// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Moq.Language.Flow;

using Xunit;

namespace Moq.Tests
{
	public class ReturnsValidationFixture
	{
		private Mock<IType> mock;
		private ISetup<IType, IType> setup;
		private ISetup<IType, IType> setupNoArgs;

		public ReturnsValidationFixture()
		{
			this.mock = new Mock<IType>();
			this.setup = this.mock.Setup(m => m.Method(It.IsAny<object>(), It.IsAny<object>()));
			this.setupNoArgs = this.mock.Setup(m => m.MethodNoArgs());
		}

		[Fact]
		public void Returns_does_not_accept_delegate_without_return_value()
		{
			Action<object> delegateWithoutReturnValue = (arg) => { };

			var ex = Record.Exception(() =>
			{
				this.setup.Returns(delegateWithoutReturnValue);
			});

			Assert.IsType<ArgumentException>(ex);
		}

		[Fact]
		public void Returns_does_not_accept_delegate_with_wrong_return_type()
		{
			Func<string> delegateWithWrongReturnType = () => "42";

			var ex = Record.Exception(() =>
			{
				this.setup.Returns(delegateWithWrongReturnType);
			});

			Assert.IsType<ArgumentException>(ex);
		}

		[Fact]
		public void Returns_accepts_parameterless_delegate_for_method_without_parameters()
		{
			Func<IType> delegateWithoutParameters = () => default(IType);
			this.setupNoArgs.Returns(delegateWithoutParameters);

			var ex = Record.Exception(() =>
			{
				this.mock.Object.MethodNoArgs();
			});

			Assert.Null(ex);
		}

		[Fact]
		public void Returns_accepts_parameterless_delegate_even_for_method_having_parameters()
		{
			Func<IType> delegateWithoutParameters = () => default(IType);
			this.setup.Returns(delegateWithoutParameters);

			var ex = Record.Exception(() =>
			{
				this.mock.Object.Method(42, 3);
			});

			Assert.Null(ex);
		}

		[Fact]
		public void Returns_does_not_accept_delegate_with_wrong_parameter_count()
		{
			Func<object, object, object, IType> delegateWithWrongParameterCount = (arg1, arg2, arg3) => default(IType);

			var ex = Record.Exception(() =>
			{
				this.setup.Returns(delegateWithWrongParameterCount);
			});

			Assert.IsType<ArgumentException>(ex);
		}

		[Fact]
		public void Returns_accepts_delegate_with_wrong_parameter_types_but_setup_invocation_will_fail()
		{
			Func<string, string, IType> delegateWithWrongParameterType = (arg1, arg2) => default(IType);
			this.setup.Returns(delegateWithWrongParameterType);

			var ex = Record.Exception(() =>
			{
				mock.Object.Method(42, 7);
			});

			Assert.IsType<ArgumentException>(ex);

			// In case you're wondering why this use case isn't "fixed" by properly validating delegates
			// passed to `Returns`... it's entirely possible that some people might do this:
			//
			// mock.Setup(m => m.Method(It.IsAny<int>()).Returns<int>(obj => ...);
			// mock.Setup(m => m.Method(It.IsAny<string>()).Returns<string>(obj => ...);
			//
			// where `Method` has a parameter of type `object`. That is, people might rely on a matcher
			// to ensure that the return callback delegate invocation (and the cast to `object` that has to
			// happen) will always succeed. See also the next test, as well as old Google Code issue 267
			// in `IssueReportsFixture.cs`.
			//
			// While not the cleanest of techniques, it might be useful to some people and probably
			// shouldn't be broken by eagerly validating everything.
		}

		[Fact]
		public void Returns_accepts_delegate_with_wrong_parameter_types_and_setup_invocation_will_succeed_if_args_convertible()
		{
			Func<string, string, IType> delegateWithWrongParameterType = (arg1, arg2) => default(IType);
			this.setup.Returns(delegateWithWrongParameterType);

			var ex = Record.Exception(() =>
			{
				mock.Object.Method(null, null);
			});

			Assert.Null(ex);
		}

		[Fact]
		public void Returns_accepts_parameterless_extension_method_for_method_without_parameters()
		{
			Func<IType> delegateWithoutParameters = new ReturnsValidationFixture().ExtensionMethodNoArgs;
			this.setupNoArgs.Returns(delegateWithoutParameters);

			var ex = Record.Exception(() =>
			{
				this.mock.Object.MethodNoArgs();
			});

			Assert.Null(ex);
		}

		[Fact]
		public void Returns_accepts_parameterless_extension_method_even_for_method_having_parameters()
		{
			Func<IType> delegateWithoutParameters = new ReturnsValidationFixture().ExtensionMethodNoArgs;
			this.setup.Returns(delegateWithoutParameters);

			var ex = Record.Exception(() =>
			{
				this.mock.Object.Method(42, 5);
			});

			Assert.Null(ex);
		}

		[Fact]
		public void Returns_does_not_accept_extension_method_with_wrong_parameter_count()
		{
			Func<object, object, IType> delegateWithWrongParameterCount = new ReturnsValidationFixture().ExtensionMethod;

			var ex = Record.Exception(() =>
			{
				this.setupNoArgs.Returns(delegateWithWrongParameterCount);
			});

			Assert.IsType<ArgumentException>(ex);
		}

		[Fact]
		public void Returns_accepts_extension_method_with_correct_parameter_count()
		{
			Func<object, object, IType> delegateWithCorrectParameterCount = new ReturnsValidationFixture().ExtensionMethod;
			this.setup.Returns(delegateWithCorrectParameterCount);

			var ex = Record.Exception(() =>
			{
				this.mock.Object.Method(42, 5);
			});

			Assert.Null(ex);
		}

		public interface IType
		{
			IType Method(object arg1, object arg2);

			IType MethodNoArgs();
		}
	}

	static class ReturnsValidationFixtureExtensions
	{
		internal static ReturnsValidationFixture.IType ExtensionMethod(this ReturnsValidationFixture fixture, object arg1, object arg2)
		{
			return default(ReturnsValidationFixture.IType);
		}

		internal static ReturnsValidationFixture.IType ExtensionMethodNoArgs(this ReturnsValidationFixture fixture)
		{
			return default(ReturnsValidationFixture.IType);
		}
	}
}
