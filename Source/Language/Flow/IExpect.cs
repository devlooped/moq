using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Moq.Language.Flow
{
	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	public interface IExpect : ICallback, IThrowsOnceVerifies, IHideObjectMembers
	{
	}

	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	public interface IExpect<TResult> : ICallback<TResult>, IReturnsThrows<TResult>, IHideObjectMembers
	{
	}
}
