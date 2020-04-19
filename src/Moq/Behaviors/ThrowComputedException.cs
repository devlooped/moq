// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq.Behaviors
{
	/// <todo/>
	public sealed class ThrowComputedException : Behavior
	{
		private readonly Func<Exception> exceptionFunction;

		/// <todo/>
		public ThrowComputedException(Func<Exception> exceptionFunction)
		{
			if (exceptionFunction == null)
			{
				throw new ArgumentNullException(nameof(exceptionFunction));
			}

			this.exceptionFunction = exceptionFunction;
		}

		/// <todo/>
		public Func<Exception> ExceptionFunction => this.exceptionFunction;

		/// <inheritdoc/>
		public override BehaviorExecution Execute(IInvocation invocation, in BehaviorExecutionContext context)
		{
			return BehaviorExecution.ThrowException(this.exceptionFunction());
		}
	}
}
