// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.ComponentModel;

using Moq.Language;

namespace Moq
{
	/// <summary>
	/// Provides additional methods on mocks.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static partial class MockExtensions
	{
		/// <summary>
		///   Perform an expectation in the trace.
		/// </summary>
		public static ISetupConditionResult<T> InSequence<T>(this Mock<T> mock, MockSequence sequence)
			where T : class
		{
			Guard.NotNull(sequence, nameof(sequence));

			return sequence.For(mock);
		}

		/// <summary>
		/// Resets this mock's state. This includes its setups, configured default return values, registered event handlers, and all recorded invocations.
		/// </summary>
		/// <param name="mock">The mock whose state should be reset.</param>
		public static void Reset(this Mock mock)
		{
			mock.ConfiguredDefaultValues.Clear();
			mock.Setups.Clear();
			mock.EventHandlers.Clear();
			mock.Invocations.Clear();
		}
	}
}
