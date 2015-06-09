using Moq.Proxy;

namespace Moq.Sequencing
{
  internal class RecordedCall : IRecordedCall
  {
    public RecordedCall(ICall invocation, Mock target)
    {
      Call = invocation;
      Target = target;
    }

    private Mock Target { get; set; }
    private ICall Call { get; set; }

    public bool Matches(IExpectedCall currentExpectedCall)
    {
      return currentExpectedCall.Matches(Call, Target);
    }
  }
}