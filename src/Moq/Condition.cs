// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class Condition
    After:
        sealed class Condition
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class Condition
    After:
        sealed class Condition
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class Condition
    After:
        sealed class Condition
    */
    sealed class Condition

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private Func<bool> condition;
            private Action success;
    After:
            Func<bool> condition;
            Action success;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private Func<bool> condition;
            private Action success;
    After:
            Func<bool> condition;
            Action success;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private Func<bool> condition;
            private Action success;
    After:
            Func<bool> condition;
            Action success;
    */
    {
        Func<bool> condition;
        Action success;

        public Condition(Func<bool> condition, Action success = null)
        {
            this.condition = condition;
            this.success = success;
        }

        public bool IsTrue => this.condition?.Invoke() == true;

        public void SetupEvaluatedSuccessfully() => this.success?.Invoke();
    }
}
