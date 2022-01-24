// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	/// <summary>
	/// This role interface represents a <see cref="Mock"/>'s ability to intercept method invocations for its <see cref="Mock.Object"/>.
	/// It is meant for use by <see cref="ProxyFactory"/>.
	/// </summary>
	public interface IInterceptor
	{
		/// <summary>
		/// </summary>
		void Intercept(IInvocation invocation);
	}
}
