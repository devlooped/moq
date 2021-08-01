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
	public class SequenceVerificationException : Exception
	{

	}

	/// <summary>
	/// 
	/// </summary>
	public class SequenceException : Exception
	{
		internal SequenceException(Times times, int executedCount, ISetup setup) : base($"{setup} {times.GetExceptionMessage(executedCount)}") { }
	}

	/// <summary>
	/// 
	/// </summary>
	public class VerifiableSetup
	{
		private readonly ITrackedSetup<Times> trackedSetup;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="trackedSetup"></param>
		public VerifiableSetup(ITrackedSetup<Times> trackedSetup)
		{
			this.trackedSetup = trackedSetup;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="times"></param>
		public void Verify(Times times)
		{
			var success = times.Validate(trackedSetup.ExecutionIndices.Count);
			if (!success)
			{
				throw new SequenceVerificationException();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Verify()
		{
			if (trackedSetup.SequenceSetups.Count == 1)
			{
				Verify(trackedSetup.SequenceSetups[0].Context);
			}
			else
			{
				throw new Exception("not valid");
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class NewMockSequence : MockSequenceBase<Times>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static Times OptionalTimes()
		{
			return Times.Between(0, int.MaxValue, Range.Inclusive);
		}

		private int currentSequenceSetupIndex;
		private ITrackedSetup<Times> lastSetup;

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
			base.InterceptSetup(setup, (ts, setupOrder) =>
			{
				if(lastSetup == ts)
				{
					throw new Exception("Consecutive setups are the same");
				}
				lastSetup = ts;
				verifiableSetup = new VerifiableSetup(ts);
				return t;
			});
			return verifiableSetup;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="trackedSetup"></param>
		/// <param name="invocationIndex"></param>
		/// <returns></returns>
		protected override bool Condition(ITrackedSetup<Times> trackedSetup, int invocationIndex)
		{
			var currentSequenceSetup = SequenceSetups[currentSequenceSetupIndex];

			if (currentSequenceSetup.TrackedSetup == trackedSetup)
			{
				ConfirmSequenceSetup(currentSequenceSetup);
			}
			else
			{
				ConfirmSequenceSetupSatisfied(currentSequenceSetup);
				var nextSequenceSetupIndex = GetNextSequenceSetupIndex(trackedSetup);
				if(nextSequenceSetupIndex != -1)
				{
					ConfirmSequenceSetupsSatisfied(nextSequenceSetupIndex);
					ConfirmSequenceSetup(SequenceSetups[nextSequenceSetupIndex]);
					currentSequenceSetupIndex = nextSequenceSetupIndex;
				}
				else
				{
					// cyclical
				}
			}

			return true;
		}

		private void ConfirmSequenceSetupsSatisfied(int upToIndex)
		{
			for (var j = currentSequenceSetupIndex + 1; j < upToIndex; j++)
			{
				ConfirmSequenceSetupSatisfied(SequenceSetups[j]);
			}
		}

		private int GetNextSequenceSetupIndex(ITrackedSetup<Times> trackedSetup)
		{
			for (var i = currentSequenceSetupIndex + 1; i < SequenceSetups.Count; i++)
			{
				var sequenceSetup = SequenceSetups[i];
				if (sequenceSetup.TrackedSetup == trackedSetup)
				{
					return i;
				}
			}
			return -1;
		}

		private void ConfirmSequenceSetupSatisfied(ISequenceSetup<Times> sequenceSetup)
		{
			var times = sequenceSetup.Context;
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
						throw new SequenceException(times,sequenceSetup.ExecutionCount,sequenceSetup.TrackedSetup.Setup);
					}
					break;
			}
		}

		private void ConfirmSequenceSetup(ISequenceSetup<Times> sequenceSetup)
		{
			var times = sequenceSetup.Context;
			times.Deconstruct(out int _, out int to);
			var kind = times.GetKind();
			sequenceSetup.ExecutionCount++;
			var shouldThrow = false;
			switch (kind)
			{
				case Times.Kind.Never:
					shouldThrow = true;
					break;
				case Times.Kind.Exactly:
				case Times.Kind.Once:
					if (times.Validate(sequenceSetup.ExecutionCount))
					{
						currentSequenceSetupIndex++;
					}
					break;
				case Times.Kind.AtLeast:
				case Times.Kind.AtLeastOnce:
					// we do not shift
					break;
				case Times.Kind.AtMost:
				case Times.Kind.AtMostOnce:
					shouldThrow = !times.Validate(sequenceSetup.ExecutionCount);
					break;
				case Times.Kind.BetweenExclusive:
				case Times.Kind.BetweenInclusive:
					if (sequenceSetup.ExecutionCount > to)
					{
						shouldThrow = true;
					}
					break;
			}

			if (shouldThrow)
			{
				throw new SequenceException(times, sequenceSetup.ExecutionCount, sequenceSetup.TrackedSetup.Setup);
			}
		}
	
		/// <summary>
		/// 
		/// </summary>
		protected override void VerifyImpl()
		{
			for(var i = currentSequenceSetupIndex; i < SequenceSetups.Count; i++)
			{
				ConfirmSequenceSetupSatisfied(SequenceSetups[i]);
			}
		}
	}

}
