// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Moq
{
	/// <summary>
	///   An <see cref="ExpressionCompiler"/> compiles LINQ expression trees (<see cref="Expression"/>) to delegates.
	///   Whenever Moq needs to compile an expression tree, it uses the instance set up by <see cref="ExpressionCompiler.Instance"/>.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public abstract class ExpressionCompiler
	{
		private static ExpressionCompiler instance = DefaultExpressionCompiler.Instance;

		/// <summary>
		///   The default <see cref="ExpressionCompiler"/> instance, which simply delegates to the framework's <see cref="LambdaExpression.Compile"/>.
		/// </summary>
		public static ExpressionCompiler Default => DefaultExpressionCompiler.Instance;

		/// <summary>
		///   Gets or sets the <see cref="ExpressionCompiler"/> instance that Moq uses to compile <see cref="Expression"/> (LINQ expression trees).
		///   Defaults to <see cref="Default"/>.
		/// </summary>
		public static ExpressionCompiler Instance
		{
			get => instance;
			set => instance = value ?? throw new ArgumentNullException(nameof(value));
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="ExpressionCompiler"/> class.
		/// </summary>
		protected ExpressionCompiler()
		{
		}

		/// <summary>
		///   Compiles the specified LINQ expression tree.
		/// </summary>
		/// <param name="expression">The LINQ expression tree that should be compiled.</param>
		public abstract Delegate Compile(LambdaExpression expression);

		/// <summary>
		///   Compiles the specified LINQ expression tree.
		/// </summary>
		/// <typeparam name="TDelegate">The type of delegate to which the expression will be compiled.</typeparam>
		/// <param name="expression">The LINQ expression tree that should be compiled.</param>
		public abstract TDelegate Compile<TDelegate>(Expression<TDelegate> expression) where TDelegate : Delegate;
	}
}
