using System.Collections.Generic;
using Moq.Sequencing.Extensibility;

namespace Moq.Sequencing
{
  internal class ExpectedCalls
  {
    private readonly List<IExpectedCall> expectedCalls = new List<IExpectedCall>();

    public void Add(IExpectedCall expectedCall)
    {
      expectedCalls.Add(expectedCall);
    }

    public bool IsSubsequenceOf(IRecordedCalls recordedCalls)
    {
      var result = false;
      while (recordedCalls.MoveToNext())
      {
        if (recordedCalls.ContainsStartingFromCurrentPosition(expectedCalls))
        {
          result = true;
          break;
        }
      }
      recordedCalls.Rewind();
      return result;
    }

  }
}