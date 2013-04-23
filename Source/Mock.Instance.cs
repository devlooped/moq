using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Moq.Proxy;

namespace Moq
{
	internal class InstanceMock<T> : Mock<T>
		where T : class
	{
		private static IProxyFactory proxyFactory = new CastleProxyFactory();
		private readonly T instance;
		private T mockInstance;

		public InstanceMock(T instance)
		{
			this.instance = instance;
			this.CallBase = true;
		}

		protected override object OnGetObject()
		{
			if (this.mockInstance == null)
			{
				PexProtector.Invoke(() =>
					{
						this.mockInstance = proxyFactory.CreateProxy<T>(this.Interceptor,
																		this.ImplementedInterfaces.ToArray(),
																		this.instance);
					});
			}

			return mockInstance;
		}
	}
}
