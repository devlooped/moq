// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Moq
{
	internal readonly struct ImmutablePopOnlyStack<T>
	{
		private readonly T[] items;
		private readonly int index;

		public ImmutablePopOnlyStack(IEnumerable<T> items)
		{
			Debug.Assert(items != null);

			this.items = items.ToArray();
			this.index = 0;
		}

		private ImmutablePopOnlyStack(T[] items, int index)
		{
			Debug.Assert(items != null);
			Debug.Assert(0 <= index && index <= items.Length);

			this.items = items;
			this.index = index;
		}

		public bool Empty => this.index == this.items.Length;

		public T Pop(out ImmutablePopOnlyStack<T> stackBelowTop)
		{
			Debug.Assert(this.index < this.items.Length);

			var top = this.items[this.index];
			stackBelowTop = new ImmutablePopOnlyStack<T>(this.items, this.index + 1);
			return top;
		}
	}
}
