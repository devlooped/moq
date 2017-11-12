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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moq
{
	internal sealed class EventHandlerCollection
	{
		private Dictionary<string, List<Delegate>> eventHandlers;

		public EventHandlerCollection()
		{
			this.eventHandlers = new Dictionary<string, List<Delegate>>();
		}

		public void Add(string eventName, Delegate eventHandler)
		{
			lock (this.eventHandlers)
			{
				List<Delegate> handlers;
				if (!this.eventHandlers.TryGetValue(eventName, out handlers))
				{
					handlers = new List<Delegate>();
					this.eventHandlers.Add(eventName, handlers);
				}

				handlers.Add(eventHandler);
			}
		}

		public void Clear()
		{
			lock (this.eventHandlers)
			{
				this.eventHandlers.Clear();
			}
		}

		public void Remove(string eventName, Delegate eventHandler)
		{
			lock (this.eventHandlers)
			{
				List<Delegate> handlers;
				if (this.eventHandlers.TryGetValue(eventName, out handlers))
				{
					handlers.Remove(eventHandler);
				}
			}
		}

		public Delegate[] ToArray(string eventName)
		{
			lock (this.eventHandlers)
			{
				List<Delegate> handlers;
				if (!this.eventHandlers.TryGetValue(eventName, out handlers))
				{
					return new Delegate[0];
				}

				return handlers.ToArray();
			}
		}
	}
}
