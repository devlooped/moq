using System.ComponentModel;

namespace Moq.Language.Flow
{
	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IExpect : ICallback, IThrowsOnceVerifies, IHideObjectMembers
	{
	}

	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IExpect<TResult> : ICallback<TResult>, IReturnsThrows<TResult>, IHideObjectMembers
	{
	}

	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IExpectGetter<TProperty> : ICallbackGetter<TProperty>, IReturnsThrowsGetter<TProperty>, IHideObjectMembers
	{
	}

	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IExpectSetter<TProperty> : ICallbackSetter<TProperty>, IThrowsOnceVerifies, IHideObjectMembers
	{
	}
}
