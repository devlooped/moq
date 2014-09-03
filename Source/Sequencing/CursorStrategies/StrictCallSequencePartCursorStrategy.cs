using System;
using System.Collections.Generic;
using Moq.Proxy;
using Moq.Sequencing.Extensibility;

namespace Moq.Sequencing.NavigationStrategies
{
  /// <summary>
  /// Verifies order of calls in strict order,
  /// however, allows skipping the initial calls,
  /// so in case actual calls are: A,B,C,D
  /// and we verify sequence: B,C,
  /// then the verification passes
  /// </summary>
  internal class StrictCallSequencePartCursorStrategy : ICallSequenceCursorStrategy
  {
    private readonly List<Tuple<ICallMatcher, Mock>> callsToVerify = new List<Tuple<ICallMatcher, Mock>>();

    public bool MovePast(ICallMatcher expected, Mock target, IRecordedCalls recordedCalls)
    {
      var isWholeSequenceUpToNowMatched = false;
      callsToVerify.Add(Tuple.Create(expected, target));
      recordedCalls.Rewind();

      isWholeSequenceUpToNowMatched = recordedCalls.MovePastSubsequence(callsToVerify);
      return isWholeSequenceUpToNowMatched;
    }
  }

  /// <summary>
  /// Works like a strict sequence with the exception
  /// that it does not require the first verified call
  /// to be the first recorded call (in other words, 
  /// the strict sequence of calls can happen anytime)
  /// </summary>
  public class StrictSequenceStartingAnywhere : CallSequence
  {
    /// <summary>
    /// Creates a new instance
    /// </summary>
    public StrictSequenceStartingAnywhere()
      : base(new StrictCallSequencePartCursorStrategy())
    {
    }
  }
}