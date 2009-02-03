//Copyright (c) 2007, Moq Team 
//http://code.google.com/p/moq/
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of the Moq Team nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

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
		Dictionary<ExpressionKey, IProxyCall> calls = new Dictionary<ExpressionKey, IProxyCall>();
		List<IProxyCall> orderedCalls = new List<IProxyCall>();
		Mock mock;
		List<IInvocation> actualInvocations = new List<IInvocation>();

		public Interceptor(MockBehavior behavior, Type targetType, Mock mock)
		{
			this.behavior = behavior;
			this.targetType = targetType;
			this.mock = mock;
		}

		internal IEnumerable<IInvocation> ActualCalls { get { return actualInvocations; } }
		internal Mock Mock { get { return mock; } }

		/// <summary>
		/// Used to call back into a caller whenever an event attach/remove is intercepted.
		/// </summary>
		internal EventInfoCallback EventCallback { get; set; }

		internal void Verify()
		{
			// The IsNever case would have thrown the moment the member is invoked, 
			// so we can safely skip here all such setups.
			VerifyOrThrow(call => call.IsVerifiable && !call.IsNever && !call.Invoked);
		}

		internal void VerifyAll()
		{
			// The IsNever case would have thrown the moment the member is invoked, 
			// so we can safely skip here all such setups.
			VerifyOrThrow(call => !call.IsNever && !call.Invoked);
		}

		private void VerifyOrThrow(Func<IProxyCall, bool> match)
		{
			var failures = new List<IProxyCall>(
				calls.Values.Where(match));

			if (failures.Count > 0)
			{
				throw new MockVerificationException(this.targetType, failures);
			}
		}

		public void AddCall(IProxyCall call, ExpectKind kind)
		{
			var expr = call.SetupExpression.PartialMatcherAwareEval();

			var s = expr.ToStringFixed();

			if (kind == ExpectKind.PropertySet)
				s = "set::" + s;

			var constants = new ConstantsVisitor(expr).Values;
			var key = new ExpressionKey(s, constants);

			if (calls.ContainsKey(key))
			{
				// Remove previous from ordered calls
				orderedCalls.Remove(calls[key]);
			}

			calls[key] = call;
			orderedCalls.Add(call);

			//if (kind == ExpectKind.PropertySet)
			//    calls["set::" + expr.ToStringFixed()] = call;
			//else
			//    calls[expr.ToStringFixed()] = call;
		}

		public void Intercept(IInvocation invocation)
		{
			// TODO: too many ifs in this method.
			// see how to refactor with strategies.
			if (invocation.Method.DeclaringType.IsGenericType &&
			  invocation.Method.DeclaringType.GetGenericTypeDefinition() == typeof(IMocked<>))
			{
				// "Mixin" of IMocked<T>.Mock
				invocation.ReturnValue = mock;
				return;
			}
			else if (invocation.Method.DeclaringType == typeof(IMocked))
			{
				// "Mixin" of IMocked.Mock
				invocation.ReturnValue = mock;
				return;
			}

			// Special case for events.
			if (IsEventAttach(invocation))
			{
				var delegateInstance = (Delegate)invocation.Arguments[0];
				// TODO: validate we can get the event?
				EventInfo eventInfo = GetEventFromName(invocation.Method.Name.Replace("add_", ""));

				if (EventCallback != null) EventCallback.SetEvent(mock, eventInfo);

				if (delegateInstance != null)
				{
					var mockEvent = delegateInstance.Target as MockedEvent;

					if (mockEvent != null)
					{
						mockEvent.Event = eventInfo;
					}
					else
					{
						mock.AddEventHandler(eventInfo, (Delegate)invocation.Arguments[0]);
					}
				}

				return;
			}
			else if (IsEventDetach(invocation))
			{
				var delegateInstance = (Delegate)invocation.Arguments[0];
				// TODO: validate we can get the event?
				EventInfo eventInfo = GetEventFromName(invocation.Method.Name.Replace("remove_", ""));

				if (EventCallback != null) EventCallback.SetEvent(mock, eventInfo);

				if (delegateInstance != null)
				{
					var mockEvent = delegateInstance.Target as MockedEvent;

					if (mockEvent != null)
					{
						mockEvent.Event = null;
					}
					else
					{
						mock.RemoveEventHandler(eventInfo, (Delegate)invocation.Arguments[0]);
					}
				}

				return;
			}

			// always save to support Verify[expression] pattern.
			actualInvocations.Add(invocation);

			var call = orderedCalls.LastOrDefault(c => c.Matches(invocation));

			if (call == null)
			{
				if (behavior == MockBehavior.Strict)
				{
					throw new MockException(
					  MockException.ExceptionReason.NoSetup,
					  behavior,
					  invocation);
				}
			}

			if (call != null)
			{
				call.SetOutParameters(invocation);

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
			  !invocation.Method.IsAbstract
				&& mock.CallBase)
			{
				// For mocked classes, if the target method was not abstract, 
				// invoke directly.
				// Will only get here for Loose behavior.
				// TODO: we may want to provide a way to skip this by the user.
				invocation.Proceed();
			}
			else if (invocation.Method != null && invocation.Method.ReturnType != null &&
			  invocation.Method.ReturnType != typeof(void))
			{
				invocation.ReturnValue = mock.DefaultValueProvider.ProvideDefault(invocation.Method, invocation.Arguments);

				// Event callbacks may need to be set if the returned value is a mock.
				if (invocation.ReturnValue is IMocked && EventCallback != null)
				{
					EventCallback.AddInterceptor(((IMocked)invocation.ReturnValue).Mock.Interceptor);
				}
			}
		}

		private static bool IsEventAttach(IInvocation invocation)
		{
			return invocation.Method.IsSpecialName &&
					  invocation.Method.Name.StartsWith("add_", StringComparison.Ordinal);
		}

		private static bool IsEventDetach(IInvocation invocation)
		{
			return invocation.Method.IsSpecialName &&
						invocation.Method.Name.StartsWith("remove_", StringComparison.Ordinal);
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

		class ExpressionKey
		{
			string fixedString;
			List<object> values;

			public ExpressionKey(string fixedString, List<object> values)
			{
				this.fixedString = fixedString;
				this.values = values;
			}

			public override bool Equals(object obj)
			{
				if (Object.ReferenceEquals(this, obj))
					return true;

				var key = obj as ExpressionKey;

				if (key == null)
					return false;

				var eq = key.fixedString == this.fixedString &&
					key.values.Count == this.values.Count;

				var i = 0;

				while (eq && i < this.values.Count)
				{
					eq |= this.values[i] == key.values[i];
					i++;
				}

				return eq;
			}

			public override int GetHashCode()
			{
				var hash = fixedString.GetHashCode();

				foreach (var value in values)
				{
					if (value != null)
						hash ^= value.GetHashCode();
				}

				return hash;
			}
		}

		class ConstantsVisitor : ExpressionVisitor
		{
			public ConstantsVisitor(Expression expression)
			{
				Values = new List<object>();
				base.Visit(expression);
			}

			public List<object> Values { get; set; }

			protected override Expression VisitConstant(ConstantExpression c)
			{
				Values.Add(c.Value);

				return base.VisitConstant(c);
			}
		}
	}
}
