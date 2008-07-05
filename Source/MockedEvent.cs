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
using System.Reflection;

namespace Moq
{
	/// <summary>
	/// Represents a generic event that has been mocked and can 
	/// be rised.
	/// </summary>
	public class MockedEvent
	{
		/// <summary>
		/// Event raised whenever the mocked event is rised.
		/// </summary>
		public event EventHandler Raised;

		Mock mock;

		internal MockedEvent(Mock mock)
		{
			this.mock = mock;
		}

		internal EventInfo Event { get; set; }

		/// <summary>
		/// Provided solely to allow the interceptor to determine when the attached 
		/// handler is coming from this mocked event so we can assign the 
		/// corresponding EventInfo for it.
		/// </summary>
		private void Handle(object sender, EventArgs args)
		{
		}

		/// <summary>
		/// Raises the associated event with the given 
		/// event argument data.
		/// </summary>
		internal void DoRaise(EventArgs args)
		{
			if (Event == null)
				throw new InvalidOperationException(Properties.Resources.RaisedUnassociatedEvent);

			if (Raised != null)
				Raised(this, EventArgs.Empty);

			foreach (var del in mock.GetInvocationList(Event))
			{
				del.InvokePreserveStack(mock.Object, args);
			}
		}

		/// <summary>
		/// Provides support for attaching a <see cref="MockedEvent"/> to 
		/// a generic <see cref="EventHandler"/> event.
		/// </summary>
		/// <param name="mockEvent">Event to convert.</param>
		public static implicit operator EventHandler(MockedEvent mockEvent)
		{
			return mockEvent.Handle;
		}
	}
}
