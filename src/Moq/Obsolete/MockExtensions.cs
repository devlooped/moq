// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;

namespace Moq
{
	static partial class MockExtensions
	{
		/// <summary>
		/// Resets all invocations recorded for this mock.
		/// </summary>
		/// <param name="mock">The mock whose recorded invocations should be reset.</param>
		[Obsolete("Use `mock.Invocations.Clear()` instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void ResetCalls(this Mock mock) => mock.Invocations.Clear();
	}
}
