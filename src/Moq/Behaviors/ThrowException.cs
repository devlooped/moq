// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq.Behaviors
{
	/// <summary>
	///   Terminates an invocation as if by a <see langword="throw"/>-ing an exception.
	/// </summary>
	public sealed class ThrowException : Behavior
	{
		private readonly Exception exception;

		/// <summary>
		///   Initializes a new instance of the <see cref="ThrowException"/> class using the specified exception.
		/// </summary>
		/// <param name="exception">
		///   The exception to be thrown when terminating invocations.
		/// </param>
		public ThrowException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			this.exception = exception;
		}

		/// <summary>
		///   The exception to be thrown when terminating invocations.
		/// </summary>
		public Exception Exception => this.exception;

		/// <inheritdoc/>
		public override BehaviorExecution Execute(IInvocation invocation, in BehaviorExecutionContext context)
		{
			return BehaviorExecution.ThrowException(this.exception);
		}
	}
}
