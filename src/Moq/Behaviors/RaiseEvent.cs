// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Moq.Behaviors
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class RaiseEvent : Behavior
    After:
        sealed class RaiseEvent : Behavior
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class RaiseEvent : Behavior
    After:
        sealed class RaiseEvent : Behavior
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class RaiseEvent : Behavior
    After:
        sealed class RaiseEvent : Behavior
    */
    sealed class RaiseEvent : Behavior

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private Mock mock;
            private LambdaExpression expression;
            private Delegate eventArgsFunc;
            private object[] eventArgsParams;
    After:
            Mock mock;
            LambdaExpression expression;
            Delegate eventArgsFunc;
            object[] eventArgsParams;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private Mock mock;
            private LambdaExpression expression;
            private Delegate eventArgsFunc;
            private object[] eventArgsParams;
    After:
            Mock mock;
            LambdaExpression expression;
            Delegate eventArgsFunc;
            object[] eventArgsParams;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private Mock mock;
            private LambdaExpression expression;
            private Delegate eventArgsFunc;
            private object[] eventArgsParams;
    After:
            Mock mock;
            LambdaExpression expression;
            Delegate eventArgsFunc;
            object[] eventArgsParams;
    */
    {
        Mock mock;
        LambdaExpression expression;
        Delegate eventArgsFunc;
        object[] eventArgsParams;

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

        public override void Execute(Invocation invocation)
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
