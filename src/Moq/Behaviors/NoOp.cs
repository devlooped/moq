// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class NoOp : Behavior
    After:
        sealed class NoOp : Behavior
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class NoOp : Behavior
    After:
        sealed class NoOp : Behavior
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class NoOp : Behavior
    After:
        sealed class NoOp : Behavior
    */
    sealed class NoOp : Behavior
    {
        public static readonly NoOp Instance = new NoOp();


        /* Unmerged change from project 'Moq(netstandard2.0)'
        Before:
                private NoOp()
        After:
                NoOp()
        */

        /* Unmerged change from project 'Moq(netstandard2.1)'
        Before:
                private NoOp()
        After:
                NoOp()
        */

        /* Unmerged change from project 'Moq(net6.0)'
        Before:
                private NoOp()
        After:
                NoOp()
        */
        NoOp()
        {
        }

        public override void Execute(Invocation invocation)
        {
        }
    }
}
