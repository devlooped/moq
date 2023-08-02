// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class EventHandlerCollection
    After:
        sealed class EventHandlerCollection
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class EventHandlerCollection
    After:
        sealed class EventHandlerCollection
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class EventHandlerCollection
    After:
        sealed class EventHandlerCollection
    */
    sealed class EventHandlerCollection

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private readonly Dictionary<EventInfo, Delegate> eventHandlers;
    After:
            readonly Dictionary<EventInfo, Delegate> eventHandlers;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private readonly Dictionary<EventInfo, Delegate> eventHandlers;
    After:
            readonly Dictionary<EventInfo, Delegate> eventHandlers;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private readonly Dictionary<EventInfo, Delegate> eventHandlers;
    After:
            readonly Dictionary<EventInfo, Delegate> eventHandlers;
    */
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
                this.eventHandlers[@event] = Delegate.Remove(this.TryGet(@event), eventHandler);
            }
        }

        public bool TryGet(EventInfo @event, out Delegate handlers)
        {
            lock (this.eventHandlers)
            {
                return this.eventHandlers.TryGetValue(@event, out handlers) && handlers != null;

                /* Unmerged change from project 'Moq(netstandard2.0)'
                Before:
                        private Delegate TryGet(EventInfo @event)
                After:
                        Delegate TryGet(EventInfo @event)
                */

                /* Unmerged change from project 'Moq(netstandard2.1)'
                Before:
                        private Delegate TryGet(EventInfo @event)
                After:
                        Delegate TryGet(EventInfo @event)
                */

                /* Unmerged change from project 'Moq(net6.0)'
                Before:
                        private Delegate TryGet(EventInfo @event)
                After:
                        Delegate TryGet(EventInfo @event)
                */
            }
        }

        Delegate TryGet(EventInfo @event)
        {
            return this.eventHandlers.TryGetValue(@event, out var handlers) ? handlers : null;
        }
    }
}
