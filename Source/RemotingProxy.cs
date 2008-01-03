using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Castle.Core.Interceptor;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Security.Permissions;

namespace Moq
{
	internal class RemotingProxy : RealProxy
	{
		Action<IInvocation> interceptor;
		Type targetType;

		public RemotingProxy(Type targetType, Action<IInvocation> interceptor)
			: base(targetType)
		{
			this.targetType = targetType;
			this.interceptor = interceptor;
		}
		
		public override IMessage Invoke(IMessage msg)
		{
			var methodCall = msg as IMethodCallMessage;
			if (methodCall != null)
			{
				var invocation = new RemotingInvocation(
					targetType,
					methodCall,
					CallUnderlyingObject);
				
				interceptor(invocation);

				return ToMessage(invocation, methodCall);
			}

			return null;
		}

		private IMethodReturnMessage CallUnderlyingObject(IMethodCallMessage methodCall)
		{
			var realObject = GetUnwrappedServer();
			if (realObject == null)
			{
				var returnMessage = InitializeServerObject(null);
				if (returnMessage.Exception == null)
				{
					realObject = GetUnwrappedServer();
					SetStubData(this, realObject);
				}
				else
				{
					throw returnMessage.Exception;
				}
			}

			return RemotingServices.ExecuteMessage(realObject, methodCall);
		}

		private IMessage ToMessage(IInvocation invocation, IMethodCallMessage original)
		{
			if (invocation.Method.ReturnType != null &&
				invocation.Method.ReturnType != typeof(void))
			{
				return new ReturnMessage(invocation.ReturnValue, null, 0, original.LogicalCallContext, original);
			}
			else
			{
				return new ReturnMessage(null, null, 0, original.LogicalCallContext, original);
			}
		}

		class RemotingInvocation : IInvocation
		{
			Type targetType;
			IMethodCallMessage call;
			Func<IMethodCallMessage, IMethodReturnMessage> realCall;

			public RemotingInvocation(Type targetType, IMethodCallMessage call, 
				Func<IMethodCallMessage, IMethodReturnMessage> realCall)
			{
				this.targetType = targetType;
				this.call = call;
				this.realCall = realCall;
			}

			public object[] Arguments
			{
				get { return call.Args; }
			}

			public MethodInfo Method
			{
				get { return call.MethodBase as MethodInfo; }
			}

			public object ReturnValue { get; set; }

			public Type TargetType
			{
				get { return targetType; }
			}

			public void Proceed()
			{
				ReturnValue = realCall(call).ReturnValue;
			}

			public Type[] GenericArguments
			{
				get { throw new NotImplementedException(); }
			}

			public object GetArgumentValue(int index)
			{
				throw new NotImplementedException();
			}

			public MethodInfo GetConcreteMethod()
			{
				throw new NotImplementedException();
			}

			public MethodInfo GetConcreteMethodInvocationTarget()
			{
				throw new NotImplementedException();
			}

			public object InvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			public MethodInfo MethodInvocationTarget
			{
				get { throw new NotImplementedException(); }
			}

			public object Proxy
			{
				get { throw new NotImplementedException(); }
			}

			public void SetArgumentValue(int index, object value)
			{
				throw new NotImplementedException();
			}
		}







		private IMessage ExecuteObjectMethod(IMethodCallMessage methodCall)
		{
			throw new NotImplementedException();
			//if (methodCall.MethodBase == ObjectGetTypeMethod)
			//{
			//    Type type = typeof(TTarget);
			//    return new ReturnMessage(type, null, 0, null, methodCall);
			//}
			//else if (methodCall.MethodBase == ObjectEqualsMethod)
			//{
			//    bool equals = object.ReferenceEquals(transparentProxy, methodCall.Args[0]);
			//    return new ReturnMessage(equals, null, 0, null, methodCall);
			//}
			//else if (methodCall.MethodBase == ObjectGetHashCodeMethod)
			//{
			//    int hashCode = GetHashCode();
			//    return new ReturnMessage(hashCode, null, 0, null, methodCall);
			//}
			//else if (methodCall.MethodBase == ObjectToStringMethod)
			//{
			//    string toString = ToString();
			//    return new ReturnMessage(toString, null, 0, null, methodCall);
			//}
			//else
			//{
			//    return new ReturnMessage(null, null, 0, null, methodCall);
			//}
		}
	}
}
