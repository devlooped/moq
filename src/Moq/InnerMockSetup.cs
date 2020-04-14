// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Diagnostics;

namespace Moq
{
	internal sealed class InnerMockSetup : SetupWithOutParameterSupport
	{
		private readonly object returnValue;

		public InnerMockSetup(IFluentSetup originalSetup, Mock mock, InvocationShape expectation, object returnValue)
			: base(originalSetup, mock, expectation)
		{
			Debug.Assert(Unwrap.ResultIfCompletedTask(returnValue) is IMocked);

			this.returnValue = returnValue;

			this.MarkAsVerifiable();
		}

		protected override void ExecuteCore(Invocation invocation)
		{
			invocation.Return(this.returnValue);
		}

		public override bool TryGetReturnValue(out object returnValue)
		{
			returnValue = this.returnValue;
			return true;
		}

		protected override void ResetCore()
		{
			this.InnerMock.MutableSetups.Reset();
		}
	}
}
