// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.ComponentModel;

namespace Moq.Behaviors
{
	/// <summary>
	///   Defines an aspect of how a setup reacts to invocations.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public abstract class Behavior
	{
		/// <summary>
		///   Initializes a new instance of the <see cref="Behavior"/> class.
		/// </summary>
		protected Behavior()
		{
		}

		/// <summary>
		///   Executes the behavior.
		/// </summary>
		/// <param name="invocation">
		///   The <see cref="IInvocation"/> to which this behavior should react.
		/// </param>
		/// <param name="context">
		///   The context in which the invocation is occurring.
		/// </param>
		/// <returns>
		///   A value specifying whether (and how) the setup should proceed with the current invocation.
		///   See the static factory methods on <see cref="BehaviorExecution"/> for available options.
		/// </returns>
		public abstract BehaviorExecution Execute(IInvocation invocation, in BehaviorExecutionContext context);
	}
}
