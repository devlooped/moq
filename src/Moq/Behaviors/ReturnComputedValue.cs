// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq.Behaviors
{
	/// <summary>
	///   Terminates an invocation as if by a <see langword="return"/> statement with a lazily computed return value.
	/// </summary>
	public sealed class ReturnComputedValue : Behavior
	{
		private readonly Func<object> valueFunction;

		/// <summary>
		///   Initializes a new instance of the <see cref="ReturnComputedValue"/> class
		///   using the specified return value factory function.
		/// </summary>
		/// <param name="valueFunction">
		///   The factory function that computes the return value(s) to be returned when terminating invocations.
		/// </param>
		public ReturnComputedValue(Func<object> valueFunction)
		{
			if (valueFunction == null)
			{
				throw new ArgumentNullException(nameof(valueFunction));
			}

			this.valueFunction = valueFunction;
		}

		/// <summary>
		///   The factory function that computes the return value(s) to be returned when terminating invocations.
		/// </summary>
		public Func<object> ValueFunction => this.valueFunction;

		/// <inheritdoc/>
		public override BehaviorExecution Execute(IInvocation invocation, in BehaviorExecutionContext context)
		{
			return BehaviorExecution.ReturnValue(this.valueFunction());
		}
	}
}
