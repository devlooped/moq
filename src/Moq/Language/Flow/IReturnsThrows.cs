// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.ComponentModel;

namespace Moq.Language.Flow
{
	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IReturnsThrows<TMock, TResult> : IReturns<TMock, TResult>, IThrows, IFluentInterface
		where TMock : class
	{
	}

	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IReturnsThrowsGetter<TMock, TProperty> : IReturnsGetter<TMock, TProperty>, IThrows, IFluentInterface
		where TMock : class
	{
	}
}
