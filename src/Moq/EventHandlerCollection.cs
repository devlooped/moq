// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

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
