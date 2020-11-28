// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Moq
{
	internal sealed class SetupCollection : ISetupList
	{
		private List<Setup> setups;
		private HashSet<InvocationShape> activeInvocationShapes;

		public SetupCollection()
		{
			this.setups = new List<Setup>();
			this.activeInvocationShapes = new HashSet<InvocationShape>();
		}

		public int Count
		{
			get
			{
				lock (this.setups)
				{
					return this.setups.Count;
				}
			}
		}

		public ISetup this[int index]
		{
			get
			{
				lock (this.setups)
				{
					return this.setups[index];
				}
			}
		}

		public void Add(Setup setup)
		{
			lock (this.setups)
			{
				this.setups.Add(setup);
				if (!this.activeInvocationShapes.Add(setup.Expectation))
				{
					this.MarkOverriddenSetups();
				}
			}
		}

		private void MarkOverriddenSetups()
		{
			var visitedSetups = new HashSet<InvocationShape>();

			// Iterating in reverse order because newer setups are more relevant than (i.e. override) older ones
			for (int i = this.setups.Count - 1; i >= 0; --i)
			{
				var setup = this.setups[i];
				if (setup.IsOverridden || setup.IsConditional) continue;

				if (!visitedSetups.Add(setup.Expectation))
				{
					// A setup with the same expression has already been iterated over,
					// meaning that this older setup is an overridden one.
					setup.MarkAsOverridden();
				}
			}
		}

		public bool Any(Func<Setup, bool> predicate)
		{
			lock (this.setups)
			{
				return this.setups.Any(predicate);
			}
		}

		public void RemoveAllPropertyAccessorSetups()
		{
			// Fast path (no `lock`) when there are no setups:
			if (this.setups.Count == 0)
			{
				return;
			}

			lock (this.setups)
			{
				this.setups.RemoveAll(x => x.Method.IsPropertyAccessor());

				// NOTE: In the general case, removing a setup means that some overridden setups might no longer
				// be shadowed, and their `IsOverridden` flag should go back from `true` to `false`.
				//
				// In this particular case however, we don't need to worry about this because we are categorically
				// removing all property accessors, and they could only have overridden other property accessors.
			}
		}

		public void Clear()
		{
			lock (this.setups)
			{
				this.setups.Clear();
				this.activeInvocationShapes.Clear();
			}
		}

		public Setup FindMatchFor(Invocation invocation)
		{
			// Fast path (no `lock`) when there are no setups:
			if (this.setups.Count == 0)
			{
				return null;
			}

			lock (this.setups)
			{
				// Iterating in reverse order because newer setups are more relevant than (i.e. override) older ones
				for (int i = this.setups.Count - 1; i >= 0; --i)
				{
					var setup = this.setups[i];
					if (setup.IsOverridden) continue;

					if (setup.Matches(invocation))
					{
						return setup;
					}
				}
			}

			return null;
		}

		public IEnumerable<Setup> GetInnerMockSetups()
		{
			return this.ToArray(setup => !setup.IsOverridden && !setup.IsConditional && setup.InnerMock != null);
		}

		public void Reset()
		{
			lock (this.setups)
			{
				foreach (var setup in this.setups)
				{
					setup.Reset();
				}
			}
		}

		public IReadOnlyList<Setup> ToArray()
		{
			lock (this.setups)
			{
				return this.setups.ToArray();
			}
		}

		public IReadOnlyList<Setup> ToArray(Func<Setup, bool> predicate)
		{
			var matchingSetups = new Stack<Setup>();

			lock (this.setups)
			{
				// Iterating in reverse order because newer setups are more relevant than (i.e. override) older ones
				for (int i = this.setups.Count - 1; i >= 0; --i)
				{
					var setup = this.setups[i];
					if (predicate(setup))
					{
						matchingSetups.Push(setup);
					}
				}
			}

			return matchingSetups.ToArray();
		}

		public IEnumerator<ISetup> GetEnumerator()
		{
			return this.ToArray().GetEnumerator();
			//         ^^^^^^^^^^
			// TODO: This is somewhat inefficient. We could avoid this array allocation by converting
			// this class to something like `InvocationCollection`, however this won't be trivial due to
			// the presence of a removal operation in `RemoveAllPropertyAccessorSetups`.
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
}
