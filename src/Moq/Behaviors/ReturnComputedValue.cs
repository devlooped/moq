// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Behaviors
{
	internal sealed class ReturnComputedValue : Behavior
	{
		private readonly Func<IInvocation, object> valueFactory;

		public ReturnComputedValue(Func<IInvocation, object> valueFactory)
		{
			Debug.Assert(valueFactory != null);

			this.valueFactory = valueFactory;
		}

		public override void Execute(IInvocation invocation)
		{
			invocation.ReturnValue = this.valueFactory.Invoke(invocation);
		}
	}
}
