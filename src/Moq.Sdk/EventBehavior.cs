using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Stunts;

namespace Moq.Sdk
{
    /// <summary>
    /// An <see cref="IStuntBehavior"/> that keeps track of backing delegates 
    /// for events, combining and removing handlers from them as += and -= 
    /// are invoked on the mock. Also raises events when an <see cref="EventRaiser"/> 
    /// exists in the <see cref="CallContext"/>.
    /// </summary>
    public class EventBehavior : IStuntBehavior
    {
        /// <summary>
        /// Determines whether the given invocation is an event 
        /// add or remove handler.
        /// </summary>
        public bool AppliesTo(IMethodInvocation invocation)
            => (invocation.MethodBase.Name.StartsWith("add_", StringComparison.Ordinal) ||
               invocation.MethodBase.Name.StartsWith("remove_", StringComparison.Ordinal));

        /// <inheritdoc />
        public IMethodReturn Execute(IMethodInvocation invocation, ExecuteHandler next)
        {
            var info = invocation.MethodBase.DeclaringType.GetRuntimeEvent(
                invocation.MethodBase.Name.Replace("add_", string.Empty).Replace("remove_", string.Empty));

            if (info != null)
            {
                if (CallContext<EventRaiser>.GetData() is EventRaiser raiser)
                {
                    try
                    {
                        var mock = ((IMocked)invocation.Target).Runtime;
                        mock.Invocations.Remove(invocation);
                        if (mock.State.TryGetValue<Delegate>(info.Name, out var handler) &&
                            handler != null)
                        {
                            switch (raiser)
                            {
                                case EmptyEventRaiser _:
                                    handler.DynamicInvoke(invocation.Target, EventArgs.Empty);
                                    break;
                                case EventArgsEventRaiser a:
                                    handler.DynamicInvoke(invocation.Target, a.EventArgs);
                                    break;
                                case CustomEventRaiser c:
                                    handler.DynamicInvoke(c.Arguments);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    finally
                    {
                        CallContext<EventRaiser>.SetData(null);
                    }
                }
                else
                {
                    if (invocation.Arguments.Count == 1 &&
                        invocation.Arguments.GetValue(0) is Delegate handler)
                    {
                        var mock = ((IMocked)invocation.Target).Runtime;
                        if (invocation.MethodBase.Name.StartsWith("add_", StringComparison.Ordinal))
                            CombineDelegate(info, handler, mock);
                        else
                            RemoveDelegate(info, handler, mock);
                    }
                }
            }

            return next(invocation, next);
        }

        static void CombineDelegate(EventInfo info, Delegate handler, IMockRuntime mock)
        {
            var state = mock.State.GetOrAdd(info.Name, () => handler);
            if (state != handler)
            {
                // Try without locking first
                var newState = Delegate.Combine(state, handler);
                if (!mock.State.TryUpdate(info.Name, newState, state))
                {
                    lock (mock.State)
                    {
                        // Retry with locking
                        state = mock.State.GetOrAdd(info.Name, () => handler);
                        if (state != handler)
                        {
                            // This should never fail, since we're inside the lock
                            Debug.Assert(
                                mock.State.TryUpdate(Delegate.Combine(state, handler), state));
                        }
                    }
                }
            }
        }

        static void RemoveDelegate(EventInfo info, Delegate handler, IMockRuntime mock)
        {
            if (mock.State.TryGetValue<Delegate>(info.Name, out var state))
            {
                // Try without locking first
                var newState = Delegate.Remove(state, handler);
                if (newState != state)
                {
                    if (!mock.State.TryUpdate(info.Name, newState, state))
                    {
                        lock (mock.State)
                        {
                            // Retry with locking
                            if (mock.State.TryGetValue(info.Name, out state))
                            {
                                newState = Delegate.Remove(state, handler);
                                if (newState != state)
                                {
                                    // This should never fail, since we're inside the lock
                                    Debug.Assert(
                                        mock.State.TryUpdate(newState, state));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
