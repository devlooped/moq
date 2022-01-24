// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Behaviors
{
	internal sealed class ThrowException : Behavior
	{
		private readonly Exception exception;

		public ThrowException(Exception exception)
		{
			Debug.Assert(exception != null);

			this.exception = exception;
		}

		public override void Execute(IInvocation invocation)
		{
			throw this.exception;
		}
	}
}
