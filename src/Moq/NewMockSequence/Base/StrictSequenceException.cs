// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;

namespace Moq
{
	/// <summary>
	/// 
	/// </summary>
	public class StrictSequenceException : SequenceException
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public StrictSequenceException(string message) : base(message) { }
		/// <summary>
		/// 
		/// </summary>
		public ISequenceInvocation UnmatchedInvocation { get; set; }
	}

}
