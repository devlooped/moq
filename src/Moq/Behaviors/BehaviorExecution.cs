// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

namespace Moq.Behaviors
{
	/// <summary>
	///   Returned by <see cref="Behavior"/>s to specify whether (and how) the setup should proceed with an invocation.
	///   Values of this type can be created using any of the static factory methods.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public readonly struct BehaviorExecution
	{
		/// <summary>
		///   The setup should proceed with the next behavior in line.
		/// </summary>
		public static BehaviorExecution Continue() => default;

		/// <summary>
		///   The setup should terminate the invocation as if a <see langword="return"/> statement had occurred.
		/// </summary>
		public static BehaviorExecution Return() => new BehaviorExecution(BehaviorExecutionKind.Return);

		/// <summary>
		///   The setup should terminate by delegating to the invoked method's <see langword="base"/> implementation.
		/// </summary>
		public static BehaviorExecution ReturnBase() => new BehaviorExecution(BehaviorExecutionKind.ReturnBase);

		/// <summary>
		///   The setup should terminate the invocation as if a <see langword="return"/> statement with a return value had occurred.
		/// </summary>
		/// <param name="value">
		///   The value that should be returned from the invoked method.
		/// </param>
		public static BehaviorExecution ReturnValue(object value) => new BehaviorExecution(BehaviorExecutionKind.ReturnValue, value);

		/// <summary>
		///   The setup should terminate the invocation by <see langword="throw"/>-ing the specified exception.
		/// </summary>
		/// <param name="exception">
		///   The exception that should be thrown.
		/// </param>
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
