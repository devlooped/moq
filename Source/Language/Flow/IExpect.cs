using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language.Primitives;

namespace Moq.Language.Flow
{
	/// <summary>
	/// Defines the verbs available for expectations on void methods.
	/// </summary>
	public interface IExpect : ICallback, IThrowsOnceVerifies, IHideObjectMembers
	{
	}

	/// <summary>
	/// Defines the verbs available for expectations on non-void methods.
	/// </summary>
	public interface IExpect<TResult> : ICallback<TResult>, IReturnsThrows<TResult>, IHideObjectMembers
	{
	}
}
