// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.ComponentModel;

namespace Moq
{
	/// <summary>
	/// Provides additional methods on mocks.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static partial class MockExtensions
	{
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


		/// <summary>
		/// Reset the mock's configured default values
		/// </summary>
		/// <param name="mock">The mock whose default values should be reset.</param>
		public static void ResetConfiguredDefaultValues(this Mock mock)
		{
			mock.ConfiguredDefaultValues.Clear();
		}

		/// <summary>
		/// Reset the mock's configured setups
		/// </summary>
		/// <param name="mock">The mock whose setups should be reset.</param>
		public static void ResetSetups(this Mock mock)
		{
			mock.Setups.Clear();
		}

		/// <summary>
		/// Reset the mock's configured event handlers
		/// </summary>
		/// <param name="mock">The mock whose event handlers should be reset.</param>
		public static void ResetEventHandlers(this Mock mock)
		{
			mock.EventHandlers.Clear();
		}

		/// <summary>
		/// Reset the mock's recorded invocations
		/// </summary>
		/// <param name="mock">The mock whose recorded invocations should be reset.</param>
		public static void ResetInvocations(this Mock mock)
		{
			mock.Invocations.Clear();
		}
	}
}