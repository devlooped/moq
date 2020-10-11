// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.ComponentModel;

namespace Moq.Language.Flow
{
	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISetup<TMock> : ICallback, ICallbackResult, IRaise<TMock>, IVerifies, IFluentInterface
		where TMock : class
	{
	}

	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISetup<TMock, TResult> : ICallback<TMock, TResult>, IReturnsThrows<TMock, TResult>, IVerifies, IFluentInterface
		where TMock : class
	{
	}

	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISetupGetter<TMock, TProperty> : ICallbackGetter<TMock, TProperty>, IReturnsThrowsGetter<TMock, TProperty>, IVerifies, IFluentInterface
		where TMock : class
	{
	}

	/// <summary>
	/// Implements the fluent API.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISetupSetter<TMock, TProperty> : ICallbackSetter<TProperty>, ICallbackResult, IRaise<TMock>, IVerifies, IFluentInterface
		where TMock : class
	{
	}
}