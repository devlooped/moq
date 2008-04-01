using System.ComponentModel;

namespace Moq
{
	/// <summary>
	/// Implemented by all generated mock object instances.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IMocked<T>
		where T : class
	{
		/// <summary>
		/// Reference the Mock that contains this as the <c>mock.Object</c> value.
		/// </summary>
		Mock<T> Mock { get; }
	}
}
