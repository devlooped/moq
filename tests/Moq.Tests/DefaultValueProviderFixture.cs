// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

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
		public void GetDefaultParameterValue_returns_same_value_as_GetDefaultValue_if_GetDefaultParameterValue_not_overridden()
		{
			var _ = this.fooMock;
			var parameter = fooActionMethodParameter;
			var expected = this.defaultValueProvider.GetDefaultValue(parameter.ParameterType, _);

			var actual = this.defaultValueProvider.GetDefaultParameterValue(parameter, _);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public void GetDefaultReturnValue_returns_same_value_as_GetDefaultValue_if_GetDefaultReturnValue_not_overridden()
		{
			var _ = this.fooMock;
			var method = fooFuncMethod;
			var expected = this.defaultValueProvider.GetDefaultValue(method.ReturnType, _);

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
			protected internal override object GetDefaultValue(Type type, Mock mock)
			{
				return 42;
			}
		}
	}
}
