// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using Moq.Language.Flow;

using Xunit;

namespace Moq.Tests
{
	/// <summary>
	///   This fixture targets the `Returns` delegate validation logic.
	/// </summary>
	/// <seealso cref="CallbackDelegateValidationFixture"/>
	public class ReturnsDelegateValidationFixture
	{
		private ISetup<IFoo, bool> setup;

		public ReturnsDelegateValidationFixture()
		{
			var mock = new Mock<IFoo>();
			this.setup = mock.Setup(m => m.Func(It.IsAny<int>()));
		}

		// Nothing surprising here.
		[Fact]
		public void Returns_accepts_instance_method_as_callback()
		{
			var instance = new Instance();
			Func<int, bool> callback = instance.Func;
			Assert.Single(callback.Method.GetParameters());
			Assert.Same(instance, callback.Target);

			this.setup.Returns(callback);
		}

		// Nothing surprising here.
		[Fact]
		public void Returns_accepts_static_method_as_callback()
		{
			Func<int, bool> callback = Static.Func;
			Assert.Single(callback.Method.GetParameters());
			Assert.Null(callback.Target);

			this.setup.Returns(callback);
		}

		// This may seem surprising because the extension method has a different number
		// of declared parameters (2) than the method being set up (1). Since C# supports
		// this syntax just fine (proof below), Moq should accept this too. The reason why
		// this works is because the first (`this`) parameter is bound to an object.
		[Fact]
		public void Returns_accepts_bound_extension_method_as_callback_despite_additional_parameter()
		{
			var instance = Enumerable.Range(1, 10);
			Func<int, bool> callback = instance.Func;
			Assert.Equal(2, callback.Method.GetParameters().Length);
			Assert.Same(instance, callback.Target);

			this.setup.Returns(callback);
		}

		// This doesn't look suspicious at all, but it is very similar to the above test.
		// Methods that result from compiling a LINQ expression tree have an additional
		// (bound) first parameter of type `System.Runtime.CompilerServices.Closure`.
		// That is, they have a different parameter count than the method being set up,
		// but Moq should still accept it.
		[Fact]
		public void Returns_accepts_compiled_method_as_callback_despite_additional_parameter()
		{
			Expression<Func<int, bool>> callbackExpr = x => x == 0;
			Func<int, bool> callback = callbackExpr.Compile();
			Assert.Equal(2, callback.Method.GetParameters().Length);
			Assert.NotNull(callback.Target);

			this.setup.Returns(callback);
		}

		[Fact]
		public void Returns_accepts_Func_of_IInvocation_and_assignable_return_type()
		{
			Func<IInvocation, bool> callback = invocation => true;
			this.setup.Returns(callback);
		}

		[Fact]
		public void Returns_accepts_Func_of_IInvocation_and_object()
		{
			Func<IInvocation, object> callback = invocation => true;
			this.setup.Returns(callback);
		}

		[Fact]
		public void Returns_does_not_accept_Func_of_IInvocation_and_incompatible_return_type()
		{
			Func<IInvocation, string> callback = invocation => "true";
			Assert.Throws<ArgumentException>(() => this.setup.Returns(callback));
		}

		public interface IFoo
		{
			bool Func(int x);
		}
	}

	public partial class Instance
	{
		public bool Func(int x)
		{
			return x == 0;
		}
	}

	public static partial class Static
	{
		public static bool Func(int x)
		{
			return x == 0;
		}
	}

	public static partial class Extension
	{
		public static bool Func(this IEnumerable<int> self, int x)
		{
			return x == 0;
		}
	}
}
