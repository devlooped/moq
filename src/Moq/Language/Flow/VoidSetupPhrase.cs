// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq.Language.Flow
{
	internal class VoidSetupPhrase<T> : SetupPhrase, ISetup<T> where T : class
	{
		public VoidSetupPhrase(MethodCall setup) : base(setup)
		{
		}

		public IVerifies Raises(Action<T> eventExpression, EventArgs args)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, new Func<EventArgs>(() => args));
			return this;
		}

		public IVerifies Raises(Action<T> eventExpression, Func<EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises(Action<T> eventExpression, params object[] args)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, args);
			return this;
		}

		public IVerifies Raises<T1>(Action<T> eventExpression, Func<T1, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2>(Action<T> eventExpression, Func<T1, T2, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3>(Action<T> eventExpression, Func<T1, T2, T3, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4>(Action<T> eventExpression, Func<T1, T2, T3, T4, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, EventArgs> func)
		{
			this.Setup.SetRaiseEventBehavior(eventExpression, func);
			return this;
		}
	}
}
