using System;
using System.Linq.Expressions;
using Moq.Sequencing.Extensibility;

namespace Moq.Sequencing
{
  internal class CallVerificationStep<T> : IVerificationStep where T : class
  {
    readonly Mock<T> mock;
    readonly Expression<Action<T>> action;

    public CallVerificationStep(Mock<T> mock, Expression<Action<T>> action)
    {
      this.mock = mock;
      this.action = action;
    }

    public void Verify()
    {
      mock.VerifyInSequence(action);
    }

    public CallSequence CallSequence { get { return mock.CallSequence; } }
  }
}