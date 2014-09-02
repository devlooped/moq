using	Moq.Proxy;
using	Moq.Sequencing.Extensibility;

namespace	Moq.Sequencing.NavigationStrategies
{
	internal class NullSequenceNavigation	:	ICallSequenceNavigation
	{
		public bool	ForwardBeyondACallTo(ICallMatchable	expected,	Mock target, IRecordedCalls	recordedCalls)
		{
			throw	new	NoSequenceAssignedException("No	sequence set up	for	this mock. Please	set	up a valid sequence.");
		}
	}
}