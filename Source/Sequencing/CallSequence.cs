using System.Collections.Generic;
using System.Linq;
using Moq.Proxy;
using Moq.Sequencing.Extensibility;
using Moq.Sequencing.NavigationStrategies;

namespace Moq.Sequencing
{

  ///<summary>Class used for call order validation</summary>
  public class CallSequence 
  {
    private readonly RecordedCalls recordedCalls = new RecordedCalls();
    private readonly ICallSequenceCursorStrategy callSequenceCursorStrategy;

    internal CallSequence(ICallSequenceCursorStrategy callSequenceCursorStrategy)
    {
      this.callSequenceCursorStrategy = callSequenceCursorStrategy;
    }

    ///<summary>
    ///Creates a new call sequence for call order verification.
    ///</summary>
    ///<param name="behavior">Determines how verification works.
    ///Loose sequence verifies that calls were made in the specified order allowing any calls to happen inbetween them.
    ///Strict sequence does not allow any calls inbetween</param>
    public CallSequence(MockBehavior behavior = MockBehavior.Default)
    {
      if (behavior == MockBehavior.Loose)
      {
        callSequenceCursorStrategy = new LooseCallSequenceCursorStrategy();
      }
      else
      {
        callSequenceCursorStrategy = new StrictCallSequenceCursorStrategy();
      }
    }

    internal bool MovePast(ICallMatcher expected, Mock target)
    {
      return callSequenceCursorStrategy.MovePast(expected, target, recordedCalls);
    }

    ///<summary>
    ///Verifies the calls were received in the correct order
    ///</summary>
    ///<param name="steps">steps to verify in order of their verification</param>
    public void Verify(params IVerificationStep[] steps)
    {
      foreach (var step in steps)
      {
        step.Verify();
      }
      recordedCalls.Rewind();
    }

    internal static CallSequence None()
    {
      return new NullSequence();
    }

    private static bool AreNotRecordedByTheSameSequence(IEnumerable<IVerificationStep> steps)
    {
      return steps.Select(s => s.CallSequence).Distinct().ToArray().Length > 1;
    }

    internal void Add (ICallContext invocation, Mock target)
    {
      recordedCalls.Add(invocation, target);
    }
  }
}

