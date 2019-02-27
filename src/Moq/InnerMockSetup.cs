// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	internal sealed class InnerMockSetup : SetupWithOutParameterSupport, IDeterministicReturnValueSetup
	{
		private readonly object returnValue;

		public InnerMockSetup(MethodInfo method, IReadOnlyList<Expression> arguments, LambdaExpression expression, object returnValue)
			: base(method, arguments, expression)
		{
			this.returnValue = returnValue;
		}

		public object ReturnValue => this.returnValue;

		public override void Execute(Invocation invocation)
		{
			invocation.Return(this.returnValue);
		}

		public override void Verify()
		{
			this.VerifyInnerMock(innerMock => innerMock.Verify());
		}

		public override void VerifyAll()
		{
			this.VerifyInnerMock(innerMock => innerMock.VerifyAll());
		}
	}
}
