using System;
using System.Collections.Generic;
using Moq.Proxy;

namespace Moq.Sequencing.Extensibility
{
  internal interface IRecordedCalls
  {
    void Add (ICallContext invocation, Mock target);
    bool CurrentCallMatches(ICallMatchable expected, Mock target);
    bool NextCallMatches(ICallMatchable expected, Mock target);
    void ForwardToNextCall();
    bool AnyUncheckedCallsLeft();
    bool NextCallExists();
    void Rewind();
    bool ContainsFurther(ICallMatchable expected, Mock target);
    bool ForwardBeyondSubsequence(List<Tuple<ICallMatchable, Mock>> callsToVerify);
  }
}