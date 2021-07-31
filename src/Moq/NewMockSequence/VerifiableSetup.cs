using System.Collections.Generic;
using System.Linq;

namespace Moq
{
	/// <summary>
	/// 
	/// </summary>
	public class VerifiableSetup
	{
		private CyclicalTimesSequenceSetup sequenceSetup;

		/// <summary>
		/// 
		/// </summary>
		internal IReadOnlyList<int> CyclicalExecutionCount
		{
			get
			{
				var cyclicalExecutionCount = new List<int>(sequenceSetup.CompletedCyclicalExecutionCount);
				cyclicalExecutionCount.Add(sequenceSetup.ExecutionCount);
				return cyclicalExecutionCount;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceSetup"></param>
		internal VerifiableSetup(CyclicalTimesSequenceSetup sequenceSetup)
		{
			this.sequenceSetup = sequenceSetup;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Verify()
		{
			VerifySequenceSetup(sequenceSetup);
		}

		private void VerifySequenceSetup(CyclicalTimesSequenceSetup sequenceSetup)
		{
			Verify(sequenceSetup.Times, sequenceSetup.ExecutionCount, sequenceSetup.Setup);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="times"></param>
		public void Verify(Times times)
		{
			Verify(times, sequenceSetup.TotalExecutionCount, sequenceSetup.Setup);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="times"></param>
		public void Verify(int times)
		{
			Verify(Times.Exactly(times));
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void VerifyAll()
		{
			foreach (var sequenceSetup in sequenceSetup.InvocationShapeSetups.MockSequenceSetups)
			{
				VerifySequenceSetup(sequenceSetup);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="times"></param>
		public void VerifyAll(Times times)
		{
			Verify(times, sequenceSetup.InvocationShapeSetups.TotalExecutions());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="times"></param>
		public void VerifyAll(int times)
		{
			VerifyAll(Times.Exactly(times));
		}

		private void Verify(Times times, int executionCount, ISetup setup = null)
		{
			var success = times.Validate(executionCount);
			if (!success)
			{
				throw new SequenceException(times, executionCount, setup);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cyclicalTimes"></param>
		public void VerifyCyclical(IEnumerable<int> cyclicalTimes)
		{
			VerifyCyclical(cyclicalTimes.Select(i => Times.Exactly(i)));
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cyclicalTimes"></param>
		public void VerifyCyclical(IEnumerable<Times> cyclicalTimes)
		{
			var count = 0;
			List<Times> cyclicalTimesList = cyclicalTimes.ToList();
			var expectedCycles = cyclicalTimesList.Count;
			var actualCycles = CyclicalExecutionCount.Count;
			AssertNumberOfCycles(actualCycles, expectedCycles);
			
			foreach (var cyclicalTime in cyclicalTimes)
			{
				var actual = CyclicalExecutionCount[count];
				if (!cyclicalTime.Validate(actual))
				{
					throw new SequenceException($"On cycle {count}. {cyclicalTime.GetExceptionMessage(actual)}");
				}
				count++;
			}
		}

		private void AssertNumberOfCycles(int actualCycles, int expectedCycles)
		{
			if (actualCycles != expectedCycles)
			{
				throw new SequenceException($"Expected cycles {expectedCycles} but was {actualCycles}");
			}
		}
	}

}
