// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Threading.Tasks;

namespace Moq.Async
{
	internal sealed class ValueTaskFactory : AwaitableFactory<ValueTask>
	{
		public static readonly ValueTaskFactory Instance = new ValueTaskFactory();

		private ValueTaskFactory()
		{
		}

		public override ValueTask CreateCompleted()
		{
			return default;
		}
	}
}
