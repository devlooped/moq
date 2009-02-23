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
using System.ComponentModel;
using System.Linq.Expressions;

namespace Moq
{
	/// <include file='Match.xdoc' path='docs/doc[@for="Match"]/*'/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class Match
	{
		/// <devdoc>
		/// Provided for the sole purpose of rendering the delegate passed to SetupSet.
		/// </devdoc>
		internal static TValue Matcher<TValue>() { return default(TValue); }

		internal abstract bool Matches(object value);

		// This would allow custom matchers in SetupSet. Need to document guidelines.
		//public static void Track<TValue>(MethodBase invocation, params object[] args)
		//{
		//}
	}

	/// <include file='Match.xdoc' path='docs/doc[@for="Match{T}"]/*'/>
	public class Match<T> : Match
	{
		Predicate<T> condition;

		/// <include file='Match.xdoc' path='docs/doc[@for="Match{T}.ctor"]/*'/>
		public Match(Predicate<T> condition)
		{
			this.condition = condition;
			SetLastMatch(this);
		}

		internal override bool Matches(object value)
		{
			if (value != null && !(value is T))
				return false;

			return condition((T)value);
		}

		/// <include file='Match.xdoc' path='docs/doc[@for="Match{T}.Convert"]/*'/>
		public T Convert()
		{
			return default(T);
		}

		/// <include file='Match.xdoc' path='docs/doc[@for="Match{T}.operator"]/*'/>
		public static implicit operator T(Match<T> match)
		{
			return default(T);
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
		internal static Match<TValue> SetLastMatch<TValue>(Match<TValue> match)
		{
			if (FluentMockContext.IsActive)
				FluentMockContext.Current.LastMatch = match;

			return match;
		}
	}

	internal class MatchExpression : Expression
	{
		public MatchExpression(Match match)
			: base(ExpressionType.Call, typeof(Match))
		{
			this.Match = match;
		}

		public Match Match { get; private set; }
	}
}
