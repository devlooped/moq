// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq.Language.Flow
{
	internal class NonVoidSetupPhrase<T, TResult> : SetupPhrase, ISetup<T, TResult>, ISetupGetter<T, TResult>, IReturnsResult<T> where T : class
	{
		public NonVoidSetupPhrase(MethodCall setup) : base(setup)
		{
		}

		public new IReturnsThrows<T, TResult> Callback(InvocationAction action)
		{
			this.Setup.SetCallbackBehavior(action.Action);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback(Delegate callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		IReturnsThrowsGetter<T, TResult> ICallbackGetter<T, TResult>.Callback(Action callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback(Action callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1>(Action<T1> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2>(Action<T1, T2> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3>(Action<T1, T2, T3> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> callback)
		{
			this.Setup.SetCallbackBehavior(callback);
			return this;
		}

		public new IReturnsResult<T> CallBase()
		{
			this.Setup.SetCallBaseBehavior();
			return this;
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

		public IReturnsResult<T> Returns(TResult value)
		{
			this.Setup.SetReturnValueBehavior(value);
			return this;
		}

		public IReturnsResult<T> Returns(InvocationFunc valueFunction)
		{
			this.Setup.SetReturnComputedValueBehavior(valueFunction.Func);
			return this;
		}

		public IReturnsResult<T> Returns(Delegate valueFunction)
		{
			this.Setup.SetReturnComputedValueBehavior(valueFunction);
			return this;
		}

		public IReturnsResult<T> Returns(Func<TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1>(Func<T1, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2>(Func<T1, T2, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> valueExpression)
		{
			this.Setup.SetReturnComputedValueBehavior(valueExpression);
			return this;
		}
	}
}
