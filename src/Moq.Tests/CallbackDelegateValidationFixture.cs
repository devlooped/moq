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
	///   This fixture targets the `Callback` delegate validation logic.
	/// </summary>
	/// <seealso cref="ReturnsDelegateValidationFixture"/>
	public class CallbackDelegateValidationFixture
	{
		private ISetup<IFoo> setup;

		public CallbackDelegateValidationFixture()
		{
			var mock = new Mock<IFoo>();
			this.setup = mock.Setup(m => m.Action(It.IsAny<int>()));
		}

		// Nothing surprising here.
		[Fact]
		public void Callback_accepts_instance_method_as_callback()
		{
			var instance = new Instance();
			Action<int> callback = instance.Action;
			Assert.Single(callback.Method.GetParameters());
			Assert.Same(instance, callback.Target);

			this.setup.Callback(callback);
		}

		// Nothing surprising here.
		[Fact]
		public void Callback_accepts_static_method_as_callback()
		{
			Action<int> callback = Static.Action;
			Assert.Single(callback.Method.GetParameters());
			Assert.Null(callback.Target);

			this.setup.Callback(callback);
		}

		// This may seem surprising because the extension method has a different number
		// of declared parameters (2) than the method being set up (1). Since C# supports
		// this syntax just fine (proof below), Moq should accept this too. The reason why
		// this works is because the first (`this`) parameter is bound to an object.
		[Fact]
		public void Callback_accepts_bound_extension_method_as_callback_despite_additional_parameter()
		{
			var instance = Enumerable.Range(1, 10);
			Action<int> callback = instance.Action;
			Assert.Equal(2, callback.Method.GetParameters().Length);
			Assert.Same(instance, callback.Target);

			this.setup.Callback(callback);
		}

		// This doesn't look suspicious at all, but it is very similar to the above test.
		// Methods that result from compiling a LINQ expression tree have an additional
		// (bound) first parameter of type `System.Runtime.CompilerServices.Closure`.
		// That is, they have a different parameter count than the method being set up,
		// but Moq should still accept it.
		[Fact]
		public void Callback_accepts_compiled_method_as_callback_despite_additional_parameter()
		{
			Expression<Action<int>> callbackExpr = x => Console.WriteLine();
			Action<int> callback = callbackExpr.Compile();
			Assert.Equal(2, callback.Method.GetParameters().Length);
			Assert.NotNull(callback.Target);

			this.setup.Callback(callback);
		}

		public interface IFoo
		{
			void Action(int x);
		}
	}

	public partial class Instance
	{
		public void Action(int x)
		{
		}
	}

	public static partial class Static
	{
		public static void Action(int x)
		{
		}
	}

	public static partial class Extension
	{
		public static void Action(this IEnumerable<int> self, int x)
		{
		}
	}
}
