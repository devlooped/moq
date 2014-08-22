using Moq.Proxy;

namespace Moq.Sequencing.Extensibility
{
  internal interface ICallSequenceNavigation
  {
    bool ForwardBeyondACallTo(ICallMatchable expected, Mock target, IRecordedCalls recordedCalls);
  }
}