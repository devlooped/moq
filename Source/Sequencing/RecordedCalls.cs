using System;
using System.Collections.Generic;
using Moq.Proxy;
using Moq.Sequencing.Extensibility;

namespace Moq.Sequencing
{
  internal class RecordedCalls : IRecordedCalls
  {
    private const int PreBeginPosition = -1;
    private readonly List<SequencedCall> callContexts = new List<SequencedCall>();
    private int currentItemIndex = PreBeginPosition;


    public ISequencedCall Current { get { return callContexts[currentItemIndex]; } }

    public void Add(ICall invocation, Mock target)
    {
      callContexts.Add(new SequencedCall(invocation, target));
    }

    public bool MoveToNext()
    {
      if (EOF) return false;
      
      currentItemIndex++;
      return !EOF;
    }

    private bool EOF
    {
      get { return currentItemIndex >= callContexts.Count; }
    }

    public void Rewind()
    {
      currentItemIndex = PreBeginPosition;
    }

    public bool MovePastSubsequence(List<Tuple<ICallMatcher, Mock>> callsToVerify)
    {
      for (var i = 0; i < callContexts.Count; ++i)
      {
        if (this.MatchSubsequenceStartingFrom(i, callsToVerify))
        {
          currentItemIndex = i + callsToVerify.Count;
          return true;
        }
      }
      return false;
    }


    private bool MatchSubsequenceStartingFrom(
      int subsequenceStartIndex,
      List<Tuple<ICallMatcher, Mock>> callsToVerify)
    {
      if (callsToVerify.Count >
         callContexts.Count - subsequenceStartIndex)
      {
        return false;
      }

      for (var i = 0; i < callsToVerify.Count; ++i)
      {
        var currentExpectedCall = callsToVerify[i];

        var currentCall = callContexts[i + subsequenceStartIndex];
        var expectedCall = currentExpectedCall.Item1;
        var expectedTarget = currentExpectedCall.Item2;
        var result = currentCall.Matches(expectedCall, expectedTarget);
        
        if (!result)
        {
          return false;
        }
      }
      return true;
    }

  }
}

