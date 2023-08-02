// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class ReturnBase : Behavior
    After:
        sealed class ReturnBase : Behavior
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class ReturnBase : Behavior
    After:
        sealed class ReturnBase : Behavior
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class ReturnBase : Behavior
    After:
        sealed class ReturnBase : Behavior
    */
    sealed class ReturnBase : Behavior
    {
        public static readonly ReturnBase Instance = new ReturnBase();


        /* Unmerged change from project 'Moq(netstandard2.0)'
        Before:
                private ReturnBase()
        After:
                ReturnBase()
        */

        /* Unmerged change from project 'Moq(netstandard2.1)'
        Before:
                private ReturnBase()
        After:
                ReturnBase()
        */

        /* Unmerged change from project 'Moq(net6.0)'
        Before:
                private ReturnBase()
        After:
                ReturnBase()
        */
        ReturnBase()
        {
        }

        public override void Execute(Invocation invocation)
        {
            invocation.ReturnValue = invocation.CallBase();
        }
    }
}
