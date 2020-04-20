// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq.Behaviors
{
	/// <summary>
	///   Terminates an invocation as if by a <see langword="throw"/>-ing a lazily computed exception.
	/// </summary>
	public sealed class ThrowComputedException : Behavior
	{
		private readonly Func<Exception> exceptionFunction;

		/// <summary>
		///   Initializes a new instance of the <see cref="ThrowComputedException"/> class
		///   using the specified exception factory function.
		/// </summary>
		/// <param name="exceptionFunction">
		///   The factory function that computes the exception(s) to be thrown when terminating invocations.
		/// </param>
		public ThrowComputedException(Func<Exception> exceptionFunction)
		{
			if (exceptionFunction == null)
			{
				throw new ArgumentNullException(nameof(exceptionFunction));
			}

			this.exceptionFunction = exceptionFunction;
		}

		/// <summary>
		///   The factory function that computes the exception(s) to be thrown when terminating invocations.
		/// </summary>
		public Func<Exception> ExceptionFunction => this.exceptionFunction;

		/// <inheritdoc/>
		public override BehaviorExecution Execute(IInvocation invocation, in BehaviorExecutionContext context)
		{
			return BehaviorExecution.ThrowException(this.exceptionFunction());
		}
	}
}
