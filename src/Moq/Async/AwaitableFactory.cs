// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Moq.Async
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal static class AwaitableFactory
    After:
        static class AwaitableFactory
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal static class AwaitableFactory
    After:
        static class AwaitableFactory
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal static class AwaitableFactory
    After:
        static class AwaitableFactory
    */
    static class AwaitableFactory

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private static readonly Dictionary<Type, Func<Type, IAwaitableFactory>> Providers;
    After:
            static readonly Dictionary<Type, Func<Type, IAwaitableFactory>> Providers;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private static readonly Dictionary<Type, Func<Type, IAwaitableFactory>> Providers;
    After:
            static readonly Dictionary<Type, Func<Type, IAwaitableFactory>> Providers;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private static readonly Dictionary<Type, Func<Type, IAwaitableFactory>> Providers;
    After:
            static readonly Dictionary<Type, Func<Type, IAwaitableFactory>> Providers;
    */
    {
        static readonly Dictionary<Type, Func<Type, IAwaitableFactory>> Providers;

        static AwaitableFactory()
        {
            AwaitableFactory.Providers = new Dictionary<Type, Func<Type, IAwaitableFactory>>
            {
                [typeof(Task)] = awaitableType => TaskFactory.Instance,
                [typeof(ValueTask)] = awaitableType => ValueTaskFactory.Instance,
                [typeof(Task<>)] = awaitableType => AwaitableFactory.Create(typeof(TaskFactory<>), awaitableType),
                [typeof(ValueTask<>)] = awaitableType => AwaitableFactory.Create(typeof(ValueTaskFactory<>), awaitableType),
            };

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                    private static IAwaitableFactory Create(Type awaitableFactoryType, Type awaitableType)
            After:
                    static IAwaitableFactory Create(Type awaitableFactoryType, Type awaitableType)
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                    private static IAwaitableFactory Create(Type awaitableFactoryType, Type awaitableType)
            After:
                    static IAwaitableFactory Create(Type awaitableFactoryType, Type awaitableType)
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                    private static IAwaitableFactory Create(Type awaitableFactoryType, Type awaitableType)
            After:
                    static IAwaitableFactory Create(Type awaitableFactoryType, Type awaitableType)
            */
        }

        static IAwaitableFactory Create(Type awaitableFactoryType, Type awaitableType)
        {
            return (IAwaitableFactory)Activator.CreateInstance(
                awaitableFactoryType.MakeGenericType(
                    awaitableType.GetGenericArguments()));
        }

        public static IAwaitableFactory TryGet(Type type)
        {
            Debug.Assert(type != null);

            var key = type.IsConstructedGenericType ? type.GetGenericTypeDefinition() : type;

            if (AwaitableFactory.Providers.TryGetValue(key, out var provider))
            {
                return provider.Invoke(type);
            }

            return null;
        }
    }
}
