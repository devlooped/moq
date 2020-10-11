// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
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
	internal static partial class StringBuilderExtensions
	{
		public static StringBuilder Append(this StringBuilder stringBuilder, string str, int startIndex)
		{
			return stringBuilder.Append(str, startIndex, str.Length - startIndex);
		}

		public static StringBuilder AppendCommaSeparated<T>(this StringBuilder stringBuilder, string prefix, IEnumerable<T> source, Func<StringBuilder, T, StringBuilder> append, string suffix)
		{
			return stringBuilder.Append(prefix)
			                    .AppendCommaSeparated(source, append)
			                    .Append(suffix);
		}

		public static StringBuilder AppendCommaSeparated<T>(this StringBuilder stringBuilder, IEnumerable<T> source, Func<StringBuilder, T, StringBuilder> append)
		{
			bool appendComma = false;
			foreach (var item in source)
			{
				if (appendComma)
				{
					stringBuilder.Append(", ");
				}
				append(stringBuilder, item);
				appendComma = true;
			}

			return stringBuilder;
		}

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
				stringBuilder.AppendCommaSeparated("<", method.GetGenericArguments(), AppendNameOf, ">");
			}

			return stringBuilder;
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
				stringBuilder.Append((parameter.Attributes & (ParameterAttributes.In | ParameterAttributes.Out)) switch
				{
					ParameterAttributes.In  => "in ",
					ParameterAttributes.Out => "out ",
					_                       => "ref ",
				});

				parameterType = parameterType.GetElementType();
			}

			if (parameterType.IsArray && parameter.IsDefined(typeof(ParamArrayAttribute), true))
			{
				stringBuilder.Append("params ");
			}

			return stringBuilder.AppendFormattedName(parameterType);
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
