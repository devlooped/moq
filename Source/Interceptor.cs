using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Interceptor;

namespace Moq
{
	/// <summary>
	/// Implements the actual interception and method invocation for 
	/// all mocks, even MBROs (via the <see cref="RemotingProxy"/>.
	/// </summary>
	class Interceptor : MarshalByRefObject, IInterceptor
	{
		MockBehavior behavior;
		List<IProxyCall> calls = new List<IProxyCall>();
		//List<MethodInfo> objectMethods = new List<MethodInfo>(new MethodInfo[] {
		//    Reflector<object>.GetMethod(x => x.GetType()), 
		//    Reflector<object>.GetMethod(x => x.Equals(null)), 
		//    Reflector<object>.GetMethod(x => x.GetHashCode()), 
		//    Reflector<object>.GetMethod(x => x.ToString())});

		public Interceptor(MockBehavior behavior)
		{
			this.behavior = behavior;
		}
		
		public void AddCall(IProxyCall call)
		{
			calls.Add(call);
		}

		public void Intercept(IInvocation invocation)
		{
			var call = calls.Find(x => x.Matches(invocation));

			if (call == null)
			{
				if (behavior == MockBehavior.Strict)
				{
					throw new MockException(
						MockException.ExceptionReason.NoExpectation,
						behavior,
						invocation);
				}
				else if (behavior == MockBehavior.Normal)
				{
					if (invocation.Method.DeclaringType.IsInterface)
					{
						throw new MockException(
							MockException.ExceptionReason.InterfaceNoExpectation,
							behavior, invocation);
					}
					else if (invocation.Method.IsAbstract)
					{
						throw new MockException(
							MockException.ExceptionReason.AbstractNoExpectation, 
							behavior, invocation);
					}
				}
			}

			if (call != null)
			{
				call.Execute(invocation);
			}
			else if (invocation.Method.DeclaringType == typeof(object))
			{
				invocation.Proceed();
			}
			else if (invocation.TargetType.IsClass &&
				!invocation.Method.IsAbstract)
			{
				// For mocked classes, if the target method was not abstract, 
				// invoke directly.
				// Will only get here for Normal and below behaviors.
				invocation.Proceed();
			}
			else if (invocation.Method != null && invocation.Method.ReturnType != null &&
				invocation.Method.ReturnType != typeof(void))
			{
				if (behavior == MockBehavior.Loose)
				{
					// Return default value.
					if (invocation.Method.ReturnType.IsValueType)
					    invocation.ReturnValue = 0;
					else
					    invocation.ReturnValue = null;
				}
				else
				{
					// Will only get this far for Relaxed.
					throw new MockException(
						MockException.ExceptionReason.ReturnValueNoExpectation,
						behavior, invocation);
				}
			}
		}
	}
}
