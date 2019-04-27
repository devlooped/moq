// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

using Moq.Properties;

namespace Moq
{
	/// <include file='Times.xdoc' path='docs/doc[@for="Times"]/*'/>
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

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.AtLeast"]/*'/>
		public static Times AtLeast(int callCount)
		{
			if (callCount < 1)
			{
				throw new ArgumentOutOfRangeException(nameof(callCount));
			}

			return new Times(Kind.AtLeast, callCount, int.MaxValue);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.AtLeastOnce"]/*'/>
		public static Times AtLeastOnce()
		{
			return new Times(Kind.AtLeastOnce, 1, int.MaxValue);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.AtMost"]/*'/>
		public static Times AtMost(int callCount)
		{
			if (callCount < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(callCount));
			}

			return new Times(Kind.AtMost, 0, callCount);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.AtMostOnce"]/*'/>
		public static Times AtMostOnce()
		{
			return new Times(Kind.AtMostOnce, 0, 1);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Between"]/*'/>
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

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Exactly"]/*'/>
		public static Times Exactly(int callCount)
		{
			if (callCount < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(callCount));
			}

			return new Times(Kind.Exactly, callCount, callCount);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Never"]/*'/>
		public static Times Never()
		{
			return new Times(Kind.Never, 0, 0);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Once"]/*'/>
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
		///   otherwise, <see langword="true"/>.
		/// </returns>
		public bool Equals(Times other)
		{
			var (from, to) = this;
			var (otherFrom, otherTo) = other;
			return from == otherFrom && to == otherTo;
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Equals"]/*'/>
		public override bool Equals(object obj)
		{
			return obj is Times other && this.Equals(other);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.GetHashCode"]/*'/>
		public override int GetHashCode()
		{
			var (from, to) = this;
			return from.GetHashCode() ^ to.GetHashCode();
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.op_Equality"]/*'/>
		public static bool operator ==(Times left, Times right)
		{
			return left.Equals(right);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.op_Inequality"]/*'/>
		public static bool operator !=(Times left, Times right)
		{
			return !left.Equals(right);
		}

		internal string GetExceptionMessage(int callCount)
		{
			var (from, to) = this;

			if (this.kind == Kind.BetweenExclusive)
			{
				--from;
				++to;
			}

			string message = null;
			switch (this.kind)
			{
				case Kind.AtLeast:          message = Resources.NoMatchingCallsAtLeast; break;
				case Kind.AtLeastOnce:      message = Resources.NoMatchingCallsAtLeastOnce; break;
				case Kind.AtMost:           message = Resources.NoMatchingCallsAtMost; break;
				case Kind.AtMostOnce:       message = Resources.NoMatchingCallsAtMostOnce; break;
				case Kind.BetweenExclusive: message = Resources.NoMatchingCallsBetweenExclusive; break;
				case Kind.BetweenInclusive: message = Resources.NoMatchingCallsBetweenInclusive; break;
				case Kind.Exactly:          message = Resources.NoMatchingCallsExactly; break;
				case Kind.Once:             message = Resources.NoMatchingCallsOnce; break;
				case Kind.Never:            message = Resources.NoMatchingCallsNever; break;
			}

			return string.Format(CultureInfo.CurrentCulture, message, from, to, callCount);
		}

		internal bool Verify(int callCount)
		{
			var (from, to) = this;
			return from <= callCount && callCount <= to;
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
