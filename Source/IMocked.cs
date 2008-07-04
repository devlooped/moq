using System.ComponentModel;

namespace Moq
{
	/// <summary>
	/// Implemented by all generated mock object instances.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IMocked<T> : IMocked
		where T : class
	{
		/// <summary>
		/// Reference the Mock that contains this as the <c>mock.Object</c> value.
		/// </summary>
		new Mock<T> Mock { get; }
	}

	/// <summary>
	/// Implemented by all generated mock object instances.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IMocked
	{
		/// <summary>
		/// Reference the Mock that contains this as the <c>mock.Object</c> value.
		/// </summary>
		Mock Mock { get; }
	}
}
