using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;

namespace Moq
{
	internal class MockProxy<TInterface> : RealProxy
		where TInterface : class
	{
		List<IProxyCall> calls = new List<IProxyCall>();
		TInterface transparentProxy = null;
		static MethodInfo ObjectGetTypeMethod = Reflector<object>.GetMethod(x => x.GetType());
		static MethodInfo ObjectEqualsMethod = Reflector<object>.GetMethod(x => x.Equals(null));
		static MethodInfo ObjectGetHashCodeMethod = Reflector<object>.GetMethod(x => x.GetHashCode());
		static MethodInfo ObjectToStringMethod = Reflector<object>.GetMethod(x => x.ToString());

		public MockProxy()
			: base(typeof(TInterface))
		{
		}

		public void AddCall(IProxyCall call)
		{
			calls.Add(call);
		}

		public TInterface TransparentProxy
		{
			get
			{
				if (transparentProxy == null)
					transparentProxy = (TInterface)this.GetTransparentProxy();

				return transparentProxy;
			}
		}

		public override IMessage Invoke(IMessage msg)
		{
			IMethodCallMessage methodCall = msg as IMethodCallMessage;
			if (methodCall != null)
			{
				var call = calls.Find(x => x.Matches(methodCall));
				if (call != null)
				{
					return call.Execute(methodCall);
				}

				if (methodCall.MethodBase.DeclaringType == typeof(object))
				{
					return ExecuteObjectMethod(methodCall);
				}

				MethodInfo method = methodCall.MethodBase as MethodInfo;
				if (method != null && method.ReturnType != null && 
					method.ReturnType != typeof(void))
				{
					List<string> values = new List<string>(methodCall.ArgCount);
					// Build arguments
					methodCall.Args.ForEach(
						x => values.Add(x == null ? "null" : (x is string ? "\"" + (string)x + "\"" : x.ToString())));

					throw new InvalidOperationException(String.Format(
						Properties.Resources.UndeterminedReturnValue,
						methodCall.MethodBase.DeclaringType.Name,
						methodCall.MethodName,
						String.Join(", ", values.ToArray())));
				}
			}

			return new ReturnMessage(null, null, 0, null, methodCall);
		}

		private IMessage ExecuteObjectMethod(IMethodCallMessage methodCall)
		{
			if (methodCall.MethodBase == ObjectGetTypeMethod)
			{
				Type type = typeof(TInterface);
				return new ReturnMessage(type, null, 0, null, methodCall);
			}
			else if (methodCall.MethodBase == ObjectEqualsMethod)
			{
				bool equals = object.ReferenceEquals(transparentProxy, methodCall.Args[0]);
				return new ReturnMessage(equals, null, 0, null, methodCall);
			}
			else if (methodCall.MethodBase == ObjectGetHashCodeMethod)
			{
				int hashCode = GetHashCode();
				return new ReturnMessage(hashCode, null, 0, null, methodCall);
			}
			else if (methodCall.MethodBase == ObjectToStringMethod)
			{
				string toString = ToString();
				return new ReturnMessage(toString, null, 0, null, methodCall);
			}
			else
			{
				return new ReturnMessage(null, null, 0, null, methodCall);
			}
		}
	}
}
