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
using System.Globalization;
using Moq.Properties;

namespace Moq
{
	/// <include file='Times.xdoc' path='docs/doc[@for="Times"]/*'/>
	public struct Times
	{
		private TimesEvaluator evaluator;

		private Times(TimesEvaluator evaluator)
		{
			this.evaluator = evaluator;
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.AtLeast"]/*'/>
		public static Times AtLeast(int callCount)
		{
			Guard.NotOutOfRangeInclusive(() => callCount, callCount, 1, int.MaxValue);

			var evaluator = new RangeInclusiveBasedTimesEvaluator(callCount, int.MaxValue, Resources.NoMatchingCallsAtLeast);
			return new Times(evaluator);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.AtLeastOnce"]/*'/>
		public static Times AtLeastOnce()
		{
			var evaluator = new RangeInclusiveBasedTimesEvaluator(1, int.MaxValue, Resources.NoMatchingCallsAtLeastOnce);
			return new Times(evaluator);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.AtMost"]/*'/>
		public static Times AtMost(int callCount)
		{
			Guard.NotOutOfRangeInclusive(() => callCount, callCount, 0, int.MaxValue);

			var evaluator = new RangeInclusiveBasedTimesEvaluator(0, callCount, Resources.NoMatchingCallsAtMost);
			return new Times(evaluator);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.AtMostOnce"]/*'/>
		public static Times AtMostOnce()
		{
			var evaluator = new RangeInclusiveBasedTimesEvaluator(0, 1, Resources.NoMatchingCallsAtMostOnce);
			return new Times(evaluator);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Between"]/*'/>
		public static Times Between(int callCountFrom, int callCountTo, Range rangeKind)
		{
			if (rangeKind == Range.Exclusive)
			{
				Guard.NotOutOfRangeExclusive(() => callCountFrom, callCountFrom, 0, callCountTo);
				if (callCountTo - callCountFrom == 1)
				{
					throw new ArgumentOutOfRangeException("callCountTo");
				}

				var evaluator = new RangeExclusiveBasedTimesEvaluator(callCountFrom, callCountTo, Resources.NoMatchingCallsBetweenExclusive);
				return new Times(evaluator);
			}
			else
			{
				Guard.NotOutOfRangeInclusive(() => callCountFrom, callCountFrom, 0, callCountTo);

				var evaluator = new RangeInclusiveBasedTimesEvaluator(callCountFrom, callCountTo, Resources.NoMatchingCallsBetweenInclusive);
				return new Times(evaluator);
			}
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Exactly"]/*'/>
		public static Times Exactly(int callCount)
		{
			Guard.NotOutOfRangeInclusive(() => callCount, callCount, 0, int.MaxValue);

			var evaluator = new RangeInclusiveBasedTimesEvaluator(callCount, callCount, Resources.NoMatchingCallsExactly);
			return new Times(evaluator);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Matching"]/*'/>
		public static Times Matching(TimesEvaluator	evaluator)
		{
			Guard.NotNull(() => evaluator, evaluator);

			return new Times(evaluator);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Never"]/*'/>
		public static Times Never()
		{
			var evaluator = new RangeInclusiveBasedTimesEvaluator(0, 0, Resources.NoMatchingCallsNever);
			return new Times(evaluator);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Once"]/*'/>
		public static Times Once()
		{
			var evaluator = new RangeInclusiveBasedTimesEvaluator(1, 1, Resources.NoMatchingCallsOnce);
			return new Times(evaluator);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Equals"]/*'/>
		public override bool Equals(object obj)
		{
			if (obj is Times)
			{
				var other = (Times)obj;
				return this.evaluator.Equals(other.evaluator);
			}

			return false;
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.GetHashCode"]/*'/>
		public override int GetHashCode()
		{
			return this.evaluator.GetHashCode();
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
				Resources.NoMatchingCalls,
				failMessage,
				this.evaluator.GetExceptionMessage(callCount),
				expression);
		}

		internal bool Verify(int callCount)
		{
			return this.evaluator.Verify(callCount);
		}

		private sealed class RangeExclusiveBasedTimesEvaluator : TimesEvaluator
		{
			private int from;
			private int to;
			private string messageFormat;

			public RangeExclusiveBasedTimesEvaluator(int from, int to, string messageFormat)
			{
				this.from = from;
				this.to = to;
				this.messageFormat = messageFormat;
			}

			public override bool Equals(TimesEvaluator other)
			{
				return other is RangeExclusiveBasedTimesEvaluator rbe && this.from == rbe.from && this.to == rbe.to;
			}

			public override string GetExceptionMessage(int callCount)
			{
				return string.Format(
					CultureInfo.CurrentCulture,
					this.messageFormat,
					this.from,
					this.to,
					callCount);
			}

			public override bool Verify(int callCount)
			{
				return callCount > this.from && callCount < this.to;
			}
		}

		private sealed class RangeInclusiveBasedTimesEvaluator : TimesEvaluator
		{
			private int from;
			private int to;
			private string messageFormat;

			public RangeInclusiveBasedTimesEvaluator(int from, int to, string messageFormat)
			{
				this.from = from;
				this.to = to;
				this.messageFormat = messageFormat;
			}

			public override bool Equals(TimesEvaluator other)
			{
				return other is RangeInclusiveBasedTimesEvaluator rbe && this.from == rbe.from && this.to == rbe.to;
			}

			public override string GetExceptionMessage(int callCount)
			{
				return string.Format(
					CultureInfo.CurrentCulture,
					this.messageFormat,
					this.from,
					this.to,
					callCount);
			}

			public override bool Verify(int callCount)
			{
				return callCount >= this.from && callCount <= this.to;
			}
		}
	}
}