using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Moq
{
	internal static class MemberInfoExtensions
	{
		public static string GetFullName(this MethodBase method)
		{
			if (method.IsGenericMethod)
			{
				return method.Name + GetGenericArguments(method.GetGenericArguments(), t => GetFullName(t));
			}

			return method.Name;
		}

		public static string GetFullName(this Type type)
		{
			if (type.IsGenericType)
			{
				return type.FullName.Substring(0, type.FullName.IndexOf('`')) +
					GetGenericArguments(type.GetGenericArguments(), t => GetFullName(t));
			}

			return type.FullName;
		}

		public static string GetName(this MethodBase method)
		{
			if (method.IsGenericMethod)
			{
				return method.Name + GetGenericArguments(method.GetGenericArguments(), t => GetName(t));
			}

			return method.Name;
		}

		public static string GetName(this Type type)
		{
			if (type.IsGenericType)
			{
				return type.Name.Substring(0, type.Name.IndexOf('`')) +
					GetGenericArguments(type.GetGenericArguments(), t => GetName(t));
			}

			return type.Name;
		}

		public static IEnumerable<Type> GetParameterTypes(this MethodBase method)
		{
			return method.GetParameters().Select(parameter => parameter.ParameterType);
		}

		public static bool IsDestructor(this MethodInfo method)
		{
			return method.Name == "Finalize" && !method.IsGenericMethod &&
				method.ReturnType == typeof(void) && method.GetParameters().Length == 0;
		}


		public static bool IsEventAttach(this MethodBase method)
		{
			return method.Name.StartsWith("add_", StringComparison.Ordinal);
		}

		public static bool IsEventDetach(this MethodBase method)
		{
			return method.Name.StartsWith("remove_", StringComparison.Ordinal);
		}

		public static bool IsPropertyGetter(this MethodBase method)
		{
			return method.Name.StartsWith("get_", StringComparison.Ordinal);
		}

		public static bool IsPropertyIndexerGetter(this MethodBase method)
		{
			return method.Name.StartsWith("get_Item", StringComparison.Ordinal);
		}

		public static bool IsPropertyIndexerSetter(this MethodBase method)
		{
			return method.Name.StartsWith("set_Item", StringComparison.Ordinal);
		}

		public static bool IsPropertySetter(this MethodBase method)
		{
			return method.Name.StartsWith("set_", StringComparison.Ordinal);
		}

		public static bool IsRefArgument(this ParameterInfo parameter)
		{
			return parameter.ParameterType.IsByRef && (parameter.Attributes & ParameterAttributes.Out) == ParameterAttributes.None;
		}

		public static bool IsOutArgument(this ParameterInfo parameter)
		{
			return parameter.ParameterType.IsByRef && (parameter.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out;
		}

		private static string GetGenericArguments(IEnumerable<Type> types, Func<Type, string> typeGetter)
		{
			return "<" + string.Join(",", types.Select(typeGetter).ToArray()) + ">";
		}
	}
}