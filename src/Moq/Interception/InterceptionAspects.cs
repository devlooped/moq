// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Moq.Behaviors;

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal static class HandleWellKnownMethods
    After:
        static class HandleWellKnownMethods
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal static class HandleWellKnownMethods
    After:
        static class HandleWellKnownMethods
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal static class HandleWellKnownMethods
    After:
        static class HandleWellKnownMethods
    */
    static class HandleWellKnownMethods

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private static Dictionary<string, Func<Invocation, Mock, bool>> specialMethods = new Dictionary<string, Func<Invocation, Mock, bool>>()
    After:
            static Dictionary<string, Func<Invocation, Mock, bool>> specialMethods = new Dictionary<string, Func<Invocation, Mock, bool>>()
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private static Dictionary<string, Func<Invocation, Mock, bool>> specialMethods = new Dictionary<string, Func<Invocation, Mock, bool>>()
    After:
            static Dictionary<string, Func<Invocation, Mock, bool>> specialMethods = new Dictionary<string, Func<Invocation, Mock, bool>>()
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private static Dictionary<string, Func<Invocation, Mock, bool>> specialMethods = new Dictionary<string, Func<Invocation, Mock, bool>>()
    After:
            static Dictionary<string, Func<Invocation, Mock, bool>> specialMethods = new Dictionary<string, Func<Invocation, Mock, bool>>()
    */
    {
        static Dictionary<string, Func<Invocation, Mock, bool>> specialMethods = new Dictionary<string, Func<Invocation, Mock, bool>>()
        {
            ["Equals"] = HandleEquals,
            ["GetHashCode"] = HandleGetHashCode,
            ["get_" + nameof(IMocked.Mock)] = HandleMockGetter,
            ["ToString"] = HandleToString,
        };

        public static bool Handle(Invocation invocation, Mock mock)
        {
            return specialMethods.TryGetValue(invocation.Method.Name, out var handler)
                && handler.Invoke(invocation, mock);

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                    private static bool HandleEquals(Invocation invocation, Mock mock)
            After:
                    static bool HandleEquals(Invocation invocation, Mock mock)
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                    private static bool HandleEquals(Invocation invocation, Mock mock)
            After:
                    static bool HandleEquals(Invocation invocation, Mock mock)
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                    private static bool HandleEquals(Invocation invocation, Mock mock)
            After:
                    static bool HandleEquals(Invocation invocation, Mock mock)
            */
        }

        static bool HandleEquals(Invocation invocation, Mock mock)
        {
            if (IsObjectMethodWithoutSetup(invocation, mock))
            {
                invocation.ReturnValue = ReferenceEquals(invocation.Arguments.First(), mock.Object);
                return true;
            }
            else
            {
                return false;

                /* Unmerged change from project 'Moq(netstandard2.0)'
                Before:
                        private static bool HandleGetHashCode(Invocation invocation, Mock mock)
                After:
                        static bool HandleGetHashCode(Invocation invocation, Mock mock)
                */

                /* Unmerged change from project 'Moq(netstandard2.1)'
                Before:
                        private static bool HandleGetHashCode(Invocation invocation, Mock mock)
                After:
                        static bool HandleGetHashCode(Invocation invocation, Mock mock)
                */

                /* Unmerged change from project 'Moq(net6.0)'
                Before:
                        private static bool HandleGetHashCode(Invocation invocation, Mock mock)
                After:
                        static bool HandleGetHashCode(Invocation invocation, Mock mock)
                */
            }
        }

        static bool HandleGetHashCode(Invocation invocation, Mock mock)
        {
            if (IsObjectMethodWithoutSetup(invocation, mock))
            {
                invocation.ReturnValue = mock.GetHashCode();
                return true;
            }
            else
            {
                return false;

                /* Unmerged change from project 'Moq(netstandard2.0)'
                Before:
                        private static bool HandleToString(Invocation invocation, Mock mock)
                After:
                        static bool HandleToString(Invocation invocation, Mock mock)
                */

                /* Unmerged change from project 'Moq(netstandard2.1)'
                Before:
                        private static bool HandleToString(Invocation invocation, Mock mock)
                After:
                        static bool HandleToString(Invocation invocation, Mock mock)
                */

                /* Unmerged change from project 'Moq(net6.0)'
                Before:
                        private static bool HandleToString(Invocation invocation, Mock mock)
                After:
                        static bool HandleToString(Invocation invocation, Mock mock)
                */
            }
        }

        static bool HandleToString(Invocation invocation, Mock mock)
        {
            if (IsObjectMethodWithoutSetup(invocation, mock))
            {
                invocation.ReturnValue = mock.ToString() + ".Object";
                return true;
            }
            else
            {
                return false;

                /* Unmerged change from project 'Moq(netstandard2.0)'
                Before:
                        private static bool HandleMockGetter(Invocation invocation, Mock mock)
                After:
                        static bool HandleMockGetter(Invocation invocation, Mock mock)
                */

                /* Unmerged change from project 'Moq(netstandard2.1)'
                Before:
                        private static bool HandleMockGetter(Invocation invocation, Mock mock)
                After:
                        static bool HandleMockGetter(Invocation invocation, Mock mock)
                */

                /* Unmerged change from project 'Moq(net6.0)'
                Before:
                        private static bool HandleMockGetter(Invocation invocation, Mock mock)
                After:
                        static bool HandleMockGetter(Invocation invocation, Mock mock)
                */
            }
        }

        static bool HandleMockGetter(Invocation invocation, Mock mock)
        {
            if (typeof(IMocked).IsAssignableFrom(invocation.Method.DeclaringType))
            {
                invocation.ReturnValue = mock;
                return true;
            }
            else
            {
                return false;

                /* Unmerged change from project 'Moq(netstandard2.0)'
                Before:
                        private static bool IsObjectMethodWithoutSetup(Invocation invocation, Mock mock)
                After:
                        static bool IsObjectMethodWithoutSetup(Invocation invocation, Mock mock)
                */

                /* Unmerged change from project 'Moq(netstandard2.1)'
                Before:
                        private static bool IsObjectMethodWithoutSetup(Invocation invocation, Mock mock)
                After:
                        static bool IsObjectMethodWithoutSetup(Invocation invocation, Mock mock)
                */

                /* Unmerged change from project 'Moq(net6.0)'
                Before:
                        private static bool IsObjectMethodWithoutSetup(Invocation invocation, Mock mock)
                After:
                        static bool IsObjectMethodWithoutSetup(Invocation invocation, Mock mock)
                */
            }
        }

        static bool IsObjectMethodWithoutSetup(Invocation invocation, Mock mock)
        {
            return invocation.Method.DeclaringType == typeof(object)
                && mock.MutableSetups.FindLast(setup => setup.Matches(invocation)) == null;

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                internal static class FindAndExecuteMatchingSetup
            After:
                static class FindAndExecuteMatchingSetup
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                internal static class FindAndExecuteMatchingSetup
            After:
                static class FindAndExecuteMatchingSetup
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                internal static class FindAndExecuteMatchingSetup
            After:
                static class FindAndExecuteMatchingSetup
            */
        }
    }

    static class FindAndExecuteMatchingSetup
    {
        public static bool Handle(Invocation invocation, Mock mock)
        {
            var matchingSetup = mock.MutableSetups.FindLast(setup => setup.Matches(invocation));
            if (matchingSetup != null)
            {
                matchingSetup.Execute(invocation);
                return true;
            }
            else
            {
                return false;

                /* Unmerged change from project 'Moq(netstandard2.0)'
                Before:
                    internal static class HandleEventSubscription
                After:
                    static class HandleEventSubscription
                */

                /* Unmerged change from project 'Moq(netstandard2.1)'
                Before:
                    internal static class HandleEventSubscription
                After:
                    static class HandleEventSubscription
                */

                /* Unmerged change from project 'Moq(net6.0)'
                Before:
                    internal static class HandleEventSubscription
                After:
                    static class HandleEventSubscription
                */
            }
        }
    }

    static class HandleEventSubscription
    {
        public static bool Handle(Invocation invocation, Mock mock)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            var methodName = invocation.Method.Name;

            // Special case for event accessors. The following, seemingly random character checks are guards against
            // more expensive checks (for the common case where the invoked method is *not* an event accessor).
            if (methodName.Length > 4)
            {
                if (methodName[0] == 'a' && methodName[3] == '_' && invocation.Method.IsEventAddAccessor())
                {
                    var implementingMethod = invocation.Method.GetImplementingMethod(invocation.ProxyType);
                    var @event = implementingMethod.DeclaringType.GetEvents(bindingFlags).SingleOrDefault(e => e.GetAddMethod(true) == implementingMethod);
                    if (@event != null)
                    {
                        if (mock.CallBase && !invocation.Method.IsAbstract)
                        {
                            invocation.ReturnValue = invocation.CallBase();
                            return true;
                        }
                        else if (invocation.Arguments.Length > 0 && invocation.Arguments[0] is Delegate delegateInstance)
                        {
                            mock.EventHandlers.Add(@event, delegateInstance);
                            return true;
                        }
                    }
                }
                else if (methodName[0] == 'r' && methodName.Length > 7 && methodName[6] == '_' && invocation.Method.IsEventRemoveAccessor())
                {
                    var implementingMethod = invocation.Method.GetImplementingMethod(invocation.ProxyType);
                    var @event = implementingMethod.DeclaringType.GetEvents(bindingFlags).SingleOrDefault(e => e.GetRemoveMethod(true) == implementingMethod);
                    if (@event != null)
                    {
                        if (mock.CallBase && !invocation.Method.IsAbstract)
                        {
                            invocation.ReturnValue = invocation.CallBase();
                            return true;
                        }
                        else if (invocation.Arguments.Length > 0 && invocation.Arguments[0] is Delegate delegateInstance)
                        {
                            mock.EventHandlers.Remove(@event, delegateInstance);
                            return true;
                        }
                    }
                }
            }

            return false;

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                internal static class RecordInvocation
            After:
                static class RecordInvocation
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                internal static class RecordInvocation
            After:
                static class RecordInvocation
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                internal static class RecordInvocation
            After:
                static class RecordInvocation
            */
        }
    }

    static class RecordInvocation
    {
        public static void Handle(Invocation invocation, Mock mock)
        {
            // Save to support Verify[expression] pattern.
            mock.MutableInvocations.Add(invocation);

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                internal static class Return
            After:
                static class Return
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                internal static class Return
            After:
                static class Return
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                internal static class Return
            After:
                static class Return
            */
        }
    }

    static class Return
    {
        public static void Handle(Invocation invocation, Mock mock)
        {
            new ReturnBaseOrDefaultValue(mock).Execute(invocation);

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                internal static class FailForStrictMock
            After:
                static class FailForStrictMock
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                internal static class FailForStrictMock
            After:
                static class FailForStrictMock
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                internal static class FailForStrictMock
            After:
                static class FailForStrictMock
            */
        }
    }

    static class FailForStrictMock
    {
        public static void Handle(Invocation invocation, Mock mock)
        {
            if (mock.Behavior == MockBehavior.Strict)
            {
                throw MockException.NoSetup(invocation);
            }
        }
    }
}
