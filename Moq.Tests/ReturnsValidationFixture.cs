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
		public void Returns_does_not_accept_delegate_with_wrong_parameter_count()
		{
			Func<object, object, IType> delegateWithWrongParameterCount = (arg1, arg2) => default(IType);

			var ex = Record.Exception(() =>
			{
				this.setup.Returns(delegateWithWrongParameterCount);
			});

			Assert.IsType<ArgumentException>(ex);
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
