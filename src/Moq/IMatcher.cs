// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal interface IMatcher
    After:
        interface IMatcher
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal interface IMatcher
    After:
        interface IMatcher
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal interface IMatcher
    After:
        interface IMatcher
    */
    interface IMatcher
    {
        bool Matches(object argument, Type parameterType);

        void SetupEvaluatedSuccessfully(object argument, Type parameterType);
    }
}
