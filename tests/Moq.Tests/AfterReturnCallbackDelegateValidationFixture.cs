// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

using Moq.Language.Flow;

using Xunit;

namespace Moq.Tests
{
	public class AfterReturnCallbackDelegateValidationFixture
	{
		private readonly ISetup<IFoo, bool> setup;

		public AfterReturnCallbackDelegateValidationFixture()
		{
			this.setup = new Mock<IFoo>().Setup(m => m.Method(It.IsAny<string>(), It.IsAny<object>()));
		}

		[Fact]
		public void Callback_before_Returns__delegate_may_not_be_null()
		{
			var setup = this.setup;
			Assert.Throws<ArgumentNullException>(() => setup.Callback(null));
		}

		[Fact]
		public void Callback_after_Returns__delegate_may_not_be_null()
		{
			var setup = this.setup.Returns(true);
			Assert.Throws<ArgumentNullException>(() => setup.Callback(null));
		}

		[Fact]
		public void Callback_before_Returns__delegate_may_completely_omit_parameters()
		{
			var setup = this.setup;
			setup.Callback(() => { });
		}

		[Fact]
		public void Callback_after_Returns__delegate_may_completely_omit_parameters()
		{
			var setup = this.setup.Returns(true);
			setup.Callback(() => { });
		}

		[Fact]
		public void Callback_before_Returns__delegate_may_not_partially_omit_parameters()
		{
			var setup = this.setup;
			Assert.Throws<ArgumentException>(() => setup.Callback((string arg1) => { }));
		}

		[Fact]
		public void Callback_after_Returns__delegate_may_not_partially_omit_parameters()
		{
			var setup = this.setup.Returns(true);
			Assert.Throws<ArgumentException>(() => setup.Callback((string arg1) => { }));
		}

		[Fact]
		public void Callback_before_Returns__delegate_may_use_less_specific_parameter_types()
		{
			var setup = this.setup;
			setup.Callback((object arg1, object arg2) => { });
		}

		[Fact]
		public void Callback_after_Returns__delegate_may_use_less_specific_parameter_types()
		{
			var setup = this.setup.Returns(true);
			setup.Callback((object arg1, object arg2) => { });
		}

		[Fact]
		public void Callback_before_Returns__delegate_may_not_use_more_specific_parameter_types()
		{
			var setup = this.setup;
			Assert.Throws<ArgumentException>(() => setup.Callback((string arg1, string arg2) => { }));
		}

		[Fact]
		public void Callback_after_Returns__delegate_may_not_use_more_specific_parameter_types()
		{
			var setup = this.setup.Returns(true);
			Assert.Throws<ArgumentException>(() => setup.Callback((string arg1, string arg2) => { }));
		}

		public interface IFoo
		{
			bool Method(string arg1, object arg2);
		}
	}
}
