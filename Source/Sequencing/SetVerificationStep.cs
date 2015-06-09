using System;
using Moq.Sequencing.Extensibility;

namespace Moq.Sequencing
{
  internal class SetVerificationStep<T> : IVerificationStep where T : class
  {
    readonly Mock<T> mock;
    readonly Action<T> action;

    public SetVerificationStep(Mock<T> mock, Action<T> action)
    {
      this.mock = mock;
      this.action = action;
    }

    public void Verify()
    {
      mock.VerifySetInSequence(action);
    }
  }
}