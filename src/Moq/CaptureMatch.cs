// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

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
			var predicateDelegate = predicate.CompileUsingExpressionCompiler();
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
