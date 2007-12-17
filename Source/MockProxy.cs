using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Reflection;

namespace Moq
{
	internal class MockProxy<TInterface> : RealProxy
	{
		List<IProxyCall> calls = new List<IProxyCall>();

		public MockProxy()
			: base(typeof(TInterface))
		{
		}

		public void AddCall(IProxyCall call)
		{
			calls.Add(call);
		}

		public TInterface Value
		{
			get
			{
				return (TInterface)this.GetTransparentProxy();
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
	}
}
