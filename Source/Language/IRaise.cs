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
using System;
using System.ComponentModel;

namespace Moq.Language
{
	/// <summary>
	/// Defines the <c>Raises</c> verb.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public partial interface IRaise<T> : IHideObjectMembers
	{
		/// <summary>
		/// Specifies the event that will be raised 
		/// when the setup is met.
		/// </summary>
		/// <param name="eventExpression">An expression that represents an event attach or detach action.</param>
		/// <param name="args">The event arguments to pass for the raised event.</param>
		/// <example>
		/// The following example shows how to raise an event when 
		/// the setup is met:
		/// <code>
		/// var mock = new Mock&lt;IContainer&gt;();
		/// 
		/// mock.Setup(add => add.Add(It.IsAny&lt;string&gt;(), It.IsAny&lt;object&gt;()))
		///     .Raises(add => add.Added += null, EventArgs.Empty);
		/// </code>
		/// </example>
		IVerifies Raises(Action<T> eventExpression, EventArgs args);

		/// <summary>
		/// Specifies the event that will be raised 
		/// when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">An expression that represents an event attach or detach action.</param>
		/// <param name="func">A function that will build the <see cref="EventArgs"/> 
		/// to pass when raising the event.</param>
		/// <seealso cref="Raises(Action{T}, EventArgs)"/>
		IVerifies Raises(Action<T> eventExpression, Func<EventArgs> func);

		/// <summary>
		/// Specifies the custom event that will be raised 
		/// when the setup is matched.
		/// </summary>
		/// <param name="eventExpression">An expression that represents an event attach or detach action.</param>
		/// <param name="args">The arguments to pass to the custom delegate (non EventHandler-compatible).</param>
		IVerifies Raises(Action<T> eventExpression, params object[] args);
	}
}