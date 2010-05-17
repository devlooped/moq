//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
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
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using Moq.Properties;

namespace Moq
{
	[DebuggerStepThrough]
	internal static class Guard
	{
		/// <summary>
		/// Ensures the given <paramref name="value"/> is not null.
		/// Throws <see cref="ArgumentNullException"/> otherwise.
		/// </summary>
		public static void NotNull<T>(Expression<Func<T>> reference, T value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(GetParameterName(reference));
			}
		}

		/// <summary>
		/// Ensures the given string <paramref name="value"/> is not null or empty.
		/// Throws <see cref="ArgumentNullException"/> in the first case, or 
		/// <see cref="ArgumentException"/> in the latter.
		/// </summary>
		public static void NotNullOrEmpty(Expression<Func<string>> reference, string value)
		{
			NotNull<string>(reference, value);
			if (value.Length == 0)
			{
				throw new ArgumentException(Resources.ArgumentCannotBeEmpty, GetParameterName(reference));
			}
		}

		/// <summary>
		/// Checks an argument to ensure it is in the specified range including the edges.
		/// </summary>
		/// <typeparam name="T">Type of the argument to check, it must be an <see cref="IComparable"/> type.
		/// </typeparam>
		/// <param name="reference">The expression containing the name of the argument.</param>
		/// <param name="value">The argument value to check.</param>
		/// <param name="from">The minimun allowed value for the argument.</param>
		/// <param name="to">The maximun allowed value for the argument.</param>
		public static void NotOutOfRangeInclusive<T>(Expression<Func<T>> reference, T value, T from, T to)
				where T : IComparable
		{
			if (value != null && (value.CompareTo(from) < 0 || value.CompareTo(to) > 0))
			{
				throw new ArgumentOutOfRangeException(GetParameterName(reference));
			}
		}

		/// <summary>
		/// Checks an argument to ensure it is in the specified range excluding the edges.
		/// </summary>
		/// <typeparam name="T">Type of the argument to check, it must be an <see cref="IComparable"/> type.
		/// </typeparam>
		/// <param name="reference">The expression containing the name of the argument.</param>
		/// <param name="value">The argument value to check.</param>
		/// <param name="from">The minimun allowed value for the argument.</param>
		/// <param name="to">The maximun allowed value for the argument.</param>
		public static void NotOutOfRangeExclusive<T>(Expression<Func<T>> reference, T value, T from, T to)
				where T : IComparable
		{
			if (value != null && (value.CompareTo(from) <= 0 || value.CompareTo(to) >= 0))
			{
				throw new ArgumentOutOfRangeException(GetParameterName(reference));
			}
		}

		public static void CanBeAssigned(Expression<Func<object>> reference, Type typeToAssign, Type targetType)
		{
			if (!targetType.IsAssignableFrom(typeToAssign))
			{
				if (targetType.IsInterface)
				{
					throw new ArgumentException(string.Format(
						CultureInfo.CurrentCulture,
						Resources.TypeNotImplementInterface,
						typeToAssign,
						targetType), GetParameterName(reference));
				}

				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.TypeNotInheritFromType,
					typeToAssign,
					targetType), GetParameterName(reference));
			}
		}

		private static string GetParameterName(LambdaExpression reference)
		{
			var member = (MemberExpression)reference.Body;
			return member.Member.Name;
		}
	}
}