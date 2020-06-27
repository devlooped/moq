// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Moq
{
	internal sealed class InvocationCollection : IInvocationList
	{
		private Invocation[] invocations;

		private int capacity = 0;
		private int count = 0;

		private readonly object invocationsLock = new object();
		private readonly Mock owner;

		public InvocationCollection(Mock owner)
		{
			Debug.Assert(owner != null);

			this.owner = owner;
		}

		public int Count
		{
			get
			{
				lock (this.invocationsLock)
				{
					return count;
				}
			}
		}

		public IInvocation this[int index]
		{
			get
			{
				lock (this.invocationsLock)
				{
					if (this.count <= index || index < 0)
					{
						throw new IndexOutOfRangeException();
					}

					return this.invocations[index];
				}
			}
		}

		public void Add(Invocation invocation)
		{
			lock (this.invocationsLock)
			{
				if (this.count == this.capacity)
				{
					var targetCapacity = this.capacity == 0 ? 4 : (this.capacity * 2);
					Array.Resize(ref this.invocations, targetCapacity);
					this.capacity = targetCapacity;
				}

				this.invocations[this.count] = invocation;
				this.count++;
			}
		}

		public void Clear()
		{
			lock (this.invocationsLock)
			{
				// Replace the collection so readers with a reference to the old collection aren't interrupted
				this.invocations = null;
				this.count = 0;
				this.capacity = 0;

				this.owner.MutableSetups.Reset();
				// ^ TODO: Currently this could cause a deadlock as another lock will be taken inside this one!
			}
		}

		public Invocation[] ToArray()
		{
			lock (this.invocationsLock)
			{
				if (this.count == 0)
				{
					return new Invocation[0];
				}

				var result = new Invocation[this.count];

				Array.Copy(this.invocations, result, this.count);

				return result;
			}
		}

		public Invocation[] ToArray(Func<Invocation, bool> predicate)
		{
			lock (this.invocationsLock)
			{
				if (this.count == 0)
				{
					return new Invocation[0];
				}
				
				var result = new List<Invocation>(this.count);

				for (var i = 0; i < this.count; i++)
				{
					var invocation = this.invocations[i];
					if (predicate(invocation))
					{
						result.Add(invocation);
					}
				}

				return result.ToArray();
			}
		}

		public IEnumerator<IInvocation> GetEnumerator()
		{
			// Take local copies of collection and count so they are isolated from changes by other threads.
			Invocation[] collection;
			int count;

			lock (this.invocationsLock)
			{
				collection = this.invocations;
				count = this.count;
			}

			for (var i = 0; i < count; i++)
			{
				yield return collection[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
