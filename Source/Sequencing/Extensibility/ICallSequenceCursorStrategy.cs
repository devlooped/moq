using Moq.Proxy;

namespace Moq.Sequencing.Extensibility
{
  /// <summary>
  /// Describes a strategy of moving through recorded call sequence when matching calls
  /// </summary>
  internal interface ICallSequenceCursorStrategy
  {
    /// <summary>
    /// Tries to find the expected call among recorded calls and moves the recorded calls cursor
    /// past the found match to indicate which part of the recorded calls is already matched
    /// </summary>
    /// <param name="expected">expected call</param>
    /// <param name="target">on which mock should the call have been invoked</param>
    /// <param name="recordedCalls">recorded calls containing current cursor position</param>
    /// <returns>true if expected call was matched by the recorded calls, otherwise false</returns>
    bool MovePast(ICallMatcher expected, Mock target, IRecordedCalls recordedCalls);
  }
}