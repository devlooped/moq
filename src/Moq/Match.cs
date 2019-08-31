// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

using Moq.Expressions.Visitors;

namespace Moq
{
	/// <summary>
	///   Allows creating custom value matchers that can be used on setups and verification,
	///   completely replacing the built-in <see cref="It"/> class with your own
	///   argument matching rules.
	/// </summary>
	/// <remarks>
	///   Argument matching is used to determine whether a concrete invocation in the mock
	///   matches a given setup. This matching mechanism is fully extensible.
	/// </remarks>
	/// <include file='Match.xdoc' path='docs/doc[@for="Match"]/*'/>
	public abstract class Match : IMatcher
	{
		/// <devdoc>
		/// Provided for the sole purpose of rendering the delegate passed to the 
		/// matcher constructor if no friendly render lambda is provided.
		/// </devdoc>
		internal static TValue Matcher<TValue>()
		{
			return default(TValue);
		}

		internal abstract bool Matches(object argument, Type parameterType);

		internal abstract void SetupEvaluatedSuccessfully(object argument, Type parameterType);

		bool IMatcher.Matches(object argument, Type parameterType) => this.Matches(argument, parameterType);

		void IMatcher.SetupEvaluatedSuccessfully(object value, Type parameterType) => this.SetupEvaluatedSuccessfully(value, parameterType);

		internal Expression RenderExpression { get; set; }

		/// <summary>
		///   Initializes the matcher with the condition that will be checked
		///   in order to match invocation values.
		/// </summary>
		/// <param name="condition">The condition to match against actual values.</param>
		public static T Create<T>(Predicate<T> condition)
		{
			return Create(new Match<T>(condition, () => Matcher<T>()));
		}

		/// <summary>
		///   Initializes the matcher with the condition that will be checked
		///   in order to match invocation values.
		/// </summary>
		/// <param name="condition">The condition to match against actual values.</param>
		/// <param name="renderExpression">
		///   A lambda representation of the matcher, to be used when rendering error messages,
		///   such as <c>() => It.IsAny&lt;string&lt;()</c>.
		/// </param>
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

			if (MatcherObserver.IsActive(out var observer))
			{
				observer.OnMatch(match);
			}

			return default(T);
		}
	}

	/// <summary>
	///   Allows creating custom value matchers that can be used on setups and verification,
	///   completely replacing the built-in <see cref="It"/> class with your own
	///   argument matching rules.
	/// </summary>
	/// <typeparam name="T">Type of the value to match.</typeparam>
	public class Match<T> : Match, IEquatable<Match<T>>
	{
		internal Predicate<T> Condition { get; set; }
		internal Action<T> Success { get; set; }

		internal Match(Predicate<T> condition, Expression<Func<T>> renderExpression, Action<T> success = null)
		{
			this.Condition = condition;
			this.RenderExpression = renderExpression.Body.Apply(EvaluateCaptures.Rewriter);
			this.Success = success;
		}

		internal override bool Matches(object argument, Type parameterType)
		{
			return CanCast(argument) && this.Condition((T)argument);
		}

		internal override void SetupEvaluatedSuccessfully(object argument, Type parameterType)
		{
			Debug.Assert(this.Matches(argument, parameterType));
			Debug.Assert(CanCast(argument));

			this.Success?.Invoke((T)argument);
		}

		private static bool CanCast(object value)
		{
			if (value != null)
			{
				return value is T;
			}
			else
			{
				var t = typeof(T);
				return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
			}
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
