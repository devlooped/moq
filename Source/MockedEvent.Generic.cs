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

//    * Neither the name of the <ORGANIZATION> nor the 
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

namespace Moq
{
	/// <summary>
	/// Provides a typed <see cref="MockedEvent"/> for a 
	/// specific type of <see cref="EventArgs"/>.
	/// </summary>
	/// <typeparam name="TEventArgs">The type of event arguments required by the event.</typeparam>
	/// <remarks>
	/// The mocked event can either be a <see cref="EventHandler{TEventArgs}"/> or custom 
	/// event handler which follows .NET practice of providing <c>object sender, EventArgs args</c> 
	/// kind of signature.
	/// </remarks>
	public class MockedEvent<TEventArgs> : MockedEvent
		where TEventArgs : EventArgs
	{
		internal MockedEvent(Mock mock)
			: base(mock)
		{
		}

		/// <summary>
		/// Raises the associated event with the given 
		/// event argument data.
		/// </summary>
		public void Raise(TEventArgs args)
		{
			base.DoRaise(args);
		}

		/// <summary>
		/// Provides support for attaching a <see cref="MockedEvent{TEventArgs}"/> to 
		/// a generic <see cref="EventHandler{TEventArgs}"/> event.
		/// </summary>
		public static implicit operator EventHandler<TEventArgs>(MockedEvent<TEventArgs> mockEvent)
		{
			return mockEvent.Handle;
		}

		/// <summary>
		/// Provided solely to allow the interceptor to determine when the attached 
		/// handler is coming from this mocked event so we can assign the 
		/// corresponding EventInfo for it.
		/// </summary>
		private void Handle(object sender, TEventArgs args)
		{
		}
	}
}
