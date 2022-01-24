// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

namespace Moq.Behaviors
{
	internal sealed class LimitInvocationCount : Behavior
	{
		private readonly MethodCall setup;
		private readonly int maxCount;
		private int count;

		public LimitInvocationCount(MethodCall setup, int maxCount)
		{
			this.setup = setup;
			this.maxCount = maxCount;
			this.count = 0;
		}

		public void Reset()
		{
			this.count = 0;
		}

		public override void Execute(IInvocation invocation)
		{
			++this.count;

			if (this.count > this.maxCount)
			{
				if (this.maxCount == 1)
				{
					throw MockException.MoreThanOneCall(this.setup, this.count);
				}
				else
				{
					throw MockException.MoreThanNCalls(this.setup, this.maxCount, this.count);
				}
			}
		}
	}
}
