using System.Collections.Generic;

namespace Moq.Sequencing.Extensibility
{
  internal interface IRecordedCalls
  {
    IRecordedCall Current { get; }
    bool MoveToNext();
    void Rewind();
    bool ContainsStartingFromCurrentPosition(List<IExpectedCall> expectedCalls);
  }
}