//Copyright (c) 2007, Moq Team 
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

//    * Neither the name of the Moq Team nor the 
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


using System;
using System.ComponentModel;
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
		/// corresponding expectation.
		/// </summary>
		Strict, 
		/// <summary>
		/// Matches the behavior of classes and interfaces 
		/// in equivalent manual mocks: abstract methods 
		/// need to have an expectation (override), as well 
		/// as all interface members. Other members (virtual 
		/// and non-virtual) can be called freely and will end up 
		/// invoking the implementation on the target type if available.
		/// </summary>
		[Obsolete("Use Strict instead. This member will be removed in v3.5", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		Normal,
		/// <summary>
		/// Will only throw exceptions for abstract methods and 
		/// interface members which need to return a value and 
		/// don't have a corresponding expectation.
		/// </summary>
		[Obsolete("Use Loose instead. This member will be removed in v3.5", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]		
		Relaxed,
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
