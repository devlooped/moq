using Moq.Proxy;

namespace Moq.Sequencing
{
  internal interface IExpectedCall
  {
    bool Matches(ICall call, Mock target);
  }
}