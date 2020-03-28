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
		private static Expression[] noArguments = new Expression[0];

		private Func<object> getter;

		public AutoImplementedPropertyGetterSetup(LambdaExpression originalExpression, MethodInfo method, Func<object> getter)
			: base(new InvocationShape(originalExpression, method, noArguments))
		{
			this.getter = getter;
		}

		public override void Execute(Invocation invocation)
		{
			invocation.Return(this.getter.Invoke());
		}

		public override bool TryGetReturnValue(out object returnValue)
		{
			returnValue = this.getter.Invoke();
			return true;
		}
	}
}
