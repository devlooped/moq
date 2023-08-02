// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class EmptyDefaultValueProvider : LookupOrFallbackDefaultValueProvider
    After:
        sealed class EmptyDefaultValueProvider : LookupOrFallbackDefaultValueProvider
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class EmptyDefaultValueProvider : LookupOrFallbackDefaultValueProvider
    After:
        sealed class EmptyDefaultValueProvider : LookupOrFallbackDefaultValueProvider
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class EmptyDefaultValueProvider : LookupOrFallbackDefaultValueProvider
    After:
        sealed class EmptyDefaultValueProvider : LookupOrFallbackDefaultValueProvider
    */
    /// <summary>
    /// A <see cref="DefaultValueProvider"/> that returns an empty default value 
    /// for invocations that do not have setups or return values, with loose mocks.
    /// This is the default behavior for a mock.
    /// </summary>
    sealed class EmptyDefaultValueProvider : LookupOrFallbackDefaultValueProvider
    {
        internal EmptyDefaultValueProvider()
        {
            base.Register(typeof(Array), CreateArray);
            base.Register(typeof(IEnumerable), CreateEnumerable);
            base.Register(typeof(IEnumerable<>), CreateEnumerableOf);
            base.Register(typeof(IQueryable), CreateQueryable);
            base.Register(typeof(IQueryable<>), CreateQueryableOf);
        }

        internal override DefaultValue Kind => DefaultValue.Empty;


        /* Unmerged change from project 'Moq(netstandard2.0)'
        Before:
                private static object CreateArray(Type type, Mock mock)
        After:
                static object CreateArray(Type type, Mock mock)
        */

        /* Unmerged change from project 'Moq(netstandard2.1)'
        Before:
                private static object CreateArray(Type type, Mock mock)
        After:
                static object CreateArray(Type type, Mock mock)
        */

        /* Unmerged change from project 'Moq(net6.0)'
        Before:
                private static object CreateArray(Type type, Mock mock)
        After:
                static object CreateArray(Type type, Mock mock)
        */
        static object CreateArray(Type type, Mock mock)
        {
            var elementType = type.GetElementType();
            var lengths = new int[type.GetArrayRank()];
            return Array.CreateInstance(elementType, lengths);

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                    private static object CreateEnumerable(Type type, Mock mock)
            After:
                    static object CreateEnumerable(Type type, Mock mock)
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                    private static object CreateEnumerable(Type type, Mock mock)
            After:
                    static object CreateEnumerable(Type type, Mock mock)
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                    private static object CreateEnumerable(Type type, Mock mock)
            After:
                    static object CreateEnumerable(Type type, Mock mock)
            */
        }

        static object CreateEnumerable(Type type, Mock mock)
        {
            return new object[0];

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                    private static object CreateEnumerableOf(Type type, Mock mock)
            After:
                    static object CreateEnumerableOf(Type type, Mock mock)
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                    private static object CreateEnumerableOf(Type type, Mock mock)
            After:
                    static object CreateEnumerableOf(Type type, Mock mock)
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                    private static object CreateEnumerableOf(Type type, Mock mock)
            After:
                    static object CreateEnumerableOf(Type type, Mock mock)
            */
        }

        static object CreateEnumerableOf(Type type, Mock mock)
        {
            var elementType = type.GetGenericArguments()[0];
            return Array.CreateInstance(elementType, 0);

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                    private static object CreateQueryable(Type type, Mock mock)
            After:
                    static object CreateQueryable(Type type, Mock mock)
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                    private static object CreateQueryable(Type type, Mock mock)
            After:
                    static object CreateQueryable(Type type, Mock mock)
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                    private static object CreateQueryable(Type type, Mock mock)
            After:
                    static object CreateQueryable(Type type, Mock mock)
            */
        }

        static object CreateQueryable(Type type, Mock mock)
        {
            return new object[0].AsQueryable();

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                    private static object CreateQueryableOf(Type type, Mock mock)
            After:
                    static object CreateQueryableOf(Type type, Mock mock)
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                    private static object CreateQueryableOf(Type type, Mock mock)
            After:
                    static object CreateQueryableOf(Type type, Mock mock)
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                    private static object CreateQueryableOf(Type type, Mock mock)
            After:
                    static object CreateQueryableOf(Type type, Mock mock)
            */
        }

        static object CreateQueryableOf(Type type, Mock mock)
        {
            var elementType = type.GetGenericArguments()[0];
            var array = Array.CreateInstance(elementType, 0);

            return typeof(Queryable).GetMethods("AsQueryable")
                .Single(x => x.IsGenericMethod)
                .MakeGenericMethod(elementType)
                .Invoke(null, new[] { array });
        }
    }
}
