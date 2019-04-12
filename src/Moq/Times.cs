// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Globalization;
using Moq.Properties;

namespace Moq
{
	/// <include file='Times.xdoc' path='docs/doc[@for="Times"]/*'/>
	public struct Times
	{
		private int from;
		private int to;
		private Kind kind;

		private Times(Kind kind, int from, int to)
		{
			this.from = from;
			this.to = to;
			this.kind = kind;
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.AtLeast"]/*'/>
		public static Times AtLeast(int callCount)
		{
			Guard.NotOutOfRangeInclusive(callCount, 1, int.MaxValue, nameof(callCount));

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
			Guard.NotOutOfRangeInclusive(callCount, 0, int.MaxValue, nameof(callCount));

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
				Guard.NotOutOfRangeExclusive(callCountFrom, 0, callCountTo, nameof(callCountFrom));
				if (callCountTo - callCountFrom == 1)
				{
					throw new ArgumentOutOfRangeException("callCountTo");
				}

				return new Times(Kind.BetweenExclusive, callCountFrom + 1, callCountTo - 1);
			}

			Guard.NotOutOfRangeInclusive(callCountFrom, 0, callCountTo, nameof(callCountFrom));
			return new Times(Kind.BetweenInclusive, callCountFrom, callCountTo);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Exactly"]/*'/>
		public static Times Exactly(int callCount)
		{
			Guard.NotOutOfRangeInclusive(callCount, 0, int.MaxValue, nameof(callCount));

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

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Equals"]/*'/>
		public override bool Equals(object obj)
		{
			if (obj is Times)
			{
				var other = (Times)obj;
				return this.from == other.from && this.to == other.to;
			}

			return false;
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.GetHashCode"]/*'/>
		public override int GetHashCode()
		{
			return this.from.GetHashCode() ^ this.to.GetHashCode();
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

		internal string GetExceptionMessage(string failMessage, string expression, int callCount)
		{
			var from = this.kind == Kind.BetweenExclusive ? this.from - 1 : this.from;
			var to   = this.kind == Kind.BetweenExclusive ? this.to   + 1 : this.to;

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

			return string.Format(
				CultureInfo.CurrentCulture,
				message,
				failMessage,
				expression,
				from,
				to,
				callCount);
		}

		internal bool Verify(int callCount)
		{
			return this.from <= callCount && callCount <= this.to;
		}

		private enum Kind
		{
			AtLeast,
			AtLeastOnce,
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
