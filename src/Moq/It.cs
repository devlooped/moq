// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

using Moq.Properties;

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
		/// <example>
		///   <code>
		///     // Throws an exception for a call to Remove with any string value.
		///     mock.Setup(x => x.Remove(It.IsAny&lt;string&gt;())).Throws(new InvalidOperationException());
		///   </code>
		/// </example>
		public static TValue IsAny<TValue>()
		{
			if (typeof(TValue).IsOrContainsTypeMatcher())
			{
				return Match.Create<TValue>(
					(argument, parameterType) => argument == null || parameterType.IsAssignableFrom(argument.GetType()),
					() => It.IsAny<TValue>());
			}
			else
			{
				return Match.Create<TValue>(
					 argument                 => argument == null || argument is TValue,
					() => It.IsAny<TValue>());
			}
		}

		private static readonly MethodInfo isAnyMethod = typeof(It).GetMethod(nameof(It.IsAny), BindingFlags.Public | BindingFlags.Static);

		internal static MethodCallExpression IsAny(Type genericArgument)
		{
			return Expression.Call(It.isAnyMethod.MakeGenericMethod(genericArgument));
		}

		/// <summary>
		///   A type matcher that matches any generic type argument.
		///   <para>
		///     If the generic type parameter is constrained to <see langword="struct"/> (C#) / <see langword="Structure"/>
		///     (VB.NET), use <see cref="It.IsValueType"/> instead.
		///   </para>
		///   <para>
		///     If the generic type parameter has more specific constraints,
		///     you can define your own type matcher inheriting from the type to which the type parameter is constrained.
		///     See <see cref="TypeMatcherAttribute"/> and <see cref="ITypeMatcher"/>.
		///   </para>
		/// </summary>
		[TypeMatcher]
		public sealed class IsAnyType : ITypeMatcher
		{
			bool ITypeMatcher.Matches(Type type)
			{
				return true;
			}
		}

		/// <summary>
		///   Matches any value of the given <typeparamref name="TValue"/> type, except null.
		/// </summary>
		/// <typeparam name="TValue">Type of the value.</typeparam>
		public static TValue IsNotNull<TValue>()
		{
			if (typeof(TValue).IsOrContainsTypeMatcher())
			{
				return Match.Create<TValue>(
					(argument, parameterType) => argument != null && parameterType.IsAssignableFrom(argument.GetType()),
					() => It.IsNotNull<TValue>());
			}
			else
			{
				return Match.Create<TValue>(
					 argument                 => argument is TValue,
					() => It.IsNotNull<TValue>());
			}
		}

		/// <summary>
		///   Matches any value that satisfies the given predicate.
		/// </summary>
		/// <param name="match">The predicate used to match the method argument.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <remarks>
		///   Allows the specification of a predicate to perform matching of method call arguments.
		/// </remarks>
		/// <example>
		///   This example shows how to return the value <c>1</c> whenever the argument to
		///   the <c>Do</c> method is an even number.
		///   <code>
		///     mock.Setup(x =&gt; x.Do(It.Is&lt;int&gt;(i =&gt; i % 2 == 0)))
		///         .Returns(1);
		///   </code>
		/// </example>
		/// <example>
		///   This example shows how to throw an exception if the argument to the method
		///   is a negative number:
		///   <code>
		///     mock.Setup(x =&gt; x.GetUser(It.Is&lt;int&gt;(i =&gt; i &lt; 0)))
		///         .Throws(new ArgumentException());
		///   </code>
		/// </example>
		public static TValue Is<TValue>(Expression<Func<TValue, bool>> match)
		{
			if (typeof(TValue).IsOrContainsTypeMatcher())
			{
				throw new ArgumentException(Resources.UseItIsOtherOverload, nameof(match));
			}

			var thisMethod = (MethodInfo)MethodBase.GetCurrentMethod();

			return Match.Create<TValue>(
				argument => match.CompileUsingExpressionCompiler().Invoke(argument),
				Expression.Lambda<Func<TValue>>(Expression.Call(thisMethod.MakeGenericMethod(typeof(TValue)), match)));
		}

		/// <summary>
		///   Matches any value that satisfies the given predicate.
		///   <para>
		///     Use this overload when you specify a type matcher for <typeparamref name="TValue"/>.
		///     The <paramref name="match"/> callback you provide will then receive the actual parameter type
		///     as well as the invocation argument.
		///   </para>
		/// </summary>
		/// <param name="match">The predicate used to match the method argument.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <remarks>
		///   Allows the specification of a predicate to perform matching of method call arguments.
		/// </remarks>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TValue Is<TValue>(Expression<Func<object, Type, bool>> match)
		{
			var thisMethod = (MethodInfo)MethodBase.GetCurrentMethod();

			return Match.Create<TValue>(
				(argument, parameterType) => match.CompileUsingExpressionCompiler().Invoke(argument, parameterType),
				Expression.Lambda<Func<TValue>>(Expression.Call(thisMethod.MakeGenericMethod(typeof(TValue)), match)));
		}

		/// <summary>
		///   Matches any value that equals the <paramref name="value"/> using the <paramref name="comparer"/>.
		///   To use the default comparer for the specified object, specify the value inline,
		///   i.e. <code>mock.Verify(service => service.DoWork(value))</code>.
		///   <para>
		///     Use this overload when you specify a value and a comparer.
		///   </para>
		/// </summary>
		/// <param name="value">The value to match with.</param>
		/// <param name="comparer">An <see cref="IEqualityComparer{T}"/> with which the values should be compared.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static TValue Is<TValue>(TValue value, IEqualityComparer<TValue> comparer)
		{
			return Match.Create<TValue>(actual => comparer.Equals(actual, value), () => It.Is<TValue>(value, comparer));
		}

		/// <summary>
		///   Matches any value that is in the range specified.
		/// </summary>
		/// <param name="from">The lower bound of the range.</param>
		/// <param name="to">The upper bound of the range.</param>
		/// <param name="rangeKind">The kind of range. See <see cref="Range"/>.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <example>
		///   The following example shows how to expect a method call with an integer argument
		///   within the 0..100 range.
		///   <code>
		///     mock.Setup(x => x.HasInventory(
		///                         It.IsAny&lt;string&gt;(),
		///                         It.IsInRange(0, 100, Range.Inclusive)))
		///         .Returns(false);
		///   </code>
		/// </example>
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
		/// <example>
		///   The following example shows how to expect a method call with an integer argument
		///   with value from a list.
		///   <code>
		///     var values = new List&lt;int&gt; { 1, 2, 3 };
		///
		///     mock.Setup(x => x.HasInventory(
		///                         It.IsAny&lt;string&gt;(),
		///                         It.IsIn(values)))
		///         .Returns(false);
		///   </code>
		/// </example>
		public static TValue IsIn<TValue>(IEnumerable<TValue> items)
		{
			return Match<TValue>.Create(value => items.Contains(value), () => It.IsIn(items));
		}

		/// <summary>
		///   Matches any value that is present in the sequence specified.
		/// </summary>
		/// <param name="items">The sequence of possible values.</param>
		/// <param name="comparer">An <see cref="IEqualityComparer{T}"/> with which the values should be compared.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		public static TValue IsIn<TValue>(IEnumerable<TValue> items, IEqualityComparer<TValue> comparer)
		{
			return Match<TValue>.Create(value => items.Contains(value, comparer), () => It.IsIn(items, comparer));
		}

		/// <summary>
		///   Matches any value that is present in the sequence specified.
		/// </summary>
		/// <param name="items">The sequence of possible values.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <example>
		///   The following example shows how to expect a method call with an integer argument
		///   with a value of 1, 2, or 3.
		///   <code>
		///     mock.Setup(x => x.HasInventory(
		///                         It.IsAny&lt;string&gt;(),
		///                         It.IsIn(1, 2, 3)))
		///         .Returns(false);
		///   </code>
		/// </example>
		public static TValue IsIn<TValue>(params TValue[] items)
		{
			return Match<TValue>.Create(value => items.Contains(value), () => It.IsIn(items));
		}

		/// <summary>
		///   Matches any value that is not found in the sequence specified.
		/// </summary>
		/// <param name="items">The sequence of disallowed values.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <example>
		///   The following example shows how to expect a method call with an integer argument
		///   with value not found from a list.
		///   <code>
		///     var values = new List&lt;int&gt; { 1, 2, 3 };
		///
		///     mock.Setup(x => x.HasInventory(
		///                         It.IsAny&lt;string&gt;(),
		///                         It.IsNotIn(values)))
		///         .Returns(false);
		///   </code>
		/// </example>
		public static TValue IsNotIn<TValue>(IEnumerable<TValue> items)
		{
			return Match<TValue>.Create(value => !items.Contains(value), () => It.IsNotIn(items));
		}

		/// <summary>
		///   Matches any value that is not found in the sequence specified.
		/// </summary>
		/// <param name="items">The sequence of disallowed values.</param>
		/// <param name="comparer">An <see cref="IEqualityComparer{T}"/> with which the values should be compared.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		public static TValue IsNotIn<TValue>(IEnumerable<TValue> items, IEqualityComparer<TValue> comparer)
		{
			return Match<TValue>.Create(value => !items.Contains(value, comparer), () => It.IsNotIn(items, comparer));
		}

		/// <summary>
		///   Matches any value that is not found in the sequence specified.
		/// </summary>
		/// <param name="items">The sequence of disallowed values.</param>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <example>
		///   The following example shows how to expect a method call with an integer argument
		///   of any value except 1, 2, or 3.
		///   <code>
		///     mock.Setup(x => x.HasInventory(
		///                         It.IsAny&lt;string&gt;(),
		///                         It.IsNotIn(1, 2, 3)))
		///         .Returns(false);
		///   </code>
		/// </example>
		public static TValue IsNotIn<TValue>(params TValue[] items)
		{
			return Match<TValue>.Create(value => !items.Contains(value), () => It.IsNotIn(items));
		}

		/// <summary>
		///   Matches a string argument if it matches the given regular expression pattern.
		/// </summary>
		/// <param name="regex">The pattern to use to match the string argument value.</param>
		/// <example>
		///   The following example shows how to expect a call to a method where the string argument
		///   matches the given regular expression:
		///   <code>
		///     mock.Setup(x => x.Check(It.IsRegex("[a-z]+")))
		///         .Returns(1);
		///   </code>
		/// </example>
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
		/// <example>
		///   The following example shows how to expect a call to a method where the string argument
		///   matches the given regular expression, in a case insensitive way:
		///   <code>
		///     mock.Setup(x => x.Check(It.IsRegex("[a-z]+", RegexOptions.IgnoreCase)))
		///         .Returns(1);
		///   </code>
		/// </example>
		public static string IsRegex(string regex, RegexOptions options)
		{
			Guard.NotNull(regex, nameof(regex));

			// The regex is constructed only once.
			var re = new Regex(regex, options);

			// But evaluated every time :)
			return Match<string>.Create(value => value != null && re.IsMatch(value), () => It.IsRegex(regex, options));
		}

		/// <summary>
		///   A type matcher that matches subtypes of <typeparamref name="T"/>, as well as <typeparamref name="T"/> itself.
		/// </summary>
		/// <typeparam name="T">The type whose subtypes should match.</typeparam>
		[TypeMatcher]
		public sealed class IsSubtype<T> : ITypeMatcher
		{
			bool ITypeMatcher.Matches(Type type)
			{
				return typeof(T).IsAssignableFrom(type);
			}
		}

		/// <summary>
		///   A type matcher that matches any value type.
		/// </summary>
		[TypeMatcher]
		public readonly struct IsValueType : ITypeMatcher
		{
			bool ITypeMatcher.Matches(Type type)
			{
				return type.IsValueType;
			}
		}
	}
}
