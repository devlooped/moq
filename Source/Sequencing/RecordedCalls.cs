using System;
using System.Collections.Generic;
using Moq.Proxy;
using Moq.Sequencing.Extensibility;

namespace Moq.Sequencing
{
  internal class RecordedCalls : IRecordedCalls
  {
		private readonly List<Tuple<ICallContext, Mock>> callContexts = new List<Tuple<ICallContext, Mock>>();
		private int currentItemIndex = 0;
		
		public void Add (ICallContext invocation, Mock target)
		{
			callContexts.Add(Tuple.Create(invocation, target));
		}
	
		public bool CurrentCallMatches(ICallMatchable expected, Mock target)
		{
			return CallMatches(currentItemIndex, expected, target);
		}
	
		public bool NextCallMatches(ICallMatchable expected, Mock target)
		{
			if(NextCallExists())
			{
				return CallMatches(currentItemIndex + 1, expected, target);
			}
			return false;
		}
		
		public void ForwardToNextCall()
		{
			currentItemIndex++;
		}
		
		public bool AnyUncheckedCallsLeft()
		{
			return currentItemIndex < callContexts.Count;
		}
	
		public bool NextCallExists()
		{
			return currentItemIndex + 1< callContexts.Count;
		}
		
		public void Rewind()
		{
			currentItemIndex = 0;
		}
	
		public bool ContainsFurther(ICallMatchable expected, Mock target)
		{
			for(var i = currentItemIndex ; i < callContexts.Count ; ++i)
			{
				if(CallMatches(i, expected, target))
				{
					return true;
				}
			}
			return false;
		}
		
		public bool ForwardBeyondSubsequence(List<Tuple<ICallMatchable, Mock>> callsToVerify)
		{
			for (var i = 0 ; i < callContexts.Count ; ++i)
			{
				if(this.MatchSubsequenceStartingFrom(i, callsToVerify))
				{
					currentItemIndex = i + callsToVerify.Count;
					return true;
				}
			}
			return false;
		}
	
		private bool CallMatches(int index, ICallMatchable expected, Mock target)
		{
			var currentContext = callContexts[index].Item1;
			var currentTarget = callContexts[index].Item2;
			
			bool result =
				IsThereAMatchBetweenContextAndExpectedCall(
					currentContext, currentTarget, expected, target);
			return result;
		}
	
		
		private bool IsThereAMatchBetweenContextAndExpectedCall(ICallContext currentContext, Mock currentTarget, ICallMatchable expected, Mock target)
		{
			var result = expected.Matches(currentContext) && target == currentTarget;
			return result;
		}
		
		private bool MatchSubsequenceStartingFrom(
			int subsequenceStartIndex,
			List<Tuple<ICallMatchable, Mock>> callsToVerify)
		{
			if(callsToVerify.Count >
			   callContexts.Count - subsequenceStartIndex)
			{
				return false;
			}
			
			for(var i = 0 ; i < callsToVerify.Count ; ++i)
			{
				var currentContextData = callContexts[i + subsequenceStartIndex];
				var currentCallData = callsToVerify[i];
				var currentContext = currentContextData.Item1;
				var currentContextTarget = currentContextData.Item2;
				var expectedCall = currentCallData.Item1;
				var expectedTarget = currentCallData.Item2;
				if(!IsThereAMatchBetweenContextAndExpectedCall(
					currentContext, 
					currentContextTarget, 
					expectedCall, 
					expectedTarget))
				{
					return false;
				}
			}
			return true;
		}
	
	}

}

