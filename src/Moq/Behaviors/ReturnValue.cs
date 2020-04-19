// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{
	/// <todo/>
	public sealed class ReturnValue : Behavior
	{
		private readonly object value;

		/// <todo/>
		public ReturnValue(object value)
		{
			this.value = value;
		}

		/// <todo/>
		public object Value => this.value;

		/// <inheritdoc/>
		public override BehaviorExecution Execute(IInvocation invocation, in BehaviorExecutionContext context)
		{
			return BehaviorExecution.ReturnValue(this.value);
		}
	}
}
