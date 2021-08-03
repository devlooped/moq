// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Language.Flow
{
	internal abstract class SetupPhrase : ICallbackResult, IVerifies, IThrowsResult
	{
		private MethodCall setup;

		protected SetupPhrase(MethodCall setup)
		{
			Debug.Assert(setup != null);

			this.setup = setup;
		}

		public MethodCall Setup => this.setup;

		public IVerifies AtMost(int callCount)
		{
			this.setup.AtMost(callCount);
			return this;
		}

		public IVerifies AtMostOnce() => this.AtMost(1);

		public ICallbackResult Callback(InvocationAction action)
		{
			this.setup.SetCallbackBehavior(action.Action);
			return this;
		}

		public ICallbackResult Callback(Delegate callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback(Action callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T>(Action<T> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2>(Action<T1, T2> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3>(Action<T1, T2, T3> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> callback)
		{
			this.setup.SetCallbackBehavior(callback);
			return this;
		}

		public ICallBaseResult CallBase()
		{
			this.setup.SetCallBaseBehavior();
			return this;
		}

		public IThrowsResult Throws(Exception exception)
		{
			this.setup.SetThrowExceptionBehavior(exception);
			return this;
		}

		public IThrowsResult Throws<TException>() where TException : Exception, new()
		{
			this.setup.SetThrowExceptionBehavior(new TException());
			return this;
		}

		public IThrowsResult Throws(Delegate exceptionFunction)
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<TException>(Func<TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}
		
		public IThrowsResult Throws<T, TException>(Func<T, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, TException>(Func<T1, T2, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, TException>(Func<T1, T2, T3, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, T4, TException>(Func<T1, T2, T3, T4, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, T4, T5, TException>(Func<T1, T2, T3, T4, T5, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, T4, T5, T6, TException>(Func<T1, T2, T3, T4, T5, T6, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, TException>(Func<T1, T2, T3, T4, T5, T6, T7, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public IThrowsResult Throws<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TException>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TException> exceptionFunction) where TException : Exception
		{
			this.Setup.SetThrowComputedExceptionBehavior(exceptionFunction);
			return this;
		}

		public void Verifiable()
		{
			this.setup.MarkAsVerifiable();
		}

		public void Verifiable(string failMessage)
		{
			this.setup.MarkAsVerifiable();
			this.setup.SetFailMessage(failMessage);
		}

		public override string ToString()
		{
			return setup.Expression.ToStringFixed();
		}
	}
}
