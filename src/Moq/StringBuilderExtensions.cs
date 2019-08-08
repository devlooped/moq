// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

using TypeNameFormatter;

namespace Moq
{
	internal static class StringBuilderExtensions
	{
		public static StringBuilder AppendIndented(this StringBuilder stringBuilder, string str, int count = 1, char indentChar = ' ')
		{
			var i = 0;
			while (i < str.Length)
			{
				stringBuilder.Append(indentChar, count);
				var j = str.IndexOf('\n', i + 1);
				if (j > i)
				{
					stringBuilder.Append(str, i, j - i + 1);
					i = j + 1;
				}
				else
				{
					break;
				}
			}
			stringBuilder.Append(str, i, str.Length - i);
			return stringBuilder;
		}

		public static StringBuilder AppendNameOf(this StringBuilder stringBuilder, MethodBase method, bool includeGenericArgumentList)
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

		public static StringBuilder AppendNameOfAddEvent(this StringBuilder stringBuilder, MethodBase method, bool includeGenericArgumentList)
		{
			return stringBuilder.Append(method.Name.Substring("add_".Length)).Append(" += ");
		}

		public static StringBuilder AppendNameOfRemoveEvent(this StringBuilder stringBuilder, MethodBase method, bool includeGenericArgumentList)
		{
			return stringBuilder.Append(method.Name.Substring("remove_".Length)).Append(" -= ");
		}

		public static StringBuilder AppendNameOf(this StringBuilder stringBuilder, Type type)
		{
			Debug.Assert(type != null);

			return stringBuilder.AppendFormattedName(type);
		}

		public static StringBuilder AppendParameterType(this StringBuilder stringBuilder, ParameterInfo parameter)
		{
			var parameterType = parameter.ParameterType;

			if (parameterType.IsByRef)
			{
				switch (parameter.Attributes & (ParameterAttributes.In | ParameterAttributes.Out))
				{
					case ParameterAttributes.In:
						stringBuilder.Append("in ");
						break;

					case ParameterAttributes.Out:
						stringBuilder.Append("out ");
						break;

					case ParameterAttributes.In | ParameterAttributes.Out:
					default:
						stringBuilder.Append("ref ");
						break;
				}

				parameterType = parameterType.GetElementType();
			}

			if (parameterType.IsArray && parameter.IsDefined(typeof(ParamArrayAttribute), true))
			{
				stringBuilder.Append("params ");
			}

			return stringBuilder.AppendFormattedName(parameterType);
		}

		public static StringBuilder AppendParameterTypeList(this StringBuilder stringBuilder, ParameterInfo[] parameters)
		{
			for (int i = 0; i < parameters.Length; ++i)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}

				stringBuilder.AppendParameterType(parameters[i]);
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
			else if (obj.GetType().IsEnum)
			{
				stringBuilder.AppendNameOf(obj.GetType()).Append('.').Append(obj);
			}
			else if (obj.GetType().IsArray || (obj.GetType().IsConstructedGenericType && obj.GetType().GetGenericTypeDefinition() == typeof(List<>)))
			{
				stringBuilder.Append('[');
				const int maxCount = 10;
				var enumerator = ((IEnumerable)obj).GetEnumerator();
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
			else
			{
				var formatted = obj.ToString();
				if (formatted == null || formatted == obj.GetType().ToString())
				{
					stringBuilder.AppendNameOf(obj.GetType());
				}
				else
				{
					stringBuilder.Append(formatted);
				}
			}

			return stringBuilder;
		}

		public static StringBuilder TrimEnd(this StringBuilder stringBuilder)
		{
			while (char.IsWhiteSpace(stringBuilder[stringBuilder.Length - 1]))
			{
				--stringBuilder.Length;
			}
			return stringBuilder;
		}
	}
}
