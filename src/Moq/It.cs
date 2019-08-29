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

using Moq.Protected;

namespace Moq
{
	/// <summary>
	///   Allows the specification of a matching condition for an argument in a method invocation,
	///   rather than a specific argument value. "It" refers to the argument being matched.
	/// </summary>
	/// <remarks>
	///   This class allows the setup to match a method invocation with an arbitrary value,
	///   with a value in a specified range, or even one that matches a given predicate.
	/// </remarks>
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

		/// <summary>
		///   Matches any value of the given <typeparamref name="TValue"/> type.
		/// </summary>
		/// <typeparam name="TValue">Type of the value.</typeparam>
		/// <remarks>
		///   Typically used when the actual argument value for a method call is not relevant.
		/// </remarks>
		/// <include file='It.xdoc' path='docs/doc[@for="It.IsAny"]/*'/>
		public static TValue IsAny<TValue>()
		{
			return Match<TValue>.Create(
#if FEATURE_COM
				value => value == null || (typeof(TValue).IsAssignableFrom(value.GetType()) || IsComObject(value)),
#else
				value => value == null || typeof(TValue).IsAssignableFrom(value.GetType()),
#endif
				() => It.IsAny<TValue>());
		}

		private static readonly MethodInfo isAnyMethod = typeof(It).GetMethod(nameof(It.IsAny), BindingFlags.Public | BindingFlags.Static);

		internal static MethodCallExpression IsAny(Type genericArgument)
		{
			return Expression.Call(It.isAnyMethod.MakeGenericMethod(genericArgument));
		}

		/// <summary>
		///   Matches any value of the given <typeparamref name="TValue"/> type, except null.
		/// </summary>
		/// <typeparam name="TValue">Type of the value.</typeparam>
		public static TValue IsNotNull<TValue>()
		{
			return Match<TValue>.Create(
#if FEATURE_COM
				value => value != null && (typeof(TValue).IsAssignableFrom(value.GetType()) || IsComObject(value)),
#else
				value => value != null && typeof(TValue).IsAssignableFrom(value.GetType()),
#endif
				() => It.IsNotNull<TValue>());
		}

		/// <summary>
		///   Matches any value that satisfies the given predicate.
		/// </summary>
		/// <param name="match">The predicate used to match the method argument.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <remarks>
		///   Allows the specification of a predicate to perform matching of method call arguments.
		/// </remarks>
		/// <include file='It.xdoc' path='docs/doc[@for="It.Is"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static TValue Is<TValue>(Expression<Func<TValue, bool>> match)
		{
			return Match<TValue>.Create(
				value => match.CompileUsingExpressionCompiler().Invoke(value),
				Expression.Lambda<Func<TValue>>(ItExpr.Is<TValue>(match)));
		}

		/// <summary>
		///   Matches any value that is in the range specified.
		/// </summary>
		/// <param name="from">The lower bound of the range.</param>
		/// <param name="to">The upper bound of the range.</param>
		/// <param name="rangeKind">The kind of range. See <see cref="Range"/>.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
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

		/// <summary>
		///   Matches any value that is present in the sequence specified.
		/// </summary>
		/// <param name="items">The sequence of possible values.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <include file='It.xdoc' path='docs/doc[@for="It.IsIn(enumerable)"]/*'/>
		public static TValue IsIn<TValue>(IEnumerable<TValue> items)
		{
			return Match<TValue>.Create(value => items.Contains(value), () => It.IsIn(items));
		}

		/// <summary>
		///   Matches any value that is present in the sequence specified.
		/// </summary>
		/// <param name="items">The sequence of possible values.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <include file='It.xdoc' path='docs/doc[@for="It.IsIn(params)"]/*'/>
		public static TValue IsIn<TValue>(params TValue[] items)
		{
			return Match<TValue>.Create(value => items.Contains(value), () => It.IsIn(items));
		}

		/// <summary>
		///   Matches any value that is not found in the sequence specified.
		/// </summary>
		/// <param name="items">The sequence of disallowed values.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <include file='It.xdoc' path='docs/doc[@for="It.IsNotIn(enumerable)"]/*'/>
		public static TValue IsNotIn<TValue>(IEnumerable<TValue> items)
		{
			return Match<TValue>.Create(value => !items.Contains(value), () => It.IsNotIn(items));
		}

		/// <summary>
		///   Matches any value that is not found in the sequence specified.
		/// </summary>
		/// <param name="items">The sequence of disallowed values.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <include file='It.xdoc' path='docs/doc[@for="It.IsNotIn(params)"]/*'/>
		public static TValue IsNotIn<TValue>(params TValue[] items)
		{
			return Match<TValue>.Create(value => !items.Contains(value), () => It.IsNotIn(items));
		}

		/// <summary>
		///   Matches a string argument if it matches the given regular expression pattern.
		/// </summary>
		/// <param name="regex">The pattern to use to match the string argument value.</param>
		/// <include file='It.xdoc' path='docs/doc[@for="It.IsRegex(regex)"]/*'/>
		public static string IsRegex(string regex)
		{
			Guard.NotNull(regex, nameof(regex));

			// The regex is constructed only once.
			var re = new Regex(regex);

			// But evaluated every time :)
			return Match<string>.Create(value => value != null && re.IsMatch(value), () => It.IsRegex(regex));
		}

		/// <summary>
		///   Matches a string argument if it matches the given regular expression pattern.
		/// </summary>
		/// <param name="regex">The pattern to use to match the string argument value.</param>
		/// <param name="options">The options used to interpret the pattern.</param>
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
