using System;
using System.Collections.Generic;
using System.Linq;
using Moq.Proxy;
using Moq.Sequencing.Extensibility;

namespace Moq.Sequencing
{
  internal class RecordedCalls : IRecordedCalls
  {
    private const int PreBeginPosition = -1;
    private readonly List<IRecordedCall> recordedCalls = new List<IRecordedCall>();
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

    public List<IRecordedCall> RangeFromCurrentToEnd()
    {
      var callsLeftTillTheEnd = recordedCalls.Count - currentItemIndex;
      return recordedCalls.GetRange(currentItemIndex, callsLeftTillTheEnd);
    }
  }


}

