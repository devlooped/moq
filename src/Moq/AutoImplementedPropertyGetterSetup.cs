// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	/// <summary>
	///   Setup used by <see cref="Mock.SetupAllProperties(Mock)"/> for property getters.
	/// </summary>
	internal sealed class AutoImplementedPropertyGetterSetup : Setup
	{
		private static IMatcher[] noArgumentMatchers = new IMatcher[0];

		private Func<object> getter;

		public AutoImplementedPropertyGetterSetup(LambdaExpression originalExpression, MethodInfo method, Func<object> getter)
			: base(new InvocationShape(method, noArgumentMatchers), originalExpression)
		{
			this.getter = getter;
		}

		public override void Execute(Invocation invocation)
		{
			invocation.Return(this.getter.Invoke());
		}

		public override bool TryVerifyAll() => true;
	}
}
