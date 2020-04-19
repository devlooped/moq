// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

namespace Moq.Behaviors
{
	/// <todo/>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public readonly struct BehaviorExecution
	{
		/// <todo/>
		public static BehaviorExecution Continue() => default;

		/// <todo/>
		public static BehaviorExecution Return() => new BehaviorExecution(BehaviorExecutionKind.Return);

		/// <todo/>
		public static BehaviorExecution ReturnBase() => new BehaviorExecution(BehaviorExecutionKind.ReturnBase);

		/// <todo/>
		public static BehaviorExecution ReturnValue(object value) => new BehaviorExecution(BehaviorExecutionKind.ReturnValue, value);

		/// <todo/>
		public static BehaviorExecution ThrowException(Exception exception) => new BehaviorExecution(BehaviorExecutionKind.ThrowException, exception);

		internal readonly BehaviorExecutionKind Kind;
		internal readonly object Argument;

		private BehaviorExecution(BehaviorExecutionKind kind, object argument = null)
		{
			this.Kind = kind;
			this.Argument = argument;
		}
	}
}
