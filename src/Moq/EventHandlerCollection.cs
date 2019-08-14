// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;

namespace Moq
{
	internal sealed class EventHandlerCollection
	{
		private readonly Dictionary<string, Delegate> eventHandlers;

		public EventHandlerCollection()
		{
			this.eventHandlers = new Dictionary<string, Delegate>();
		}

		public void Add(string eventName, Delegate eventHandler)
		{
			lock (this.eventHandlers)
			{
				this.eventHandlers[eventName] = Delegate.Combine(this.TryGet(eventName), eventHandler);
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
				this.eventHandlers[eventName] = Delegate.Remove(this.TryGet(eventName), eventHandler);
			}
		}

		public bool TryGet(string eventName, out Delegate handlers)
		{
			lock (this.eventHandlers)
			{
				return this.eventHandlers.TryGetValue(eventName, out handlers) && handlers != null;
			}
		}

		private Delegate TryGet(string eventName)
		{
			return this.eventHandlers.TryGetValue(eventName, out var handlers) ? handlers : null;
		}
	}
}
