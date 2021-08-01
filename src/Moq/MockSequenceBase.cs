// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Moq
{
	/// <summary>
	/// 
	/// </summary>
	public interface ITrackedSetup<TContext>
	{
		/// <summary>
		/// 
		/// </summary>
		ISetup Setup { get; }

		/// <summary>
		/// 
		/// </summary>
		IReadOnlyList<int> ExecutionIndices { get; }

		/// <summary>
		/// 
		/// </summary>
		IReadOnlyList<ISequenceSetup<TContext>> SequenceSetups { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TContext"></typeparam>
	public interface ISequenceSetup<TContext>
	{
		/// <summary>
		/// 
		/// </summary>
		ITrackedSetup<TContext> TrackedSetup { get; }

		/// <summary>
		/// 
		/// </summary>
		TContext Context { get; }

		/// <summary>
		/// 
		/// </summary>
		int SetupIndex { get; }

		/// <summary>
		/// 
		/// </summary>
		int ExecutionCount { get; set; }

	}

	internal class SequenceSetup<TContext> : ISequenceSetup<TContext>
	{
		public ITrackedSetup<TContext> TrackedSetup { get; set; }
		public TContext Context { get; set; }
		public int SetupIndex { get; set; }
		public int ExecutionCount { get; set; }
	}

	internal class TrackedSetup<TContext> : ITrackedSetup<TContext>
	{
		public Setup SetupInternal { get; set; }
		public ISetup Setup => SetupInternal;
		internal readonly List<int> executionIndicesInternal = new List<int>();
		public IReadOnlyList<int> ExecutionIndices => executionIndicesInternal;
		internal readonly List<ISequenceSetup<TContext>> sequenceSetupsInternal = new List<ISequenceSetup<TContext>>();
		public IReadOnlyList<ISequenceSetup<TContext>> SequenceSetups => sequenceSetupsInternal;
	}

	/// <summary>
	/// 
	/// </summary>
	public class SequenceInvocation
	{
		/// <summary>
		/// 
		/// </summary>
		public Mock Mock { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public IInvocation Invocation { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool Matched { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public partial class SequenceException : Exception
	{
		internal SequenceException() { }
		internal SequenceException(string message) : base(message) { }
	}

	/// <summary>
	/// 
	/// </summary>
	public class StrictSequenceException : SequenceException
	{
		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<SequenceInvocation> UnmatchedSequenceInvocations { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public abstract class MockSequenceBase<TContext>
	{
		private readonly bool strict;
		private int setupCount = -1;
		private int executionCount = -1;
		private readonly Mock[] mocks;
		private readonly List<Mock> listenedToMocks = new List<Mock>();
		private readonly List<ITrackedSetup<TContext>> trackedSetups = new List<ITrackedSetup<TContext>>();
		private readonly List<ISequenceSetup<TContext>> sequenceSetups = new List<ISequenceSetup<TContext>>();
		internal readonly List<SequenceInvocation> sequenceInvocations = new List<SequenceInvocation>();
		internal readonly List<TrackedSetup<TContext>> allTrackedSetups = new List<TrackedSetup<TContext>>();
		/// <summary>
		/// 
		/// </summary>
		protected IReadOnlyList<ITrackedSetup<TContext>> TrackedSetups => trackedSetups;
		/// <summary>
		/// 
		/// </summary>
		protected IReadOnlyList<ISequenceSetup<TContext>> SequenceSetups => sequenceSetups;
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="strict"></param>
		/// <param name="mocks"></param>
		public MockSequenceBase(bool strict,params Mock[] mocks)
		{
			if(mocks.Length == 0)
			{
				throw new ArgumentException("No mocks", nameof(mocks));
			}
			this.mocks = mocks;
			this.strict = strict;
			ListenForInvocations();
		}

		private void ListenForInvocations()
		{
			ListenForInvocations(mocks);
		}

		private void ListenForInvocations(IEnumerable<Mock> mocks)
		{
			foreach (var mock in mocks)
			{
				ListenForInvocation(mock);
			}
		}

		private void ListenForInvocation(Mock mock)
		{
			ListenForInvocations(mock.MutableSetups.Where(s => s.InnerMock != null).Select(s=>s.InnerMock));
			if (!listenedToMocks.Contains(mock))
			{
				mock.AddInvocationListener(invocation => sequenceInvocations.Add(new SequenceInvocation { Invocation = invocation, Mock = mock }));
				listenedToMocks.Add(mock);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="trackedSetupCallback"></param>
		protected void InterceptSetup(Action setup, Func<ISequenceSetup<TContext>,TContext> trackedSetupCallback)
		{
			setupCount++;
			List<List<SetupWithDepth>> allSetupsBefore = mocks.Select(m => MoqSetupFinder.GetAllSetups(m)).ToList();
			setup();
			List<List<SetupWithDepth>> allSetupsAfter = mocks.Select(m => MoqSetupFinder.GetAllSetups(m)).ToList();
			for(var i = 0; i < allSetupsBefore.Count; i++)
			{
				var result = allSetupsBefore[i].NewSetups(allSetupsAfter[i]);
				if (!result.NoChange)
				{
					var terminalSetup = result.TerminalSetup;
					var setupsExceptTerminal = result.NewSetups.Except(new SetupWithDepth[] { terminalSetup });
					ListenForInvocations(result.NewSetups.Select(s => s.Setup.Mock));
					var terminalTrackedSetup = ResolveSetups(setupsExceptTerminal, terminalSetup);
					SetCondition(terminalTrackedSetup);
					var sequenceSetup = new SequenceSetup<TContext> { TrackedSetup = terminalTrackedSetup, SetupIndex = setupCount };
					var context = trackedSetupCallback(sequenceSetup);
					sequenceSetup.Context = context;
					sequenceSetups.Add(sequenceSetup);
					terminalTrackedSetup.sequenceSetupsInternal.Add(sequenceSetup);

					return;
				}

			}

			throw new ArgumentException("No setup performed",nameof(setup));
		}

		private TrackedSetup<TContext> ResolveSetups(IEnumerable<SetupWithDepth> setupsExceptTerminal,SetupWithDepth terminal)
		{
			foreach(var setup in setupsExceptTerminal)
			{
				ResolveSetup(setup);
			}
			return ResolveSetup(terminal, true);
		}
		
		private TrackedSetup<TContext> ResolveSetup(SetupWithDepth setupWithDepth, bool sequenceSetup = false)
		{
			var setup = setupWithDepth.Setup;
			var trackedSetup = allTrackedSetups.FirstOrDefault(ts => ts.SetupInternal.Expectation.Equals(setup.Expectation));
			if(trackedSetup == null)
			{
				trackedSetup = new TrackedSetup<TContext> { SetupInternal = setup };
				allTrackedSetups.Add(trackedSetup);
				if (sequenceSetup)
				{
					trackedSetups.Add(trackedSetup);
				}
			}
			else
			{
				setupWithDepth.ContainingMutableSetups.Remove(setup);
			}
			return trackedSetup;
		}

		private void SetCondition(TrackedSetup<TContext> trackedSetup)
		{
			var setup = trackedSetup.Setup;
			if(setup is MethodCall methodCall)
			{
				methodCall.SetCondition(
					new Condition(() =>
					{
						var potentialSuccessIndex = executionCount + 1;
						var success = Condition(trackedSetup, potentialSuccessIndex);
						if (success)
						{
							executionCount++;
						}
						return success;
					}, 
					() => {
						/*
							Invocation.MatchingSetup is not set until the condition has returned true. 
							If need to move the test in to the condition part
							Will need to change Setup.Matches to attach the Invocation to the Condition
						*/
						if (strict)
						{
							if (!InvocationsHaveMatchingSequenceSetup())
							{
								StrictnessFailure(UnmatchedInvocations());
							}
						}
						
						trackedSetup.executionIndicesInternal.Add(executionCount);
						SetupExecuted(trackedSetup);
					})
				);
			} // should be throwing otherwise ?
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="unmatchedInvocations"></param>
		protected virtual void StrictnessFailure(IEnumerable<SequenceInvocation> unmatchedInvocations)
		{
			throw new StrictSequenceException { UnmatchedSequenceInvocations = unmatchedInvocations };
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="trackedSetup"></param>
		/// <param name="invocationIndex"></param>
		/// <returns></returns>
		protected abstract bool Condition(ITrackedSetup<TContext> trackedSetup, int invocationIndex);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="trackedSetup"></param>
		protected virtual void SetupExecuted(ITrackedSetup<TContext> trackedSetup)
		{

		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected IEnumerable<SequenceInvocation> UnmatchedInvocations()
		{
			return sequenceInvocations.Where(si => !si.Matched);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected bool InvocationsHaveMatchingSequenceSetup()
		{
			foreach(var sequenceInvocation in UnmatchedInvocations())
			{
				if (!InvocationHasMatchingSequenceSetup(sequenceInvocation))
				{
					return false;
				}
			}
			return true;
		}

		private bool InvocationHasMatchingSequenceSetup(SequenceInvocation sequenceInvocation)
		{
			var invocation = sequenceInvocation.Invocation;
			if (invocation.MatchingSetup == null)
			{
				return false;
			}

			foreach (var trackedSetup in allTrackedSetups)
			{
				if (invocation.MatchingSetup == trackedSetup.Setup)
				{
					sequenceInvocation.Matched = true;
					break;
				}
			}
			return sequenceInvocation.Matched;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Verify()
		{
			if (strict && !InvocationsHaveMatchingSequenceSetup())
			{
				throw new StrictSequenceException { UnmatchedSequenceInvocations = UnmatchedInvocations() };
			}
			VerifyImpl();
		}

		/// <summary>
		/// 
		/// </summary>
		protected abstract void VerifyImpl();
		
	}

	internal class SetupWithDepth : IEquatable<SetupWithDepth>
	{
		public int Depth { get; set; }
		public Setup Setup { get; set; }
		public SetupCollection ContainingMutableSetups { get; set; }

		public bool Equals(SetupWithDepth other)
		{
			return Setup == other.Setup;
		}

		public override int GetHashCode()
		{
			return Setup.GetHashCode();
		}
		
	}

	internal static class SetupWithDepthExtensions
	{
		public class SetupDepthComparisonResult
		{
			public bool NoChange { get; set; }
			public List<SetupWithDepth> NewSetups { get; set; }
			public SetupWithDepth TerminalSetup { get; set; }
		}

		public static SetupDepthComparisonResult NewSetups(this List<SetupWithDepth> before, List<SetupWithDepth> after)
		{
			if (after.Count == before.Count)
			{
				return new SetupDepthComparisonResult { NoChange = true };
			}

			var difference = after.Except(before, EqualityComparer<SetupWithDepth>.Default);
			var orderedByDepth = difference.OrderBy(sd => sd.Depth).ToList();
			var terminalSetup = orderedByDepth.Last();
			return new SetupDepthComparisonResult
			{
				NewSetups = orderedByDepth,
				TerminalSetup = terminalSetup
			};
		}
	}

	internal static class MoqSetupFinder
	{
		private static void GetAllSetups(SetupCollection setups, List<SetupWithDepth> setupsWithDepth, int depth)
		{
			setupsWithDepth.AddRange(setups.ToArray().Select(s => new SetupWithDepth { Depth = depth, Setup = s, ContainingMutableSetups = setups }));
			foreach (var setup in setups)
			{
				if (setup.InnerMock != null)
				{
					GetAllSetups(setup.InnerMock.MutableSetups, setupsWithDepth, depth + 1);
				}
			}
		}
		private static void GetAllSetups(SetupCollection setups, List<SetupWithDepth> setupsWithDepth)
		{
			GetAllSetups(setups, setupsWithDepth, 0);
		}
		public static List<SetupWithDepth> GetAllSetups(Mock mock)
		{
			var setupsWithDepth = new List<SetupWithDepth>();
			GetAllSetups(mock.MutableSetups, setupsWithDepth);
			return setupsWithDepth;
		}
	}

}
