using Moq.Sequencing.NavigationStrategies;

namespace Moq.Sequencing
{
  internal class NullSequence : CallSequence
  {
    internal NullSequence()
      : base(new NullCallSequenceCursorStrategy())
    {
    }
  }

}
