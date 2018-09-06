//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
//All rights reserved.
//
//Redistribution and use in source and binary forms,
//with or without modification, are permitted provided
//that the following conditions are met:
//
//    * Redistributions of source code must retain the
//    above copyright notice, this list of conditions and
//    the following disclaimer.
//
//    * Redistributions in binary form must reproduce
//    the above copyright notice, this list of conditions
//    and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the
//    names of its contributors may be used to endorse
//    or promote products derived from this software
//    without specific prior written permission.
//
//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
//SUCH DAMAGE.
//
//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System;

namespace Moq.Language.Flow
{
	internal class NonVoidSetupPhrase<T, TResult> : SetupPhrase<MethodCallReturn>, ISetup<T, TResult>, ISetupGetter<T, TResult>, IReturnsResult<T>, ICallbackAfterResult where T : class
	{
		public NonVoidSetupPhrase(MethodCallReturn setup) : base(setup)
		{
		}

		ICallbackAfterResult ICallbackAfter.Callback(Delegate callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback(Delegate callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback(Action callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		IReturnsThrowsGetter<T, TResult> ICallbackGetter<T, TResult>.Callback(Action callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback(Action callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1>(Action<T1> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1>(Action<T1> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2>(Action<T1, T2> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2>(Action<T1, T2> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3>(Action<T1, T2, T3> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3>(Action<T1, T2, T3> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9,  T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		ICallbackAfterResult ICallbackAfter.Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsThrows<T, TResult> Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> callback)
		{
			this.Setup.SetCallbackResponse(callback);
			return this;
		}

		public new IReturnsResult<T> CallBase()
		{
			this.Setup.SetCallBaseResponse();
			return this;
		}

		public IVerifies Raises(Action<T> eventExpression, EventArgs args)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, new Func<EventArgs>(() => args));
			return this;
		}

		public IVerifies Raises(Action<T> eventExpression, Func<EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises(Action<T> eventExpression, params object[] args)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, args);
			return this;
		}

		public IVerifies Raises<T1>(Action<T> eventExpression, Func<T1, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2>(Action<T> eventExpression, Func<T1, T2, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3>(Action<T> eventExpression, Func<T1, T2, T3, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4>(Action<T> eventExpression, Func<T1, T2, T3, T4, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IVerifies Raises<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T> eventExpression, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, EventArgs> func)
		{
			this.Setup.SetRaiseEventResponse(eventExpression, func);
			return this;
		}

		public IReturnsResult<T> Returns(TResult value)
		{
			this.Setup.SetReturnsResponse(new Func<TResult>(() => value));
			return this;
		}

		public IReturnsResult<T> Returns(Delegate valueFunction)
		{
			this.Setup.SetReturnsResponse(valueFunction);
			return this;
		}

		public IReturnsResult<T> Returns(Func<TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1>(Func<T1, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2>(Func<T1, T2, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}

		public IReturnsResult<T> Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> valueExpression)
		{
			this.Setup.SetReturnsResponse(valueExpression);
			return this;
		}
	}
}
