// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Matchers
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class AnyMatcher : IMatcher
    After:
        sealed class AnyMatcher : IMatcher
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class AnyMatcher : IMatcher
    After:
        sealed class AnyMatcher : IMatcher
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class AnyMatcher : IMatcher
    After:
        sealed class AnyMatcher : IMatcher
    */
    sealed class AnyMatcher : IMatcher
    {
        public static AnyMatcher Instance { get; } = new AnyMatcher();


        /* Unmerged change from project 'Moq(netstandard2.0)'
        Before:
                private AnyMatcher()
        After:
                AnyMatcher()
        */

        /* Unmerged change from project 'Moq(netstandard2.1)'
        Before:
                private AnyMatcher()
        After:
                AnyMatcher()
        */

        /* Unmerged change from project 'Moq(net6.0)'
        Before:
                private AnyMatcher()
        After:
                AnyMatcher()
        */
        AnyMatcher()
        {
        }

        public bool Matches(object argument, Type parameterType) => true;

        public void SetupEvaluatedSuccessfully(object argument, Type parameterType)
        {
            Debug.Assert(this.Matches(argument, parameterType));
        }
    }
}
