using System.ComponentModel;

namespace Moq.Language.Flow
{
	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IReturnsThrows<TResult> : IReturns<TResult>, IThrows, IHideObjectMembers
	{
	}

	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IReturnsThrowsGetter<TProperty> : IReturnsGetter<TProperty>, IThrows, IHideObjectMembers
	{
	}
}
