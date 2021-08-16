using System.Collections.Generic;

namespace Moq
{
	/// <summary>
	/// 
	/// </summary>
	public class CyclicalTimesSequenceSetup : SequenceSetupBase
	{
		private int invocationCount;
		private List<int> completedCyclicalExecutionCount = new List<int>();
		internal int InvocationCount
		{
			get
			{
				return invocationCount;
			}
		}

		
		internal IReadOnlyList<int> CompletedCyclicalExecutionCount => completedCyclicalExecutionCount;

		internal Times Times { get; set; }

		internal int TotalInvocationCount { get; set; }

		internal CyclicalInvocationShapeSetups InvocationShapeSetups { get => (CyclicalInvocationShapeSetups)InvocationShapeSetupsObject; }

		internal bool Invoked()
		{
			invocationCount++;
			TotalInvocationCount++;
			return ValidateTimes();
		}
		
		private bool ValidateTimes()
		{
			Times.Deconstruct(out int from, out int to);
			var kind = Times.GetKind();

			var valid = true;
			
			switch (kind)
			{
				case Times.Kind.Never:
					valid = false;
					break;
				case Times.Kind.Exactly:
				case Times.Kind.Once:
					valid = InvocationCount <= from;
					break;
				case Times.Kind.AtLeast:
				case Times.Kind.AtLeastOnce:
					break;
				case Times.Kind.AtMost:
				case Times.Kind.AtMostOnce:
					valid = Times.Validate(InvocationCount);
					break;
				case Times.Kind.BetweenExclusive:
				case Times.Kind.BetweenInclusive:
					if (InvocationCount > to)
					{
						valid = false;
					}
					break;
			}

			return valid;
			
		}

		internal bool CanMoveToConsecutive()
		{
			Times.Deconstruct(out int from, out int to);
			var kind = Times.GetKind();

			var canMoveToConsecutive = false;

			switch (kind)
			{
				case Times.Kind.Exactly:
				case Times.Kind.Once:
				case Times.Kind.AtLeast:
				case Times.Kind.AtLeastOnce:
					canMoveToConsecutive = from == InvocationCount;
					break;
				case Times.Kind.AtMost:
				case Times.Kind.AtMostOnce:
					canMoveToConsecutive = to == InvocationCount;
					break;
			}

			return canMoveToConsecutive;
		}

		internal CyclicalTimesSequenceSetup AdvancedToConsecutiveInvocationShapeSetup(bool cyclic,int totalSetups)
		{
			if (CanMoveToConsecutive())
			{
				return InvocationShapeSetups.TryGetNextConsecutiveInvocationShapeSetup(this.SetupIndex, cyclic, totalSetups);
			}
			return null;
		}

		internal bool IsNextSequenceSetup(int currentSetupIndex)
		{
			return SetupIndex == this.InvocationShapeSetups.GetNextSequenceSetupIndex(currentSetupIndex);
		}

		internal void ResetForCyclical()
		{
			completedCyclicalExecutionCount.Add(invocationCount);
			invocationCount = 0;
		}

		internal bool ValidateSatisfied()
		{
			var valid = true;
			var kind = Times.GetKind();
			switch (kind)
			{
				case Times.Kind.Never: // ValidateTimes will have already thrown
				case Times.Kind.AtMost: // ValidateTimes will have already thrown
				case Times.Kind.AtMostOnce: // ValidateTimes will have already thrown
					break;
				case Times.Kind.Exactly:
				case Times.Kind.Once:
				case Times.Kind.BetweenExclusive:
				case Times.Kind.BetweenInclusive:
				case Times.Kind.AtLeast:
				case Times.Kind.AtLeastOnce:
					if (!Times.Validate(InvocationCount))
					{
						//throw new SequenceException(times, sequenceSetup.InvocationCount, sequenceSetup.Setup);
						valid = false;
					}
					break;
			}
			return valid;
		}
	}

}
