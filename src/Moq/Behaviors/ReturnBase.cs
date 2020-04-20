// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{
	/// <summary>
	///   Terminates an invocation by delegating to the invoked method's <see langword="base"/> implementation.
	/// </summary>
	public sealed class ReturnBase : Behavior
	{
		/// <inheritdoc/>
		public override BehaviorExecution Execute(IInvocation invocation, in BehaviorExecutionContext context)
		{
			return BehaviorExecution.ReturnBase();
		}
	}
}
