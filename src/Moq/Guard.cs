// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using Moq.Properties;
using System.Reflection;

namespace Moq
{
	[DebuggerStepThrough]
	internal static class Guard
	{
		/// <summary>
		/// Ensures the given <paramref name="value"/> is not null.
		/// Throws <see cref="ArgumentNullException"/> otherwise.
		/// </summary>
		public static void NotNull(object value, string paramName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(paramName);
			}
		}

		/// <summary>
		/// Ensures the given string <paramref name="value"/> is not null or empty.
		/// Throws <see cref="ArgumentNullException"/> in the first case, or 
		/// <see cref="ArgumentException"/> in the latter.
		/// </summary>
		public static void NotNullOrEmpty(string value, string paramName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(paramName);
			}

			if (value.Length == 0)
			{
				throw new ArgumentException(Resources.ArgumentCannotBeEmpty, paramName);
			}
		}

		/// <summary>
		/// Checks an argument to ensure it is in the specified range including the edges.
		/// </summary>
		/// <typeparam name="T">Type of the argument to check, it must be an <see cref="IComparable"/> type.
		/// </typeparam>
		/// <param name="value">The argument value to check.</param>
		/// <param name="from">The minimum allowed value for the argument.</param>
		/// <param name="to">The maximum allowed value for the argument.</param>
		/// <param name="paramName">The name of the parameter.</param>
		public static void NotOutOfRangeInclusive<T>(T value, T from, T to, string paramName)
				where T : IComparable
		{
			if (value != null && (value.CompareTo(from) < 0 || value.CompareTo(to) > 0))
			{
				throw new ArgumentOutOfRangeException(paramName);
			}
		}

		/// <summary>
		/// Checks an argument to ensure it is in the specified range excluding the edges.
		/// </summary>
		/// <typeparam name="T">Type of the argument to check, it must be an <see cref="IComparable"/> type.
		/// </typeparam>
		/// <param name="value">The argument value to check.</param>
		/// <param name="from">The minimum allowed value for the argument.</param>
		/// <param name="to">The maximum allowed value for the argument.</param>
		/// <param name="paramName">The name of the parameter.</param>
		public static void NotOutOfRangeExclusive<T>(T value, T from, T to, string paramName)
				where T : IComparable
		{
			if (value != null && (value.CompareTo(from) <= 0 || value.CompareTo(to) >= 0))
			{
				throw new ArgumentOutOfRangeException(paramName);
			}
		}

		public static void CanBeAssigned(Type typeToAssign, Type targetType, string paramName)
		{
			if (!targetType.IsAssignableFrom(typeToAssign))
			{
				if (targetType.GetTypeInfo().IsInterface)
				{
					throw new ArgumentException(string.Format(
						CultureInfo.CurrentCulture,
						Resources.TypeNotImplementInterface,
						typeToAssign,
						targetType), paramName);
				}

				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.TypeNotInheritFromType,
					typeToAssign,
					targetType), paramName);
			}
		}

		public static void Mockable(Type type)
		{
			if (!type.IsMockeable())
			{
				throw new NotSupportedException(Resources.InvalidMockClass);
			}
		}

		public static void Positive(TimeSpan delay)
		{
			if (delay <= TimeSpan.Zero)
			{
				throw new ArgumentException(Resources.DelaysMustBeGreaterThanZero);
			}
		}

		public static void CanRead(PropertyInfo property)
		{
			if (!property.CanRead)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.PropertyGetNotFound,
					property.DeclaringType.Name, property.Name));
			}
		}

		public static void CanWrite(PropertyInfo property)
		{
			if (!property.CanWrite)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.PropertySetNotFound,
					property.DeclaringType.Name, property.Name));
			}
		}
	}
}
