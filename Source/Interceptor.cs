using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Core.Interceptor;

namespace Moq
{
	/// <summary>
	/// Implements the actual interception and method invocation for 
	/// all mocks.
	/// </summary>
	internal class Interceptor : MarshalByRefObject, IInterceptor
	{
		MockBehavior behavior;
		Type targetType;
		Dictionary<string, IProxyCall> calls = new Dictionary<string, IProxyCall>();
		Mock mock;
		List<IInvocation> actualInvocations = new List<IInvocation>();

		public Interceptor(MockBehavior behavior, Type targetType, Mock mock)
		{
			this.behavior = behavior;
			this.targetType = targetType;
			this.mock = mock;
		}

		internal void Verify()
		{
			VerifyOrThrow(call => call.IsVerifiable && !call.Invoked);
		}

		internal void VerifyAll()
		{
			VerifyOrThrow(call => !call.Invoked);
		}

		private void VerifyOrThrow(Predicate<IProxyCall> match)
		{
			var failures = new List<Expression>();
			foreach (var call in calls.Values)
			{
				if (match(call))
					failures.Add(call.ExpectExpression.PartialMatcherAwareEval());
			}

			if (failures.Count > 0)
			{
				throw new MockVerificationException(this.targetType, failures);
			}
		}

		public void AddCall(IProxyCall call, ExpectKind kind)
		{
			var expr = call.ExpectExpression.PartialMatcherAwareEval();

			if (kind == ExpectKind.PropertySet)
				calls["set::" + expr.ToStringFixed()] = call;
			else
				calls[expr.ToStringFixed()] = call;
		}

		public void Intercept(IInvocation invocation)
		{
			// TODO: too many ifs in this method.
			// see how to refactor with strategies.
			if (invocation.Method.DeclaringType.IsGenericType &&
			  invocation.Method.DeclaringType.GetGenericTypeDefinition() == typeof(IMocked<>))
			{
				// "Mixin" of IMocked
				invocation.ReturnValue = mock;
				return;
			}

			// Special case for events.
			if (IsEventAttach(invocation))
			{
				var delegateInstance = (Delegate)invocation.Arguments[0];
				// TODO: validate we can get the event?
				EventInfo eventInfo = GetEventFromName(invocation.Method.Name.Replace("add_", ""));
				var mockEvent = delegateInstance.Target as MockedEvent;

				if (mockEvent != null)
				{
					mockEvent.Event = eventInfo;
				}
				else
				{
					mock.AddEventHandler(eventInfo, (Delegate)invocation.Arguments[0]);
				}

				return;
			}

			// always save to support Verify[expression] pattern.
			actualInvocations.Add(invocation);

			var call = (from c in calls.Values
							where c.Matches(invocation)
							select c).FirstOrDefault();

			if (call == null)
			{
				if (behavior == MockBehavior.Strict)
				{
					throw new MockException(
					  MockException.ExceptionReason.NoExpectation,
					  behavior,
					  invocation);
				}
				// TODO: remove ExceptionReason and corresponding string resource
				//else if (behavior == MockBehavior.Normal)
				//{
				//   if (invocation.Method.DeclaringType.IsInterface)
				//   {
				//      throw new MockException(
				//         MockException.ExceptionReason.InterfaceNoExpectation,
				//         behavior, invocation);
				//   }
				//   else if (invocation.Method.IsAbstract)
				//   {
				//      throw new MockException(
				//         MockException.ExceptionReason.AbstractNoExpectation,
				//         behavior, invocation);
				//   }
				//}
			}

			if (call != null)
			{
				// We first execute, as there may be a Throws 
				// and therefore we might never get to the 
				// next line.
				call.Execute(invocation);
				ThrowIfReturnValueRequired(call, invocation);
			}
			else if (invocation.Method.DeclaringType == typeof(object))
			{
				// Invoke underlying implementation.
				invocation.Proceed();
			}
			else if (invocation.TargetType.IsClass &&
			  !invocation.Method.IsAbstract)
			{
				// For mocked classes, if the target method was not abstract, 
				// invoke directly.
				// Will only get here for Normal and below behaviors.
				// TODO: we may want to provide a way to skip this by the user.
				invocation.Proceed();
			}
			else if (invocation.Method != null && invocation.Method.ReturnType != null &&
			  invocation.Method.ReturnType != typeof(void))
			{
				if (behavior == MockBehavior.Loose)
				{
					invocation.ReturnValue = new DefaultValue(invocation.Method.ReturnType).Value;
				}
				else
				{
					// TODO: remove when we remove Relaxed.
					throw new MockException(
					  MockException.ExceptionReason.ReturnValueRequired,
					  behavior, invocation);
				}
			}
		}

		private static bool IsEventAttach(IInvocation invocation)
		{
			return invocation.Method.IsSpecialName &&
					  invocation.Method.Name.StartsWith("add_");
		}

		/// <summary>
		/// Get an eventInfo for a given event name.  Search type ancestors depth first if necessary.
		/// </summary>
		/// <param name="eventName">Name of the event, with the set_ or get_ prefix already removed</param>
		private EventInfo GetEventFromName(string eventName)
		{
			var depthFirstProgress = new Queue<Type>();
			depthFirstProgress.Enqueue(targetType);
			while (depthFirstProgress.Count > 0)
			{
				var currentType = depthFirstProgress.Dequeue();
				var eventInfo = currentType.GetEvent(eventName);
				if (eventInfo != null)
					return eventInfo;
				//else
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
		private IEnumerable<Type> GetAncestorTypes(Type initialType)
		{
			Type baseType = initialType.BaseType;
			if (baseType != null)
				yield return baseType;
			foreach (var implementedInterface in initialType.FindInterfaces(new TypeFilter((foo, bar) => true), null))
			{
				yield return implementedInterface;
			}
		}

		private void ThrowIfReturnValueRequired(IProxyCall call, IInvocation invocation)
		{
			if (behavior != MockBehavior.Loose &&
				invocation.Method != null &&
				invocation.Method.ReturnType != null &&
				invocation.Method.ReturnType != typeof(void))
			{
				var methodCall = call as MethodCallReturn;
				if (methodCall == null || !methodCall.HasReturnValue)
				{
					throw new MockException(
						MockException.ExceptionReason.ReturnValueRequired,
						behavior, invocation);
				}
			}
		}
	}
}
