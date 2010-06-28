using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq.Language;

namespace Moq
{
	/// <summary>
	/// Helper class to setup a full trace between many mocks
	/// </summary>
	public class MockSequence
	{
		int sequenceStep;
		int sequenceLength;

		/// <summary>
		/// Initialize a trace setup
		/// </summary>
		public MockSequence()
		{
			sequenceLength = 0;
			sequenceStep = 0;
		}

		/// <summary>
		/// Allow sequence to be repeated
		/// </summary>
		public bool Cyclic { get; set; }

		private void NextStep()
		{
			sequenceStep++;
			if (Cyclic)
				sequenceStep = sequenceStep % sequenceLength;
		}

		internal ISetupConditionResult<TMock> For<TMock>(Mock<TMock> mock)
			where TMock : class
		{
			var expectationPosition = sequenceLength++;

			// HACK assume condition is only
			// evaluated once. issues to attach callback lately.
			return mock.When(() =>
			{
				var c = expectationPosition == sequenceStep;
				if (c)
				{
					this.NextStep();
				}

				return c;
			});
		}
	}

	/// <summary>
	/// define nice api
	/// </summary>
	public static class MockSequenceHelper
	{
		/// <summary>
		/// Perform an expectation in the trace.
		/// </summary>
		public static ISetupConditionResult<TMock> InSequence<TMock>(
			this Mock<TMock> mock,
			MockSequence sequence)
			where TMock : class
		{
			Guard.NotNull(() => sequence, sequence);

			return sequence.For(mock);
		}
	}
}
