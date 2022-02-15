// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Moq
{
	internal sealed class SetupCollection : ISetupList
	{
		private List<Setup> setups;
		private HashSet<Expectation> activeSetups;

		public SetupCollection()
		{
			this.setups = new List<Setup>();
			this.activeSetups = new HashSet<Expectation>();
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
				if (!this.activeSetups.Add(setup.Expectation))
				{
					this.MarkOverriddenSetups();
				}
			}
		}

		private void MarkOverriddenSetups()
		{
			var visitedSetups = new HashSet<Expectation>();

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

		public void Clear()
		{
			lock (this.setups)
			{
				this.setups.Clear();
				this.activeSetups.Clear();
			}
		}

		public List<Setup> FindAll(Func<Setup, bool> predicate)
		{
			var setups = new List<Setup>();

			lock (this.setups)
			{
				for (int i = 0; i < this.setups.Count; ++i)
				{
					var setup = this.setups[i];
					if (setup.IsOverridden) continue;

					if (predicate(setup))
					{
						setups.Add(setup);
					}
				}
			}

			return setups;
		}

		public Setup FindLast(Func<Setup, bool> predicate)
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

					if (predicate(setup))
					{
						return setup;
					}
				}
			}

			return null;
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

		public IEnumerator<ISetup> GetEnumerator()
		{
			lock (this.setups)
			{
				IEnumerable<Setup> array = this.setups.ToArray();
				//                                    ^^^^^^^^^^
				// TODO: This is somewhat inefficient. We could avoid this array allocation by converting
				// this class to something like `InvocationCollection`, however this won't be trivial due to
				// the presence of a removal operation in `RemoveAllPropertyAccessorSetups`.
				return array.GetEnumerator();
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
	}
}
