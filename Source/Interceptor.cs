//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
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

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq.Proxy;

namespace Moq
{
	/// <summary>
	/// Implements the actual interception and method invocation for 
	/// all mocks.
	/// </summary>
	internal class Interceptor : ICallInterceptor
	{
		private MockBehavior behavior;
		private Type targetType;
		private Dictionary<ExpressionKey, IProxyCall> calls = new Dictionary<ExpressionKey, IProxyCall>();
		private Dictionary<string, List<Delegate>> invocationLists = new Dictionary<string, List<Delegate>>();
		private List<IProxyCall> orderedCalls = new List<IProxyCall>();
		private List<ICallContext> actualInvocations = new List<ICallContext>();

		public Interceptor(MockBehavior behavior, Type targetType, Mock mock)
		{
			this.behavior = behavior;
			this.targetType = targetType;
			this.Mock = mock;
		}

		internal IEnumerable<ICallContext> ActualCalls
		{
			get
			{
			    lock (actualInvocations)
			    {
                    return this.actualInvocations.ToArray();
			    }
			}
		}

		internal Mock Mock { get; private set; }

		internal IEnumerable<IProxyCall> OrderedCalls
		{
			get { return this.orderedCalls; }
		}

		internal void Verify()
		{
			VerifyOrThrow(call => call.IsVerifiable && !call.Invoked);
		}

		internal void VerifyAll()
		{
			VerifyOrThrow(call => !call.Invoked);
		}

		private void VerifyOrThrow(Func<IProxyCall, bool> match)
		{
			var failures = calls.Values.Where(match).ToArray();
			if (failures.Length > 0)
			{
				throw new MockVerificationException(failures);
			}
		}

		public void AddCall(IProxyCall call, SetupKind kind)
		{
			var expr = call.SetupExpression.PartialMatcherAwareEval();
			var keyText = call.Method.DeclaringType.FullName + "::" + expr.ToStringFixed(true);
			if (kind == SetupKind.PropertySet)
			{
				keyText = "set::" + keyText;
			}

			var constants = new ConstantsVisitor(expr).Values;
			var key = new ExpressionKey(keyText, constants);

			if (!call.IsConditional)
			{
				// if it's not a conditional call, we do
				// all the override setups.
				// TODO maybe add the conditionals to other
				// record like calls to be user friendly and display
				// somethig like: non of this calls were performed.
				if (calls.ContainsKey(key))
				{
					// Remove previous from ordered calls
					orderedCalls.Remove(calls[key]);
				}

				calls[key] = call;
			}

			orderedCalls.Add(call);
		}

		private IEnumerable<IInterceptStrategy> InterceptionStrategies()
		{
			yield return new HandleDestructor();
			yield return new HandleTracking();
			yield return new CheckMockMixing();
			yield return new AddActualInvocation();
			yield return new ExtractProxyCall();
			yield return new ExecuteCall();
			yield return new InvokeBase();
			yield return new HandleMockRecursion();
		}

		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public void Intercept(ICallContext invocation)
		{

            lock (Mock) // this solves issue #249
            {
                var interceptionContext = new InterceptStrategyContext(Mock
                                                                    , targetType
                                                                    , invocationLists
                                                                    , actualInvocations
                                                                    , behavior
                                                                    , orderedCalls
                                                                    );
                foreach (var strategy in InterceptionStrategies())
                {
                    if (InterceptionAction.Stop == strategy.HandleIntercept(invocation, interceptionContext))
                    {
                        break;
                    }
                }
            }
		}
		

		internal IEnumerable<Delegate> GetInvocationList(EventInfo ev)
		{
			List<Delegate> handlers;
			if (!this.invocationLists.TryGetValue(ev.Name, out handlers))
			{
				return new Delegate[0];
			}

			return handlers;
		}

		private class ExpressionKey
		{
			private string fixedString;
			private List<object> values;

			public ExpressionKey(string fixedString, List<object> values)
			{
				this.fixedString = fixedString;
				this.values = values;
			}

			public override bool Equals(object obj)
			{
				if (object.ReferenceEquals(this, obj))
				{
					return true;
				}

				var key = obj as ExpressionKey;
				if (key == null)
				{
					return false;
				}

				var eq = key.fixedString == this.fixedString && key.values.Count == this.values.Count;

				var index = 0;
				while (eq && index < this.values.Count)
				{
					eq |= this.values[index] == key.values[index];
					index++;
				}

				return eq;
			}

			public override int GetHashCode()
			{
				var hash = fixedString.GetHashCode();

				foreach (var value in values)
				{
					if (value != null)
					{
						hash ^= value.GetHashCode();
					}
				}

				return hash;
			}
		}

		private class ConstantsVisitor : ExpressionVisitor
		{
			public ConstantsVisitor(Expression expression)
			{
				this.Values = new List<object>();
				base.Visit(expression);
			}

			public List<object> Values { get; set; }

			protected override Expression VisitConstant(ConstantExpression c)
			{
				if (c != null)
				{
					Values.Add(c.Value);
				}

				return base.VisitConstant(c);
			}
		}
	}
}