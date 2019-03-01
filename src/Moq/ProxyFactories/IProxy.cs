// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	/// <summary>
	///   All proxies created by a <see cref="ProxyFactory"/> should implement this interface.
	/// </summary>
	// Interface proxies require this to be able to forward invocations of `object` members
	// to their `IInterceptor`.
	internal interface IProxy
	{
		IInterceptor Interceptor { get; }
	}
}
