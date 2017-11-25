using System;
using System.Reflection;

using Xunit;

namespace Moq.Tests
{
	/// <summary>
	/// Tests for the <see cref="DefaultValueProvider"/> abstract base class.
	/// </summary>
    public class DefaultValueProviderFixture
    {
		private static MethodInfo fooActionMethod = typeof(IFoo).GetMethod(nameof(IFoo.Action));
		private static ParameterInfo fooActionMethodParameter = typeof(IFoo).GetMethod(nameof(IFoo.Action)).GetParameters()[0];
		private static MethodInfo fooFuncMethod = typeof(IFoo).GetMethod(nameof(IFoo.Func));

		private DefaultValueProvider defaultValueProvider;
		private Mock<IFoo> fooMock;

		public DefaultValueProviderFixture()
		{
			this.defaultValueProvider = new DefaultValueProviderStub();
			this.fooMock = new Mock<IFoo>();
		}

		[Fact]
		public void GetDefaultValue_throws_if_type_argument_is_null()
		{
			var _ = this.fooMock;

			Assert.Throws<ArgumentNullException>(() =>
			{
				this.defaultValueProvider.GetDefaultValue(null, _);
			});
		}

		[Fact]
		public void GetDefaultValue_throws_if_type_argument_is_void()
		{
			var _ = this.fooMock;

			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				this.defaultValueProvider.GetDefaultValue(typeof(void), _);
			});
		}

		[Fact]
		public void GetDefaultValue_throws_if_mock_argument_is_null()
		{
			var _ = typeof(object);

			Assert.Throws<ArgumentNullException>(() =>
			{
				this.defaultValueProvider.GetDefaultValue(_, null);
			});
		}

		[Fact]
		public void GetDefaultParameterValue_throws_if_parameter_argument_is_null()
		{
			var _ = this.fooMock;

			Assert.Throws<ArgumentNullException>(() =>
			{
				this.defaultValueProvider.GetDefaultParameterValue(null, _);
			});
		}

		[Fact]
		public void GetDefaultParameterValue_throws_if_parameter_argument_is_void_parameter()
		{
			var voidParameter = fooActionMethod.ReturnParameter;
			var _ = this.fooMock;

			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				this.defaultValueProvider.GetDefaultParameterValue(voidParameter, _);
			});
		}

		[Fact]
		public void GetDefaultParameterValue_throws_if_mock_argument_is_null()
		{
			var _ = fooActionMethodParameter;

			Assert.Throws<ArgumentNullException>(() =>
			{
				this.defaultValueProvider.GetDefaultParameterValue(_, null);
			});
		}

		[Fact]
		public void GetDefaultReturnValue_throws_if_method_argument_is_null()
		{
			var _ = this.fooMock;

			Assert.Throws<ArgumentNullException>(() =>
			{
				this.defaultValueProvider.GetDefaultReturnValue(null, _);
			});
		}

		[Fact]
		public void GetDefaultReturnValue_throws_if_method_argument_is_void_method()
		{
			var voidMethod = fooActionMethod;
			var _ = this.fooMock;

			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				this.defaultValueProvider.GetDefaultReturnValue(voidMethod, _);
			});
		}

		[Fact]
		public void GetDefaultReturnValue_throws_if_mock_argument_is_null()
		{
			var _ = fooFuncMethod;

			Assert.Throws<ArgumentNullException>(() =>
			{
				this.defaultValueProvider.GetDefaultReturnValue(_, null);
			});
		}

		[Fact]
		public void GetDefaultParameterValue_returns_same_value_as_GetDefaultValueImpl_if_GetDefaultParameterValueImpl_not_overridden()
		{
			var _ = this.fooMock;
			var parameter = fooActionMethodParameter;
			var expected = this.defaultValueProvider.GetDefaultValueImpl(parameter.ParameterType, _);

			var actual = this.defaultValueProvider.GetDefaultParameterValue(parameter, _);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public void GetDefaultReturnValue_returns_same_value_as_GetDefaultValueImpl_if_GetDefaultReturnValueImpl_not_overridden()
		{
			var _ = this.fooMock;
			var method = fooFuncMethod;
			var expected = this.defaultValueProvider.GetDefaultValueImpl(method.ReturnType, _);

			var actual = this.defaultValueProvider.GetDefaultReturnValue(method, _);

			Assert.Equal(expected, actual);
		}

		public interface IFoo
		{
			void Action(object arg);
			object Func();
		}

		private sealed class DefaultValueProviderStub : DefaultValueProvider
		{
			public override DefaultValue Kind => throw new NotImplementedException();

			protected internal override object GetDefaultValueImpl(Type type, Mock mock)
			{
				return 42;
			}
		}
	}
}
