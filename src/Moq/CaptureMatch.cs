// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
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
		private static readonly Predicate<T> matchAllPredicate = _ => true;

		/// <summary>
		/// Initializes an instance of the capture match.
		/// </summary>
		/// <param name="captureCallback">An action to run on captured value</param>
		public CaptureMatch(Action<T> captureCallback)
			: base(matchAllPredicate, () => It.IsAny<T>(), captureCallback)
		{
		}

		/// <summary>
		/// Initializes an instance of the capture match.
		/// </summary>
		/// <param name="captureCallback">An action to run on captured value</param>
		/// <param name="predicate">A predicate used to filter captured parameters</param>
		public CaptureMatch(Action<T> captureCallback, Expression<Func<T, bool>> predicate)
			: base(BuildCondition(predicate), () => It.Is(predicate), captureCallback)
		{
		}

		private static Predicate<T> BuildCondition(Expression<Func<T, bool>> predicateExpression)
		{
			var predicate = predicateExpression.CompileUsingExpressionCompiler();
			return value => predicate.Invoke(value);
		}
	}
}
