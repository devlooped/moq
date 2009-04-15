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
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Moq
{
	/// <include file='It.xdoc' path='docs/doc[@for="It"]/*'/>
	public static class It
	{
		/// <include file='It.xdoc' path='docs/doc[@for="It.IsAny"]/*'/>
		public static TValue IsAny<TValue>()
		{
			return Match<TValue>.Create(value =>
			{
				return value == null || typeof(TValue).IsAssignableFrom(value.GetType());
			}, () => It.IsAny<TValue>());
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.Is"]/*'/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "match")]
		public static TValue Is<TValue>(Expression<Predicate<TValue>> match)
		{
			return Match<TValue>.Create(
				value => match.Compile().Invoke(value), 
				() => It.Is<TValue>(match));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsInRange"]/*'/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "to")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "rangeKind")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "from")]
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
					return value.CompareTo(from) > 0 &&
						value.CompareTo(to) < 0;
				}
				else
				{
					return value.CompareTo(from) >= 0 &&
						value.CompareTo(to) <= 0;
				}
			}, 
			() => It.IsInRange(from, to, rangeKind));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsRegex(regex)"]/*'/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "regex")]
		public static string IsRegex(string regex)
		{
			// The regex is constructed only once.
			var re = new Regex(regex);

			return Match<string>.Create(value =>
			{
				// But evaluated every time :)
				return re.IsMatch(value);
			}, 
			() => It.IsRegex(regex));
		}

		/// <include file='It.xdoc' path='docs/doc[@for="It.IsRegex(regex,options)"]/*'/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "regex")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId="options")]
		public static string IsRegex(string regex, RegexOptions options)
		{
			// The regex is constructed only once.
			var re = new Regex(regex, options);

			return Match<string>.Create(value =>
			{
				// But evaluated every time :)
				return re.IsMatch(value);
			}, 
			() => It.IsRegex(regex, options));
		}
	}
}
