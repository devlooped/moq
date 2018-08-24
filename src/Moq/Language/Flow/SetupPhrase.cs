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
using System.Diagnostics;

namespace Moq.Language.Flow
{
	internal abstract class SetupPhrase<TSetup> : ICallbackResult, IVerifies, IThrowsResult where TSetup : MethodCall
	{
		private TSetup setup;

		protected SetupPhrase(TSetup setup)
		{
			Debug.Assert(setup != null);

			this.setup = setup;
		}

		public TSetup Setup => this.setup;

		public IVerifies AtMost(int callCount)
		{
			this.setup.AtMost(callCount);
			return this;
		}

		public IVerifies AtMostOnce() => this.AtMost(1);

		public ICallbackResult Callback(Delegate callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback(Action callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T>(Action<T> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2>(Action<T1, T2> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3>(Action<T1, T2, T3> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public ICallbackResult Callback<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> callback)
		{
			this.setup.Callback(callback);
			return this;
		}

		public IThrowsResult Throws(Exception exception)
		{
			this.setup.Throws(exception);
			return this;
		}

		public IThrowsResult Throws<TException>() where TException : Exception, new()
		{
			this.setup.Throws<TException>();
			return this;
		}

		public void Verifiable()
		{
			this.setup.Verifiable();
		}

		public void Verifiable(string failMessage)
		{
			this.setup.Verifiable(failMessage);
		}
	}
}
