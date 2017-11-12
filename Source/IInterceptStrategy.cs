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
		/// <param name="mock">The mock on which the current invocation is occurring.</param>
		/// <returns>InterceptionAction.Continue if further interception has to be processed, otherwise InterceptionAction.Stop</returns>
		InterceptionAction HandleIntercept(ICallContext invocation, Mock mock);
	}
}
