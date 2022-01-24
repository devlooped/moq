// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{
	internal sealed class ReturnBase : Behavior
	{
		public static readonly ReturnBase Instance = new ReturnBase();

		private ReturnBase()
		{
		}

		public override void Execute(IInvocation invocation)
		{
			invocation.ReturnValue = invocation.CallBase();
		}
	}
}
