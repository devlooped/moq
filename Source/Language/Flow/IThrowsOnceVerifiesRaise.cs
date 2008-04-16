using System.ComponentModel;

namespace Moq.Language.Flow
{
	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IThrowsOnceVerifiesRaise : IThrows, IOnceVerifiesRaise, IHideObjectMembers
	{
	}
}
