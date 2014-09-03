using Moq.Proxy;

namespace Moq.Sequencing
{
  internal interface ISequencedCall
  {
    bool Matches(ICallMatcher callMatcher, Mock target);
  }
}