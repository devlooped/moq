// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	/// <summary>
	///   A setup which returns a fixed return value.
	/// </summary>
	internal class FixedReturnValueSetup : SetupWithOutParameterSupport
	{
		private readonly object returnValue;

		public FixedReturnValueSetup(MethodInfo method, IReadOnlyList<Expression> arguments, LambdaExpression expression, object returnValue)
			: base(method, arguments, expression)
		{
			this.returnValue = returnValue;
		}

		public object ReturnValue => this.returnValue;

		public override void Execute(Invocation invocation)
		{
			invocation.Return(this.returnValue);
		}

		public override bool TryVerifyAll()
		{
			return true;
		}
	}
}
