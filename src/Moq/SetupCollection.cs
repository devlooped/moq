// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Moq
{
	internal sealed class SetupCollection
	{
		private List<Setup> setups;
		private uint overridden;  // bit mask for the first 32 setups flagging those known to be overridden
		private volatile bool hasEventSetup;

		public SetupCollection()
		{
			this.setups = new List<Setup>();
			this.overridden = 0U;
			this.hasEventSetup = false;
		}

		public bool HasEventSetup
		{
			get
			{
				return this.hasEventSetup;
			}
		}

		public void Add(Setup setup)
		{
			lock (this.setups)
			{
				if (setup.Method.IsEventAddAccessor() || setup.Method.IsEventRemoveAccessor())
				{
					this.hasEventSetup = true;
				}

				this.setups.Add(setup);
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
				this.overridden = 0U;
			}
		}

		public void Clear()
		{
			lock (this.setups)
			{
				this.setups.Clear();
				this.overridden = 0U;
				this.hasEventSetup = false;
			}
		}

		public Setup FindMatchFor(Invocation invocation)
		{
			// Fast path (no `lock`) when there are no setups:
			if (this.setups.Count == 0)
			{
				return null;
			}

			Setup matchingSetup = null;

			lock (this.setups)
			{
				// Iterating in reverse order because newer setups are more relevant than (i.e. override) older ones
				for (int i = this.setups.Count - 1; i >= 0; --i)
				{
					if (i < 32 && (this.overridden & (1U << i)) != 0) continue;

					var setup = this.setups[i];

					// the following conditions are repetitive, but were written that way to avoid
					// unnecessary expensive calls to `setup.Matches`; cheap tests are run first.
					if (matchingSetup == null && setup.Matches(invocation))
					{
						matchingSetup = setup;
						if (setup.Method == invocation.Method)
						{
							break;
						}
					}
					else if (setup.Method == invocation.Method && setup.Matches(invocation))
					{
						matchingSetup = setup;
						break;
					}
				}
			}

			return matchingSetup;
		}

		public IEnumerable<Setup> GetInnerMockSetups()
		{
			return this.ToArrayLive(setup => setup.ReturnsInnerMock(out _));
		}

		public void UninvokeAll()
		{
			lock (this.setups)
			{
				foreach (var setup in this.setups)
				{
					setup.Uninvoke();
				}
			}
		}

		public Setup[] ToArrayLive(Func<Setup, bool> predicate)
		{
			var matchingSetups = new Stack<Setup>();
			var visitedSetups = new HashSet<InvocationShape>();

			lock (this.setups)
			{
				// Iterating in reverse order because newer setups are more relevant than (i.e. override) older ones
				for (int i = this.setups.Count - 1; i >= 0; --i)
				{
					if (i < 32 && (this.overridden & (1U << i)) != 0) continue;

					var setup = this.setups[i];

					if (setup.Condition != null)
					{
						continue;
					}

					if (!visitedSetups.Add(setup.Expectation))
					{
						// A setup with the same expression has already been iterated over,
						// meaning that this older setup is an overridden one.
						if (i < 32) this.overridden |= 1U << i;
						continue;
					}

					if (predicate(setup))
					{
						matchingSetups.Push(setup);
					}
				}
			}

			return matchingSetups.ToArray();
		}
	}
}
