// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Behaviors
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class ReturnComputedValue : Behavior
    After:
        sealed class ReturnComputedValue : Behavior
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class ReturnComputedValue : Behavior
    After:
        sealed class ReturnComputedValue : Behavior
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class ReturnComputedValue : Behavior
    After:
        sealed class ReturnComputedValue : Behavior
    */
    sealed class ReturnComputedValue : Behavior

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private readonly Func<IInvocation, object> valueFactory;
    After:
            readonly Func<IInvocation, object> valueFactory;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private readonly Func<IInvocation, object> valueFactory;
    After:
            readonly Func<IInvocation, object> valueFactory;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private readonly Func<IInvocation, object> valueFactory;
    After:
            readonly Func<IInvocation, object> valueFactory;
    */
    {
        readonly Func<IInvocation, object> valueFactory;

        public ReturnComputedValue(Func<IInvocation, object> valueFactory)
        {
            Debug.Assert(valueFactory != null);

            this.valueFactory = valueFactory;
        }

        public override void Execute(Invocation invocation)
        {
            invocation.ReturnValue = this.valueFactory.Invoke(invocation);
        }
    }
}
