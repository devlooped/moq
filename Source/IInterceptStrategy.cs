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

		public InterceptorContext(Mock Mock, Type targetType, MockBehavior behavior)
		{
			this.Behavior = behavior;
			this.Mock = Mock;
			this.TargetType = targetType;
		}
		public Mock Mock { get; private set; }
		public Type TargetType { get; private set; }
		public MockBehavior Behavior { get; private set; }
	}
}
