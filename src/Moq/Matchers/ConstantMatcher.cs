// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace Moq.Matchers
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal class ConstantMatcher : IMatcher
    After:
        class ConstantMatcher : IMatcher
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal class ConstantMatcher : IMatcher
    After:
        class ConstantMatcher : IMatcher
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal class ConstantMatcher : IMatcher
    After:
        class ConstantMatcher : IMatcher
    */
    class ConstantMatcher : IMatcher

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private object constantValue;
    After:
            object constantValue;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private object constantValue;
    After:
            object constantValue;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private object constantValue;
    After:
            object constantValue;
    */
    {
        object constantValue;

        public ConstantMatcher(object constantValue)
        {
            this.constantValue = constantValue;
        }

        public bool Matches(object argument, Type parameterType)
        {
            if (object.Equals(argument, constantValue))
            {
                return true;
            }

            if (this.constantValue is IEnumerable && argument is IEnumerable enumerable &&
                !(this.constantValue is IMocked) && !(argument is IMocked))
            // the above checks on the second line are necessary to ensure we have usable
            // implementations of IEnumerable, which might very well not be the case for
            // mocked objects.
            {
                return this.MatchesEnumerable(enumerable);
            }

            return false;
        }

        public void SetupEvaluatedSuccessfully(object argument, Type parameterType)
        {
            Debug.Assert(this.Matches(argument, parameterType));

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                    private bool MatchesEnumerable(IEnumerable enumerable)
            After:
                    bool MatchesEnumerable(IEnumerable enumerable)
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                    private bool MatchesEnumerable(IEnumerable enumerable)
            After:
                    bool MatchesEnumerable(IEnumerable enumerable)
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                    private bool MatchesEnumerable(IEnumerable enumerable)
            After:
                    bool MatchesEnumerable(IEnumerable enumerable)
            */
        }

        bool MatchesEnumerable(IEnumerable enumerable)
        {
            var constValues = (IEnumerable)constantValue;
            return constValues.Cast<object>().SequenceEqual(enumerable.Cast<object>());
        }
    }
}
