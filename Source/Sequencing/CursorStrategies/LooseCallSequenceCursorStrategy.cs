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
  internal class LooseCallSequenceCursorStrategy : ICallSequenceCursorStrategy
  {
    public bool MovePast(ICallMatcher expected, Mock target, IRecordedCalls recordedCalls)
    {
      var isCurrentCallStillUnmatched = false;

      while (!isCurrentCallStillUnmatched && recordedCalls.MoveToNext())
      {
        if (recordedCalls.Current.Matches(expected, target))
        {
          isCurrentCallStillUnmatched = true;
        }
      }
      return isCurrentCallStillUnmatched;
    }
  }
}
