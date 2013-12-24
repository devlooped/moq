using System;
using System.Collections.Generic;
using System.Reflection;

namespace Moq.Proxy.Factory
{
	internal class MethodInfoEqualityComparer : IEqualityComparer<MethodInfo>
	{
		public bool Equals(MethodInfo x, MethodInfo y)
		{
			return (x == null && y == null) || (x != null && y != null &&
				string.Equals(x.Name, y.Name) && EqualGenericParameters(x, y) &&
				EqualType(x.ReturnType, y.ReturnType) && EqualTypes(x.GetParameters(), y.GetParameters(), p => p.ParameterType));
		}

		public int GetHashCode(MethodInfo obj)
		{
			return obj.GetHashCode();
		}

		private static bool EqualGenericParameters(MethodInfo x, MethodInfo y)
		{
			if (x.IsGenericMethod && y.IsGenericMethod)
			{
				return EqualTypes(x.GetGenericArguments(), y.GetGenericArguments(), t => t);
			}

			return x.IsGenericMethod == y.IsGenericMethod;
		}

		private static bool EqualType(Type x, Type y)
		{
			return x == y || (string.Equals(x.FullName, y.FullName) && string.Equals(x.Name, y.Name));
		}

		private static bool EqualTypes<T>(T[] x, T[] y, Func<T, Type> tt)
		{
			if (x.Length == y.Length)
			{
				for (int index = 0; index < x.Length; index++)
				{
					if (!EqualType(tt(x[index]), tt(y[index])))
					{
						return false;
					}
				}

				return true;
			}

			return false;
		}
	}
}