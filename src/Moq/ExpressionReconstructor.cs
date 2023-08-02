// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Linq.Expressions;

namespace Moq
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal abstract class ExpressionReconstructor
    After:
        abstract class ExpressionReconstructor
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal abstract class ExpressionReconstructor
    After:
        abstract class ExpressionReconstructor
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal abstract class ExpressionReconstructor
    After:
        abstract class ExpressionReconstructor
    */
    /// <summary>
    ///   A <see cref="ExpressionReconstructor"/> reconstructs LINQ expression trees (<see cref="LambdaExpression"/>)
    ///   from <see cref="Action"/> delegates. It is the counterpart to <see cref="ExpressionCompiler"/>.
    /// </summary>
    abstract class ExpressionReconstructor

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private static ExpressionReconstructor instance = new ActionObserver();
    After:
            static ExpressionReconstructor instance = new ActionObserver();
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private static ExpressionReconstructor instance = new ActionObserver();
    After:
            static ExpressionReconstructor instance = new ActionObserver();
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private static ExpressionReconstructor instance = new ActionObserver();
    After:
            static ExpressionReconstructor instance = new ActionObserver();
    */
    {
        static ExpressionReconstructor instance = new ActionObserver();

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
