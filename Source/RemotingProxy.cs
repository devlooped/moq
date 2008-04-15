using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Castle.Core.Interceptor;

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
		/// <summary>
		/// Type of the generated remoting proxy.
		/// </summary>
		Type targetType;
		/// <summary>
		/// Type of the original mock.
		/// </summary>
		Type realType;
		object instance;

		public RemotingProxy(Type targetType, Type realType, Action<IInvocation> interceptor, object[] ctorArgs)
			: base(targetType)
		{
			this.targetType = targetType;
			this.realType = realType;
			this.interceptor = interceptor;

			// During construction, the object is not proxied, and calls
			// made within the MBRO ctor are done directly on the "this" 
			// object, not via this proxy. Interception and therefore 
			// expectations don't work during construction.
			this.instance = Activator.CreateInstance(targetType,
				new object[] { new IInterceptor[] { new NullInterceptor() } }.Concat(ctorArgs).ToArray());

			base.AttachServer((MarshalByRefObject)instance);
		}

		public override IMessage Invoke(IMessage msg)
		{
			var methodCall = msg as IMethodCallMessage;

			if (methodCall != null)
			{
				if (methodCall.MethodName == "FieldGetter" &&
					methodCall.MethodBase.DeclaringType == typeof(object))
				{
					// For consistency with non-MBROs, fields 
					// are not mockeable and are passed-through to 
					// the underlying object.
					object value = realType.GetField(
						(string)methodCall.Args[1],
						BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
						.GetValue(GetUnwrappedServer());

					return new ReturnMessage(
						value,
						null,
						0,
						null,
						methodCall);
				}
				else
				{
					var invocation = new RemotingInvocation(
						realType,
						methodCall,
						CallUnderlyingObject);

					interceptor(invocation);

					return ToMessage(invocation, methodCall);
				}
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

		/// <summary>
		/// We use a null interceptor for the generated class as we'll be already intercepting 
		/// through remoting, which is more powerful and gives us additional interception 
		/// points (private members, non-virtuals, etc.).
		/// </summary>
		class NullInterceptor : IInterceptor
		{
			public void Intercept(IInvocation invocation)
			{
			}
		}
	}
}
