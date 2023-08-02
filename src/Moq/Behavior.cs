// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal abstract class Behavior
    After:
        abstract class Behavior
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal abstract class Behavior
    After:
        abstract class Behavior
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal abstract class Behavior
    After:
        abstract class Behavior
    */
    abstract class Behavior
    {
        protected Behavior()
        {
        }

        public abstract void Execute(Invocation invocation);
    }
}
