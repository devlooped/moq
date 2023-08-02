// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal abstract class MethodSetup : Setup
    After:
        abstract class MethodSetup : Setup
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal abstract class MethodSetup : Setup
    After:
        abstract class MethodSetup : Setup
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal abstract class MethodSetup : Setup
    After:
        abstract class MethodSetup : Setup
    */
    /// <summary>
    ///   Abstract base class for setups that target a single, specific method.
    /// </summary>
    abstract class MethodSetup : Setup
    {
        protected MethodSetup(Expression originalExpression, Mock mock, MethodExpectation expectation)
            : base(originalExpression, mock, expectation)
        {
        }

        public MethodInfo Method => ((MethodExpectation)this.Expectation).Method;
    }
}
