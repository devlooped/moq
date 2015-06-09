using System;
using System.Linq.Expressions;
using Moq.Sequencing.Extensibility;

namespace Moq.Sequencing
{
  internal class GetVerificationStep<T, TProperty> : IVerificationStep where T : class
  {
    readonly Mock<T> mock;
    readonly Expression<Func<T, TProperty>> action;

    public GetVerificationStep(Mock<T> mock, Expression<Func<T, TProperty>> action)
    {
      this.mock = mock;
      this.action = action;
    }

    public void Verify()
    {
      mock.VerifyGetInSequence(action);
    }
  }
}