// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Behaviors
{
	internal sealed class ThrowComputedException : Behavior
	{
		private readonly Func<IInvocation, Exception> exceptionFactory;

		public ThrowComputedException(Func<IInvocation, Exception> exceptionFactory)
		{
			Debug.Assert(exceptionFactory != null);

			this.exceptionFactory = exceptionFactory;
		}

		public override void Execute(Invocation invocation)
		{
			throw this.exceptionFactory.Invoke(invocation);
		}
	}
}
