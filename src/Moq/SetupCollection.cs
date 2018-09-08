// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Moq
{
	internal sealed class SetupCollection
	{
		// Using a stack has the advantage that enumeration returns the items in reverse order (last added to first added).
		// This helps in implementing the rule that "given two matching setups, the last one wins."
		private Stack<Setup> setups;

		public SetupCollection()
		{
			this.setups = new Stack<Setup>();
		}

		public void Add(Setup setup)
		{
			lock (this.setups)
			{
				this.setups.Push(setup);
			}
		}

		public bool Any(Func<Setup, bool> predicate)
		{
			lock (this.setups)
			{
				return this.setups.Any(predicate);
			}
		}

		public void Clear()
		{
			lock (this.setups)
			{
				this.setups.Clear();
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
				foreach (var setup in this.setups)
				{
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

		public Setup[] ToArrayLive(Func<Setup, bool> predicate)
		{
			var matchingSetups = new Stack<Setup>();

			// The following verification logic will remember each processed setup so that duplicate setups
			// (that is, setups overridden by later setups with an equivalent expression) can be detected.
			// To speed up duplicate detection, they are partitioned according to the method they target.
			var visitedSetupsPerMethod = new Dictionary<MethodInfo, List<Expression>>();

			lock (this.setups)
			{
				foreach (var setup in this.setups)
				{
					if (setup.Condition != null)
					{
						continue;
					}

					List<Expression> visitedSetupsForMethod;
					if (!visitedSetupsPerMethod.TryGetValue(setup.Method, out visitedSetupsForMethod))
					{
						visitedSetupsForMethod = new List<Expression>();
						visitedSetupsPerMethod.Add(setup.Method, visitedSetupsForMethod);
					}

					var expr = setup.Expression.PartialMatcherAwareEval();
					if (visitedSetupsForMethod.Any(vc => ExpressionComparer.Default.Equals(vc, expr)))
					{
						continue;
					}

					if (predicate(setup))
					{
						matchingSetups.Push(setup);
					}

					visitedSetupsForMethod.Add(expr);
				}
			}

			return matchingSetups.ToArray();
		}
	}
}
