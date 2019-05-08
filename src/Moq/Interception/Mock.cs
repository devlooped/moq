// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	partial class Mock : IInterceptor
	{
		void IInterceptor.Intercept(Invocation invocation)
		{
			if (HandleWellKnownMethods.Handle(invocation, this))
			{
				return;
			}

			if (HandleEventSubscription.Handle(invocation, this))
			{
				return;
			}

			if (Switches.HasFlag(Switches.AutoSetupProperties))
			{
				HandleAutoSetupProperties.Handle(invocation, this);
			}

			RecordInvocation.Handle(invocation, this);

			if (FindAndExecuteMatchingSetup.Handle(invocation, this))
			{
				return;
			}

			Return.Handle(invocation, this);
		}
	}
}
