using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Castle.Core.Interceptor;

namespace Moq
{
	internal class MethodCallReturn<TResult> : MethodCall, ICallReturn<TResult>, IProxyCall
	{
		//TResult value;
		//Func<TResult> valueFunc;

        Delegate valueDel;

		public MethodCallReturn(Expression originalExpression, MethodInfo method, params Expression[] arguments)
			: base(originalExpression, method, arguments)
		{
		}

        public ICallReturn<TResult> Returns(Func<TResult> valueExpression)
		{
            SetReturnDelegate(valueExpression);
			//this.valueFunc = valueExpression;
            return this;
		}

        public ICallReturn<TResult> Returns(TResult value)
		{
            Returns(() => value);
            return this;
		}

		public new ICallReturn<TResult> Callback(Action callback)
		{
			base.Callback(callback);
			return this;
		}

		public new ICallReturn<TResult> Verifiable()
		{
			IsVerifiable = true;

			return this;
		}

        private void SetReturnDelegate(Delegate valueDel)
        {
            this.valueDel = valueDel;
        }

		public override void Execute(IInvocation call)
		{
			base.Execute(call);
            
            if (valueDel.Method.GetParameters().Length != 0)
                call.ReturnValue = valueDel.DynamicInvoke(call.Arguments); //will throw if parameters mismatch
            else
                call.ReturnValue = valueDel.DynamicInvoke(); //so need this, for the user being able to still use parameterless way
		}
	}
}
