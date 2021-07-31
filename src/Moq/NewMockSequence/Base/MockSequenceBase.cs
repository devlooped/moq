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
	public abstract class MockSequenceBase<TSequenceSetup, TInvocationShapeSetups> 
		where TSequenceSetup : SequenceSetupBase 
		where  TInvocationShapeSetups : InvocationShapeSetupsBase<TSequenceSetup>
	{
		private int setupCount = -1;
		private readonly Mock[] mocks;
		private readonly SequenceInvocationListener sequenceInvocationListener;
		private readonly List<Setup> allSetups = new List<Setup>();
		private readonly List< TInvocationShapeSetups> allInvocationShapeSetups = new List< TInvocationShapeSetups>();
		private readonly List<TSequenceSetup> sequenceSetups = new List<TSequenceSetup>();
		/// <summary>
		/// 
		/// </summary>
		protected readonly bool strict;
		/// <summary>
		/// 
		/// </summary>
		protected IReadOnlyList<TSequenceSetup> SequenceSetups => sequenceSetups;

		internal List<SequenceInvocation> SequenceInvocations => sequenceInvocationListener.SequenceInvocations;
		internal IReadOnlyList<ISetup> AllSetups => allSetups;
		
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
			sequenceInvocationListener = new SequenceInvocationListener(mocks);
			sequenceInvocationListener.NewInvocationEvent += SequenceInvocationListener_NewInvocationEvent;
			sequenceInvocationListener.ListenForInvocations();
		}

		// todo - for MockAs get duplicate events
		private void SequenceInvocationListener_NewInvocationEvent(object sender, SequenceInvocation sequenceInvocation)
		{
			var invocation = sequenceInvocation.InvocationInternal;

			var noMatch = !allSetups.Any(setup =>
				{
					var isMatch = false;
					// second condition for mock.As
					if (setup.Mock == sequenceInvocation.Mock || setup.Mock.MutableSetups == sequenceInvocation.Mock.MutableSetups)
					{
						isMatch = setup.Expectation.IsMatch(invocation);
					}
					return isMatch;
				});
			if (noMatch)
			{
				if (strict)
				{
					StrictnessFailure(sequenceInvocation);
				}
				
			}
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="sequenceSetupCallback"></param>
		protected void InterceptSetup(Action setup, Action<TSequenceSetup> sequenceSetupCallback)
		{
			setupCount++;
			List<List<SetupWithDepth>> allSetupsBefore = mocks.Select(m => SetupFinder.GetAllSetups(m)).ToList();
			setup();
			List<List<SetupWithDepth>> allSetupsAfter = mocks.Select(m => SetupFinder.GetAllSetups(m)).ToList();
			for(var i = 0; i < allSetupsBefore.Count; i++)
			{
				var result = allSetupsBefore[i].NewSetups(allSetupsAfter[i]);
				if (!result.NoChange)
				{
					allSetups.AddRange(result.NewSetups.Select(sd => sd.Setup));
					sequenceInvocationListener.ListenForInvocations(result.NewSetups.Select(s => s.Setup.Mock));
					var terminalSetup = result.TerminalSetup.Setup;

					var sequenceSetup = CreateSequenceSetup(terminalSetup);
					InitializeSequenceSetup(sequenceSetup);
					sequenceSetupCallback(sequenceSetup);

					return;
				}

			}

			throw new ArgumentException("No setup performed",nameof(setup));
		}
		
		private void InitializeSequenceSetup(TSequenceSetup sequenceSetup)
		{
			sequenceSetups.Add(sequenceSetup);
			ApplyInvocationShapeSetups(sequenceSetup);
			SetCondition(sequenceSetup, sequenceSetup.SetupInternal);

		}
		
		private TSequenceSetup CreateSequenceSetup(Setup setup)
		{
			var sequenceSetup = (TSequenceSetup)Activator.CreateInstance(typeof(TSequenceSetup));
			sequenceSetup.SetupIndex = setupCount;
			sequenceSetup.SetupInternal = setup;
			
			return sequenceSetup;
		}

		private void ApplyInvocationShapeSetups(TSequenceSetup newSequenceSetup)
		{
			var invocationShape = newSequenceSetup.SetupInternal.Expectation;
			var invocationShapeSetups = allInvocationShapeSetups.SingleOrDefault(ts => ts.InvocationShape.Equals(invocationShape));
			if (invocationShapeSetups == null)
			{
				invocationShapeSetups = (TInvocationShapeSetups) Activator.CreateInstance(typeof( TInvocationShapeSetups),newSequenceSetup);
				allInvocationShapeSetups.Add(invocationShapeSetups);
			}
			else
			{
				invocationShapeSetups.AddSequenceSetup(newSequenceSetup);
			}

		}

		private void SetCondition(TSequenceSetup sequenceSetup,ISetup setup)
		{
			if (setup is MethodCall methodCall)
			{
				methodCall.SetCondition(
					new Condition(() =>
					{
						return Condition(sequenceSetup);
					},
					() =>
					{
						SetupExecuted(sequenceSetup);
					})
				);
			}
			else
			{
				throw new Exception("todo");//todo
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="unmatchedInvocation"></param>
		protected virtual void StrictnessFailure(ISequenceInvocation unmatchedInvocation)
		{
			throw new StrictSequenceException($"Invocation without sequence setup. {unmatchedInvocation.Invocation}") { UnmatchedInvocation = unmatchedInvocation };
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceSetup"></param>
		/// <returns></returns>
		protected abstract bool Condition(TSequenceSetup sequenceSetup);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceSetup"></param>
		protected virtual void SetupExecuted(TSequenceSetup sequenceSetup)
		{

		}

		/// <summary>
		/// 
		/// </summary>
		public void VerifyNoOtherCalls()
		{
			var invocationsWithoutSequenceSetup = SequenceInvocations.Where(si => si.Invocation.MatchingSetup == null).ToList();
			if (invocationsWithoutSequenceSetup.Count > 0)
			{
				throw new SequenceException($"Expected no invocations without sequence setup but found {invocationsWithoutSequenceSetup.Count}");
			}
		}
	}

}
