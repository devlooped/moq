using System.ComponentModel;

namespace Moq
{

	/// <summary>
	/// Provides additional methods on mocks.
	/// </summary>
	/// <remarks>
	/// Those methods are useful for Testeroids support.
	/// </remarks>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class MockExtensions
	{
		/// <summary>
		/// Resets the calls previously made on the specified mock.
		/// </summary>
		/// <param name="mock">The mock whose calls need to be reset.</param>
		public static void ResetCalls(this Mock mock)
		{
			mock.Invocations.Clear();
		}

		/// <summary>
		/// Resets mock state, including setups and any previously made calls.
		/// </summary>
		/// <param name="mock">The mock that needs to be reset.</param>
		public static void Reset(this Mock mock)
		{
			mock.Setups.Clear();
			mock.EventHandlers.Clear();
			mock.ResetCalls();
		}
	}
}
