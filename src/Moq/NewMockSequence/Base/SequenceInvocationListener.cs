// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Moq
{
	internal class SequenceInvocationListener
	{
		public event EventHandler<SequenceInvocation> NewInvocationEvent;
		private readonly List<Mock> listenedToMocks = new List<Mock>();
		private readonly Mock[] mocks;
		internal List<SequenceInvocation> SequenceInvocations { get; } = new List<SequenceInvocation>();

		public SequenceInvocationListener(Mock[] mocks)
		{
			this.mocks = mocks;
		}

		internal void ListenForInvocations()
		{
			ListenForInvocations(mocks);
		}

		internal void ListenForInvocations(IEnumerable<Mock> mocks)
		{
			foreach (var mock in mocks)
			{
				ListenForInvocation(mock);
			}
		}

		private void ListenForInvocation(Mock mock)
		{
			ListenForInvocations(mock.MutableSetups.Where(s => s.InnerMock != null).Select(s => s.InnerMock));
			if (!listenedToMocks.Contains(mock))
			{
				mock.AddInvocationListener(invocation => NewInvocation(new SequenceInvocation(mock, invocation)));
				listenedToMocks.Add(mock);
			}
		}

		private void NewInvocation(SequenceInvocation sequenceInvocation)
		{
			SequenceInvocations.Add(sequenceInvocation);
			NewInvocationEvent?.Invoke(this, sequenceInvocation);

		}

	}

}
