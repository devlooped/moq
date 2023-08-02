// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal interface IInterceptor
    After:
        interface IInterceptor
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal interface IInterceptor
    After:
        interface IInterceptor
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal interface IInterceptor
    After:
        interface IInterceptor
    */
    /// <summary>
    /// This role interface represents a <see cref="Mock"/>'s ability to intercept method invocations for its <see cref="Mock.Object"/>.
    /// It is meant for use by <see cref="ProxyFactory"/>.
    /// </summary>
    interface IInterceptor
    {
        void Intercept(Invocation invocation);
    }
}
