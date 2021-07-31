// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	/// <summary>
	/// 
	/// </summary>
	public interface ISequenceInvocation
	{
		/// <summary>
		/// 
		/// </summary>
		IInvocation Invocation { get; }
		
		/// <summary>
		/// 
		/// </summary>
		Mock Mock { get; }
	}
}
