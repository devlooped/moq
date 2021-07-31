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
			var condition = true;
			var currentSequenceSetup = SequenceSetups[currentSequenceSetupIndex];

			if (currentSequenceSetup.InvocationShapeSetups == sequenceSetup.InvocationShapeSetups)
			{
				if (currentSequenceSetup == sequenceSetup)
				{
					CurrentSequenceSetupInvoked();
				}
				else
				{
					return false; // the one we are interested in will come along soon
				}
			}
			else
			{
				ConfirmSequenceSetupSatisfied(currentSequenceSetup);

				// first ( of common ) that comes after or the first setup
				var nextSequenceSetupIndex = sequenceSetup.GetNextSequenceSetupIndex(currentSequenceSetupIndex);
				if (nextSequenceSetupIndex != sequenceSetup.SetupIndex)
				{
					return false; // there will be another along
				}

				if (nextSequenceSetupIndex > currentSequenceSetupIndex)
				{
					ConfirmSequenceSetupsOptionalFromCurrentToExclusive(nextSequenceSetupIndex);
					currentSequenceSetupIndex = nextSequenceSetupIndex;
					CurrentSequenceSetupInvoked();
				}
				else
				{
					condition = SequenceSetupBeforeCurrentInvoked(sequenceSetup);
				}
			}

			return condition;
		}

		private void CurrentSequenceSetupInvoked()
		{
			var currentSequenceSetup = SequenceSetups[currentSequenceSetupIndex];
			currentSequenceSetup.Executed();

			var times = currentSequenceSetup.Times;
			times.Deconstruct(out int from, out int to);
			var kind = times.GetKind();

			ThrowForBadTimes(times, from, to, currentSequenceSetup);
			TryMoveToConsecutiveForApplicableTimes(kind, from, to, currentSequenceSetup);
		}

		private void ThrowForBadTimes(Times times, int from, int to, CyclicalTimesSequenceSetup currentSequenceSetup)
		{
			var shouldThrow = false;
			var kind = times.GetKind();
			switch (kind)
			{
				case Times.Kind.Never:
					shouldThrow = true;
					break;
				case Times.Kind.Exactly:
				case Times.Kind.Once:
					shouldThrow = currentSequenceSetup.ExecutionCount > from;
					break;
				case Times.Kind.AtLeast:
				case Times.Kind.AtLeastOnce:
					break;
				case Times.Kind.AtMost:
				case Times.Kind.AtMostOnce:
					shouldThrow = !currentSequenceSetup.Times.Validate(currentSequenceSetup.ExecutionCount);
					break;
				case Times.Kind.BetweenExclusive:
				case Times.Kind.BetweenInclusive:
					if (currentSequenceSetup.ExecutionCount > to)
					{
						shouldThrow = true;
					}
					break;
			}

			if (shouldThrow)
			{
				throw new SequenceException(times, currentSequenceSetup.ExecutionCount, currentSequenceSetup.Setup);
			}
		}

		private void TryMoveToConsecutiveForApplicableTimes(Times.Kind kind, int from, int to, CyclicalTimesSequenceSetup sequenceSetup)
		{
			var shouldTryMoveToConsecutive = false;
			var executionCount = sequenceSetup.ExecutionCount;
			switch (kind)
			{
				case Times.Kind.Exactly:
				case Times.Kind.Once:
				case Times.Kind.AtLeast:
				case Times.Kind.AtLeastOnce:
					shouldTryMoveToConsecutive = from == executionCount;
					break;
				case Times.Kind.AtMost:
				case Times.Kind.AtMostOnce:
					shouldTryMoveToConsecutive = to == executionCount;
					break;
			}

			if (shouldTryMoveToConsecutive)
			{
				TryMoveToConsecutive(sequenceSetup);
			}
		}

		private bool TryMoveToConsecutive(CyclicalTimesSequenceSetup sequenceSetup)
		{
			var nextConsecutiveInvocationShapeSetup = sequenceSetup.TryGetNextConsecutiveInvocationShapeSetup(Cyclical, SequenceSetups.Count);
			if (nextConsecutiveInvocationShapeSetup != null)
			{
				currentSequenceSetupIndex = nextConsecutiveInvocationShapeSetup.SetupIndex;
				if (currentSequenceSetupIndex == 0)
				{
					ResetForCyclical();
				}
				return true;
			}
			return false;
		}

		private void ConfirmSequenceSetupSatisfied(CyclicalTimesSequenceSetup sequenceSetup)
		{
			var times = sequenceSetup.Times;
			var kind = times.GetKind();
			switch (kind)
			{
				case Times.Kind.Never:
				case Times.Kind.AtMost:
				case Times.Kind.AtMostOnce:
					break;
				case Times.Kind.Exactly:
				case Times.Kind.Once:
				case Times.Kind.BetweenExclusive:
				case Times.Kind.BetweenInclusive:
				case Times.Kind.AtLeast:
				case Times.Kind.AtLeastOnce:
					if (!times.Validate(sequenceSetup.ExecutionCount))
					{
						throw new SequenceException(times,sequenceSetup.ExecutionCount,sequenceSetup.Setup);
					}
					break;
			}
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
