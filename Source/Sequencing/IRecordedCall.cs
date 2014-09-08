namespace Moq.Sequencing
{
  internal interface IRecordedCall
  {
    bool Matches(IExpectedCall currentExpectedCall);
  }
}