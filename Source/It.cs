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
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Moq
{
	/// <summary>
	/// Allows the specification of a matching condition for an 
	/// argument in a method invocation, rather than a specific 
	/// argument value. "It" refers to the argument being matched.
	/// </summary>
	/// <remarks>
	/// This class allows the setup to match a method invocation 
	/// with an arbitrary value, with a value in a specified range, or 
	/// even one that matches a given predicate.
	/// </remarks>
	public static class It
	{
		/// <summary>
		/// Matches any value of the given <paramref name="TValue"/> type.
		/// </summary>
		/// <remarks>
		/// Typically used when the actual argument value for a method 
		/// call is not relevant.
		/// </remarks>
		/// <example>
		/// <code>
		/// // Throws an exception for a call to Remove with any string value.
		/// mock.Setup(x => x.Remove(It.IsAny&lt;string&gt;())).Throws(new InvalidOperationException());
		/// </code>
		/// </example>
		/// <typeparam name="TValue">Type of the value.</typeparam>
		[AdvancedMatcher(typeof(AnyMatcher))]
		public static TValue IsAny<TValue>()
		{
			SetLastMatcher<TValue>(MethodBase.GetCurrentMethod());
			return default(TValue);
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
		/// mock.Setup(x =&gt; x.Do(It.Is&lt;int&gt;(i =&gt; i % 2 == 0)))
		///     .Returns(1);
		/// </code>
		/// This example shows how to throw an exception if the argument to the 
		/// method is a negative number:
		/// <code>
		/// mock.Setup(x =&gt; x.GetUser(It.Is&lt;int&gt;(i =&gt; i &lt; 0)))
		///     .Throws(new ArgumentException());
		/// </code>
		/// </example>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "match")]
		[AdvancedMatcher(typeof(PredicateMatcher))]
		public static TValue Is<TValue>(Expression<Predicate<TValue>> match)
		{
			SetLastMatcher<TValue>(MethodBase.GetCurrentMethod(), match);
			return default(TValue);
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
		/// mock.Setup(x =&gt; x.HasInventory(
		///                          It.IsAny&lt;string&gt;(),
		///                          It.IsInRange(0, 100, Range.Inclusive)))
		///     .Returns(false);
		/// </code>
		/// </example>
		[System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="to")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "rangeKind")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "from")]
		[AdvancedMatcher(typeof(RangeMatcher))]
		public static TValue IsInRange<TValue>(TValue from, TValue to, Range rangeKind)
			where TValue : IComparable
		{
			SetLastMatcher<TValue>(MethodBase.GetCurrentMethod(), from, to, rangeKind);
			return default(TValue);
		}

		/// <summary>
		/// Matches a string argument if it matches the given regular expression pattern.
		/// </summary>
		/// <param name="regex">The pattern to use to match the string argument value.</param>
		/// <example>
		/// The following example shows how to expect a call to a method where the 
		/// string argument matches the given regular expression:
		/// <code>
		/// mock.Setup(x => x.Check(It.IsRegex("[a-z]+"))).Returns(1);
		/// </code>
		/// </example>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "regex")]
		[AdvancedMatcher(typeof(RegexMatcher))]
		public static string IsRegex(string regex)
		{
			SetLastMatcher<string>(MethodBase.GetCurrentMethod(), regex);
			return default(string);
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
		/// mock.Setup(x => x.Check(It.IsRegex("[a-z]+", RegexOptions.IgnoreCase))).Returns(1);
		/// </code>
		/// </example>
		[System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="regex")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="options")]
		[AdvancedMatcher(typeof(RegexMatcher))]
		public static string IsRegex(string regex, RegexOptions options)
		{
			SetLastMatcher<string>(MethodBase.GetCurrentMethod(), regex, options);
			return default(string);
		}

		/// <devdoc>
		/// This method is used to set an expression as the last matcher invoked, 
		/// which is used in the SetupSet to allow matchers in the prop = value 
		/// delegate expression. This delegate is executed in "fluent" mode in 
		/// order to capture the value being set, and construct the corresponding 
		/// methodcall. This method ensures that when we execute the delegate, we 
		/// also track the matcher that was invoked, so that when we create the 
		/// methodcall we build the expression using it, rather than the null/default 
		/// value returned from the actual invocation.
		/// </devdoc>
		private static void SetLastMatcher<TValue>(MethodBase invokedMethod, params object[] args)
		{
			if (FluentMockContext.IsActive)
			{
				var concreteMethod = invokedMethod.IsGenericMethodDefinition ?
					((MethodInfo)invokedMethod).MakeGenericMethod(typeof(TValue)) :
					(MethodInfo)invokedMethod;

				FluentMockContext.Current.LastMatcherInvocation =
					Expression.Call(
						concreteMethod,
						args.Select(a => a is Expression ? (Expression)a : Expression.Constant(a))
							.ToArray()
					);
			}
		}
	}
}
