using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Castle.Core.Interceptor;
using Moq.Language.Flow;
using Moq.Language;

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

	internal class MethodCallReturn<TResult> : MethodCallReturn, IProxyCall, IExpect<TResult>, IExpectGetter<TResult>
	{
		Delegate valueDel = (Func<TResult>)(() => (TResult)new DefaultValue(typeof(TResult)).Value);

		public MethodCallReturn(Expression originalExpression, MethodInfo method, params Expression[] arguments)
			: base(originalExpression, method, arguments)
		{
			HasReturnValue = false;
		}

		public IOnceVerifies Returns(Func<TResult> valueExpression)
		{
			SetReturnDelegate(valueExpression);
			return this;
		}

		public IOnceVerifies Returns(TResult value)
		{
			Returns(() => value);
			return this;
		}

		public IOnceVerifies Returns<T>(Func<T, TResult> valueExpression)
		{
			SetReturnDelegate(valueExpression);
			return this;
		}

		public IOnceVerifies Returns<T1, T2>(Func<T1, T2, TResult> valueExpression)
		{
			SetReturnDelegate(valueExpression);
			return this;
		}

		public IOnceVerifies Returns<T1, T2, T3>(Func<T1, T2, T3, TResult> valueExpression)
		{
			SetReturnDelegate(valueExpression);
			return this;
		}

		public IOnceVerifies Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> valueExpression)
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

		public override void Execute(IInvocation call)
		{
			base.Execute(call);

			if (valueDel.Method.GetParameters().Length != 0)
				call.ReturnValue = valueDel.DynamicInvoke(call.Arguments); //will throw if parameters mismatch
			else
				call.ReturnValue = valueDel.DynamicInvoke(); //we need this, for the user to be able to use parameterless methods
		}
	}
}
