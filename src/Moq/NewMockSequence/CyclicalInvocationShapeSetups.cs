using System.Collections.Generic;
using System.Linq;

namespace Moq
{
	/// <summary>
	/// 
	/// </summary>
	public class CyclicalInvocationShapeSetups : InvocationShapeSetupsBase<CyclicalTimesSequenceSetup>
	{
		internal IEnumerable<CyclicalTimesSequenceSetup> MockSequenceSetups => SequenceSetups;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceSetup"></param>
		public CyclicalInvocationShapeSetups(CyclicalTimesSequenceSetup sequenceSetup) : base(sequenceSetup) { }
		
		internal CyclicalTimesSequenceSetup TryGetNextConsecutiveInvocationShapeSetup(int relativeTo,bool cyclic,int totalSetups)
		{
			var isLast = relativeTo == totalSetups - 1;
			if (isLast)
			{
				if (cyclic)
				{
					return SequenceSetups.SingleOrDefault(s => s.SetupIndex == 0);
				}
				return null;
			}

			return SequenceSetups.SingleOrDefault(s => s.SetupIndex == relativeTo + 1);
		}

		internal int TotalExecutions()
		{
			return SequenceSetups.Sum(ss => ss.TotalExecutionCount);
		}

		// first that comes after or the first setup
		internal int GetNextSequenceSetupIndex(int currentSetupIndex)
		{
			int firstSequenceSetupIndex = -1;
			int nextSequenceSetupIndex = -1;
			foreach (var sequenceSetup in SequenceSetups)
			{
				var sequenceSetupIndex = sequenceSetup.SetupIndex;
				if (firstSequenceSetupIndex == -1)
				{
					firstSequenceSetupIndex = sequenceSetupIndex;
				}
				if (sequenceSetupIndex > currentSetupIndex)
				{
					nextSequenceSetupIndex = sequenceSetupIndex;
					break;
				}
			}

			if (nextSequenceSetupIndex != -1)
			{
				return nextSequenceSetupIndex;
			}
			return firstSequenceSetupIndex;
		}
	}

}
