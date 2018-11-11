// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	partial class Mock : IInterceptor
	{
		private static InterceptionAspect[] aspects = new InterceptionAspect[]
		{
			HandleWellKnownMethods.Instance,
			RecordInvocation.Instance,
			FindAndExecuteMatchingSetup.Instance,
			Return.Instance,
		};

		void IInterceptor.Intercept(Invocation invocation)
		{
			foreach (var aspect in aspects)
			{
				if (aspect.Handle(invocation, this) == InterceptionAction.Stop)
				{
					break;
				}
			}
		}
	}
}
