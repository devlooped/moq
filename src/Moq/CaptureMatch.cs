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
		private readonly Matcher matcher;

		/// <summary>
		/// Initializes an instance of the capture match.
		/// </summary>
		/// <param name="captureCallback">An action to run on captured value</param>
		public CaptureMatch(Action<T> captureCallback)
			: this(captureCallback, false)
		{
		}
		
		/// <summary>
		/// Initializes an instance of the capture match.
		/// </summary>
		/// <param name="captureCallback">An action to run on captured value</param>
		/// <param name="captureOnSuccessOnly">
		/// Indicates whether <paramref name="captureCallback"/> should be invoked on parameter evaluation
		/// or only after all parameters were successfully evaluated.
		/// </param>
		public CaptureMatch(Action<T> captureCallback, bool captureOnSuccessOnly)
			: this(new Matcher(captureCallback, null, captureOnSuccessOnly))
		{
		}

		/// <summary>
		/// Initializes an instance of the capture match.
		/// </summary>
		/// <param name="captureCallback">An action to run on captured value</param>
		/// <param name="predicate">A predicate used to filter captured parameters</param>
		public CaptureMatch(Action<T> captureCallback, Expression<Func<T, bool>> predicate)
			: this(captureCallback, predicate, false)
		{
		}
		
		/// <summary>
		/// Initializes an instance of the capture match.
		/// </summary>
		/// <param name="captureCallback">An action to run on captured value</param>
		/// <param name="predicate">A predicate used to filter captured parameters</param>
		/// <param name="captureOnSuccessOnly">
		/// Indicates whether <paramref name="captureCallback"/> should be invoked on parameter evaluation
		/// or only after all parameters were successfully evaluated.
		/// </param>
		public CaptureMatch(Action<T> captureCallback, Expression<Func<T, bool>> predicate, bool captureOnSuccessOnly)
			: this(new Matcher(captureCallback, predicate, captureOnSuccessOnly))
		{
		}

		private CaptureMatch(Matcher matcher)
			: base(matcher.Evaluate, matcher.RenderExpression)
		{
			this.matcher = matcher;
		}

		internal override void OnSuccess()
		{
			this.matcher.OnSuccess();
		}

		private class Matcher
		{
			private readonly Action<T> captureCallback;
			private readonly Func<T, bool> predicate;
			private readonly bool captureOnSuccessOnly;
			private T capturedValue;

			public Matcher(Action<T> captureCallback, Expression<Func<T, bool>> predicate, bool captureOnSuccessOnly)
			{
				this.captureCallback = captureCallback;

				if (predicate != null)
				{
					this.predicate = predicate.CompileUsingExpressionCompiler();
					this.RenderExpression = () => It.Is(predicate);
				}
				else
				{
					this.predicate = _ => true;
					this.RenderExpression = () => It.IsAny<T>();
				}
				
				this.captureOnSuccessOnly = captureOnSuccessOnly;
			}

			public Expression<Func<T>> RenderExpression { get; }

			public bool Evaluate(T value)
			{
				if (!this.predicate(value))
					return false;

				if (this.captureOnSuccessOnly)
					this.capturedValue = value;
				else
					this.captureCallback.Invoke(value);

				return true;
			}

			public void OnSuccess()
			{
				if (this.captureOnSuccessOnly)
					this.captureCallback.Invoke(this.capturedValue);
			}
		}
	}
}
