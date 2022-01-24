// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Behaviors
{
	internal sealed class Callback : Behavior
	{
		private readonly Action<IInvocation> callback;

		public Callback(Action<IInvocation> callback)
		{
			Debug.Assert(callback != null);

			this.callback = callback;
		}

		public override void Execute(IInvocation invocation)
		{
			this.callback.Invoke(invocation);
		}
	}
}
