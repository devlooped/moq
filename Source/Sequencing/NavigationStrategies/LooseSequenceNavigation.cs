using Moq.Proxy;
using Moq.Sequencing.Extensibility;

namespace Moq.Sequencing.NavigationStrategies
{
  /// <summary>
  /// Loose sequence, that allows skipping calls that happen
  /// between expected calls. E.g.
  /// We're expecting the sequence: A(), B()
  /// And it can be matched by a sequence A(), C(), B()
  /// </summary>
  internal class LooseSequenceNavigation : ICallSequenceNavigation
  {
    public bool ForwardBeyondACallTo(ICallMatchable expected, Mock target, IRecordedCalls recordedCalls)
    {
      var isCurrentCallStillUnmatched = false;

      while (recordedCalls.AnyUncheckedCallsLeft() && !isCurrentCallStillUnmatched)
      {
        if (recordedCalls.CurrentCallMatches(expected, target))
        {
          isCurrentCallStillUnmatched = true;
        }
        recordedCalls.ForwardToNextCall();
      }
      return isCurrentCallStillUnmatched;
    }
  }
}