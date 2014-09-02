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
    private readonly ICallSequenceNavigation callSequenceNavigation;

    internal CallSequence(ICallSequenceNavigation callSequenceNavigation)
    {
      this.callSequenceNavigation = callSequenceNavigation;
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
        callSequenceNavigation = new LooseSequenceNavigation();
      }
      else
      {
        callSequenceNavigation = new StrictSequenceNavigation();
      }
    }

    internal bool ForwardBeyondACallTo(ICallMatchable expected, Mock target)
    {
      return callSequenceNavigation.ForwardBeyondACallTo(expected, target, recordedCalls);
    }

    ///<summary>
    ///Verifies the calls were received in the correct order
    ///</summary>
    ///<param name="steps">steps to verify in order of their verification</param>
    public void VerifyCalls(params IVerificationStep[] steps)
    {
      foreach (var step in steps)
      {
        step.Verify();
      }
      recordedCalls.Rewind();
    }

    /// <summary>
    /// Verifies the order of calls. This static version of verification requires 
    /// that all mocks used in the verification have the same sequence assigned.
    /// If this is not true, an exception is thrown.
    /// </summary>
    /// <param name="steps">Order of steps to verify</param>
    public static void Verify(params IVerificationStep[] steps)
    {
      if (steps.Any())
      {
        if (AreNotRecordedByTheSameSequence(steps))
        {
          throw new MockException(MockException.ExceptionReason.VerificationFailed, "The calls being verified are not recorded by the same sequence");
        }
        else
        {
          steps.First().CallSequence.VerifyCalls(steps);
        }
      }
    }

    internal static CallSequence None()
    {
      return new NullSequence();
    }

    private static bool AreNotRecordedByTheSameSequence(IEnumerable<IVerificationStep> steps)
    {
      return steps.Select(s => s.CallSequence).Distinct().ToArray().Length > 1;
    }

    internal void Rewind()
    {
      recordedCalls.Rewind();
    }

    internal void Add (ICallContext invocation, Mock target)
    {
      recordedCalls.Add(invocation, target);
    }
  }
}

