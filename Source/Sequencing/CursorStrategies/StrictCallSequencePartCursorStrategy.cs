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
    private readonly ExpectedCalls expectedCalls = new ExpectedCalls();

    public bool MovePast(IExpectedCall expectedCall, IRecordedCalls recordedCalls)
    {
      expectedCalls.Add(expectedCall);
      var isWholeSequenceUpToNowMatched = false;

      isWholeSequenceUpToNowMatched = expectedCalls.IsSubsequenceOf(recordedCalls);
      
      return isWholeSequenceUpToNowMatched;
    }
  }
}