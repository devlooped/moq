//Copyright (c) 2007, Moq Team 
//http://code.google.com/p/moq/
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of the <ORGANIZATION> nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

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

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Core.Interceptor;
using Moq.Language;
using Moq.Language.Flow;

namespace Moq
{
	/// <devdoc>
	/// We need this non-generics base class so that 
	/// we can use <see cref="HasReturnValue"/> from 
	/// generic code.
	/// </devdoc>
	internal class MethodCallReturn : MethodCall
	{
		public MethodCallReturn(Expression originalExpression, MethodInfo method, params Expression[] arguments)
			: base(originalExpression, method, arguments)
		{
		}

		public bool HasReturnValue { get; protected set; }
	}

	internal class MethodCallReturn<TResult> : MethodCallReturn, IProxyCall, IExpect<TResult>, IExpectGetter<TResult>, IReturnsResult
	{
		Delegate valueDel = (Func<TResult>)(() => (TResult)new DefaultValue(typeof(TResult)).Value);
		Action<object[]> afterReturnCallback;

		public MethodCallReturn(Expression originalExpression, MethodInfo method, params Expression[] arguments)
			: base(originalExpression, method, arguments)
		{
			HasReturnValue = false;
		}

		public IReturnsResult Returns(Func<TResult> valueExpression)
		{
			SetReturnDelegate(valueExpression);
			return this;
		}

		public IReturnsResult Returns(TResult value)
		{
			Returns(() => value);
			return this;
		}

		public IReturnsResult Returns<T>(Func<T, TResult> valueExpression)
		{
			SetReturnDelegate(valueExpression);
			return this;
		}

		public IReturnsResult Returns<T1, T2>(Func<T1, T2, TResult> valueExpression)
		{
			SetReturnDelegate(valueExpression);
			return this;
		}

		public IReturnsResult Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueExpression)
		{
			SetReturnDelegate(valueExpression);
			return this;
		}

		public IReturnsResult Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> valueExpression)
		{
			SetReturnDelegate(valueExpression);
			return this;
		}

		IReturnsThrowsGetter<TResult> ICallbackGetter<TResult>.Callback(Action callback)
		{
			base.Callback(callback);
			return this;
		}

		public new IReturnsThrows<TResult> Callback(Action callback)
		{
			base.Callback(callback);
			return this;
		}

		public new IReturnsThrows<TResult> Callback<T>(Action<T> callback)
		{
			base.Callback(callback);
			return this;
		}

		public new IReturnsThrows<TResult> Callback<T1, T2>(Action<T1, T2> callback)
		{
			base.Callback(callback);
			return this;
		}

		public new IReturnsThrows<TResult> Callback<T1, T2, T3>(Action<T1, T2, T3> callback)
		{
			base.Callback(callback);
			return this;
		}

		public new IReturnsThrows<TResult> Callback<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callback)
		{
			base.Callback(callback);
			return this;
		}

		private void SetReturnDelegate(Delegate valueDel)
		{
			if (valueDel == null)
				this.valueDel = (Func<TResult>)(() => default(TResult));
			else
				this.valueDel = valueDel;

			HasReturnValue = true;
		}

		protected override void SetCallbackWithoutArguments(Action callback)
		{
			if (HasReturnValue)
				this.afterReturnCallback = delegate { callback(); };
			else
				base.SetCallbackWithoutArguments(callback);
		}

		protected override void SetCallbackWithArguments(Delegate callback)
		{
			if (HasReturnValue)
				this.afterReturnCallback = delegate(object[] args) { callback.InvokePreserveStack(args); };
			else
				base.SetCallbackWithArguments(callback);
		}

		public override void Execute(IInvocation call)
		{
			base.Execute(call);

			if (valueDel.Method.GetParameters().Length != 0)
				call.ReturnValue = valueDel.InvokePreserveStack(call.Arguments); //will throw if parameters mismatch
			else
				call.ReturnValue = valueDel.InvokePreserveStack(); //we need this, for the user to be able to use parameterless methods
		}
	}
}
