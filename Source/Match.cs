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
	/// <summary>
	/// Allows creation custom value matchers that can be used on setups and verification, 
	/// completely replacing the built-in <see cref="It"/> class with your own argument 
	/// matching rules.
	/// </summary>
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

	/// <summary>
	/// Allows creation custom value matchers that can be used on setups and verification.
	/// </summary>
	/// <remarks>
	/// The argument matching is used to determine whether a concrete 
	/// invocation in the mock matches a given setup. This 
	/// matching mechanism is fully extensible. 
	/// </remarks>
	/// <example>
	/// Creating a custom matcher is straightforward. You just need to create a method 
	/// that returns an instance of <see cref="Match{T}"/> with your matching condition:
	/// <code>
	/// public Order IsBigOrder()
	/// {
	///   return new Match&lt;Order&gt;(o => o.GrandTotal &gt;= 5000);
	/// }
	/// </code>
	/// The <see cref="Match{T}"/> class is implicitly convertible to the return type, 
	/// which must be the type expected by the mock setup invocation:
	/// <code>
	/// mock.Setup(m => m.Submit(IsBigOrder()).Throws&lt;UnauthorizedAccessException&gt;();
	/// </code>
	/// By returning the appropriate type from the custom matcher method, the compiler 
	/// is satisfied, but at runtime Moq knows that the return value was a matcher and 
	/// evaluates your predicate with the actual value passed-in.
	/// <para>
	/// Another example might be a case where you want to match a lists of orders 
	/// that contains a particular one. You might create matcher like the following:
	/// </para>
	/// <code>
	/// public static class Orders
	/// {
	///   public static IEnumerable&lt;Order&gt; Contains(Order order)
	///   {
	///     return new Match&lt;IEnumerable&lt;Order&gt;&gt;(orders => orders.Contains(order));
	///   }
	/// }
	/// </code>
	/// Now we can invoke this static method instead of an argument in an 
	/// invocation:
	/// <code>
	/// var order = new Order { ... };
	/// var mock = new Mock&lt;IRepository&lt;Order&gt;&gt;();
	/// 
	/// mock.Setup(x =&gt; x.Save(Orders.Contains(order)))
	///     .Throws&lt;ArgumentException&gt;();
	/// </code>
	/// </example>
	public class Match<T> : Match
	{
		Predicate<T> condition;

		/// <summary>
		/// Initializes the match with the condition that 
		/// will be checked in order to match invocation 
		/// values.
		/// </summary>
		public Match(Predicate<T> condition)
		{
			this.condition = condition;
			if (FluentMockContext.IsActive)
				FluentMockContext.Current.LastMatch = this;
		}

		internal override bool Matches(object value)
		{
			if (value != null && !(value is T))
				return false;

			return condition((T)value);
		}

		/// <summary>
		/// If the target type to match is an interface, the implicit cast will not work and 
		/// you need to explicitly convert the matcher to the target type to satisfy the compiler.
		/// </summary>
		/// <remarks>
		/// For more information about this well-known .NET limitation, see 
		/// https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=318122
		/// </remarks>
		public T Convert()
		{
			return default(T);
		}

		/// <summary>
		/// Satisfies the compiler when the matcher is used 
		/// to specify an argument value.
		/// </summary>
		public static implicit operator T(Match<T> match)
		{
			return default(T);
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
