// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	/// <summary>
	/// Options to customize the behavior of the mock. 
	/// </summary>
	public enum MockBehavior
	{
		/// <summary>
		/// Causes the mock to always throw 
		/// an exception for invocations that don't have a 
		/// corresponding setup.
		/// </summary>
		Strict,
		/// <summary>
		/// Will never throw exceptions, returning default  
		/// values when necessary (null for reference types, 
		/// zero for value types or empty enumerables and arrays).
		/// </summary>
		Loose,
		/// <summary>
		/// Default mock behavior, which equals <see cref="Loose"/>.
		/// </summary>
		Default = Loose,
	}
}
