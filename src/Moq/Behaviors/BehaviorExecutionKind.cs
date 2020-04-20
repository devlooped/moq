// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{
	internal enum BehaviorExecutionKind
	{
		/// <seealso cref="BehaviorExecution.Continue"/>
		Continue = default,

		/// <seealso cref="BehaviorExecution.Return"/>
		Return,

		/// <seealso cref="BehaviorExecution.ReturnBase"/>
		ReturnBase,

		/// <seealso cref="BehaviorExecution.ReturnValue"/>
		ReturnValue,

		/// <seealso cref="BehaviorExecution.ThrowException(System.Exception)"/>
		ThrowException,
	}
}
