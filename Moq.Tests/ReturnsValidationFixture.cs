using System;
using System.Reflection;

using Moq.Language.Flow;

using Xunit;

namespace Moq.Tests
{
	public class ReturnsValidationFixture
	{
		private Mock<IType> mock;
		private ISetup<IType, IType> setup;

		public ReturnsValidationFixture()
		{
			this.mock = new Mock<IType>();
			this.setup = this.mock.Setup(m => m.Method(It.IsAny<object>()));
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
		public void Returns_accepts_delegate_with_wrong_return_type_but_setup_invocation_will_fail()
		{
			Func<string> delegateWithWrongReturnType = () => "42";
			this.setup.Returns(delegateWithWrongReturnType);

			var ex = Record.Exception(() =>
			{
				this.mock.Object.Method(42);
			});

			Assert.IsType<InvalidCastException>(ex);
		}

		[Fact]
		public void Returns_accepts_delegate_with_wrong_return_type_and_setup_invocation_will_succeed_if_retval_convertible()
		{
			Func<string> delegateWithWrongReturnType = () => null;
			this.setup.Returns(delegateWithWrongReturnType);

			var ex = Record.Exception(() =>
			{
				this.mock.Object.Method(42);
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
				this.mock.Object.Method(42);
			});

			Assert.Null(ex);
		}

		[Fact]
		public void Returns_accepts_delegate_with_wrong_parameter_count_but_setup_invocation_will_fail()
		{
			Func<object, object, IType> delegateWithWrongParameterCount = (arg1, arg2) => default(IType);
			this.setup.Returns(delegateWithWrongParameterCount);

			var ex = Record.Exception(() =>
			{
				this.mock.Object.Method(42);
			});

			Assert.IsType<TargetParameterCountException>(ex);
		}

		[Fact]
		public void Returns_accepts_delegate_with_wrong_parameter_types_but_setup_invocation_will_fail()
		{
			Func<string, IType> delegateWithWrongParameterType = (arg) => default(IType);
			this.setup.Returns(delegateWithWrongParameterType);

			var ex = Record.Exception(() =>
			{
				mock.Object.Method(42);
			});

			Assert.IsType<ArgumentException>(ex);
		}

		[Fact]
		public void Returns_accepts_delegate_with_wrong_parameter_types_and_setup_invocation_will_succeed_if_args_convertible()
		{
			Func<string, IType> delegateWithWrongParameterType = (arg) => default(IType);
			this.setup.Returns(delegateWithWrongParameterType);

			var ex = Record.Exception(() =>
			{
				mock.Object.Method(null);
			});

			Assert.Null(ex);
		}

		public interface IType
		{
			IType Method(object arg);
		}
	}
}
