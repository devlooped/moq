using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Moq.Proxy;

namespace Moq
{
	internal enum InterceptionAction
	{
 		Continue,
		Stop
	}

	internal interface IInterceptStrategy
	{
		/// <summary>
		/// Handle interception
		/// </summary>
		/// <param name="invocation">the current invocation context</param>
		/// <param name="ctx">shared data for the interceptor as a whole</param>
		/// <returns>InterceptionAction.Continue if further interception has to be processed, otherwise InterceptionAction.Stop</returns>
		InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx);
	}

	internal class InterceptorContext
	{
		private Dictionary<string, List<Delegate>> invocationLists = new Dictionary<string, List<Delegate>>();
		private List<ICallContext> actualInvocations = new List<ICallContext>();

		// Using a stack has the advantage that enumeration returns the items in reverse order (last added to first added).
		// This helps in implementing the rule that "given two matching setups, the last one wins."
		private Stack<IProxyCall> orderedCalls = new Stack<IProxyCall>();

		public InterceptorContext(Mock Mock, Type targetType, MockBehavior behavior)
		{
			this.Behavior = behavior;
			this.Mock = Mock;
			this.TargetType = targetType;
		}
		public Mock Mock { get; private set; }
		public Type TargetType { get; private set; }
		public MockBehavior Behavior { get; private set; }
		
		#region InvocationLists
		internal IEnumerable<Delegate> GetInvocationList(EventInfo ev)
		{
			lock (invocationLists)
			{
				List<Delegate> handlers;
				if (!invocationLists.TryGetValue(ev.Name, out handlers))
				{
					return new Delegate[0];
				}

				return handlers.ToList();
			}
		}

		internal void AddEventHandler(EventInfo ev, Delegate handler)
		{
			lock (invocationLists)
			{
				List<Delegate> handlers;
				if (!invocationLists.TryGetValue(ev.Name, out handlers))
				{
					handlers = new List<Delegate>();
					invocationLists.Add(ev.Name, handlers);
				}

				handlers.Add(handler);
			}
		}
		internal void RemoveEventHandler(EventInfo ev, Delegate handler)
		{
			lock (invocationLists)
			{
				List<Delegate> handlers;
				if (invocationLists.TryGetValue(ev.Name, out handlers))
				{
					handlers.Remove(handler);
				}
			}
		}
		internal void ClearEventHandlers()
		{
			lock (invocationLists)
			{
				invocationLists.Clear();
			}
		}
		#endregion
		#region ActualInvocations
		internal void AddInvocation(ICallContext invocation)
		{
			lock (actualInvocations)
			{
				actualInvocations.Add(invocation);
			}
		}
		internal IEnumerable<ICallContext> GetActualInvocations()
		{
			lock (actualInvocations)
			{
				return actualInvocations.ToArray();
			}
		}
		internal void ClearInvocations()
		{
			lock (actualInvocations)
			{
				actualInvocations.Clear();
			}
		}
		#endregion
		#region OrderedCalls
		internal void AddOrderedCall(IProxyCall call)
		{
			lock (orderedCalls)
			{
				orderedCalls.Push(call);
			}
		}
		internal void ClearOrderedCalls()
		{
			lock (orderedCalls)
			{
				orderedCalls.Clear();
			}
		}

		internal IProxyCall GetOrderedCallFor(ICallContext invocation)
		{
			IProxyCall matchingSetup = null;

			lock (this.orderedCalls)
			{
				foreach (var setup in this.orderedCalls)
				{
					// the following conditions are repetitive, but were written that way to avoid
					// unnecessary expensive calls to `setup.Matches`; cheap tests are run first.
					if (matchingSetup == null && setup.Matches(invocation))
					{
						matchingSetup = setup;
						if (setup.Method == invocation.Method)
						{
							break;
						}
					}
					else if (setup.Method == invocation.Method && setup.Matches(invocation))
					{
						matchingSetup = setup;
						break;
					}
				}
			}

			return matchingSetup;
		}

		internal IEnumerable<IProxyCall> GetOrderedCalls()
		{
			lock (orderedCalls)
			{
				return orderedCalls.ToArray();
			}
		}
		#endregion

	}
}
