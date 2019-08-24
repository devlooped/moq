// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;

using Moq.Properties;

namespace Moq
{
	internal static class Extensions
	{
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

				var map = proxyType.GetInterfaceMap(method.DeclaringType);
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

		public static object InvokePreserveStack(this Delegate del, params object[] args)
		{
			try
			{
				return del.DynamicInvoke(args);
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

		public static bool CanOverride(this MethodBase method)
		{
			return method.IsVirtual && !method.IsFinal && !method.IsPrivate;
		}

		public static bool CanOverrideGet(this PropertyInfo property)
		{
			if (property.CanRead)
			{
				var getter = property.GetGetMethod(true);
				return getter != null && getter.CanOverride();
			}

			return false;
		}

		public static bool CanOverrideSet(this PropertyInfo property)
		{
			if (property.CanWrite)
			{
				var setter = property.GetSetMethod(true);
				return setter != null && setter.CanOverride();
			}

			return false;
		}

		public static IEnumerable<MethodInfo> GetMethods(this Type type, string name)
		{
			return type.GetMember(name).OfType<MethodInfo>();
		}

		public static bool CompareTo<TTypes, TOtherTypes>(this TTypes types, TOtherTypes otherTypes, bool exact)
			where TTypes : IReadOnlyList<Type>
			where TOtherTypes : IReadOnlyList<Type>
		{
			var count = otherTypes.Count;

			if (types.Count != count)
			{
				return false;
			}

			if (exact)
			{
				for (int i = 0; i < count; ++i)
				{
					if (types[i] != otherTypes[i])
					{
						return false;
					}
				}
			}
			else
			{
				for (int i = 0; i < count; ++i)
				{
					if (types[i].IsAssignableFrom(otherTypes[i]) == false)
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
			if (method.GetParameterTypes().CompareTo(otherTypes, exact: false))
			{
				// the backing method for the literal delegate is compatible, DynamicInvoke(...) will succeed
				return true;
			}

			// it's possible for the .Method property (backing method for a delegate) to have
			// differing parameter types than the actual delegate signature. This occurs in C# when
			// an instance delegate invocation is created for an extension method (bundled with a receiver)
			// or at times for DLR code generation paths because the CLR is optimized for instance methods.
			var invokeMethod = GetInvokeMethodFromUntypedDelegateCallback(function);
			if (invokeMethod != null && invokeMethod.GetParameterTypes().CompareTo(otherTypes, exact: false))
			{
				// the Invoke(...) method is compatible instead. DynamicInvoke(...) will succeed.
				return true;
			}

			// Neither the literal backing field of the delegate was compatible
			// nor the delegate invoke signature.
			return false;
		}

		private static MethodInfo GetInvokeMethodFromUntypedDelegateCallback(Delegate callback)
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

		public static bool TryFind(this IEnumerable<Setup> innerMockSetups, InvocationShape expectation, out Setup setup)
		{
			Debug.Assert(innerMockSetups.All(s => s.ReturnsInnerMock(out _)));

			foreach (Setup innerMockSetup in innerMockSetups)
			{
				if (innerMockSetup.Expectation.Equals(expectation))
				{
					setup = innerMockSetup;
					return true;
				}
			}

			setup = default;
			return false;
		}

		public static bool TryFind(this IEnumerable<Setup> innerMockSetups, Invocation invocation, out Setup setup)
		{
			foreach (Setup innerMockSetup in innerMockSetups)
			{
				if (innerMockSetup.Matches(invocation))
				{
					setup = innerMockSetup;
					return true;
				}
			}

			setup = default;
			return false;
		}
	}
}
