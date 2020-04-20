// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.ComponentModel;
using System.Diagnostics;

namespace Moq.Behaviors
{
	/// <summary>
	///   Describes the context in which an <see cref="IInvocation"/> is occurring.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public readonly ref struct BehaviorExecutionContext
	{
		private readonly Mock mock;

		internal BehaviorExecutionContext(Mock mock)
		{
			Debug.Assert(mock != null);

			this.mock = mock;
		}

		/// <summary>
		///   The <see cref="Mock"/> on which an invocation is occurring.
		/// </summary>
		public Mock Mock => this.mock;
	}
}
