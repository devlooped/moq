// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Behaviors
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class ThrowComputedException : Behavior
    After:
        sealed class ThrowComputedException : Behavior
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class ThrowComputedException : Behavior
    After:
        sealed class ThrowComputedException : Behavior
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class ThrowComputedException : Behavior
    After:
        sealed class ThrowComputedException : Behavior
    */
    sealed class ThrowComputedException : Behavior

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private readonly Func<IInvocation, Exception> exceptionFactory;
    After:
            readonly Func<IInvocation, Exception> exceptionFactory;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private readonly Func<IInvocation, Exception> exceptionFactory;
    After:
            readonly Func<IInvocation, Exception> exceptionFactory;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private readonly Func<IInvocation, Exception> exceptionFactory;
    After:
            readonly Func<IInvocation, Exception> exceptionFactory;
    */
    {
        readonly Func<IInvocation, Exception> exceptionFactory;

        public ThrowComputedException(Func<IInvocation, Exception> exceptionFactory)
        {
            Debug.Assert(exceptionFactory != null);

            this.exceptionFactory = exceptionFactory;
        }

        public override void Execute(Invocation invocation)
        {
            throw this.exceptionFactory.Invoke(invocation);
        }
    }
}
