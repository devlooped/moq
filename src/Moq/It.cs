// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
#if FEATURE_COM
using static System.Runtime.InteropServices.Marshal;
#endif

namespace Moq
{
	/// <include file='It.xdoc' path='docs/doc[@for="It"]/*'/>
	public static class It
	{
		/// <summary>
		/// Contains matchers for <see langword="ref"/> (C#) / <see langword="ByRef"/> (VB.NET) parameters of type <typeparamref name="TValue"/>.
		/// </summary>
		/// <typeparam name="TValue">The parameter type.</typeparam>
		public static class Ref<TValue>
		{
			/// <summary>
			/// Matches any value that is assignment-compatible with type <typeparamref name="TValue"/>.
			/// </summary>
			public static TValue IsAny;
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsAny"]/*'/>
		public static TValue IsAny<TValue>()
		{
			return Match<TValue>.Create(
#if FEATURE_COM
				value => value == null || (typeof(TValue).IsAssignableFrom(value.GetType())
				                           || (IsComObject(value) && value is TValue)),
#else
				value => value == null || typeof(TValue).IsAssignableFrom(value.GetType()),
#endif
				() => It.IsAny<TValue>());
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsNotNull"]/*'/>
		public static TValue IsNotNull<TValue>()
		{
			return Match<TValue>.Create(
#if FEATURE_COM
				value => value != null && (typeof(TValue).IsAssignableFrom(value.GetType())
				                           || (IsComObject(value) && value is TValue)),
#else
				value => value != null && typeof(TValue).IsAssignableFrom(value.GetType()),
#endif
				() => It.IsNotNull<TValue>());
		}


		/// <include file='It.xdoc' path='docs/doc[@for="It.Is"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static TValue Is<TValue>(Expression<Func<TValue, bool>> match)
		{
			return Match<TValue>.Create(
				value => match.CompileUsingExpressionCompiler().Invoke(value),
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
					return value.CompareTo(from) > 0 && value.CompareTo(to) < 0;
				}

				return value.CompareTo(from) >= 0 && value.CompareTo(to) <= 0;
			},
			() => It.IsInRange(from, to, rangeKind));
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
			Guard.NotNull(regex, nameof(regex));

			// The regex is constructed only once.
			var re = new Regex(regex);

			// But evaluated every time :)
			return Match<string>.Create(value => value != null && re.IsMatch(value), () => It.IsRegex(regex));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsRegex(regex,options)"]/*'/>
		public static string IsRegex(string regex, RegexOptions options)
		{
			Guard.NotNull(regex, nameof(regex));

			// The regex is constructed only once.
			var re = new Regex(regex, options);

			// But evaluated every time :)
			return Match<string>.Create(value => value != null && re.IsMatch(value), () => It.IsRegex(regex, options));
		}
	}
}
