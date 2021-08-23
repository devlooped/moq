// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.ComponentModel;

namespace Moq.Internals
{
	/// <summary>Do not use. (Moq requires this interface so that <see langword="object"/> methods can be set up on interface mocks.)</summary>
	// NOTE: This type is actually specific to our DynamicProxy implementation of `ProxyFactory`.
	// It is directly related to `InterfaceProxy`, see the comment there.
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IProxy
	{
		/// <summary/>
		object Interceptor { get; }
	}
}
