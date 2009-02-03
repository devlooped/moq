//Copyright (c) 2007, Moq Team 
//http://code.google.com/p/moq/
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

//    * Neither the name of the Moq Team nor the 
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core.Interceptor;

namespace Moq
{
	internal static class Extensions
	{
		static readonly FieldInfo remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString",
										 BindingFlags.Instance | BindingFlags.NonPublic);

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

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification="The linq expression is way more readable this way.")]
		public static string Format(this IInvocation invocation)
		{
			// Special-case for getters && setters
			if (invocation.Method.IsSpecialName)
			{
				if (invocation.Method.Name.StartsWith("get_", StringComparison.Ordinal))
					return
						invocation.Method.DeclaringType.Name + "." +
						invocation.Method.Name.Substring(4);
				else if (invocation.Method.Name.StartsWith("set_", StringComparison.Ordinal))
					return
						invocation.Method.DeclaringType.Name + "." +
						invocation.Method.Name.Substring(4) + " = " +
						(from x in invocation.Arguments
						 select x == null ?
								"null" :
								x is string ?
									"\"" + (string)x + "\"" :
									x.ToString()).First();
			}

			return
				invocation.Method.DeclaringType.Name + "." +
				invocation.Method.Name + "(" +
				String.Join(", ",
					(from x in invocation.Arguments
					 select x == null ?
						"null" :
						x is string ?
							"\"" + (string)x + "\"" :
							x.ToString())
					.ToArray()
				) + ")";
		}

		public static object InvokePreserveStack(this Delegate del, params object[] args)
		{
			try
			{
				return del.DynamicInvoke(args);
			}
			catch (TargetInvocationException ex)
			{
				ex.InnerException.SetStackTrace(ex.InnerException.StackTrace);
				throw ex.InnerException;
			}
		}

		public static void SetStackTrace(this Exception exception, string stackTrace)
		{
			remoteStackTraceString.SetValue(exception, stackTrace);
		}

		public static bool IsMockeable(this Type typeToMock)
		{
			return 
				typeToMock.IsInterface ||
				typeToMock.IsAbstract ||
				!typeToMock.IsSealed;
		}

		public static bool CanOverrideGet(this PropertyInfo property)
		{
			if (property.CanRead)
			{
				var getter = property.GetGetMethod();
				return getter != null && getter.IsVirtual && !getter.IsFinal;
			}

			return false;
		}

		public static bool CanOverrideSet(this PropertyInfo property)
		{
			if (property.CanWrite)
			{
				var setter = property.GetSetMethod();
				return setter != null && setter.IsVirtual && !setter.IsFinal;
			}

			return false;
		}
	}
}
