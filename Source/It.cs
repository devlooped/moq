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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;
using Moq.Matchers;
#if FEATURE_COM
using static System.Runtime.InteropServices.Marshal;
#endif

namespace Moq
{
	/// <include file='It.xdoc' path='docs/doc[@for="It"]/*'/>
	public static class It
	{
		/// <include file='It.xdoc' path='docs/doc[@for="It.IsAny"]/*'/>
		public static TValue IsAny<TValue>()
		{
			return Match<TValue>.Create(
#if FEATURE_COM
				value => value == null || (IsComObject(value) ? value is TValue
				                                              : typeof(TValue).IsAssignableFrom(value.GetType()) || typeof(TValue).IsAnyType()),
#else
				value => value == null || typeof(TValue).IsAssignableFrom(value.GetType()) || typeof(TValue).IsAnyType(),
#endif
				() => It.IsAny<TValue>());
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsNotNull"]/*'/>
		public static TValue IsNotNull<TValue>()
		{
			return Match<TValue>.Create(
#if FEATURE_COM
				value => value != null && (IsComObject(value) ? value is TValue
				                                              : typeof(TValue).IsAssignableFrom(value.GetType()) || typeof(TValue).IsAnyType()),
#else
				value => value != null && typeof(TValue).IsAssignableFrom(value.GetType()) || typeof(TValue).IsAnyType(),
#endif
				() => It.IsNotNull<TValue>());
		}


		/// <include file='It.xdoc' path='docs/doc[@for="It.Is"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static TValue Is<TValue>(Expression<Func<TValue, bool>> match)
		{
			return Match<TValue>.Create(
				value => match.Compile().Invoke(value),
				() => It.Is<TValue>(match));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsInRange"]/*'/>
		public static TValue IsInRange<TValue>(TValue from, TValue to, Range rangeKind)
			where TValue : IComparable
		{
			return Match<TValue>.Create(value =>
			{
				if (value == null)
				{
					return false;
				}

				if (rangeKind == Range.Exclusive)
				{
					return value.CompareTo(@from) > 0 && value.CompareTo(to) < 0;
				}

				return value.CompareTo(@from) >= 0 && value.CompareTo(to) <= 0;
			},
			() => It.IsInRange(@from, to, rangeKind));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsIn(enumerable)"]/*'/>
		public static TValue IsIn<TValue>(IEnumerable<TValue> items)
		{
			return Match<TValue>.Create(value => items.Contains(value), () => It.IsIn(items));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsIn(params)"]/*'/>
		public static TValue IsIn<TValue>(params TValue[] items)
		{
			return Match<TValue>.Create(value => items.Contains(value), () => It.IsIn(items));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsNotIn(enumerable)"]/*'/>
		public static TValue IsNotIn<TValue>(IEnumerable<TValue> items)
		{
			return Match<TValue>.Create(value => !items.Contains(value), () => It.IsNotIn(items));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsNotIn(params)"]/*'/>
		public static TValue IsNotIn<TValue>(params TValue[] items)
		{
			return Match<TValue>.Create(value => !items.Contains(value), () => It.IsNotIn(items));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsRegex(regex)"]/*'/>
		public static string IsRegex(string regex)
		{
			// The regex is constructed only once.
			var re = new Regex(regex);

			// But evaluated every time :)
			return Match<string>.Create(value => re.IsMatch(value), () => It.IsRegex(regex));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsRegex(regex,options)"]/*'/>
		public static string IsRegex(string regex, RegexOptions options)
		{
			// The regex is constructed only once.
			var re = new Regex(regex, options);

			// But evaluated every time :)
			return Match<string>.Create(value => re.IsMatch(value), () => It.IsRegex(regex, options));
		}

		/// <summary>
		/// Allows for setting up open generic methods on Mock Fixtures
		/// </summary>
		public interface AnyType
		{
		}

		/// <summary>
		/// Allows for setting up open generic methods which have where restrictions on Mock Fixtures
		/// </summary>
		/// <typeparam name="T">Base class/interface that should be inherited from</typeparam>
		/// <returns>null</returns>
		[AdvancedMatcher(typeof(AnySubTypeMatcher))]
		public static T IsAnySubTypeOf<T>() where T : class
		{
			return new AnySubTypeOf<T>();
		}

		internal interface IAnySubTypeOf
		{
			Type SubType { get; }
		}

		internal class AnySubTypeOf<T> : AnyType, IAnySubTypeOf where T : class
		{
			public static implicit operator T(AnySubTypeOf<T> t)
			{
				return null;
			}

			public Type SubType => typeof(T);
		}

		internal class AnyTypeImplementation : AnyType
		{
			public object Object { get; set; }
		}
	}

	internal static class IsAnyTypeExtensions
	{
		public static bool IsAnyType(this Type type)
		{
			return typeof(It.AnyType).IsAssignableFrom(type);
		}

		public static bool IsAnyType<T>(this T type)
		{
			return typeof(It.AnyType).IsAssignableFrom(typeof(T));
		}
	}
}
