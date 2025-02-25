// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Moq
{
    sealed class EventHandlerCollection
    {
        readonly Dictionary<EventInfo, Delegate> eventHandlers;

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
                var resultingDelegate = Delegate.Remove(this.TryGet(@event), eventHandler);
                if (resultingDelegate == null)
                {
                    eventHandlers.Remove(@event);
                }
                else
                {
                    eventHandlers[@event] = resultingDelegate;
                }
            }
        }

#if NULLABLE_REFERENCE_TYPES
        public bool TryGet(EventInfo @event, [NotNullWhen(true)] out Delegate? handlers)
#else
        public bool TryGet(EventInfo @event, out Delegate? handlers)
#endif
        {
            lock (this.eventHandlers)
            {
                return this.eventHandlers.TryGetValue(@event, out handlers);
            }
        }

        Delegate? TryGet(EventInfo @event)
        {
            return this.eventHandlers.TryGetValue(@event, out var handlers) ? handlers : null;
        }
    }
}
