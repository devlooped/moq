using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Castle.Core.Interceptor;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Security.Permissions;

namespace Moq
{
	/// <summary>
	/// Adapter for Remoting interception. 
	/// It allows invocation of the underlying proxied object 
	/// if a given member is not abstract on the target type.
	/// </summary>
	internal class RemotingProxy : RealProxy
	{
		Action<IInvocation> interceptor;
		Type targetType;

		public RemotingProxy(Type targetType, Action<IInvocation> interceptor)
			: this(targetType, interceptor, new object[0])
		{
		}

		public RemotingProxy(Type targetType, Action<IInvocation> interceptor, object[] ctorArgs)
			: base(targetType)
		{
			this.targetType = targetType;
			this.interceptor = interceptor;

			// During construction, the object is not proxied, and calls
			// made within the MBRO ctor are done directly on the "this" 
			// object, not via this proxy. Interception and therefore 
			// expectations don't work during construction.
			var instance = Activator.CreateInstance(targetType, ctorArgs);
			base.AttachServer((MarshalByRefObject)instance);
		}

		public override object GetTransparentProxy()
		{
			return base.GetTransparentProxy();
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

			return RemotingServices.ExecuteMessage(realObject, methodCall);
		}

		private IMessage ToMessage(IInvocation invocation, IMethodCallMessage original)
		{
			// TODO: we do not support byref and out arguments yet.
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

		//class CtorMethodCall : IConstructionCallMessage
		//{
		//    object[] ctorArgs;
		//    System.Collections.Hashtable properties = new System.Collections.Hashtable();
		//    Type proxyType;
		//    MethodBase ctorMethod;
		//    LogicalCallContext context;

		//    public CtorMethodCall(object[] ctorArgs, Type proxyType, LogicalCallContext context)
		//    {
		//        this.ctorArgs = ctorArgs;
		//        this.proxyType = proxyType;
		//        // Same logic applied internally on Activator.CreateInstance.
		//        Type[] argTypes = (from arg in ctorArgs
		//                           select arg == null ? (Type)null : arg.GetType()).ToArray();

		//        ctorMethod = proxyType.GetConstructor(argTypes);
		//        this.context = context;

		//        //properties.Add("__MethodName", ConstructorInfo.ConstructorName);
		//        //properties.Add("__ActivationTypeName", proxyType.AssemblyQualifiedName);
		//        //properties.Add("__Args", ctorArgs);
		//    }

		//    public object GetInArg(int argNum)
		//    {
		//        return ctorArgs[argNum];
		//    }

		//    public string GetInArgName(int index)
		//    {
		//        throw new NotImplementedException();
		//    }

		//    public int InArgCount
		//    {
		//        get { return ctorArgs.Length; }
		//    }

		//    public object[] InArgs
		//    {
		//        get { return ctorArgs; }
		//    }

		//    public int ArgCount
		//    {
		//        get { return ctorArgs.Length; }
		//    }

		//    public object[] Args
		//    {
		//        get { return ctorArgs; }
		//    }

		//    public object GetArg(int argNum)
		//    {
		//        return ctorArgs[argNum];
		//    }

		//    public string GetArgName(int index)
		//    {
		//        throw new NotImplementedException();
		//    }

		//    public bool HasVarArgs
		//    {
		//        get { return false; }
		//    }

		//    public LogicalCallContext LogicalCallContext
		//    {
		//        get { return context; }
		//    }

		//    public MethodBase MethodBase
		//    {
		//        get { return ctorMethod; }
		//    }

		//    public string MethodName
		//    {
		//        get { return ctorMethod.Name; }
		//    }

		//    public object MethodSignature
		//    {
		//        get { throw new NotImplementedException(); }
		//    }

		//    public string TypeName
		//    {
		//        get { return proxyType.AssemblyQualifiedName; }
		//    }

		//    public string Uri
		//    {
		//        get { return String.Empty; }
		//    }

		//    public System.Collections.IDictionary Properties
		//    {
		//        get { return new System.Collections.Hashtable(); }
		//    }

		//    public Type ActivationType
		//    {
		//        get { return proxyType; }
		//    }

		//    public string ActivationTypeName
		//    {
		//        get { throw new NotImplementedException(); }
		//    }

		//    public IActivator Activator
		//    {
		//        get
		//        {
		//            throw new NotImplementedException();
		//        }
		//        set
		//        {
		//            throw new NotImplementedException();
		//        }
		//    }

		//    public object[] CallSiteActivationAttributes
		//    {
		//        get { throw new NotImplementedException(); }
		//    }

		//    public System.Collections.IList ContextProperties
		//    {
		//        get { throw new NotImplementedException(); }
		//    }
		//}
	}
}
