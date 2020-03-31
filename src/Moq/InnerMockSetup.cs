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

		public override bool IsVerifiable => true;

		protected override void ExecuteCore(Invocation invocation)
		{
			invocation.Return(this.returnValue);
		}

		public override bool TryGetReturnValue(out object returnValue)
		{
			returnValue = this.returnValue;
			return true;
		}

		public override void Reset()
		{
			if (this.ReturnsInnerMock(out var innerMock))
			{
				innerMock.MutableSetups.Reset();
			}
		}
	}
}
