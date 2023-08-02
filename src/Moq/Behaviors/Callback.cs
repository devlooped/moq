// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Behaviors
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class Callback : Behavior
    After:
        sealed class Callback : Behavior
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class Callback : Behavior
    After:
        sealed class Callback : Behavior
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class Callback : Behavior
    After:
        sealed class Callback : Behavior
    */
    sealed class Callback : Behavior

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private readonly Action<IInvocation> callback;
    After:
            readonly Action<IInvocation> callback;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private readonly Action<IInvocation> callback;
    After:
            readonly Action<IInvocation> callback;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private readonly Action<IInvocation> callback;
    After:
            readonly Action<IInvocation> callback;
    */
    {
        readonly Action<IInvocation> callback;

        public Callback(Action<IInvocation> callback)
        {
            Debug.Assert(callback != null);

            this.callback = callback;
        }

        public override void Execute(Invocation invocation)
        {
            this.callback.Invoke(invocation);
        }
    }
}
