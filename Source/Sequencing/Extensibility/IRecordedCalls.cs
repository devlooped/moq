using System.Collections.Generic;

namespace Moq.Sequencing.Extensibility
{
  internal interface IRecordedCalls
  {
    IRecordedCall Current { get; }
    bool MoveToNext();
    void Rewind();
    List<IRecordedCall> RangeFromCurrentToEnd();
  }
}