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
#if NET462 || NETSTANDARD2_0
        public static IAwaitableFactory? TryGet(Type type) => LegacyAwaitableFactory.TryGet(type);
#else
        // Modern TFMs (ValueTask lives in platform BCL; direct, no static cctor, direct
        // type checks for non-generic, simple creation for generics. This path performs
        // no assembly name/FullName probing or deferred loading tricks.
        // Non-VT (Task) path has no ValueTask tokens or VT-specific code in its IL when
        // the helper is NoInlining.
        public static IAwaitableFactory? TryGet(Type type)
        {
            Debug.Assert(type != null);

            if (type == typeof(Task))
                return TaskFactory.Instance;

            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
                return Create(typeof(TaskFactory<>), type);

            // VT handling isolated so common Task path never touches VT metadata at jit time for this method
            return TryGetValueTask(type);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        static IAwaitableFactory? TryGetValueTask(Type type)
        {
            if (type == typeof(ValueTask))
                return ValueTaskFactory.Instance;

            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(ValueTask<>))
                return Create(typeof(ValueTaskFactory<>), type);

            return null;
        }

        static IAwaitableFactory Create(Type awaitableFactoryType, Type awaitableType)
        {
            return (IAwaitableFactory)Activator.CreateInstance(
                awaitableFactoryType.MakeGenericType(
                    awaitableType.GetGenericArguments()))!;
        }
#endif
    }

#if NET462 || NETSTANDARD2_0
    internal static class LegacyAwaitableFactory
    {
        static readonly Dictionary<Type, Func<Type, IAwaitableFactory>> Providers;

        static LegacyAwaitableFactory()
        {
            LegacyAwaitableFactory.Providers = new Dictionary<Type, Func<Type, IAwaitableFactory>>
            {
                [typeof(Task)] = awaitableType => TaskFactory.Instance,
                [typeof(Task<>)] = awaitableType => LegacyAwaitableFactory.Create(typeof(TaskFactory<>), awaitableType),
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
            var asm = typeof(LegacyAwaitableFactory).Assembly;
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
            var asm = typeof(LegacyAwaitableFactory).Assembly;
            var factoryType = asm.GetType("Moq.Async.ValueTaskFactory`1", throwOnError: true)!;
            factoryType = factoryType.MakeGenericType(awaitableType.GetGenericArguments());
            return (IAwaitableFactory)Activator.CreateInstance(factoryType)!;
        }

        public static IAwaitableFactory? TryGet(Type type)
        {
            Debug.Assert(type != null);

            var key = type.IsConstructedGenericType ? type.GetGenericTypeDefinition() : type;

            if (LegacyAwaitableFactory.Providers.TryGetValue(key, out var provider))
            {
                return provider.Invoke(type);
            }

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
#endif
}
