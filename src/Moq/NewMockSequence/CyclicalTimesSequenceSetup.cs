using System.Collections.Generic;

namespace Moq
{
	/// <summary>
	/// 
	/// </summary>
	public class CyclicalTimesSequenceSetup : SequenceSetupBase
	{
		private int executionCount;
		private List<int> completedCyclicalExecutionCount = new List<int>();
		internal int ExecutionCount
		{
			get
			{
				return executionCount;
			}
		}

		
		internal IReadOnlyList<int> CompletedCyclicalExecutionCount => completedCyclicalExecutionCount;

		internal Times Times { get; set; }

		internal int TotalExecutionCount { get; set; }

		internal CyclicalInvocationShapeSetups InvocationShapeSetups { get => (CyclicalInvocationShapeSetups)InvocationShapeSetupsObject; }

		internal void Executed()
		{
			executionCount++;
			TotalExecutionCount++;
		}

		internal CyclicalTimesSequenceSetup TryGetNextConsecutiveInvocationShapeSetup(bool cyclic,int totalSetups)
		{
			return InvocationShapeSetups.TryGetNextConsecutiveInvocationShapeSetup(this.SetupIndex,cyclic,totalSetups);
		}

		internal int GetNextSequenceSetupIndex(int currentSetupIndex)
		{
			return this.InvocationShapeSetups.GetNextSequenceSetupIndex(currentSetupIndex);
		}

		internal void ResetForCyclical()
		{
			completedCyclicalExecutionCount.Add(executionCount);
			executionCount = 0;
		}
	}

}
