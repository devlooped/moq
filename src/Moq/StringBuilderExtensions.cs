// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Moq
{
	internal static class StringBuilderExtensions
	{
		public static StringBuilder AppendNameOf(this StringBuilder stringBuilder, MethodInfo method, bool includeGenericArgumentList)
		{
			stringBuilder.Append(method.Name);

			if (includeGenericArgumentList && method.IsGenericMethod)
			{
				stringBuilder.Append('<');
				var genericArguments = method.GetGenericArguments();
				for (int i = 0, n = genericArguments.Length; i < n; ++i)
				{
					if (i > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.AppendNameOf(genericArguments[i]);
				}

				stringBuilder.Append('>');
			}

			return stringBuilder;
		}

		public static StringBuilder AppendNameOf(this StringBuilder stringBuilder, Type type)
		{
			Debug.Assert(type != null);

			var name = type.Name;
			var backtickIndex = name.IndexOf('`');
			if (backtickIndex >= 0)
			{
				stringBuilder.Append(name, 0, backtickIndex);
			}
			else
			{
				stringBuilder.Append(name);
			}

			if (type.GetTypeInfo().IsGenericType)
			{
				var genericArguments = type.GetGenericArguments();
				stringBuilder.Append('<');
				for (int i = 0, n = genericArguments.Length; i < n; ++i)
				{
					if (i > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.AppendNameOf(genericArguments[i]);
				}
				stringBuilder.Append('>');
			}

			return stringBuilder;
		}

		public static StringBuilder AppendValueOf(this StringBuilder stringBuilder, object obj)
		{
			if (obj == null)
			{
				stringBuilder.Append("null");
			}
			else if (obj is string str)
			{
				stringBuilder.Append('"').Append(str).Append('"');
			}
			else if (obj is float f)
			{
				stringBuilder.Append(f.ToString("G9"));
			}
			else if (obj is double d)
			{
				stringBuilder.Append(d.ToString("G17"));
			}
			else if (obj is IEnumerable enumerable && !(obj is IMocked))
			{                                      // ^^^^^^^^^^^^^^^^^
			                                       // This second check ensures that we have a usable implementation of IEnumerable.
			                                       // If value is a mocked object, its IEnumerable implementation might very well
			                                       // not work correctly.
				stringBuilder.Append('[');
				const int maxCount = 10;
				var enumerator = enumerable.GetEnumerator();
				for (int i = 0; enumerator.MoveNext() && i < maxCount + 1; ++i)
				{
					if (i > 0)
					{
						stringBuilder.Append(", ");
					}

					if (i == maxCount)
					{
						stringBuilder.Append("...");
						break;
					}

					stringBuilder.AppendValueOf(enumerator.Current);
				}
				stringBuilder.Append(']');
			}
			else if (obj.ToString() == obj.GetType().ToString())
			{
				stringBuilder.AppendNameOf(obj.GetType());
			}
			else if (obj.GetType().GetTypeInfo().IsEnum)
			{
				stringBuilder.AppendNameOf(obj.GetType()).Append('.').Append(obj);
			}
			else
			{
				stringBuilder.Append(obj.ToString());
			}

			return stringBuilder;
		}
	}
}
