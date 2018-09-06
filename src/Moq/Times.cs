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

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.AtLeast"]/*'/>
		public static Times AtLeast(int callCount)
		{
			Guard.NotOutOfRangeInclusive(callCount, 1, int.MaxValue, nameof(callCount));

			return new Times(c => c >= callCount, callCount, int.MaxValue, Resources.NoMatchingCallsAtLeast);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.AtLeastOnce"]/*'/>
		public static Times AtLeastOnce()
		{
			return new Times(c => c >= 1, 1, int.MaxValue, Resources.NoMatchingCallsAtLeastOnce);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.AtMost"]/*'/>
		public static Times AtMost(int callCount)
		{
			Guard.NotOutOfRangeInclusive(callCount, 0, int.MaxValue, nameof(callCount));

			return new Times(c => c >= 0 && c <= callCount, 0, callCount, Resources.NoMatchingCallsAtMost);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.AtMostOnce"]/*'/>
		public static Times AtMostOnce()
		{
			return new Times(c => c >= 0 && c <= 1, 0, 1, Resources.NoMatchingCallsAtMostOnce);
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

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Exactly"]/*'/>
		public static Times Exactly(int callCount)
		{
			Guard.NotOutOfRangeInclusive(callCount, 0, int.MaxValue, nameof(callCount));

			return new Times(c => c == callCount, callCount, callCount, Resources.NoMatchingCallsExactly);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Never"]/*'/>
		public static Times Never()
		{
			return new Times(c => c == 0, 0, 0, Resources.NoMatchingCallsNever);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Once"]/*'/>
		public static Times Once()
		{
			return new Times(c => c == 1, 1, 1, Resources.NoMatchingCallsOnce);
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
