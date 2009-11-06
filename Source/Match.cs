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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	/// <include file='Match.xdoc' path='docs/doc[@for="Match"]/*'/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class Match
	{
		/// <devdoc>
		/// Provided for the sole purpose of rendering the delegate passed to the 
		/// matcher constructor if no friendly render lambda is provided.
		/// </devdoc>
		internal static TValue Matcher<TValue>() { return default(TValue); }

		internal abstract bool Matches(object value);

		internal Expression RenderExpression { get; set; }

		// This would allow custom matchers in SetupSet. Need to document guidelines.
		//public static void Track<TValue>(MethodBase invocation, params object[] args)
		//{
		//}
	}

	/// <include file='Match.xdoc' path='docs/doc[@for="Match{T}"]/*'/>
	public class Match<T> : Match
	{
		static readonly Expression<Func<T>> defaultRender = Expression.Lambda<Func<T>>(
			Expression.Call(
				typeof(Match)
					.GetMethod("Matcher", BindingFlags.Static | BindingFlags.NonPublic)
					.MakeGenericMethod(typeof(T))));

		internal Predicate<T> Condition { get; set; }

		private Match(Predicate<T> condition, Expression<Func<T>> renderExpression)
		{
			this.Condition = condition;
			this.RenderExpression = renderExpression.Body;
			SetLastMatch(this);
		}

		private Match(Predicate<T> condition)
			: this(condition, defaultRender)
		{
		}

		/// <include file='Match.xdoc' path='docs/doc[@for="Match{T}.Create(condition)"]/*'/>
		public static T Create(Predicate<T> condition)
		{
			return new Match<T>(condition).Convert();
		}

		/// <include file='Match.xdoc' path='docs/doc[@for="Match{T}.Create(condition,renderExpression"]/*'/>
		public static T Create(Predicate<T> condition, Expression<Func<T>> renderExpression)
		{
			return new Match<T>(condition, renderExpression).Convert();
		}

		internal override bool Matches(object value)
		{
			if (value != null && !(value is T))
				return false;

			return this.Condition((T)value);
		}

		/// <include file='Match.xdoc' path='docs/doc[@for="Match{T}.Convert"]/*'/>
		private T Convert()
		{
			return default(T);
		}

		/// <devdoc>
		/// This method is used to set an expression as the last matcher invoked, 
		/// which is used in the SetupSet to allow matchers in the prop = value 
		/// delegate expression. This delegate is executed in "fluent" mode in 
		/// order to capture the value being set, and construct the corresponding 
		/// methodcall.
		/// This is also used in the MatcherFactory for each argument expression.
		/// This method ensures that when we execute the delegate, we 
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
#if NET35
			: base(ExpressionType.Call, typeof(Match))
#endif
		{
			this.Match = match;
		}

		public Match Match { get; private set; }

#if !NET35
		public override ExpressionType NodeType
		{
			get { return ExpressionType.Call; }
		}

		public override Type Type
		{
			get { return typeof(Match); }
		}
#endif
	}
}