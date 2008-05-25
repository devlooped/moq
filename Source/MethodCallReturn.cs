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
