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
		private Func<int, bool> evaluator;
		private string messageFormat;
		private int from;
		private int to;
        private Range kind; // TODO: is the Times a struct as an smallclass optimization? adding a field makes it bigger..

		private Times(Func<int, bool> evaluator, int from, int to, string messageFormat)
		{
			this.evaluator = evaluator;
			this.from = from;
			this.to = to;
			this.messageFormat = messageFormat;
            this.kind = 0;
		}
        private Times(Func<int, bool> evaluator, int from, int to, string messageFormat, Range kind)
        {
            this.evaluator = evaluator;
            this.from = from;
            this.to = to;
            this.messageFormat = messageFormat;
            this.kind = kind;
        }

        /// <include file='Times.xdoc' path='docs/doc[@for="Times.Add"]/*'/>
        public Times Add(int by)
        {
            if (by < 0)
                throw new ArgumentException("Subtracting expected invocaction counts makes no sense");

            var newfrom = (int)Math.Max(0, Math.Min(int.MaxValue, (long)from + (long)by));
            var newto = (int)Math.Max(0, Math.Min(int.MaxValue, (long)to + (long)by));

            if (newto < to || newfrom < from)
                throw new OverflowException();

            if (to == int.MaxValue && from == 0) // was in mode: any
                return Times.AcceptAny();

            else if (to == int.MaxValue && from == int.MaxValue) // was in mode: none
                return Times.AcceptNone();

            else if (to == int.MaxValue) // was in mode: atleast
                if (newfrom == 0)
                    return Times.AcceptAny();
                else if (newfrom == 1)
                    return Times.AtLeastOnce();
                else
                    return Times.AtLeast(newfrom);

            else if (from == 0) // was in mode: atmost
                if (newto == int.MaxValue)
                    return Times.AcceptAny();
                else if (newto == 1)
                    return Times.AtMostOnce();
                else
                    return Times.AtMost(newto);

            else if (newfrom == newto)
                if (newto == 0)
                    return Times.Never();
                else if (newto == 1)
                    return Times.Once();
                else
                    return Times.Exactly(newto);

            else
                return Times.Between(newfrom, newto, kind);
        }

        /// <include file='Times.xdoc' path='docs/doc[@for="Times.AcceptAny"]/*'/>
        public static Times AcceptAny()
        {
            return new Times(c => true, 0, int.MaxValue, "This case always succedes. You shouldn't be able to see this message."); // TODO: resourcize;
        }

        /// <include file='Times.xdoc' path='docs/doc[@for="Times.AcceptNone"]/*'/>
        public static Times AcceptNone()
        {
            return new Times(c => false, int.MaxValue, int.MaxValue, "This case always fails"); // TODO: resourcize
        }

        /// <include file='Times.xdoc' path='docs/doc[@for="Times.AtLeast"]/*'/>
		public static Times AtLeast(int callCount)
		{
			Guard.NotOutOfRangeInclusive(() => callCount, callCount, 1, int.MaxValue);

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
			Guard.NotOutOfRangeInclusive(() => callCount, callCount, 0, int.MaxValue);

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
				Guard.NotOutOfRangeExclusive(() => callCountFrom, callCountFrom, 0, callCountTo);
				if (callCountTo - callCountFrom == 1)
				{
					throw new ArgumentOutOfRangeException("callCountTo");
				}

				return new Times(
					c => c > callCountFrom && c < callCountTo,
					callCountFrom,
					callCountTo,
					Resources.NoMatchingCallsBetweenExclusive,
                    rangeKind);
			}

			Guard.NotOutOfRangeInclusive(() => callCountFrom, callCountFrom, 0, callCountTo);
			return new Times(
				c => c >= callCountFrom && c <= callCountTo,
				callCountFrom,
				callCountTo,
				Resources.NoMatchingCallsBetweenInclusive,
                rangeKind);
		}

		/// <include file='Times.xdoc' path='docs/doc[@for="Times.Exactly"]/*'/>
		public static Times Exactly(int callCount)
		{
			Guard.NotOutOfRangeInclusive(() => callCount, callCount, 0, int.MaxValue);

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