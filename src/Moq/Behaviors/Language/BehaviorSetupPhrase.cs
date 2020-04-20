// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;

namespace Moq.Behaviors.Language
{
	internal sealed class BehaviorSetupPhrase : IBehaviorSetupResult
	{
		private readonly BehaviorSetup setup;

		public BehaviorSetupPhrase(BehaviorSetup setup)
		{
			Debug.Assert(setup != null);

			this.setup = setup;
		}

		public void Verifiable()
		{
			setup.MarkAsVerifiable();
		}

		public void Verifiable(string failMessage)
		{
			Guard.NotNull(failMessage, nameof(failMessage));

			setup.MarkAsVerifiable();
			setup.SetFailMessage(failMessage);
		}
	}
}
