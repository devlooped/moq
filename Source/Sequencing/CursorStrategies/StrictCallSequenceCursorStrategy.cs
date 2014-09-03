using Moq.Proxy;
using Moq.Sequencing.Extensibility;

namespace Moq.Sequencing.NavigationStrategies
{
  /// <summary>
  /// Verifies order of calls in strict order,
  /// however, from the first recorded call.
  /// Does not verify that no more calls have been made
  /// than the verified ones.
  /// So in case actual calls are: A,B,C,D
  /// and we verify sequence: A,B,
  /// then the verification passes
  /// </summary>
  internal class StrictCallSequenceCursorStrategy : ICallSequenceCursorStrategy
  {
    public bool MovePast(ICallMatcher expected, Mock target, IRecordedCalls recordedCalls)
    {
      if (recordedCalls.MoveToNext())
      {
        return recordedCalls.Current.Matches(expected, target);
      }

      return false;
    }
  }
}