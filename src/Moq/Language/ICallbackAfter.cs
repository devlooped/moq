//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
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

using System;
using System.ComponentModel;

using Moq.Language.Flow;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Callback</c> verb (and overloads) after a preceding <c>Returns</c> or <c>CallBase</c>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public partial interface ICallbackAfter : ICallback
	{
		/// <summary>
		/// Specifies a callback of any delegate type to invoke after a method's return value has been determined.
		/// This overload specifically allows you to define callbacks for methods with by-ref parameters.
		/// By-ref parameters can be assigned to.
		/// </summary>
		/// <param name="callback">The callback method to invoke. Must have return type <c>void</c> (C#) or be a <c>Sub</c> (VB.NET).</param>
		new ICallbackAfterResult Callback(Delegate callback);

		/// <summary>
		/// Specifies a callback to invoke after a method's return value has been determined.
		/// </summary>
		/// <param name="action">The callback method to invoke.</param>
		new ICallbackAfterResult Callback(Action action);

		/// <summary>
		/// Specifies a callback to invoke after a method's return value has been determined.
		/// </summary>
		/// <typeparam name="T">The argument type of the invoked method.</typeparam>
		/// <param name="action">The callback method to invoke.</param>
		new ICallbackAfterResult Callback<T>(Action<T> action);
	}
}
