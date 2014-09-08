namespace Moq.Sequencing.Extensibility
{
  /// <summary>
  /// Single step of verification of call order
  /// </summary>
  public interface IVerificationStep
  {
    /// <summary>
    /// Verifies the step. Throws MockException on failure
    /// </summary>
    void Verify();
  }
}