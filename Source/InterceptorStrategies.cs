using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Proxy;
using System.Reflection;

namespace Moq
{
	internal class HandleMockRecursion : IInterceptStrategy
	{
		public bool HandleIntercept(ICallContext invocation, InterceptStrategyContext ctx)
		{
			if (invocation.Method != null && invocation.Method.ReturnType != null &&
					invocation.Method.ReturnType != typeof(void))
			{
				Mock recursiveMock;
				if (ctx.Mock.InnerMocks.TryGetValue(invocation.Method, out recursiveMock))
				{
					invocation.ReturnValue = recursiveMock.Object;
				}
				else
				{
					invocation.ReturnValue = ctx.Mock.DefaultValueProvider.ProvideDefault(invocation.Method);
				}
				return false;
			}
			return true;
		}
	}
	
	internal class InvokeBase : IInterceptStrategy
	{
		public bool HandleIntercept(ICallContext invocation, InterceptStrategyContext ctx)
		{
			if (invocation.Method.DeclaringType == typeof(object)
				||
				invocation.Method.DeclaringType.IsClass && !invocation.Method.IsAbstract && ctx.Mock.CallBase
				)
			{
				// Invoke underlying implementation.

				// For mocked classes, if the target method was not abstract, 
				// invoke directly.
				// Will only get here for Loose behavior.
				// TODO: we may want to provide a way to skip this by the user.
				invocation.InvokeBase();
				return false;
			}
			else
			{
				return true;
			}
		}
	}

	internal class ExecuteCall : IInterceptStrategy
	{
		InterceptStrategyContext ctx;
		public bool HandleIntercept(ICallContext invocation, InterceptStrategyContext ctx)
		{
			this.ctx = ctx;
			if (ctx.CurrentCall != null)
			{
				ctx.CurrentCall.SetOutParameters(invocation);

				// We first execute, as there may be a Throws 
				// and therefore we might never get to the 
				// next line.
				ctx.CurrentCall.Execute(invocation);
				ThrowIfReturnValueRequired(ctx.CurrentCall, invocation);
				return false;
			}
			else
			{
				return true;
			}
		}
		private void ThrowIfReturnValueRequired(IProxyCall call, ICallContext invocation)
		{
			if (ctx.Behavior != MockBehavior.Loose &&
				invocation.Method != null &&
				invocation.Method.ReturnType != null &&
				invocation.Method.ReturnType != typeof(void))
			{
				var methodCall = call as MethodCallReturn;
				if (methodCall == null || !methodCall.HasReturnValue)
				{
					throw new MockException(
						MockException.ExceptionReason.ReturnValueRequired,
						ctx.Behavior,
						invocation);
				}
			}
		}
	}

	internal class ExtractProxyCall : IInterceptStrategy
	{

		public bool HandleIntercept(ICallContext invocation, InterceptStrategyContext ctx)
		{
			ctx.CurrentCall = FluentMockContext.IsActive ? (IProxyCall)null : ctx.OrderedCalls.LastOrDefault(c => c.Matches(invocation));
			if (ctx.CurrentCall == null && !FluentMockContext.IsActive && ctx.Behavior == MockBehavior.Strict)
			{
				throw new MockException(MockException.ExceptionReason.NoSetup, ctx.Behavior, invocation);
			}
			return true;
		}
	}

	internal class CheckMockMixing:IInterceptStrategy
	{
		
		public CheckMockMixing()
		{
			
		}
		public bool HandleIntercept(ICallContext invocation, InterceptStrategyContext ctx)
		{
			if (invocation.Method.DeclaringType.IsGenericType &&
					invocation.Method.DeclaringType.GetGenericTypeDefinition() == typeof(IMocked<>))
			{
				// "Mixin" of IMocked<T>.Mock
				invocation.ReturnValue = ctx.Mock;
				return false;
			}
			else if (invocation.Method.DeclaringType == typeof(IMocked))
			{
				// "Mixin" of IMocked.Mock
				invocation.ReturnValue = ctx.Mock;
				return false;
			}
			return true;
		}
	}

	internal class HandleTracking : IInterceptStrategy
	{
		
		public HandleTracking()
		{
			
		}
		public bool HandleIntercept(ICallContext invocation, InterceptStrategyContext ctx)
		{
			// Track current invocation if we're in "record" mode in a fluent invocation context.
			if (FluentMockContext.IsActive)
			{
				FluentMockContext.Current.Add(ctx.Mock, invocation);
			}
			return true;
		}
	}

	internal class HandleDestructor : IInterceptStrategy
	{
		public bool HandleIntercept(ICallContext invocation, InterceptStrategyContext ctx)
		{
			if (invocation.Method.IsDestructor())
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}

	internal class AddActualInvocation : IInterceptStrategy
	{
		
		public AddActualInvocation()
		{
			
		}
		/// <summary>
		/// Get an eventInfo for a given event name.  Search type ancestors depth first if necessary.
		/// </summary>
		/// <param name="eventName">Name of the event, with the set_ or get_ prefix already removed</param>
		private EventInfo GetEventFromName(string eventName)
		{
			var depthFirstProgress = new Queue<Type>(ctx.Mock.ImplementedInterfaces.Skip(1));
			depthFirstProgress.Enqueue(ctx.TargetType);
			while (depthFirstProgress.Count > 0)
			{
				var currentType = depthFirstProgress.Dequeue();
				var eventInfo = currentType.GetEvent(eventName);
				if (eventInfo != null)
				{
					return eventInfo;
				}

				foreach (var implementedType in GetAncestorTypes(currentType))
				{
					depthFirstProgress.Enqueue(implementedType);
				}
			}

			return null;
		}
		/// <summary>
		/// Given a type return all of its ancestors, both types and interfaces.
		/// </summary>
		/// <param name="initialType">The type to find immediate ancestors of</param>
		private static IEnumerable<Type> GetAncestorTypes(Type initialType)
		{
			var baseType = initialType.BaseType;
			if (baseType != null)
			{
				return new[] { baseType };
			}

			return initialType.GetInterfaces();
		}
		internal void AddEventHandler(EventInfo ev, Delegate handler)
		{
			List<Delegate> handlers;
			if (!ctx.InvocationLists.TryGetValue(ev.Name, out handlers))
			{
				handlers = new List<Delegate>();
				ctx.InvocationLists.Add(ev.Name, handlers);
			}

			handlers.Add(handler);
		}
		internal void RemoveEventHandler(EventInfo ev, Delegate handler)
		{
			List<Delegate> handlers;
			if (ctx.InvocationLists.TryGetValue(ev.Name, out handlers))
			{
				handlers.Remove(handler);
			}
		}
		InterceptStrategyContext ctx;
		public bool HandleIntercept(ICallContext invocation, InterceptStrategyContext ctx)
		{
			this.ctx = ctx;
			if (!FluentMockContext.IsActive)
			{
				//Special case for events
				if (invocation.Method.IsEventAttach())
				{
					var delegateInstance = (Delegate)invocation.Arguments[0];
					// TODO: validate we can get the event?
					var eventInfo = this.GetEventFromName(invocation.Method.Name.Substring(4));

					if (ctx.Mock.CallBase)
					{
						invocation.InvokeBase();
					}
					else if (delegateInstance != null)
					{
						this.AddEventHandler(eventInfo, (Delegate)invocation.Arguments[0]);
					}

					return false;
				}
				else if (invocation.Method.IsEventDetach())
				{


					if (ctx.Mock.CallBase)
					{
						invocation.InvokeBase();
					}
					else
					{
						var delegateInstance = (Delegate)invocation.Arguments[0];
						if (delegateInstance != null)
						{
							// TODO: validate we can get the event?
							var eventInfo = this.GetEventFromName(invocation.Method.Name.Substring(7));
							this.RemoveEventHandler(eventInfo, (Delegate)invocation.Arguments[0]);
						}
					}

					return false;
				}

				// Save to support Verify[expression] pattern.
				// In a fluent invocation context, which is a recorder-like 
				// mode we use to evaluate delegates by actually running them, 
				// we don't want to count the invocation, or actually run 
				// previous setups.
				ctx.ActualInvocations.Add(invocation);
			}
			return true;
		}
	}
}
