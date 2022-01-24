// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Moq.Behaviors
{
	internal sealed class RaiseEvent : Behavior
	{
		private Mock mock;
		private LambdaExpression expression;
		private Delegate eventArgsFunc;
		private object[] eventArgsParams;

		public RaiseEvent(Mock mock, LambdaExpression expression, Delegate eventArgsFunc, object[] eventArgsParams)
		{
			Debug.Assert(mock != null);
			Debug.Assert(expression != null);
			Debug.Assert(eventArgsFunc != null ^ eventArgsParams != null);

			this.mock = mock;
			this.expression = expression;
			this.eventArgsFunc = eventArgsFunc;
			this.eventArgsParams = eventArgsParams;
		}

		public override void Execute(IInvocation invocation)
		{
			object[] args;

			if (this.eventArgsParams != null)
			{
				args = this.eventArgsParams;
			}
			else
			{
				var argsFuncType = this.eventArgsFunc.GetType();
				if (argsFuncType.IsGenericType && argsFuncType.GetGenericArguments().Length == 1)
				{
					args = new object[] { this.mock.Object, this.eventArgsFunc.InvokePreserveStack() };
				}
				else
				{
					args = new object[] { this.mock.Object, this.eventArgsFunc.InvokePreserveStack(invocation.Arguments) };
				}
			}

			Mock.RaiseEvent(this.mock, this.expression, this.expression.Split(), args);
		}
	}
}
