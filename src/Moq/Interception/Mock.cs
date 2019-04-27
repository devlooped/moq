// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	partial class Mock : IInterceptor
	{
		void IInterceptor.Intercept(Invocation invocation)
		{
			if (HandleWellKnownMethods.Instance.Handle(invocation, this) == InterceptionAction.Stop)
			{
				return;
			}

			if (RecordInvocation.Instance.Handle(invocation, this) == InterceptionAction.Stop)
			{
				return;
			}

			if (FindAndExecuteMatchingSetup.Instance.Handle(invocation, this) == InterceptionAction.Stop)
			{
				return;
			}

			if (Return.Instance.Handle(invocation, this) == InterceptionAction.Stop)
			{
				return;
			}
		}
	}
}
