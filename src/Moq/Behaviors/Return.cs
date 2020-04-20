// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{
	/// <summary>
	///   Terminates an invocation as if by a <see langword="return"/> statement.
	/// </summary>
	public sealed class Return : Behavior
	{
		/// <inheritdoc/>
		public override BehaviorExecution Execute(IInvocation invocation, in BehaviorExecutionContext context)
		{
			return BehaviorExecution.Return();
		}
	}
}
