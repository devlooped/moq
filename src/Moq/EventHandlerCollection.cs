// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Moq
{
	internal sealed class EventHandlerCollection
	{
		private readonly Dictionary<EventInfo, Delegate> eventHandlers;

		public EventHandlerCollection()
		{
			this.eventHandlers = new Dictionary<EventInfo, Delegate>();
		}

		public void Add(EventInfo @event, Delegate eventHandler)
		{
			lock (this.eventHandlers)
			{
				this.eventHandlers[@event] = Delegate.Combine(this.TryGet(@event), eventHandler);
			}
		}

		public void Clear()
		{
			lock (this.eventHandlers)
			{
				this.eventHandlers.Clear();
			}
		}

		public void Remove(EventInfo @event, Delegate eventHandler)
		{
			lock (this.eventHandlers)
			{
				this.eventHandlers[@event] = Delegate.Remove(this.TryGet(@event), eventHandler);
			}
		}

		public bool TryGet(EventInfo @event, out Delegate handlers)
		{
			lock (this.eventHandlers)
			{
				return this.eventHandlers.TryGetValue(@event, out handlers) && handlers != null;
			}
		}

		private Delegate TryGet(EventInfo @event)
		{
			return this.eventHandlers.TryGetValue(@event, out var handlers) ? handlers : null;
		}
	}
}
