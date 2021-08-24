// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	/// <summary>
	///   Abstract base class for setups that target a single, specific method.
	/// </summary>
	internal abstract class MethodSetup : Setup
	{
		protected MethodSetup(Expression originalExpression, Mock mock, MethodExpectation expectation)
			: base(originalExpression, mock, expectation)
		{
		}

		public MethodInfo Method => ((MethodExpectation)this.Expectation).Method;
	}
}
