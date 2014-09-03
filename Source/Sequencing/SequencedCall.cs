using Moq.Proxy;

namespace Moq.Sequencing
{
  internal class SequencedCall : ISequencedCall
  {
    public SequencedCall(ICall invocation, Mock target)
    {
      Call = invocation;
      Target = target;
    }

    private Mock Target { get; set; }
    private ICall Call { get; set; }
    
    public bool Matches(ICallMatcher expected, Mock target)
    {
      return expected.Matches(Call) && target == Target;
    }
  }
}