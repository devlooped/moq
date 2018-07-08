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
