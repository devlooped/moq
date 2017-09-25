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
		public static HandleMockRecursion Instance { get; } = new HandleMockRecursion();

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
		public static InvokeBase Instance { get; } = new InvokeBase();

		public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
		{
			if (invocation.Method.DeclaringType == typeof(object) || // interface proxy
				ctx.Mock.ImplementedInterfaces.Contains(invocation.Method.DeclaringType) && !invocation.Method.LooksLikeEventAttach() && !invocation.Method.LooksLikeEventDetach() && ctx.Mock.CallBase && !ctx.Mock.MockedType.GetTypeInfo().IsInterface || // class proxy with explicitly implemented interfaces. The method's declaring type is the interface and the method couldn't be abstract
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
		public static ExecuteCall Instance { get; } = new ExecuteCall();

		public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
		{
			IProxyCall currentCall = localctx.Call;

			if (currentCall != null)
			{
				currentCall.SetOutParameters(invocation);

				// We first execute, as there may be a Throws 
				// and therefore we might never get to the 
				// next line.
				currentCall.Execute(invocation);
				ThrowIfReturnValueRequired(currentCall, invocation, ctx);
				return InterceptionAction.Stop;
			}
			else
			{
				return InterceptionAction.Continue;
			}
		}

		private static void ThrowIfReturnValueRequired(IProxyCall call, ICallContext invocation, InterceptorContext ctx)
		{
			if (ctx.Behavior != MockBehavior.Loose &&
				invocation.Method != null &&
				invocation.Method.ReturnType != null &&
				invocation.Method.ReturnType != typeof(void))
			{
				if (!(call is IMethodCallReturn methodCallReturn && methodCallReturn.ProvidesReturnValue()))
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
		public static ExtractProxyCall Instance { get; } = new ExtractProxyCall();

		public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
		{
			if (FluentMockContext.IsActive)
			{
				localctx.Call = null;
			}
			else
			{
				IProxyCall lastMatchingSetup = null;
				IProxyCall lastMatchingSetupForSameMethod = null;
				foreach (var setup in ctx.OrderedCalls)
				{
					if (setup.Matches(invocation))
					{
						lastMatchingSetup = setup;
						if (setup.Method == invocation.Method)
						{
							lastMatchingSetupForSameMethod = setup;
						}
					}
				}
				localctx.Call = lastMatchingSetupForSameMethod ?? lastMatchingSetup;
			}

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
		public static InterceptMockPropertyMixin Instance { get; } = new InterceptMockPropertyMixin();

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
		public static InterceptObjectMethodsMixin Instance { get; } = new InterceptObjectMethodsMixin();

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

		private static bool IsObjectMethod(MethodInfo method, string name)
		{
			return method.DeclaringType == typeof(object) && method.Name == name;
		}
	}

	internal class HandleTracking : IInterceptStrategy
	{
		public static HandleTracking Instance { get; } = new HandleTracking();

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
		public static HandleDestructor Instance { get; } = new HandleDestructor();

		public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
		{
			return invocation.Method.IsDestructor() ? InterceptionAction.Stop : InterceptionAction.Continue;
		}
	}

	internal class AddActualInvocation : IInterceptStrategy
	{
		public static AddActualInvocation Instance { get; } = new AddActualInvocation();

		public InterceptionAction HandleIntercept(ICallContext invocation, InterceptorContext ctx, CurrentInterceptContext localctx)
		{
			if (!FluentMockContext.IsActive)
			{
				//Special case for events
				if (invocation.Method.LooksLikeEventAttach())
				{
					var eventInfo = GetEventFromName(invocation.Method.Name.Substring("add_".Length), ctx);
					if (eventInfo != null)
					{
						// TODO: We could compare `invocation.Method` and `eventInfo.GetAddMethod()` here.
						// If they are equal, then `invocation.Method` is definitely an event `add` accessor.
						// Not sure whether this would work with F# and COM; see commit 44070a9.

						if (ctx.Mock.CallBase && !invocation.Method.IsAbstract)
						{
							invocation.InvokeBase();
							return InterceptionAction.Stop;
						}
						else if (invocation.Arguments.Length > 0 && invocation.Arguments[0] is Delegate delegateInstance)
						{
							ctx.AddEventHandler(eventInfo, delegateInstance);
							return InterceptionAction.Stop;
						}
					}

					// wasn't an event attach accessor after all
					return InterceptionAction.Continue;
				}
				else if (invocation.Method.LooksLikeEventDetach())
				{
					var eventInfo = GetEventFromName(invocation.Method.Name.Substring("remove_".Length), ctx);
					if (eventInfo != null)
					{
						// TODO: We could compare `invocation.Method` and `eventInfo.GetRemoveMethod()` here.
						// If they are equal, then `invocation.Method` is definitely an event `remove` accessor.
						// Not sure whether this would work with F# and COM; see commit 44070a9.

						if (ctx.Mock.CallBase && !invocation.Method.IsAbstract)
						{
							invocation.InvokeBase();
							return InterceptionAction.Stop;
						}
						else if (invocation.Arguments.Length > 0 && invocation.Arguments[0] is Delegate delegateInstance)
						{
							ctx.RemoveEventHandler(eventInfo, delegateInstance);
							return InterceptionAction.Stop;
						}
					}

					// wasn't an event detach accessor after all
					return InterceptionAction.Continue;
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

		/// <summary>
		/// Get an eventInfo for a given event name.  Search type ancestors depth first if necessary.
		/// </summary>
		/// <param name="eventName">Name of the event, with the set_ or get_ prefix already removed</param>
		/// <param name="ctx"/>
		private static EventInfo GetEventFromName(string eventName, InterceptorContext ctx)
		{
			return GetEventFromName(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, ctx)
				?? GetEventFromName(eventName, BindingFlags.Instance | BindingFlags.NonPublic, ctx);
		}

		/// <summary>
		/// Get an eventInfo for a given event name.  Search type ancestors depth first if necessary.
		/// Searches events using the specified binding constraints.
		/// </summary>
		/// <param name="eventName">Name of the event, with the set_ or get_ prefix already removed</param>
		/// <param name="bindingAttr">Specifies how the search for events is conducted</param>
		/// <param name="ctx"/>
		private static EventInfo GetEventFromName(string eventName, BindingFlags bindingAttr, InterceptorContext ctx)
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
	}
}
