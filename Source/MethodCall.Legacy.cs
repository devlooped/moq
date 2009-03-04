using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language;

namespace Moq
{
	partial class MethodCall
	{
		public IVerifies Raises(MockedEvent eventHandler, EventArgs args)
		{
			Guard.ArgumentNotNull(args, "args");

			return RaisesImpl(eventHandler, (Func<EventArgs>)(() => args));
		}

		public IVerifies Raises(MockedEvent eventHandler, Func<EventArgs> func)
		{
			return RaisesImpl(eventHandler, func);
		}

		public IVerifies Raises<T>(MockedEvent eventHandler, Func<T, EventArgs> func)
		{
			return RaisesImpl(eventHandler, func);
		}

		public IVerifies Raises<T1, T2>(MockedEvent eventHandler, Func<T1, T2, EventArgs> func)
		{
			return RaisesImpl(eventHandler, func);
		}

		public IVerifies Raises<T1, T2, T3>(MockedEvent eventHandler, Func<T1, T2, T3, EventArgs> func)
		{
			return RaisesImpl(eventHandler, func);
		}

		public IVerifies Raises<T1, T2, T3, T4>(MockedEvent eventHandler, Func<T1, T2, T3, T4, EventArgs> func)
		{
			return RaisesImpl(eventHandler, func);
		}

		private IVerifies RaisesImpl(MockedEvent eventHandler, Delegate func)
		{
			Guard.ArgumentNotNull(eventHandler, "eventHandler");
			Guard.ArgumentNotNull(func, "func");

			mockEvent = eventHandler;
			mockEventArgsFunc = func;

			return this;
		}
	}
}
