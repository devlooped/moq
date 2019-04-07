// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Globalization;
using Moq.Properties;

namespace Moq
{
	/// <summary>
	///   Defines the number of invocations allowed by a mocked method.
	/// </summary>
	public struct Times
	{
		private Func<int, bool> evaluator;
		private string messageFormat;
		private int from;
		private int to;

		private Times(Func<int, bool> evaluator, int from, int to, string messageFormat)
		{
			this.evaluator = evaluator;
			this.from = from;
			this.to = to;
			this.messageFormat = messageFormat;
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked <paramref name="callCount"/> times as minimum.
		/// </summary>
		/// <param name="callCount">The minimum number of times.</param>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times AtLeast(int callCount)
		{
			Guard.NotOutOfRangeInclusive(callCount, 1, int.MaxValue, nameof(callCount));

			return new Times(c => c >= callCount, callCount, int.MaxValue, Resources.NoMatchingCallsAtLeast);
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked one time as minimum.
		/// </summary>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times AtLeastOnce()
		{
			return new Times(c => c >= 1, 1, int.MaxValue, Resources.NoMatchingCallsAtLeastOnce);
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked <paramref name="callCount"/> time as maximum.
		/// </summary>
		/// <param name="callCount">The maximum number of times.</param>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times AtMost(int callCount)
		{
			Guard.NotOutOfRangeInclusive(callCount, 0, int.MaxValue, nameof(callCount));

			return new Times(c => c >= 0 && c <= callCount, 0, callCount, Resources.NoMatchingCallsAtMost);
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked one time as maximum.
		/// </summary>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times AtMostOnce()
		{
			return new Times(c => c >= 0 && c <= 1, 0, 1, Resources.NoMatchingCallsAtMostOnce);
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked between <paramref name="callCountFrom"/>
		///   and <paramref name="callCountTo"/> times.
		/// </summary>
		/// <param name="callCountFrom">The minimum number of times.</param>
		/// <param name="callCountTo">The maximum number of times.</param>
		/// <param name="rangeKind">The kind of range. See <see cref="Range"/>.</param>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times Between(int callCountFrom, int callCountTo, Range rangeKind)
		{
			if (rangeKind == Range.Exclusive)
			{
				Guard.NotOutOfRangeExclusive(callCountFrom, 0, callCountTo, nameof(callCountFrom));
				if (callCountTo - callCountFrom == 1)
				{
					throw new ArgumentOutOfRangeException("callCountTo");
				}

				return new Times(
					c => c > callCountFrom && c < callCountTo,
					callCountFrom,
					callCountTo,
					Resources.NoMatchingCallsBetweenExclusive);
			}

			Guard.NotOutOfRangeInclusive(callCountFrom, 0, callCountTo, nameof(callCountFrom));
			return new Times(
				c => c >= callCountFrom && c <= callCountTo,
				callCountFrom,
				callCountTo,
				Resources.NoMatchingCallsBetweenInclusive);
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked exactly <paramref name="callCount"/> times.
		/// </summary>
		/// <param name="callCount">The times that a method or property can be called.</param>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times Exactly(int callCount)
		{
			Guard.NotOutOfRangeInclusive(callCount, 0, int.MaxValue, nameof(callCount));

			return new Times(c => c == callCount, callCount, callCount, Resources.NoMatchingCallsExactly);
		}

		/// <summary>
		///   Specifies that a mocked method should not be invoked.
		/// </summary>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times Never()
		{
			return new Times(c => c == 0, 0, 0, Resources.NoMatchingCallsNever);
		}

		/// <summary>
		///   Specifies that a mocked method should be invoked exactly one time.
		/// </summary>
		/// <returns>An object defining the allowed number of invocations.</returns>
		public static Times Once()
		{
			return new Times(c => c == 1, 1, 1, Resources.NoMatchingCallsOnce);
		}

		/// <summary>
		///   Determines whether the specified <see cref="object"/> is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
		/// <returns>
		///   <see langword="true"/> if the specified <see cref="object"/> is equal to this instance;
		///   otherwise, <see langword="false"/>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj is Times)
			{
				var other = (Times)obj;
				return this.from == other.from && this.to == other.to;
			}

			return false;
		}

		/// <summary>
		///   Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
		/// </returns>
		public override int GetHashCode()
		{
			return this.from.GetHashCode() ^ this.to.GetHashCode();
		}

		/// <summary>
		///   Determines whether two specified <see cref="Times"/> objects have the same value.
		/// </summary>
		/// <param name="left">The first <see cref="Times"/>.</param>
		/// <param name="right">The second <see cref="Times"/>.</param>
		/// <returns>
		///   <see langword="true"/> if the value of <paramref name="left"/> is the same as
		///   the value of <paramref name="right"/>; otherwise, <see langword="false"/>.
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
		///   the value of <paramref name="right"/>; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool operator !=(Times left, Times right)
		{
			return !left.Equals(right);
		}

		internal string GetExceptionMessage(string failMessage, string expression, int callCount)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				this.messageFormat,
				failMessage,
				expression,
				this.from,
				this.to,
				callCount);
		}

		internal bool Verify(int callCount)
		{
			return this.evaluator(callCount);
		}
	}
}
