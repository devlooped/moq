// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

namespace Moq
{
	/// <summary>
	///   A <see cref="ExpressionReconstructor"/> reconstructs LINQ expression trees (<see cref="LambdaExpression"/>)
	///   from <see cref="Action"/> delegates. It is the counterpart to <see cref="ExpressionCompiler"/>.
	/// </summary>
	internal abstract class ExpressionReconstructor
	{
		private static ExpressionReconstructor instance = new ActionObserver();

		public static ExpressionReconstructor Instance
		{
			get => instance;
			set => instance = value ?? throw new ArgumentNullException(nameof(value));
		}

		protected ExpressionReconstructor()
		{
		}

		/// <summary>
		///   Reconstructs a <see cref="LambdaExpression"/> from the given <see cref="Action{T}"/> delegate.
		/// </summary>
		/// <param name="action">The <see cref="Action"/> delegate for which to reconstruct a LINQ expression tree.</param>
		/// <param name="ctorArgs">Arguments to pass to a parameterized constructor of <typeparamref name="T"/>. (Optional.)</param>
		public abstract Expression<Action<T>> ReconstructExpression<T>(Action<T> action, object[] ctorArgs = null);
	}
}
