//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Moq.Proxy;
using System.Linq.Expressions;
using Moq.Properties;
using System.Runtime.ExceptionServices;

namespace Moq
{
	internal static class Extensions
	{
		public static string Format(this ICallContext invocation)
		{
			if (invocation.Method.IsPropertyGetter())
			{
				return invocation.Method.DeclaringType.Name + "." + invocation.Method.Name.Substring(4);
			}

			if (invocation.Method.IsPropertySetter())
			{
				return invocation.Method.DeclaringType.Name + "." +
					invocation.Method.Name.Substring(4) + " = " + GetValue(invocation.Arguments.First());
			}
			
			var genericParameters = invocation.Method.IsGenericMethod
				? "<" + string.Join(", ", invocation.Method.GetGenericArguments().Select(t => t.Name).ToArray()) + ">"
				: "";

			return invocation.Method.DeclaringType.Name + "." + invocation.Method.Name + genericParameters + "(" +
				string.Join(", ", invocation.Arguments.Select(a => GetValue(a)).ToArray()) + ")";
		}

		public static string GetValue(object value)
		{
			if (value == null)
			{
				return "null";
			}

			var typedValue = value as string;
			if (typedValue != null)
			{
				return "\"" + typedValue + "\"";
			}
			if (value is IEnumerable enumerable && enumerable.GetEnumerator() != null)
			{                                   // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
				// This second check ensures that we have a usable implementation of IEnumerable.
				// If value is a mocked object, its IEnumerable implementation might very well
				// not work correctly.
				const int maxCount = 10;
				var objs = enumerable.Cast<object>().Take(maxCount + 1);
				var more = objs.Count() > maxCount ? ", ..." : string.Empty;
				return "[" + string.Join(", ", objs.Take(maxCount).Select(GetValue)) + more + "]";
			}
			return value.ToString();
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

		/// <summary>
		/// Tests if a type is a delegate type (subclasses <see cref="Delegate" />).
		/// </summary>
		public static bool IsDelegate(this Type t)
		{
			return t.GetTypeInfo().IsSubclassOf(typeof(Delegate));
		}

		public static void ThrowIfNotMockeable(this Type typeToMock)
		{
			if (!IsMockeable(typeToMock))
				throw new NotSupportedException(Properties.Resources.InvalidMockClass);
		}

		public static void ThrowIfNotMockeable(this MemberExpression memberAccess)
		{
			if (memberAccess.Member is FieldInfo)
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.FieldsNotSupported,
					memberAccess.ToStringFixed()));
		}

		public static void ThrowIfNoGetter(this PropertyInfo property)
		{
			if (property.GetGetMethod(true) == null)
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.PropertyGetNotFound,
					property.DeclaringType.Name, property.Name));
		}

		public static void ThrowIfNoSetter(this PropertyInfo property)
		{
			if (property.GetSetMethod(true) == null)
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.PropertySetNotFound,
					property.DeclaringType.Name, property.Name));
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

		public static MemberInfoWithTarget<EventInfo, Mock> GetEvent<TMock>(this Action<TMock> eventExpression, TMock mock)
			where TMock : class
		{
			Guard.NotNull(eventExpression, nameof(eventExpression));

			MethodBase addRemove;
			Mock target;

			using (var context = new FluentMockContext())
			{
				eventExpression(mock);

				if (context.LastInvocation == null)
				{
					throw new ArgumentException(Resources.ExpressionIsNotEventAttachOrDetachOrIsNotVirtual);
				}

				addRemove = context.LastInvocation.Invocation.Method;
				target = context.LastInvocation.Mock;
			}

			var ev = addRemove.DeclaringType.GetEvent(
				addRemove.Name.Replace("add_", string.Empty).Replace("remove_", string.Empty));

			if (ev == null)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.EventNofFound,
					addRemove));
			}

			return new MemberInfoWithTarget<EventInfo, Mock>(ev, target);
		}

#if !NETCORE
		public static TAttribute GetCustomAttribute<TAttribute>(this ICustomAttributeProvider source, bool inherit)
			where TAttribute : Attribute
		{
			object[] attrs = source.GetCustomAttributes(typeof(TAttribute), inherit);

			if (attrs.Length == 0)
			{
				return default(TAttribute);
			}
			else
			{
				return (TAttribute)attrs[0];
			}
		}
#endif

		public static bool HasCompatibleParameterTypes(this MethodInfo method, Type[] paramTypes, bool exactParameterMatch)
		{
			var types = method.GetParameterTypes().ToArray();
			if (types.Length != paramTypes.Length)
			{
				return false;
			}

			for (int i = 0; i < types.Length; i++)
			{
				var parameterType = paramTypes[i];
				if (parameterType == typeof(object))
				{
					continue;
				}
				else if (exactParameterMatch && types[i] != parameterType)
				{
					return false;
				}
				else if (!types[i].IsAssignableFrom(parameterType))
				{
					return false;
				}
			}

			return true;
		}

		public static bool HasCompatibleParameterList(this Delegate function, ParameterInfo[] expectedParams)
		{
			var method = function.GetMethodInfo();
			if (HasCompatibleParameterList(expectedParams, method))
			{
				// the backing method for the literal delegate is compatible, DynamicInvoke(...) will succeed
				return true;
			}

			// it's possible for the .Method property (backing method for a delegate) to have
			// differing parameter types than the actual delegate signature. This occurs in C# when
			// an instance delegate invocation is created for an extension method (bundled with a receiver)
			// or at times for DLR code generation paths because the CLR is optimized for instance methods.
			var invokeMethod = GetInvokeMethodFromUntypedDelegateCallback(function);
			if (invokeMethod != null && HasCompatibleParameterList(expectedParams, invokeMethod))
			{
				// the Invoke(...) method is compatible instead. DynamicInvoke(...) will succeed.
				return true;
			}

			// Neither the literal backing field of the delegate was compatible
			// nor the delegate invoke signature.
			return false;
		}

		private static bool HasCompatibleParameterList(ParameterInfo[] expectedParams, MethodInfo method)
		{
			var actualParams = method.GetParameters();
			if (expectedParams.Length != actualParams.Length)
			{
				return false;
			}

			for (int i = 0; i < expectedParams.Length; i++)
			{
				if (!actualParams[i].ParameterType.IsAssignableFrom(expectedParams[i].ParameterType))
				{
					return false;
				}
			}

			return true;
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

		public static bool IsExtensionMethod(this MethodInfo method)
		{
			return method.IsStatic && method.IsDefined(typeof(ExtensionAttribute));
			// The above check is perhaps "good enough for now", but admittedly incomplete:
			// We should also check whether the method is defined in a non-nested static
			// class, and whether it has at least one parameter.
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
}
