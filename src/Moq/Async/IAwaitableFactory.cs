// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Moq.Async
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal interface IAwaitableFactory
    After:
        interface IAwaitableFactory
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal interface IAwaitableFactory
    After:
        interface IAwaitableFactory
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal interface IAwaitableFactory
    After:
        interface IAwaitableFactory
    */
    interface IAwaitableFactory
    {
        Type ResultType { get; }

        object CreateCompleted(object result = null);

        object CreateFaulted(Exception exception);

        object CreateFaulted(IEnumerable<Exception> exceptions);

        Expression CreateResultExpression(Expression awaitableExpression);

        bool TryGetResult(object awaitable, out object result);
    }
}
