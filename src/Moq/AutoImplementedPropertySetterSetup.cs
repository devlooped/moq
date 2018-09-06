// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Matchers;

namespace Moq
{
	/// <summary>
	///   Setup used by <see cref="Mock.SetupAllProperties(Mock)"/> for property setters.
	/// </summary>
	internal sealed class AutoImplementedPropertySetterSetup : Setup
	{
		private static IMatcher[] anyMatcherForSingleArgument = new IMatcher[] { AnyMatcher.Instance };

		private Action<object> setter;

		public AutoImplementedPropertySetterSetup(LambdaExpression originalExpression, MethodInfo method, Action<object> setter)
			: base(new InvocationShape(method, anyMatcherForSingleArgument), originalExpression)
		{
			this.setter = setter;
		}

		public override void Execute(Invocation invocation)
		{
			this.setter.Invoke(invocation.Arguments[0]);
			invocation.Return();
		}

		public override bool TryVerifyAll() => true;
	}
}
