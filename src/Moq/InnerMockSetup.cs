// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	internal sealed class InnerMockSetup : SetupWithOutParameterSupport
	{
		private readonly object returnValue;

		public InnerMockSetup(InvocationShape expectation, object returnValue)
			: base(expectation)
		{
			this.returnValue = returnValue;
		}

		public override void Execute(Invocation invocation)
		{
			invocation.Return(this.returnValue);
		}

		public override bool TryGetReturnValue(out object returnValue)
		{
			returnValue = this.returnValue;
			return true;
		}

		public override MockException TryVerify()
		{
			return this.TryVerifyInnerMock(innerMock => innerMock.TryVerify());
		}

		public override MockException TryVerifyAll()
		{
			return this.TryVerifyInnerMock(innerMock => innerMock.TryVerifyAll());
		}
	}
}
