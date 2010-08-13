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
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Moq.Matchers;

namespace Moq.Protected
{
	/// <summary>
	/// Allows the specification of a matching condition for an 
	/// argument in a protected member setup, rather than a specific 
	/// argument value. "ItExpr" refers to the argument being matched.
	/// </summary>
	/// <remarks>
	/// <para>Use this variant of argument matching instead of 
	/// <see cref="It"/> for protected setups.</para>
	/// This class allows the setup to match a method invocation 
	/// with an arbitrary value, with a value in a specified range, or 
	/// even one that matches a given predicate, or null.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Expr")]
	public static class ItExpr
	{
		/// <summary>
		/// Matches a null value of the given <typeparamref name="TValue"/> type.
		/// </summary>
		/// <remarks>
		/// Required for protected mocks as the null value cannot be used 
		/// directly as it prevents proper method overload selection.
		/// </remarks>
		/// <example>
		/// <code>
		/// // Throws an exception for a call to Remove with a null string value.
		/// mock.Protected()
		///     .Setup("Remove", ItExpr.IsNull&lt;string&gt;())
		///     .Throws(new InvalidOperationException());
		/// </code>
		/// </example>
		/// <typeparam name="TValue">Type of the value.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static Expression IsNull<TValue>()
		{
			Expression<Func<TValue>> expr = () => It.Is<TValue>(v => Object.Equals(v, default(TValue)));

			return expr.Body;
		}

		/// <summary>
		/// Matches any value of the given <typeparamref name="TValue"/> type.
		/// </summary>
		/// <remarks>
		/// Typically used when the actual argument value for a method 
		/// call is not relevant.
		/// </remarks>
		/// <example>
		/// <code>
		/// // Throws an exception for a call to Remove with any string value.
		/// mock.Protected()
		///     .Setup("Remove", ItExpr.IsAny&lt;string&gt;())
		///     .Throws(new InvalidOperationException());
		/// </code>
		/// </example>
		/// <typeparam name="TValue">Type of the value.</typeparam>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static Expression IsAny<TValue>()
		{
			Expression<Func<TValue>> expr = () => It.IsAny<TValue>();
			return expr.Body;
		}

		/// <summary>
		/// Matches any value that satisfies the given predicate.
		/// </summary>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <param name="match">The predicate used to match the method argument.</param>
		/// <remarks>
		/// Allows the specification of a predicate to perform matching 
		/// of method call arguments.
		/// </remarks>
		/// <example>
		/// This example shows how to return the value <c>1</c> whenever the argument to the 
		/// <c>Do</c> method is an even number.
		/// <code>
		/// mock.Protected()
		///     .Setup("Do", ItExpr.Is&lt;int&gt;(i =&gt; i % 2 == 0))
		///     .Returns(1);
		/// </code>
		/// This example shows how to throw an exception if the argument to the 
		/// method is a negative number:
		/// <code>
		/// mock.Protected()
		///     .Setup("GetUser", ItExpr.Is&lt;int&gt;(i =&gt; i &lt; 0))
		///     .Throws(new ArgumentException());
		/// </code>
		/// </example>
		[AdvancedMatcher(typeof(PredicateMatcher))]
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static Expression Is<TValue>(Expression<Func<TValue, bool>> match)
		{
			return Expression.Call(null,
				typeof(It).GetMethod("Is").MakeGenericMethod(typeof(TValue)),
				match);
		}

		/// <summary>
		/// Matches any value that is in the range specified.
		/// </summary>
		/// <typeparam name="TValue">Type of the argument to check.</typeparam>
		/// <param name="from">The lower bound of the range.</param>
		/// <param name="to">The upper bound of the range.</param>
		/// <param name="rangeKind">The kind of range. See <see cref="Range"/>.</param>
		/// <example>
		/// The following example shows how to expect a method call 
		/// with an integer argument within the 0..100 range.
		/// <code>
		/// mock.Protected()
		///     .Setup("HasInventory",
		///             ItExpr.IsAny&lt;string&gt;(),
		///             ItExpr.IsInRange(0, 100, Range.Inclusive))
		///     .Returns(false);
		/// </code>
		/// </example>
		public static Expression IsInRange<TValue>(TValue from, TValue to, Range rangeKind)
			where TValue : IComparable
		{
			Expression<Func<TValue>> expr = () => It.IsInRange<TValue>(from, to, rangeKind);

			return expr.Body;
		}

		/// <summary>
		/// Matches a string argument if it matches the given regular expression pattern.
		/// </summary>
		/// <param name="regex">The pattern to use to match the string argument value.</param>
		/// <example>
		/// The following example shows how to expect a call to a method where the 
		/// string argument matches the given regular expression:
		/// <code>
		/// mock.Protected()
		///     .Setup("Check", ItExpr.IsRegex("[a-z]+"))
		///     .Returns(1);
		/// </code>
		/// </example>
		public static Expression IsRegex(string regex)
		{
			Expression<Func<string>> expr = () => It.IsRegex(regex);

			return expr.Body;
		}

		/// <summary>
		/// Matches a string argument if it matches the given regular expression pattern.
		/// </summary>
		/// <param name="regex">The pattern to use to match the string argument value.</param>
		/// <param name="options">The options used to interpret the pattern.</param>
		/// <example>
		/// The following example shows how to expect a call to a method where the 
		/// string argument matches the given regular expression, in a case insensitive way:
		/// <code>
		/// mock.Protected()
		///     .Setup("Check", ItExpr.IsRegex("[a-z]+", RegexOptions.IgnoreCase))
		///     .Returns(1);
		/// </code>
		/// </example>
		public static Expression IsRegex(string regex, RegexOptions options)
		{
			Expression<Func<string>> expr = () => It.IsRegex(regex, options);

			return expr.Body;
		}
	}
}
