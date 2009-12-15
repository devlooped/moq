//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//http://code.google.com/p/moq/
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System.ComponentModel;
using System;

namespace Moq.Language
{
	/// <summary>
	/// Defines occurrence members to constraint setups.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IOccurrence : IHideObjectMembers
	{
		/// <summary>
		/// The expected invocation can happen at most once.
		/// </summary>
		/// <example>
		/// <code>
		/// var mock = new Mock&lt;ICommand&gt;();
		/// mock.Setup(foo => foo.Execute("ping"))
		///     .AtMostOnce();
		/// </code>
		/// </example>
		[Obsolete("To verify this condition, use the overload to Verify that receives Times.AtMostOnce().")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		IVerifies AtMostOnce();
		/// <summary>
		/// The expected invocation can happen at most specified number of times.
		/// </summary>
		/// <param name="callCount">The number of times to accept calls.</param>
		/// <example>
		/// <code>
		/// var mock = new Mock&lt;ICommand&gt;();
		/// mock.Setup(foo => foo.Execute("ping"))
		///     .AtMost( 5 );
		/// </code>
		/// </example>
		[Obsolete("To verify this condition, use the overload to Verify that receives Times.AtMost(callCount).")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		IVerifies AtMost(int callCount);
	}
}