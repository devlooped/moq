// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.ComponentModel;

namespace Moq.Behaviors
{
	/// <todo/>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public abstract class Behavior
	{
		/// <todo/>
		protected Behavior()
		{
		}

		/// <todo/>
		public abstract BehaviorExecution Execute(IInvocation invocation, in BehaviorExecutionContext context);
	}
}
