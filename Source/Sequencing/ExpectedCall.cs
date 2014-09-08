using Moq.Proxy;

namespace Moq.Sequencing
{
  internal class ExpectedCall : IExpectedCall
  {
    private ICallMatcher Matcher { get; set; }
    private Mock Target { get; set; }

    public ExpectedCall(ICallMatcher matcher, Mock target)
    {
      Matcher = matcher;
      Target = target;
    }
    
    public bool Matches(ICall call, Mock target)
    {
      return Matcher.Matches(call) && Target == target;
    }
  }
}