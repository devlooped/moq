//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
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
using System.Linq.Expressions;

namespace Moq
{
	/// <summary>
	/// Allows creation custom matchers that can be used on setups to capture parameter values.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class CaptureMatch<T> : Match<T>
	{
		/// <summary>
		/// Initializes an instance of the capture match.
		/// </summary>
		/// <param name="captureCallback">An action to run on captured value</param>
		public CaptureMatch(Action<T> captureCallback)
			: base(CreatePredicate(captureCallback), () => It.IsAny<T>())
		{
		}

		/// <summary>
		/// Initializes an instance of the capture match.
		/// </summary>
		/// <param name="captureCallback">An action to run on captured value</param>
		/// <param name="predicate">A predicate used to filter captured parameters</param>
		public CaptureMatch(Action<T> captureCallback, Expression<Func<T, bool>> predicate)
			: base(CreatePredicate(captureCallback, predicate), () => It.Is(predicate))
		{
		}

		private static Predicate<T> CreatePredicate(Action<T> captureCallback)
		{
			return value =>
			{
				captureCallback.Invoke(value);
				return true;
			};
		}

		private static Predicate<T> CreatePredicate(Action<T> captureCallback, Expression<Func<T, bool>> predicate)
		{
			var predicateDelegate = predicate.Compile();
			return value =>
			{
				var matches = predicateDelegate.Invoke(value);
				if (matches)
					captureCallback.Invoke(value);

				return matches;
			};
		}
	}
}
