using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Moq.Visualizer
{
	internal static class MemberExtensions
	{
		public static string GetName(this MethodBase method)
		{
			if (method.IsGenericMethod)
			{
				return method.Name + GetGenericArguments(method.GetGenericArguments(), t => GetName(t));
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

		public static string GetName(this Type type)
		{
			if (type.IsGenericType)
			{
				return type.Name.Substring(0, type.Name.IndexOf('`')) +
					GetGenericArguments(type.GetGenericArguments(), t => GetName(t));
			}

			return type.Name;
		}

		private static string GetGenericArguments(IEnumerable<Type> types, Func<Type, string> typeGetter)
		{
			return "<" + string.Join(",", types.Select(typeGetter).ToArray()) + ">";
		}
	}
}