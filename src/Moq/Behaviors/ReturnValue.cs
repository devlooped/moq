// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{
	/// <summary>
	///   Terminates an invocation as if by a <see langword="return"/> statement with a return value.
	/// </summary>
	public sealed class ReturnValue : Behavior
	{
		private readonly object value;

		/// <summary>
		///   Initializes a new instance of the <see cref="ReturnValue"/> class using the specified return value.
		/// </summary>
		/// <param name="value">
		///   The return value to be returned when terminating invocations.
		/// </param>
		public ReturnValue(object value)
		{
			this.value = value;
		}

		/// <summary>
		///   The return value to be returned when terminating invocations.
		/// </summary>
		public object Value => this.value;

		/// <inheritdoc/>
		public override BehaviorExecution Execute(IInvocation invocation, in BehaviorExecutionContext context)
		{
			return BehaviorExecution.ReturnValue(this.value);
		}
	}
}
