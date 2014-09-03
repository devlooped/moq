using System;
using System.Collections.Generic;
using Moq.Proxy;

namespace Moq.Sequencing.Extensibility
{
  internal interface IRecordedCalls
  {
    ISequencedCall Current { get; }
    bool MoveToNext();
    void Rewind();
    bool MovePastSubsequence(List<Tuple<ICallMatcher, Mock>> callsToVerify);
  }
}