namespace Moq.Sequencing.NavigationStrategies
{
  /// <summary>
  /// Works like a strict sequence with the exception
  /// that it does not require the first verified call
  /// to be the first recorded call (in other words, 
  /// the strict sequence of calls can happen anytime)
  /// </summary>
  public class StrictSequenceStartingAnywhere : CallSequence
  {
    /// <summary>
    /// Creates a new instance
    /// </summary>
    public StrictSequenceStartingAnywhere()
      : base(new StrictCallSequencePartCursorStrategy())
    {
    }
  }
}