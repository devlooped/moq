// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

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
			return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
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

		public static bool IsPropertyGetter(this MethodInfo method)
		{
			return method.IsSpecialName && method.Name.StartsWith("get_", StringComparison.Ordinal);
		}

		public static bool IsPropertyIndexerGetter(this MethodInfo method)
		{
			return method.IsSpecialName && method.Name.StartsWith("get_Item", StringComparison.Ordinal);
		}

		public static bool IsPropertyIndexerSetter(this MethodInfo method)
		{
			return method.IsSpecialName && method.Name.StartsWith("set_Item", StringComparison.Ordinal);
		}

		public static bool IsPropertySetter(this MethodInfo method)
		{
			return method.IsSpecialName && method.Name.StartsWith("set_", StringComparison.Ordinal);
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

		public static bool LooksLikeEventAttach(this MethodInfo method)
		{
			return method.Name.StartsWith("add_", StringComparison.Ordinal);
		}

		public static bool LooksLikeEventDetach(this MethodInfo method)
		{
			return method.Name.StartsWith("remove_", StringComparison.Ordinal);
		}

		/// <summary>
		/// Tests if a type is a delegate type (subclasses <see cref="Delegate" />).
		/// </summary>
		public static bool IsDelegate(this Type t)
		{
			return t.GetTypeInfo().IsSubclassOf(typeof(Delegate));
		}

		public static void ThrowIfNotMockeable(this MemberExpression memberAccess)
		{
			if (memberAccess.Member is FieldInfo)
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.FieldsNotSupported,
					memberAccess.ToStringFixed()));
		}

		public static bool IsMockeable(this Type typeToMock)
		{
			// A value type does not match any of these three 
			// condition and therefore returns false.
			return typeToMock.GetTypeInfo().IsInterface || typeToMock.GetTypeInfo().IsAbstract || typeToMock.IsDelegate() || (typeToMock.GetTypeInfo().IsClass && !typeToMock.GetTypeInfo().IsSealed);
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

		public static EventWithTarget GetEventWithTarget<TMock>(this Action<TMock> eventExpression, TMock mock)
			where TMock : class
		{
			Guard.NotNull(eventExpression, nameof(eventExpression));

			MethodBase addRemove;
			Mock target;

			using (var observer = AmbientObserver.Activate())
			{
				eventExpression(mock);

				if (!observer.LastIsInvocation(out target, out var invocation, out _))
				{
					throw new ArgumentException(Resources.ExpressionIsNotEventAttachOrDetachOrIsNotVirtual);
				}

				addRemove = invocation.Method;
			}

			var ev = addRemove.DeclaringType.GetEvent(
				addRemove.Name.Replace("add_", string.Empty).Replace("remove_", string.Empty));

			if (ev == null)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.EventNotFound,
					addRemove));
			}

			return new EventWithTarget(ev, target);
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

		/// <summary>
		/// Gets all properties of the specified type in depth-first order.
		/// That is, properties of the furthest ancestors are returned first,
		/// and the type's own properties are returned last.
		/// </summary>
		/// <param name="type">The type whose properties are to be returned.</param>
		internal static List<PropertyInfo> GetAllPropertiesInDepthFirstOrder(this Type type)
		{
			var properties = new List<PropertyInfo>();
			var none = new HashSet<Type>();

			type.AddPropertiesInDepthFirstOrderTo(properties, typesAlreadyVisited: none);

			return properties;
		}

		/// <summary>
		/// This is a helper method supporting <see cref="GetAllPropertiesInDepthFirstOrder(Type)"/>
		/// and is not supposed to be called directly.
		/// </summary>
		private static void AddPropertiesInDepthFirstOrderTo(this Type type, List<PropertyInfo> properties, HashSet<Type> typesAlreadyVisited)
		{
			if (!typesAlreadyVisited.Contains(type))
			{
				// make sure we do not process properties of the current type twice:
				typesAlreadyVisited.Add(type);

				//// follow down axis 1: add properties of base class. note that this is currently
				//// disabled, since it wasn't done previously and this can only result in changed
				//// behavior.
				//if (type.GetTypeInfo().BaseType != null)
				//{
				//	type.GetTypeInfo().BaseType.AddPropertiesInDepthFirstOrderTo(properties, typesAlreadyVisited);
				//}

				// follow down axis 2: add properties of inherited / implemented interfaces:
				var superInterfaceTypes = type.GetInterfaces();
				foreach (var superInterfaceType in superInterfaceTypes)
				{
					superInterfaceType.AddPropertiesInDepthFirstOrderTo(properties, typesAlreadyVisited);
				}

				// add own properties:
				foreach (var property in type.GetProperties())
				{
					properties.Add(property);
				}
			}
		}
	}

	internal readonly struct EventWithTarget
	{
		private readonly EventInfo @event;
		private readonly Mock target;

		public EventWithTarget(EventInfo @event, Mock target)
		{
			this.@event = @event;
			this.target = target;
		}

		public void Deconstruct(out EventInfo @event, out Mock target)
		{
			@event = this.@event;
			target = this.target;
		}
	}
}
