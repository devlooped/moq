using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Proxy;

namespace Moq
{

	internal enum InterceptionAction
	{
 		Continue,Stop
	}
	internal interface IInterceptStrategy
	{
		/// <summary>
		/// Handle interception
		/// </summary>
		/// <param name="invocation">the current invocation context</param>
		/// <param name="ctx">shared data among the strategies during an interception</param>
		/// <returns>true if further interception has to be processed, otherwise false</returns>
		InterceptionAction HandleIntercept(ICallContext invocation, InterceptStrategyContext ctx);
		
	}

	internal class InterceptStrategyContext
	{
		public InterceptStrategyContext(Mock Mock
										, Type targetType
										, Dictionary<string, List<Delegate>> invocationLists
										, List<ICallContext> actualInvocations
										, MockBehavior behavior
										, List<IProxyCall> orderedCalls
			)
		{
			this.Behavior = behavior;
			this.Mock = Mock;
			this.InvocationLists = invocationLists;
			this.ActualInvocations = actualInvocations;
			this.TargetType = targetType;
			this.OrderedCalls = orderedCalls;
		}
		public Mock Mock {get;private set;}
		public Type TargetType { get; private set; }
		public Dictionary<string, List<Delegate>> InvocationLists { get; private set; }
		public List<ICallContext> ActualInvocations { get; private set; }
		public MockBehavior Behavior { get; private set; }
		public List<IProxyCall> OrderedCalls { get; private set; }
		public IProxyCall CurrentCall { get; set; }
	}

}
