using System.Reflection;
using Moq;

namespace Moq.Proxy.Factory
{
	internal class MethodCall : IMethodCall
	{
		internal MethodCall(MethodInfo method, object instance, object[] arguments)
		{
			Guard.NotNull(() => method, method);
			Guard.NotNull(() => instance, instance);
			Guard.NotNull(() => arguments, arguments);

			this.Method = method;
			this.Instance = instance;
			this.InArgs = arguments;
		}

		public object[] InArgs { get; private set; }

		public object Instance { get; private set; }

		public MethodInfo Method { get; private set; }
	}
}