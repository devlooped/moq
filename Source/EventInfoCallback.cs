using System;
using System.Collections.Generic;
using System.Reflection;

namespace Moq
{
	internal class EventInfoCallback : IDisposable
	{
		List<Interceptor> interceptors = new List<Interceptor>();

		public void AddInterceptor(Interceptor interceptor)
		{
			if (!interceptors.Contains(interceptor))
				interceptors.Add(interceptor);

			interceptor.EventCallback = this;
		}

		public EventInfo EventInfo { get; private set; }
		public Mock Mock { get; private set; }

		public void SetEvent(Mock mock, EventInfo info)
		{
			this.Mock = mock;
			this.EventInfo = info;
		}

		public void Dispose()
		{
			foreach (var interceptor in interceptors)
			{
				interceptor.EventCallback = null;
			}
		}
	}
}
