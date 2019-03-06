// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Expressions.Visitors;

namespace Moq
{
	/// <include file='Match.xdoc' path='docs/doc[@for="Match"]/*'/>
	public abstract class Match : IMatcher
	{
		/// <devdoc>
		/// Provided for the sole purpose of rendering the delegate passed to the 
		/// matcher constructor if no friendly render lambda is provided.
		/// </devdoc>
		[Matcher]
		internal static TValue Matcher<TValue>()
		{
			return default(TValue);
		}

		internal abstract bool Matches(object value);

		bool IMatcher.Matches(object value) => this.Matches(value);

		internal Expression RenderExpression { get; set; }

		/// <include file='Match.xdoc' path='docs/doc[@for="Match.Create{T}(condition)"]/*'/>
		public static T Create<T>(Predicate<T> condition)
		{
			return Create(new Match<T>(condition, () => Matcher<T>()));
		}

		/// <include file='Match.xdoc' path='docs/doc[@for="Match.Create{T}(condition,renderExpression"]/*'/>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public static T Create<T>(Predicate<T> condition, Expression<Func<T>> renderExpression)
		{
			return Create(new Match<T>(condition, renderExpression));
		}

		internal static T Create<T>(Match<T> match)
		{
			// This method is used to set an expression as the last matcher invoked,
			// which is used in the SetupSet to allow matchers in the prop = value
			// delegate expression. This delegate is executed in "fluent" mode in
			// order to capture the value being set, and construct the corresponding
			// methodcall.
			// This is also used in the MatcherFactory for each argument expression.
			// This method ensures that when we execute the delegate, we
			// also track the matcher that was invoked, so that when we create the
			// methodcall we build the expression using it, rather than the null/default
			// value returned from the actual invocation.

			if (AmbientObserver.IsActive(out var observer))
			{
				observer.OnMatch(match);
			}

			return default(T);
		}
	}

	/// <include file='Match.xdoc' path='docs/doc[@for="Match{T}"]/*'/>
	public class Match<T> : Match, IEquatable<Match<T>>
	{
		internal Predicate<T> Condition { get; set; }

		internal Match(Predicate<T> condition, Expression<Func<T>> renderExpression)
		{
			this.Condition = condition;
			this.RenderExpression = renderExpression.Body.Apply(EvaluateCaptures.Rewriter);
		}

		internal override bool Matches(object value)
		{
			if (value != null && !(value is T))
			{
				return false;
			}

			var matchType = typeof(T);
			if (value == null && matchType.GetTypeInfo().IsValueType
				&& (!matchType.GetTypeInfo().IsGenericType || matchType.GetGenericTypeDefinition() != typeof(Nullable<>)))
			{
				// If this.Condition expects a value type and we've been passed null,
				// it can't possibly match.
				// This tends to happen when you are trying to match a parameter of type int?
				// with IsAny<int> but then pass null into the mock.
				// We have to return early from here because you can't cast null to T
				// when T is a value type.
				//
				// See GitHub issue #90: https://github.com/moq/moq4/issues/90
				return false;
			}
			return this.Condition((T)value);
		}

		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			return obj is Match<T> other && this.Equals(other);
		}

		/// <inheritdoc/>
		public bool Equals(Match<T> other)
		{
			if (this.Condition == other.Condition)
			{
				return true;
			}
			else if (this.Condition.GetMethodInfo() != other.Condition.GetMethodInfo())
			{
				return false;
			}
			else if (!(this.RenderExpression is MethodCallExpression ce && ce.Method.DeclaringType == typeof(Match)))
			{
				return ExpressionComparer.Default.Equals(this.RenderExpression, other.RenderExpression);
			}
			else
			{
				return false;  // The test documented in `MatchFixture.Equality_ambiguity` is caused by this.
				               // Returning true would break equality even worse. The only way to resolve the
				               // ambiguity is to either add a render expression to your custom matcher, or
				               // to test both `Condition.Target` objects for structural equality.
			}
		}

		/// <inheritdoc/>
		public override int GetHashCode() => 0;
	}
}
