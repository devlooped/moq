using System;
using System.Collections.Generic;
using Moq.Proxy;
using Moq.Sequencing.Extensibility;

namespace Moq.Sequencing
{
  internal class RecordedCalls : IRecordedCalls
  {
    private const int PreBeginPosition = -1;
    private readonly List<RecordedCall> recordedCalls = new List<RecordedCall>();
    private int currentItemIndex = PreBeginPosition;

    public IRecordedCall Current
    {
      get { return recordedCalls[currentItemIndex]; }
    }

    public void Add(ICall invocation, Mock target)
    {
      recordedCalls.Add(new RecordedCall(invocation, target));
    }

    public bool MoveToNext()
    {
      if (EOF) return false;

      currentItemIndex++;
      return !EOF;
    }

    private bool EOF
    {
      get { return currentItemIndex >= recordedCalls.Count; }
    }

    public void Rewind()
    {
      currentItemIndex = PreBeginPosition;
    }


    public bool ContainsStartingFromCurrentPosition(List<IExpectedCall> expectedCalls)
    {
      if (expectedCalls.Count > recordedCalls.Count - currentItemIndex)
      {
        return false;
      }

      for (var i = 0; i < expectedCalls.Count; ++i)
      {
        var currentExpectedCall = expectedCalls[i];

        var currentCall = recordedCalls[i + currentItemIndex];
        var result = currentCall.Matches(currentExpectedCall);

        if (!result)
        {
          return false;
        }
      }
      return true;
    }
  }


}

