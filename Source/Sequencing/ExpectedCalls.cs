using System.Collections.Generic;
using System.Linq;
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
        if (!SequenceEqual(expectedCalls, recordedCalls.RangeFromCurrentToEnd()))
        {
          result = true;
          break;
        }
      }
      recordedCalls.Rewind();
      return result;
    }

    private static bool SequenceEqual(IEnumerable<IExpectedCall> expectedCalls, IList<IRecordedCall> recordedCalls)
    {
      return expectedCalls.Where((currentExpectedCall, i) => !recordedCalls[i].Matches(currentExpectedCall)).Any();
    }


  }
}