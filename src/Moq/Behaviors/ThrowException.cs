// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq.Behaviors
{
	/// <todo/>
	public sealed class ThrowException : Behavior
	{
		private readonly Exception exception;

		/// <todo/>
		public ThrowException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			this.exception = exception;
		}

		/// <todo/>
		public Exception Exception => this.exception;

		/// <inheritdoc/>
		public override BehaviorExecution Execute(IInvocation invocation, in BehaviorExecutionContext context)
		{
			return BehaviorExecution.ThrowException(this.exception);
		}
	}
}
