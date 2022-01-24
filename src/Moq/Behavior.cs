// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq
{
	internal abstract class Behavior
	{
		protected Behavior()
		{
		}

		public abstract void Execute(IInvocation invocation);
	}
}
