// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	partial class Mock : IInterceptor
	{
		void IInterceptor.Intercept(Invocation invocation)
		{
			if (HandleWellKnownMethods.Handle(invocation, this) == InterceptionAction.Stop)
			{
				return;
			}

			if (RecordInvocation.Handle(invocation, this) == InterceptionAction.Stop)
			{
				return;
			}

			if (FindAndExecuteMatchingSetup.Handle(invocation, this) == InterceptionAction.Stop)
			{
				return;
			}

			if (Return.Handle(invocation, this) == InterceptionAction.Stop)
			{
				return;
			}
		}
	}
}
