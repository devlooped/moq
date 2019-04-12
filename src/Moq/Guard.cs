// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Properties;

using TypeNameFormatter;

namespace Moq
{
	[DebuggerStepThrough]
	internal static class Guard
	{
		public static void IsAssignmentToPropertyOrIndexer(LambdaExpression expression, string paramName)
		{
			Debug.Assert(expression != null);

			switch (expression.Body.NodeType)
			{
				case ExpressionType.Assign:
					var assignment = (BinaryExpression)expression.Body;
					if (assignment.Left is MemberExpression || assignment.Left is IndexExpression) return;
					break;

				case ExpressionType.Call:
					var call = (MethodCallExpression)expression.Body;
					if (call.Method.IsPropertySetter() || call.Method.IsPropertyIndexerSetter()) return;
					break;
			}

			throw new ArgumentException(
				string.Format(
					CultureInfo.CurrentCulture,
					Resources.SetupNotProperty,
					expression.ToStringFixed()),
				paramName);
		}

		public static void IsOverridable(MethodInfo method, Expression expression)
		{
			if (method.IsStatic)
			{
				throw new NotSupportedException(
					string.Format(
						CultureInfo.CurrentCulture,
						Resources.UnsupportedExpressionWithHint,
						expression.ToStringFixed(),
						string.Format(
							CultureInfo.CurrentCulture,
							method.IsExtensionMethod() ? Resources.UnsupportedExtensionMethod : Resources.UnsupportedStaticMember,
							$"{method.DeclaringType.GetFormattedName()}.{method.Name}")));
			}
			else if (!method.CanOverride())
			{
				throw new NotSupportedException(
					string.Format(
						CultureInfo.CurrentCulture,
						Resources.UnsupportedExpressionWithHint,
						expression.ToStringFixed(),
						string.Format(
							CultureInfo.CurrentCulture,
							Resources.UnsupportedNonOverridableMember,
							$"{method.DeclaringType.GetFormattedName()}.{method.Name}")));
			}
		}

		public static void IsVisibleToProxyFactory(MethodInfo method)
		{
			if (ProxyFactory.Instance.IsMethodVisible(method, out string messageIfNotVisible) == false)
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.MethodNotVisibleToProxyFactory,
					method.DeclaringType.Name,
					method.Name,
					messageIfNotVisible));
			}
		}


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

		public static void CanBeAssigned(Type typeToAssign, Type targetType, string paramName)
		{
			if (!targetType.IsAssignableFrom(typeToAssign))
			{
				if (targetType.IsInterface)
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
