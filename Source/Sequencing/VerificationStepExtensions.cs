using System;
using System.Linq.Expressions;
using Moq.Sequencing.Extensibility;

namespace Moq.Sequencing
{
  /// <summary>
  /// Extension for mock to gain CallTo, CallToGet and CallToSet method
  /// </summary>
  public static class VerificationStepExtensions
  {
    /// <summary>
    /// Returns verifiable call verification step object. Used with sequences
    /// </summary>
    /// <param name="mock"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IVerificationStep CallTo<T>(
      this Mock<T> mock,
      Expression<Action<T>> action) where T : class
    {
      return new CallVerificationStep<T>(mock, action);
    }

    /// <summary>
    /// Returns verifiable set verification step object. Used with sequences
    /// </summary>
    /// <param name="mock"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IVerificationStep CallToSet<T>(
      this Mock<T> mock,
      Action<T> action) where T : class
    {
      return new SetVerificationStep<T>(mock, action);
    }

    /// <summary>
    /// Returns verifiable get verification step object. Used with sequences
    /// </summary>
    /// <param name="mock"></param>
    /// <param name="action"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TProperty"> </typeparam>
    /// <returns></returns>
    public static IVerificationStep CallToGet<T, TProperty>(
      this Mock<T> mock,
      Expression<Func<T, TProperty>> action) where T : class
    {
      return new GetVerificationStep<T, TProperty>(mock, action);
    }
  }
}