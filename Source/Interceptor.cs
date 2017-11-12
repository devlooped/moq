//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
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

using Moq.Diagnostics.Errors;
using Moq.Proxy;

namespace Moq
{
	/// <summary>
	/// Implements the actual interception and method invocation for 
	/// all mocks.
	/// </summary>
	internal class Interceptor : ICallInterceptor
	{
		public Interceptor(MockBehavior behavior, Type targetType, Mock mock)
		{
			InterceptionContext = new InterceptorContext(mock, targetType, behavior);
		}

		internal InterceptorContext InterceptionContext { get; private set; }

		internal bool TryVerify(out UnmatchedSetups error)
		{
			return this.TryVerifyOrThrow(call => call.IsVerifiable && !call.Invoked, out error);
		}

		internal bool TryVerifyAll(out UnmatchedSetups error)
		{
			return this.TryVerifyOrThrow(call => !call.Invoked, out error);
		}

		private bool TryVerifyOrThrow(Func<IProxyCall, bool> match, out UnmatchedSetups error)
		{
			var failures = new Stack<IProxyCall>();

			// The following verification logic will remember each processed setup so that duplicate setups
			// (that is, setups overridden by later setups with an equivalent expression) can be detected.
			// To speed up duplicate detection, they are partitioned according to the method they target.
			var verifiedSetupsPerMethod = new Dictionary<MethodInfo, List<Expression>>();

			foreach (var setup in this.InterceptionContext.GetOrderedCalls())
			{
				if (setup.IsConditional)
				{
					continue;
				}

				List<Expression> verifiedSetupsForMethod;
				if (!verifiedSetupsPerMethod.TryGetValue(setup.Method, out verifiedSetupsForMethod))
				{
					verifiedSetupsForMethod = new List<Expression>();
					verifiedSetupsPerMethod.Add(setup.Method, verifiedSetupsForMethod);
				}

				var expr = setup.SetupExpression.PartialMatcherAwareEval();
				if (verifiedSetupsForMethod.Any(vc => ExpressionComparer.Default.Equals(vc, expr)))
				{
					continue;
				}

				if (match(setup))
				{
					failures.Push(setup);
				}

				verifiedSetupsForMethod.Add(expr);
			}

			if (failures.Any())
			{
				error = new UnmatchedSetups(failures);
				return false;
			}
			else
			{
				error = null;
				return true;
			}
		}

		public void AddCall(IProxyCall call)
		{
			InterceptionContext.AddOrderedCall(call);
		}

		private static Lazy<IInterceptStrategy[]> interceptionStrategies =
			new Lazy<IInterceptStrategy[]>(
				() => new IInterceptStrategy[]
				{
					HandleTracking.Instance,
					HandleWellKnownMethods.Instance,
					AddActualInvocation.Instance,
					ExtractAndExecuteProxyCall.Instance,
					InvokeBase.Instance,
					HandleMockRecursion.Instance,
				});

		private static IInterceptStrategy[] InterceptionStrategies => interceptionStrategies.Value;

		[SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		public void Intercept(ICallContext invocation)
		{
			foreach (var strategy in InterceptionStrategies)
			{
				if (InterceptionAction.Stop == strategy.HandleIntercept(invocation, InterceptionContext))
				{
					break;
				}
			}
		}
	}
}
