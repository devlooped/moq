// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Moq.Async
{
    static class AwaitableFactory
    {
        static readonly Dictionary<Type, Func<Type, IAwaitableFactory>> Providers;

        static AwaitableFactory()
        {
            AwaitableFactory.Providers = new Dictionary<Type, Func<Type, IAwaitableFactory>>
            {
                [typeof(Task)] = awaitableType => TaskFactory.Instance,
                [typeof(Task<>)] = awaitableType => AwaitableFactory.Create(typeof(TaskFactory<>), awaitableType),
            };
        }

        static IAwaitableFactory Create(Type awaitableFactoryType, Type awaitableType)
        {
            return (IAwaitableFactory)Activator.CreateInstance(
                awaitableFactoryType.MakeGenericType(
                    awaitableType.GetGenericArguments()))!;
        }

        static IAwaitableFactory GetValueTaskFactory()
        {
            // Use string-based lookup + reflection so that AwaitableFactory's type initializer
            // and common TryGet paths contain no tokens referencing ValueTask types.
            // This prevents premature loading of System.Threading.Tasks.Extensions (and version conflicts)
            // during early initialization for code paths that never use ValueTask.
            var asm = typeof(AwaitableFactory).Assembly;
            var factoryType = asm.GetType("Moq.Async.ValueTaskFactory", throwOnError: true)!;
            var instanceField = factoryType.GetField("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            if (instanceField != null)
            {
                return (IAwaitableFactory)instanceField.GetValue(null)!;
            }
            return (IAwaitableFactory)Activator.CreateInstance(factoryType)!;
        }

        static IAwaitableFactory GetValueTaskFactoryGeneric(Type awaitableType)
        {
            // Deferred creation for ValueTask<T> using only runtime strings/Activator.
            var asm = typeof(AwaitableFactory).Assembly;
            var factoryType = asm.GetType("Moq.Async.ValueTaskFactory`1", throwOnError: true)!;
            factoryType = factoryType.MakeGenericType(awaitableType.GetGenericArguments());
            return (IAwaitableFactory)Activator.CreateInstance(factoryType)!;
        }

        public static IAwaitableFactory? TryGet(Type type)
        {
            Debug.Assert(type != null);

            var key = type.IsConstructedGenericType ? type.GetGenericTypeDefinition() : type;

            if (AwaitableFactory.Providers.TryGetValue(key, out var provider))
            {
                return provider.Invoke(type);
            }

            // Runtime FullName-based detection for ValueTask cases (no typeof(ValueTask) in init or this method source).
            // Only exercised when a ValueTask* type is actually passed to TryGet (i.e. async usage paths).
            var fullName = key.FullName;
            if (fullName == "System.Threading.Tasks.ValueTask")
            {
                return GetValueTaskFactory();
            }
            if (fullName == "System.Threading.Tasks.ValueTask`1")
            {
                return GetValueTaskFactoryGeneric(type);
            }

            return null;
        }
    }
}
