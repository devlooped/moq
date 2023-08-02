// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class ReturnValue : Behavior
    After:
        sealed class ReturnValue : Behavior
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class ReturnValue : Behavior
    After:
        sealed class ReturnValue : Behavior
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class ReturnValue : Behavior
    After:
        sealed class ReturnValue : Behavior
    */
    sealed class ReturnValue : Behavior

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private readonly object value;
    After:
            readonly object value;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private readonly object value;
    After:
            readonly object value;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private readonly object value;
    After:
            readonly object value;
    */
    {
        readonly object value;

        public ReturnValue(object value)
        {
            this.value = value;
        }

        public object Value => this.value;

        public override void Execute(Invocation invocation)
        {
            invocation.ReturnValue = this.value;
        }
    }
}
