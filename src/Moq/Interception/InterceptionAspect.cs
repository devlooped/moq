// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	internal enum InterceptionAction
	{
 		Continue,
		Stop
	}

	/// <summary>
	/// Abstract base class representing a specific aspect of mock method interception.
	/// </summary>
	internal abstract class InterceptionAspect
	{
		protected InterceptionAspect()
		{
		}

		/// <summary>
		/// Handle interception
		/// </summary>
		/// <param name="invocation">The current invocation.</param>
		/// <param name="mock">The mock on which the current invocation is occurring.</param>
		/// <returns>InterceptionAction.Continue if further interception has to be processed, otherwise InterceptionAction.Stop</returns>
		public abstract InterceptionAction Handle(Invocation invocation, Mock mock);
	}
}
