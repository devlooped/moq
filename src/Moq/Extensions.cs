// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal static class Extensions
    After:
        static class Extensions
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal static class Extensions
    After:
        static class Extensions
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal static class Extensions
    After:
        static class Extensions
    */
    static class Extensions
    {
        public static bool CanCreateInstance(this Type type)
        {
            return type.IsValueType || type.GetConstructor(Type.EmptyTypes) != null;
        }

        public static bool CanRead(this PropertyInfo property, out MethodInfo getter)
        {
            return property.CanRead(out getter, out _);
        }

        public static bool CanRead(this PropertyInfo property, out MethodInfo getter, out PropertyInfo getterProperty)
        {
            if (property.CanRead)
            {
                // The given `PropertyInfo` should be able to provide a getter:
                getter = property.GetGetMethod(nonPublic: true);
                getterProperty = property;
                Debug.Assert(getter != null);
                return true;
            }
            else
            {
                // The given `PropertyInfo` cannot provide a getter... but there may still be one in a base class'
                // corresponding `PropertyInfo`! We need to find that base `PropertyInfo`, and because `PropertyInfo`
                // does not have `.GetBaseDefinition()`, we'll find it via the setter's `.GetBaseDefinition()`.
                // (We may assume that there's a setter because properties/indexers must have at least one accessor.)
                Debug.Assert(property.CanWrite);
                var setter = property.GetSetMethod(nonPublic: true);
                Debug.Assert(setter != null);

                var baseSetter = setter.GetBaseDefinition();
                if (baseSetter != setter)
                {
                    var baseProperty =
                        baseSetter
                        .DeclaringType
                        .GetMember(property.Name, MemberTypes.Property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Cast<PropertyInfo>()
                        .First(p => p.GetSetMethod(nonPublic: true) == baseSetter);
                    return baseProperty.CanRead(out getter, out getterProperty);
                }
            }

            getter = null;
            getterProperty = null;
            return false;
        }

        public static bool CanWrite(this PropertyInfo property, out MethodInfo setter)
        {
            return property.CanWrite(out setter, out _);
        }

        public static bool CanWrite(this PropertyInfo property, out MethodInfo setter, out PropertyInfo setterProperty)
        {
            if (property.CanWrite)
            {
                // The given `PropertyInfo` should be able to provide a setter:
                setter = property.GetSetMethod(nonPublic: true);
                setterProperty = property;
                Debug.Assert(setter != null);
                return true;
            }
            else
            {
                // The given `PropertyInfo` cannot provide a setter... but there may still be one in a base class'
                // corresponding `PropertyInfo`! We need to find that base `PropertyInfo`, and because `PropertyInfo`
                // does not have `.GetBaseDefinition()`, we'll find it via the getter's `.GetBaseDefinition()`.
                // (We may assume that there's a getter because properties/indexers must have at least one accessor.)
                Debug.Assert(property.CanRead);
                var getter = property.GetGetMethod(nonPublic: true);
                Debug.Assert(getter != null);

                var baseGetter = getter.GetBaseDefinition();
                if (baseGetter != getter)
                {
                    var baseProperty =
                        baseGetter
                        .DeclaringType
                        .GetMember(property.Name, MemberTypes.Property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Cast<PropertyInfo>()
                        .First(p => p.GetGetMethod(nonPublic: true) == baseGetter);
                    return baseProperty.CanWrite(out setter, out setterProperty);
                }
            }

            setter = null;
            setterProperty = null;
            return false;
        }

        /// <summary>
        ///   Gets the default value for the specified type. This is the Reflection counterpart of C#'s <see langword="default"/> operator.
        /// </summary>
        public static object GetDefaultValue(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        ///   Gets the least-derived <see cref="MethodInfo"/> in the given type that provides
        ///   the implementation for the given <paramref name="method"/>.
        /// </summary>
        public static MethodInfo GetImplementingMethod(this MethodInfo method, Type proxyType)
        {
            Debug.Assert(method != null);
            Debug.Assert(proxyType != null);
            Debug.Assert(proxyType.IsClass);

            if (method.IsGenericMethod)
            {
                method = method.GetGenericMethodDefinition();
            }

            var declaringType = method.DeclaringType;

            if (declaringType.IsInterface)
            {
                Debug.Assert(declaringType.IsAssignableFrom(proxyType));

                var map = GetInterfaceMap(proxyType, method.DeclaringType);
                var index = Array.IndexOf(map.InterfaceMethods, method);
                Debug.Assert(index >= 0);
                return map.TargetMethods[index].GetBaseDefinition();
            }
            else if (declaringType.IsDelegateType())
            {
                return proxyType.GetMethod("Invoke");
            }
            else
            {
                Debug.Assert(declaringType.IsAssignableFrom(proxyType));

                return method.GetBaseDefinition();
            }
        }

        public static object InvokePreserveStack(this Delegate del, IReadOnlyList<object> args = null)
        {
            try
            {
                return del.DynamicInvoke((args as object[]) ?? args?.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;
            }
        }

        public static bool IsExtensionMethod(this MethodInfo method)
        {
            return method.IsStatic && method.IsDefined(typeof(ExtensionAttribute));
        }

        public static bool IsGetAccessor(this MethodInfo method)
        {
            return method.IsSpecialName && method.Name.StartsWith("get_", StringComparison.Ordinal);
        }

        public static bool IsSetAccessor(this MethodInfo method)
        {
            return method.IsSpecialName && method.Name.StartsWith("set_", StringComparison.Ordinal);
        }

        public static bool IsIndexerAccessor(this MethodInfo method)
        {
            var parameterCount = method.GetParameters().Length;
            return (method.IsGetAccessor() && parameterCount > 0)
                || (method.IsSetAccessor() && parameterCount > 1);
        }

        public static bool IsPropertyAccessor(this MethodInfo method)
        {
            var parameterCount = method.GetParameters().Length;
            return (method.IsGetAccessor() && parameterCount == 0)
                || (method.IsSetAccessor() && parameterCount == 1);
        }

        // NOTE: The following two methods used to first check whether `method.IsSpecialName` was set
        // as a quick guard against non-event accessor methods. This was removed in commit 44070a90
        // to "increase compatibility with F# and COM". More specifically:
        //
        //  1. COM does not really have events. Some COM interop assemblies define events, but do not
        //     mark those with the IL `specialname` flag. See:
        //      - https://code.google.com/archive/p/moq/issues/226
        //     - the `Microsoft.Office.Interop.Word.ApplicationEvents4_Event` interface in Office PIA
        //
        //  2. F# does not mark abstract events' accessors with the IL `specialname` flag. See:
        //      - https://github.com/Microsoft/visualfsharp/issues/5834
        //      - https://code.google.com/archive/p/moq/issues/238
        //      - the unit tests in `FSharpCompatibilityFixture`

        public static bool IsEventAddAccessor(this MethodInfo method)
        {
            return method.Name.StartsWith("add_", StringComparison.Ordinal);
        }

        public static bool IsEventRemoveAccessor(this MethodInfo method)
        {
            return method.Name.StartsWith("remove_", StringComparison.Ordinal);
        }

        /// <summary>
        ///   Gets whether the given <paramref name="type"/> is a delegate type.
        /// </summary>
        public static bool IsDelegateType(this Type type)
        {
            Debug.Assert(type != null);
            return type.BaseType == typeof(MulticastDelegate);
        }

        public static bool IsMockable(this Type type)
        {
            return !type.IsSealed || type.IsDelegateType();
        }

        public static bool IsTypeMatcher(this Type type)
        {
            return Attribute.IsDefined(type, typeof(TypeMatcherAttribute));
        }

        public static bool IsTypeMatcher(this Type type, out Type typeMatcherType)
        {
            if (type.IsTypeMatcher())
            {
                var attr = (TypeMatcherAttribute)Attribute.GetCustomAttribute(type, typeof(TypeMatcherAttribute));
                typeMatcherType = attr.Type ?? type;
                Guard.ImplementsTypeMatcherProtocol(typeMatcherType);
                return true;
            }
            else
            {
                typeMatcherType = null;
                return false;
            }
        }

        public static bool IsOrContainsTypeMatcher(this Type type)
        {
            if (type.IsTypeMatcher())
            {
                return true;
            }
            else if (type.HasElementType)
            {
                return type.GetElementType().IsOrContainsTypeMatcher();
            }
            else if (type.IsGenericType)
            {
                return type.GetGenericArguments().Any(IsOrContainsTypeMatcher);
            }
            else
            {
                return false;
            }
        }

        public static bool ImplementsTypeMatcherProtocol(this Type type)
        {
            return typeof(ITypeMatcher).IsAssignableFrom(type) && type.CanCreateInstance();
        }

        public static bool CanOverride(this MethodBase method)
        {
            return method.IsVirtual && !method.IsFinal && !method.IsPrivate;
        }

        public static bool CanOverrideGet(this PropertyInfo property)
        {
            return property.CanRead(out var getter) && getter.CanOverride();
        }

        public static bool CanOverrideSet(this PropertyInfo property)
        {
            return property.CanWrite(out var setter) && setter.CanOverride();
        }

        public static IEnumerable<MethodInfo> GetMethods(this Type type, string name)
        {
            return type.GetMember(name).OfType<MethodInfo>();
        }

        public static bool CompareTo<TTypes, TOtherTypes>(this TTypes types, TOtherTypes otherTypes, bool exact, bool considerTypeMatchers)
            where TTypes : IReadOnlyList<Type>
            where TOtherTypes : IReadOnlyList<Type>
        {
            var count = otherTypes.Count;

            if (types.Count != count)
            {
                return false;
            }

            for (int i = 0; i < count; ++i)
            {
                var t = types[i];

                if (considerTypeMatchers && t.IsOrContainsTypeMatcher())
                {
                    t = t.SubstituteTypeMatchers(otherTypes[i]);
                }

                if (exact)
                {
                    if (t.Equals(otherTypes[i]) == false)
                    {
                        return false;
                    }
                }
                else
                {
                    if (t.IsAssignableFrom(otherTypes[i]) == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static string GetParameterTypeList(this MethodInfo method)
        {
            return new StringBuilder().AppendCommaSeparated(method.GetParameters(), StringBuilderExtensions.AppendParameterType).ToString();
        }

        public static ParameterTypes GetParameterTypes(this MethodInfo method)
        {
            return new ParameterTypes(method.GetParameters());
        }

        public static bool CompareParameterTypesTo<TOtherTypes>(this Delegate function, TOtherTypes otherTypes)
            where TOtherTypes : IReadOnlyList<Type>
        {
            var method = function.GetMethodInfo();
            if (method.GetParameterTypes().CompareTo(otherTypes, exact: false, considerTypeMatchers: false))
            {
                // the backing method for the literal delegate is compatible, DynamicInvoke(...) will succeed
                return true;
            }

            // it's possible for the .Method property (backing method for a delegate) to have
            // differing parameter types than the actual delegate signature. This occurs in C# when
            // an instance delegate invocation is created for an extension method (bundled with a receiver)
            // or at times for DLR code generation paths because the CLR is optimized for instance methods.
            var invokeMethod = GetInvokeMethodFromUntypedDelegateCallback(function);
            if (invokeMethod != null && invokeMethod.GetParameterTypes().CompareTo(otherTypes, exact: false, considerTypeMatchers: false))
            {
                // the Invoke(...) method is compatible instead. DynamicInvoke(...) will succeed.
                return true;
            }

            // Neither the literal backing field of the delegate was compatible
            // nor the delegate invoke signature.
            return false;

            /* Unmerged change from project 'Moq(netstandard2.0)'
            Before:
                    private static MethodInfo GetInvokeMethodFromUntypedDelegateCallback(Delegate callback)
            After:
                    static MethodInfo GetInvokeMethodFromUntypedDelegateCallback(Delegate callback)
            */

            /* Unmerged change from project 'Moq(netstandard2.1)'
            Before:
                    private static MethodInfo GetInvokeMethodFromUntypedDelegateCallback(Delegate callback)
            After:
                    static MethodInfo GetInvokeMethodFromUntypedDelegateCallback(Delegate callback)
            */

            /* Unmerged change from project 'Moq(net6.0)'
            Before:
                    private static MethodInfo GetInvokeMethodFromUntypedDelegateCallback(Delegate callback)
            After:
                    static MethodInfo GetInvokeMethodFromUntypedDelegateCallback(Delegate callback)
            */
        }

        static MethodInfo GetInvokeMethodFromUntypedDelegateCallback(Delegate callback)
        {
            // Section 8.9.3 of 4th Ed ECMA 335 CLI spec requires delegates to have an 'Invoke' method.
            // However, there is not a requirement for 'public', or for it to be unambiguous.
            try
            {
                return callback.GetType().GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }
            catch (AmbiguousMatchException)
            {
                return null;
            }
        }

        /// <summary>
        ///   Visits all constituent parts of <paramref name="type"/>, replacing all type matchers
        ///   that match the type argument at the corresponding position in <paramref name="other"/>.
        /// </summary>
        /// <param name="type">The type to be matched. May be, or contain, type matchers.</param>
        /// <param name="other">The type argument to match against <paramref name="type"/>.</param>
        public static Type SubstituteTypeMatchers(this Type type, Type other)
        {
            // If a type matcher `T` successfully matches its corresponding type `O` from `other`, `T` in `type`
            // gets replaced by `O`. If all type matchers successfully matched, and they have been replaced by
            // their arguments in `other`, callers can then perform a final `IsAssignableFrom` check to match
            // everything else (fixed types). Being able to defer to `IsAssignableFrom` saves us from having to
            // re-implement all of its type equivalence rules (polymorphism, co-/contravariance, etc.).
            //
            // We still need to do some checks ourselves, however: In order to traverse both `type` and `other`
            // in lockstep, we need to ensure that they have the same basic structure.

            if (type.IsTypeMatcher(out var typeMatcherType))
            {
                var typeMatcher = (ITypeMatcher)Activator.CreateInstance(typeMatcherType);

                if (typeMatcher.Matches(other))
                {
                    return other;
                }
            }
            else if (type.HasElementType && other.HasElementType)
            {
                var te = type.GetElementType();
                var oe = other.GetElementType();

                if (type.IsArray && other.IsArray)
                {
                    var tr = type.GetArrayRank();
                    var or = other.GetArrayRank();

                    if (tr == or)
                    {
                        var se = te.SubstituteTypeMatchers(oe);
                        if (se.Equals(te))
                        {
                            return type;
                        }
                        else
                        {
                            return tr == 1 ? se.MakeArrayType() : se.MakeArrayType(tr);
                        }
                    }
                }
                else if (type.IsByRef && other.IsByRef)
                {
                    var se = te.SubstituteTypeMatchers(oe);
                    return se == te ? type : se.MakeByRefType();
                }
                else if (type.IsPointer && other.IsPointer)
                {
                    var se = te.SubstituteTypeMatchers(oe);
                    return se == te ? type : se.MakePointerType();
                }
            }
            else if (type.IsGenericType && other.IsGenericType)
            {
                var td = type.GetGenericTypeDefinition();
                var od = other.GetGenericTypeDefinition();

                if (td.Equals(od))
                {
                    var ta = type.GetGenericArguments();
                    var oa = other.GetGenericArguments();
                    var changed = false;

                    Debug.Assert(oa.Length == ta.Length);

                    for (int i = 0; i < ta.Length; ++i)
                    {
                        var sa = ta[i].SubstituteTypeMatchers(oa[i]);
                        if (sa.Equals(ta[i]) == false)
                        {
                            changed = true;
                            ta[i] = sa;
                        }
                    }

                    return changed ? td.MakeGenericType(ta) : type;
                }
            }

            return type;
        }

        static readonly ConcurrentDictionary<Tuple<Type, Type>, InterfaceMapping> mappingsCache = new();

        static InterfaceMapping GetInterfaceMap(Type type, Type interfaceType)
        {
            return mappingsCache.GetOrAdd(Tuple.Create(type, interfaceType), tuple => tuple.Item1.GetInterfaceMap(tuple.Item2));
        }

        public static IEnumerable<Mock> FindAllInnerMocks(this SetupCollection setups)
        {
            return setups.FindAll(setup => !setup.IsConditional)
                         .SelectMany(setup => setup.InnerMocks)
                         .Where(innerMock => innerMock != null);
        }

        public static Mock FindLastInnerMock(this SetupCollection setups, Func<Setup, bool> predicate)
        {
            return setups.FindLast(setup => !setup.IsConditional && predicate(setup))?.InnerMocks.SingleOrDefault();
        }
    }
}
