using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moq
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class NewMockSequence : MockSequenceBase<CyclicalTimesSequenceSetup, CyclicalInvocationShapeSetups>
	{
		private int currentSequenceSetupIndex;
		
		/// <summary>
		/// 
		/// </summary>
		public bool Cyclical { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static Times OptionalTimes()
		{
			return Times.Between(0, int.MaxValue, Range.Inclusive);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strict"></param>
		/// <param name="mocks"></param>
		public NewMockSequence(bool strict, params Mock[] mocks) : base(strict, mocks) { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="times"></param>
		/// <returns></returns>
		public VerifiableSetup Setup(Action setup, Times? times = null)
		{
			Times t = times ?? Times.Once();
			VerifiableSetup verifiableSetup = null;
			base.InterceptSetup(setup, (sequenceSetup) =>
			{
				sequenceSetup.Times = t;
				verifiableSetup = new VerifiableSetup(sequenceSetup);
			});
			return verifiableSetup;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceSetup"></param>
		/// <returns></returns>
		protected override bool Condition(CyclicalTimesSequenceSetup sequenceSetup)
		{
			bool condition;
			var currentSequenceSetup = SequenceSetups[currentSequenceSetupIndex];

			if (currentSequenceSetup.InvocationShapeSetups == sequenceSetup.InvocationShapeSetups)
			{
				condition = CurrentInvocationShapeSetupsCondition(sequenceSetup, currentSequenceSetup);
			}
			else
			{
				condition = NewSetupCondition(sequenceSetup, currentSequenceSetup);
			}

			return condition;
		}

		private bool CurrentInvocationShapeSetupsCondition(CyclicalTimesSequenceSetup sequenceSetup, CyclicalTimesSequenceSetup currentSequenceSetup)
		{
			var condition = true;
			if (currentSequenceSetup == sequenceSetup)
			{
				CurrentSequenceSetupInvoked();
			}
			else
			{
				condition = false; // the one we are interested in will come along soon
			}
			return condition;
		}

		private bool NewSetupCondition(CyclicalTimesSequenceSetup sequenceSetup, CyclicalTimesSequenceSetup currentSequenceSetup)
		{
			var condition = true;
			
			if (sequenceSetup.IsNextSequenceSetup(currentSequenceSetupIndex))
			{
				ConfirmSequenceSetupSatisfied(currentSequenceSetup);

				if (sequenceSetup.SetupIndex > currentSequenceSetupIndex)
				{
					ConfirmSequenceSetupsOptionalFromCurrentToExclusive(sequenceSetup.SetupIndex);
					currentSequenceSetupIndex = sequenceSetup.SetupIndex;
					CurrentSequenceSetupInvoked();
				}
				else
				{
					condition = SequenceSetupBeforeCurrentInvoked(sequenceSetup);
				}
			}
			else
			{
				condition = false; // there will be another along
			}

			return condition;
		}

		private void CurrentSequenceSetupInvoked()
		{
			var currentSequenceSetup = SequenceSetups[currentSequenceSetupIndex];
			ThrowForInvalidSequence(currentSequenceSetup.Invoked(), currentSequenceSetup);
			TryAdvanceToConsecutiveInvocationShapeSetup(currentSequenceSetup);
		}

		private void TryAdvanceToConsecutiveInvocationShapeSetup(CyclicalTimesSequenceSetup currentSequenceSetup)
		{
			var nextConsecutiveInvocationShapeSetup = currentSequenceSetup.AdvancedToConsecutiveInvocationShapeSetup(Cyclical, SequenceSetups.Count);
			if (nextConsecutiveInvocationShapeSetup != null)
			{
				currentSequenceSetupIndex = nextConsecutiveInvocationShapeSetup.SetupIndex;
				if (currentSequenceSetupIndex == 0)
				{
					ResetForCyclical();
				}
			}
		}

		private void ConfirmSequenceSetupSatisfied(CyclicalTimesSequenceSetup sequenceSetup)
		{
			ThrowForInvalidSequence(sequenceSetup.ValidateSatisfied(),sequenceSetup);
		}

		
		private void ConfirmSequenceSetupsOptionalFromCurrentToExclusive(int upToIndex)
		{
			for (var i = currentSequenceSetupIndex + 1; i < upToIndex; i++)
			{
				ConfirmSequenceSetupSatisfied(SequenceSetups[i]);
			}
		}

		private bool SequenceSetupBeforeCurrentInvoked(CyclicalTimesSequenceSetup newSequenceSetup)
		{
			if (Cyclical)
			{
				ConfirmRemainingSequenceSetupsOptional();
				ResetForCyclical();
				ConfirmPreviousSequenceSetupsOptional(newSequenceSetup.SetupIndex);
				currentSequenceSetupIndex = newSequenceSetup.SetupIndex;
				CurrentSequenceSetupInvoked();
			}
			else
			{
				if (strict) // to be determined
				{
					var unmatchedInvocation = SequenceInvocations.Last();
					throw new StrictSequenceException($"Cyclical invocation but not cyclic. {newSequenceSetup.Setup}") { UnmatchedInvocation = unmatchedInvocation };
				}
			}

			return true;
		}

		private void ConfirmRemainingSequenceSetupsOptional()
		{
			ConfirmSequenceSetupsOptionalFromCurrentToExclusive(SequenceSetups.Count);
		}

		private void ConfirmPreviousSequenceSetupsOptional(int upToIndex)
		{
			for (var i = 0; i < upToIndex; i++)
			{
				ConfirmSequenceSetupSatisfied(SequenceSetups[i]);
			}
		}
		
		private void ResetForCyclical()
		{
			foreach (var sequenceSetup in SequenceSetups)
			{
				sequenceSetup.ResetForCyclical();
			}
		}

		private void ThrowForInvalidSequence(bool valid, CyclicalTimesSequenceSetup sequenceSetup)
		{
			if (!valid)
			{
				throw new SequenceException(sequenceSetup.Times, sequenceSetup.InvocationCount, sequenceSetup.Setup);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Verify()
		{
			for (var i = currentSequenceSetupIndex; i < SequenceSetups.Count; i++)
			{
				ConfirmSequenceSetupSatisfied(SequenceSetups[i]);
			}
		}
	}
}
