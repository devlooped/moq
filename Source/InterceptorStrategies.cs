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
		public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
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
                return InterceptionAction.Stop;
            }
            return InterceptionAction.Continue;
        }
    }

    internal class InvokeBase : IInterceptStrategy
    {
        public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
        {
            if (invocation.Method.DeclaringType == typeof(object) || // interface proxy
                ctx.Mock.ImplementedInterfaces.Contains(invocation.Method.DeclaringType) && !invocation.Method.IsEventAttach() && !invocation.Method.IsEventDetach() && ctx.Mock.CallBase && !ctx.Mock.MockedType.GetTypeInfo().IsInterface || // class proxy with explicitly implemented interfaces. The method's declaring type is the interface and the method couldn't be abstract
                invocation.Method.DeclaringType.GetTypeInfo().IsClass && !invocation.Method.IsAbstract && ctx.Mock.CallBase // class proxy
                )
            {
                // Invoke underlying implementation.

                // For mocked classes, if the target method was not abstract, 
                // invoke directly.
                // Will only get here for Loose behavior.
                // TODO: we may want to provide a way to skip this by the user.
                invocation.InvokeBase();
                return InterceptionAction.Stop;
            }
            else
            {
                return InterceptionAction.Continue;
            }
        }
    }

    internal class ExecuteCall : IInterceptStrategy
    {
        InterceptorContext ctx;
        public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
        {
            this.ctx = ctx;
            IProxyCall currentCall = localctx.Call;

            if (currentCall != null)
            {
                currentCall.SetOutParameters(invocation);

                // We first execute, as there may be a Throws 
                // and therefore we might never get to the 
                // next line.
                currentCall.Execute(invocation);
                ThrowIfReturnValueRequired(currentCall, invocation);
                return InterceptionAction.Stop;
            }
            else
            {
                return InterceptionAction.Continue;
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

        public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
        {
            localctx.Call = FluentMockContext.IsActive ? (IProxyCall)null : ctx.OrderedCalls.LastOrDefault(c => c.Matches(invocation));
            if (localctx.Call != null)
            {
                localctx.Call.EvaluatedSuccessfully();
            }
            else if (!FluentMockContext.IsActive && ctx.Behavior == MockBehavior.Strict)
            {
                throw new MockException(MockException.ExceptionReason.NoSetup, ctx.Behavior, invocation);
            }

            return InterceptionAction.Continue;
        }

    }

    internal class InterceptMockPropertyMixin : IInterceptStrategy
    {
        public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
        {
            var method = invocation.Method;

            if (typeof(IMocked).IsAssignableFrom(method.DeclaringType) && method.Name == "get_Mock")
            {
                invocation.ReturnValue = ctx.Mock;
                return InterceptionAction.Stop;
            }

            return InterceptionAction.Continue;
        }
    }

	/// <summary>
	/// Intercept strategy that handles `System.Object` methods.
	/// </summary>
	internal class InterceptObjectMethodsMixin : IInterceptStrategy
	{
		public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
		{
			var method = invocation.Method;

			// Only if there is no corresponding setup for `ToString()`
			if (IsObjectMethod(method, "ToString") && !ctx.OrderedCalls.Any(c => IsObjectMethod(c.Method, "ToString")))
			{
				invocation.ReturnValue = ctx.Mock.ToString() + ".Object";
				return InterceptionAction.Stop;
			}

			// Only if there is no corresponding setup for `GetHashCode()`
			if (IsObjectMethod(method, "GetHashCode") && !ctx.OrderedCalls.Any(c => IsObjectMethod(c.Method, "GetHashCode")))
			{
				invocation.ReturnValue = ctx.Mock.GetHashCode();
				return InterceptionAction.Stop;
			}

			// Only if there is no corresponding setup for `Equals()`
			if (IsObjectMethod(method, "Equals") && !ctx.OrderedCalls.Any(c => IsObjectMethod(c.Method, "Equals")))
			{
				invocation.ReturnValue = ReferenceEquals(invocation.Arguments.First(), ctx.Mock.Object);
				return InterceptionAction.Stop;
			}

			return InterceptionAction.Continue;
		}

		protected bool IsObjectMethod(MethodInfo method, string name)
		{
			if (method.DeclaringType == typeof(object) && method.Name == name)
			{
				return true;
			}

			return false;
		}
	}

    internal class HandleTracking : IInterceptStrategy
    {

        public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
        {
            // Track current invocation if we're in "record" mode in a fluent invocation context.
            if (FluentMockContext.IsActive)
            {
                FluentMockContext.Current.Add(ctx.Mock, invocation);
            }
            return InterceptionAction.Continue;
        }
    }

    internal class HandleDestructor : IInterceptStrategy
    {
        public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
        {
            return invocation.Method.IsDestructor() ? InterceptionAction.Stop : InterceptionAction.Continue;
        }
    }

    internal class AddActualInvocation : IInterceptStrategy
    {

		/// <summary>
		/// Get an eventInfo for a given event name.  Search type ancestors depth first if necessary.
		/// </summary>
		/// <param name="eventName">Name of the event, with the set_ or get_ prefix already removed</param>
		private EventInfo GetEventFromName(string eventName)
		{
			return GetEventFromName(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public) ??
				GetEventFromName(eventName, BindingFlags.Instance | BindingFlags.NonPublic);
		}

		/// <summary>
		/// Get an eventInfo for a given event name.  Search type ancestors depth first if necessary.
		/// Searches events using the specified binding constraints.
		/// </summary>
		/// <param name="eventName">Name of the event, with the set_ or get_ prefix already removed</param>
		/// <param name="bindingAttr">Specifies how the search for events is conducted</param>
		private EventInfo GetEventFromName(string eventName, BindingFlags bindingAttr)
		{
			// Ignore internally implemented interfaces
			var depthFirstProgress = new Queue<Type>(ctx.Mock.ImplementedInterfaces.Skip(ctx.Mock.InternallyImplementedInterfaceCount));
			depthFirstProgress.Enqueue(ctx.TargetType);
			while (depthFirstProgress.Count > 0)
			{
				var currentType = depthFirstProgress.Dequeue();
				var eventInfo = currentType.GetEvent(eventName, bindingAttr);
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
            var baseType = initialType.GetTypeInfo().BaseType;
            if (baseType != null)
            {
                return new[] { baseType };
            }

            return initialType.GetInterfaces();
        }        
        InterceptorContext ctx;
        public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
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

                    if (ctx.Mock.CallBase && !eventInfo.DeclaringType.GetTypeInfo().IsInterface)
                    {
                        invocation.InvokeBase();
                    }
                    else if (delegateInstance != null)
                    {
                        ctx.AddEventHandler(eventInfo, (Delegate)invocation.Arguments[0]);
                    }

                    return InterceptionAction.Stop;
                }
                else if (invocation.Method.IsEventDetach())
                {
                    var delegateInstance = (Delegate)invocation.Arguments[0];
                    // TODO: validate we can get the event?
                    var eventInfo = this.GetEventFromName(invocation.Method.Name.Substring(7));

                    if (ctx.Mock.CallBase && !eventInfo.DeclaringType.GetTypeInfo().IsInterface)
                    {
                        invocation.InvokeBase();
                    }
                    else if (delegateInstance != null)
                    {
                        ctx.RemoveEventHandler(eventInfo, (Delegate)invocation.Arguments[0]);
                    }

                    return InterceptionAction.Stop;
                }

                // Save to support Verify[expression] pattern.
                // In a fluent invocation context, which is a recorder-like 
                // mode we use to evaluate delegates by actually running them, 
                // we don't want to count the invocation, or actually run 
                // previous setups.
                ctx.AddInvocation(invocation);
            }
            return InterceptionAction.Continue;
        }
    }
}
