// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

using Moq.Properties;

namespace Moq
{
	/// <summary>
	///   Defines the number of invocations allowed by a mocked method.
	/// </summary>
	public readonly struct Times : IEquatable<Times>
	{
		private readonly int from;
		private readonly int to;
		private readonly Kind kind;

		private Times(Kind kind, int from, int to)
		{
			this.from = from;
			this.to = to;
			this.kind = kind;
		}

		/// <summary>Deconstructs this instance.</summary>
		/// <param name="from">This output parameter will receive the minimum required number of calls satisfying this instance (i.e. the lower inclusive bound).</param>
		/// <param name="to">This output parameter will receive the maximum allowed number of calls satisfying this instance (i.e. the upper inclusive bound).</param>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void Deconstruct(out int from, out int to)
		{
			if (this.kind == default)
			{
				// This branch makes `default(Times)` equivalent to `Times.AtLeastOnce()`,
				// which is the implicit default across Moq's API for overloads that don't
				// accept a `Times` instance. While user code shouldn't use `default(Times)`
				// (but instead either specify `Times` explicitly or not at all), it is
				// easy enough to correct:

				Debug.Assert(this.kind == Kind.AtLeastOnce);

				from = 1;
				to = int.MaxValue;
			}
			else
			{
				from = this.from;
				to = this.to;
			}
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked <paramref name="callCount"/> times
		///   as minimum.
		/// </summary>
		/// <param name="callCount">The minimum number of times.</param>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times AtLeast(int callCount)
		{
			if (callCount < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(callCount));
			}

			return new Times(Kind.AtLeast, callCount, int.MaxValue);
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked one time as minimum.
		/// </summary>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times AtLeastOnce()
		{
			return new Times(Kind.AtLeastOnce, 1, int.MaxValue);
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked <paramref name="callCount"/> times
		///   as maximum.
		/// </summary>
		/// <param name="callCount">The maximum number of times.</param>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times AtMost(int callCount)
		{
			if (callCount < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(callCount));
			}

			return new Times(Kind.AtMost, 0, callCount);
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked one time as maximum.
		/// </summary>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times AtMostOnce()
		{
			return new Times(Kind.AtMostOnce, 0, 1);
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked between
		///   <paramref name="callCountFrom"/> and <paramref name="callCountTo"/> times.
		/// </summary>
		/// <param name="callCountFrom">The minimum number of times.</param>
		/// <param name="callCountTo">The maximum number of times.</param>
		/// <param name="rangeKind">The kind of range. See <see cref="Range"/>.</param>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times Between(int callCountFrom, int callCountTo, Range rangeKind)
		{
			if (rangeKind == Range.Exclusive)
			{
				if (callCountFrom <= 0 || callCountTo <= callCountFrom)
				{
					throw new ArgumentOutOfRangeException(nameof(callCountFrom));
				}

				if (callCountTo - callCountFrom == 1)
				{
					throw new ArgumentOutOfRangeException(nameof(callCountTo));
				}

				return new Times(Kind.BetweenExclusive, callCountFrom + 1, callCountTo - 1);
			}

			if (callCountFrom < 0 || callCountTo < callCountFrom)
			{
				throw new ArgumentOutOfRangeException(nameof(callCountFrom));
			}

			return new Times(Kind.BetweenInclusive, callCountFrom, callCountTo);
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked exactly
		///   <paramref name="callCount"/> times.
		/// </summary>
		/// <param name="callCount">The times that a method or property can be called.</param>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times Exactly(int callCount)
		{
			if (callCount < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(callCount));
			}

			return new Times(Kind.Exactly, callCount, callCount);
		}

		/// <summary>
		///   Specifies that a mocked method should not be invoked.
		/// </summary>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times Never()
		{
			return new Times(Kind.Never, 0, 0);
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked exactly one time.
		/// </summary>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times Once()
		{
			return new Times(Kind.Once, 1, 1);
		}

		/// <summary>
		///   Returns a value indicating whether this instance is equal to a specified <see cref="Times"/> value.
		/// </summary>
		/// <param name="other">A <see cref="Times"/> value to compare to this instance.</param>
		/// <returns>
		///   <see langword="true"/> if <paramref name="other"/> has the same value as this instance;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		public bool Equals(Times other)
		{
			var (from, to) = this;
			var (otherFrom, otherTo) = other;
			return from == otherFrom && to == otherTo;
		}

		/// <summary>
		///   Returns a value indicating whether this instance is equal to a specified <see cref="Times"/> value.
		/// </summary>
		/// <param name="obj">An object to compare to this instance.</param>
		/// <returns>
		///   <see langword="true"/> if <paramref name="obj"/> has the same value as this instance;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		public override bool Equals(object obj)
		{
			return obj is Times other && this.Equals(other);
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///  A hash code for this instance, suitable for use in hashing algorithms
		///  and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			var (from, to) = this;
			return from.GetHashCode() ^ to.GetHashCode();
		}

		/// <summary>
		///   Determines whether two specified <see cref="Times"/> objects have the same value.
		/// </summary>
		/// <param name="left">The first <see cref="Times"/>.</param>
		/// <param name="right">The second <see cref="Times"/>.</param>
		/// <returns>
		///   <see langword="true"/> if <paramref name="left"/> has the same value as <paramref name="right"/>;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		public static bool operator ==(Times left, Times right)
		{
			return left.Equals(right);
		}

		/// <summary>
		///   Determines whether two specified <see cref="Times"/> objects have different values.
		/// </summary>
		/// <param name="left">The first <see cref="Times"/>.</param>
		/// <param name="right">The second <see cref="Times"/>.</param>
		/// <returns>
		///   <see langword="true"/> if the value of <paramref name="left"/> is different from
		///   <paramref name="right"/>'s; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool operator !=(Times left, Times right)
		{
			return !left.Equals(right);
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return this.kind switch
			{
				Kind.AtLeastOnce      =>  "AtLeastOnce",
				Kind.AtLeast          => $"AtLeast({this.from})",
				Kind.AtMost           => $"AtMost({this.to})",
				Kind.AtMostOnce       =>  "AtMostOnce",
				Kind.BetweenExclusive => $"Between({this.from - 1}, {this.to + 1}, Exclusive)",
				Kind.BetweenInclusive => $"Between({this.from}, {this.to}, Inclusive)",
				Kind.Exactly          => $"Exactly({this.from})",
				Kind.Once             =>  "Once",
				Kind.Never            =>  "Never",
				_                     => throw new InvalidOperationException(),
			};
		}

		internal string GetExceptionMessage(int callCount)
		{
			var (from, to) = this;

			if (this.kind == Kind.BetweenExclusive)
			{
				--from;
				++to;
			}

			var message = this.kind switch
			{
				Kind.AtLeastOnce      => Resources.NoMatchingCallsAtLeastOnce,
				Kind.AtLeast          => Resources.NoMatchingCallsAtLeast,
				Kind.AtMost           => Resources.NoMatchingCallsAtMost,
				Kind.AtMostOnce       => Resources.NoMatchingCallsAtMostOnce,
				Kind.BetweenExclusive => Resources.NoMatchingCallsBetweenExclusive,
				Kind.BetweenInclusive => Resources.NoMatchingCallsBetweenInclusive,
				Kind.Exactly          => Resources.NoMatchingCallsExactly,
				Kind.Once             => Resources.NoMatchingCallsOnce,
				Kind.Never            => Resources.NoMatchingCallsNever,
				_                     => throw new InvalidOperationException(),
			};

			return string.Format(CultureInfo.CurrentCulture, message, from, to, callCount);
		}

		/// <summary>
		///   Checks whether the specified number of invocations matches the constraint described by this instance.
		/// </summary>
		/// <param name="count">The number of invocations to check.</param>
		/// <returns>
		///   <see langword="true"/> if <paramref name="count"/> matches the constraint described by this instance;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		public bool Validate(int count)
		{
			var (from, to) = this;
			return from <= count && count <= to;
		}

		private enum Kind
		{
			AtLeastOnce,
			AtLeast,
			AtMost,
			AtMostOnce,
			BetweenExclusive,
			BetweenInclusive,
			Exactly,
			Once,
			Never,
		}
	}
}
