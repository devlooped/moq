//Copyright (c) 2007. Clarius Consulting, Manas Technology Solutions, InSTEDD
//https://github.com/moq/moq4
//All rights reserved.

//Redistribution and use in source and binary forms, 
//with or without modification, are permitted provided 
//that the following conditions are met:

//    * Redistributions of source code must retain the 
//    above copyright notice, this list of conditions and 
//    the following disclaimer.

//    * Redistributions in binary form must reproduce 
//    the above copyright notice, this list of conditions 
//    and the following disclaimer in the documentation 
//    and/or other materials provided with the distribution.

//    * Neither the name of Clarius Consulting, Manas Technology Solutions or InSTEDD nor the 
//    names of its contributors may be used to endorse 
//    or promote products derived from this software 
//    without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
//CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
//INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
//MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
//SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
//INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
//WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
//NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
//SUCH DAMAGE.

//[This is the BSD license, see
// http://www.opensource.org/licenses/bsd-license.php]

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

		public Setup[] ToArray()
		{
			lock (this.setups)
			{
				return this.setups.ToArray();
			}
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

					var expr = setup.SetupExpression.PartialMatcherAwareEval();
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
