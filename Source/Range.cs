
namespace Moq
{
	/// <summary>
	/// Kind of range to use in a filter specified through 
	/// <see cref="It.IsInRange"/>.
	/// </summary>
	public enum Range
	{
		/// <summary>
		/// The range includes the <c>to</c> and 
		/// <c>from</c> values.
		/// </summary>
		Inclusive,
		/// <summary>
		/// The range does not include the <c>to</c> and 
		/// <c>from</c> values.
		/// </summary>
		Exclusive
	}
}