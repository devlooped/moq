// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Behaviors
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class ThrowException : Behavior
    After:
        sealed class ThrowException : Behavior
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class ThrowException : Behavior
    After:
        sealed class ThrowException : Behavior
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class ThrowException : Behavior
    After:
        sealed class ThrowException : Behavior
    */
    sealed class ThrowException : Behavior

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private readonly Exception exception;
    After:
            readonly Exception exception;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private readonly Exception exception;
    After:
            readonly Exception exception;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private readonly Exception exception;
    After:
            readonly Exception exception;
    */
    {
        readonly Exception exception;

        public ThrowException(Exception exception)
        {
            Debug.Assert(exception != null);

            this.exception = exception;
        }

        public override void Execute(Invocation invocation)
        {
            throw this.exception;
        }
    }
}
